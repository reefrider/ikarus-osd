/* 
 * (c) 2011 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  IKARUS_OSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IKARUS_OSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <string.h>
#include <stdio.h>
#include <math.h>



#include "c8051f340.h"
#include "AntTracker.h"
#include "config.h"

#include "Servos.h"
#include "Utils.h"
#include "Calculate.h"

#define BUILD_DATE "Build: " __DATE2__ //"-"__TIME__ 

code char lock_byte _at_ 0xFBFF;

extern code struct StoredConfig storedConfig;
xdata struct DatosAntena datosAntena;
xdata struct DatosAvion datosAvion;
xdata char UpdatedDatosAvion;

xdata float v_bateria;

xdata struct Debug debugInfo;

xdata unsigned long tics;
xdata unsigned long timeout;

//xdata float heading;
xdata float pan, tilt;
xdata float offset_pan;

xdata char flag_10hz;

void InitUSB();
void atc_parser();

extern xdata unsigned int pageRAMaddr;
extern xdata unsigned int servos[MAX_CH_OUT];

void CheckConfig() large
{
	struct StoredConfig initConfig;

	if(storedConfig.useInternalGPS==-1)
	{
	//initConfig.ServoPAN;
	//initConfig.ServoTILT;
	//initConfig.ServoAUX;

	//initConfig.GradosPAN;
	//initConfig.GradosTILT;

	initConfig.useInternalGPS = 1;
    initConfig.AltitudSinGPS = 0;

    initConfig.useInternalCompas = 1;
    initConfig.offsetCompas = 90.0f;
    initConfig.enableCompasOverride = 1;
    initConfig.speedCompasOverride = 20.0f;

    initConfig.useLipo = 1;
    initConfig.numCellsLipo = 3;
    initConfig.Vmin = 9.6f;
    initConfig.Vmax = 12.6f;
    initConfig.Valarm= 10.6f;

    initConfig.offsetPan = 0;
    initConfig.decodeTelemetry = 1;

	ram2rom((char volatile*)&storedConfig, (char*)&initConfig,sizeof(struct StoredConfig));
	flush_rom(); 
	}   
}

void InitPWM()
{
    PCA0CPM0  = 0x4D;
    PCA0CPM1  = 0x4D;
}

void InitAntTracker()
{
	pageRAMaddr=0;

	LED = 1;

	CheckConfig();

	tics=0;
	set_servo(SERVO_PAN,1500);
	set_servo(SERVO_TILT,1500);
//	set_servo(SERVO_AUX,1500);
	
	UpdatedDatosAvion=0;

	datosAvion.lon = -6.0f;
	datosAvion.lat = 37.0f;
	datosAvion.alt = 0.0f;

	datosAvion.home_lon = -6.0f;
	datosAvion.home_lat = 37.0f;
	datosAvion.home_alt = 0.0f;

	debugInfo.EnableDebug = 0;
	debugInfo.pan = 1500;
	debugInfo.tilt = 1500;
	debugInfo.grados_pan = 0.0f;
	debugInfo.grados_tilt = 0.0f;

	InitUSB();
	InitPWM();

	offset_pan = 0.0f;

	AD0BUSY = 1;
	
}



void main() large
{
//	char cad[20];
	unsigned int i=0;
	unsigned int j=1000;

	float servo=-170.0f;
	
	Init_Device();

	#ifdef SECURITY_FW
	if(lock_byte==-1)
		flash_write(&lock_byte, -125);		// -120 dejar sin proteger datos. -125 todo		
	#endif

	InitAntTracker();

	while(1)
	{
		BuclePrincipal();
	}
}

void CheckeaPulsaciones()
{
	if((BUTTON0_RMT == 0) && (BUTTON1_RMT == 0))
	{
		offset_pan = 0.0f;
	}
	if(BUTTON0_RMT == 0)
	{
		offset_pan -= 1.0f;
	}
	else if(BUTTON1_RMT == 0)
	{
		offset_pan += 1.0f;
	}
}

void BuclePrincipal()
{
	if(flag_10hz)
	{
		flag_10hz=0;
	
		atc_parser();

		Copy2DatosAntena();	

		//CalculateHeading();

		CalculaPanTilt();	
		
		CheckeaPulsaciones();	
	
		if((debugInfo.EnableDebug&0x03) == 0)
		{
			set_servo_pan(pan+offset_pan);
		}
		else if((debugInfo.EnableDebug&0x03) == 1)
		{
			set_servo(SERVO_PAN,debugInfo.pan);
		}
		else
			set_servo_pan(debugInfo.grados_pan+offset_pan);
		
			
		if((debugInfo.EnableDebug&0x0C) == 0)
		{
			set_servo_tilt(tilt);
		}
		else if((debugInfo.EnableDebug&0x0C) == 4)
		{
			set_servo(SERVO_TILT,debugInfo.tilt);
		}
		else
			set_servo_tilt(debugInfo.grados_tilt);
	

		if(!datosAntena.v_bateria_alarm)
		{
			LED=1;
		}
		else if(tics&0x40)
		{
			LED=0;
		}
		else
		{
			LED=1;
		}

	}
}

void Timer3_handle () interrupt 14 //5
{
	static unsigned char paso;
	
	CLR_WDT();

	if(paso<10)
		paso++;
	else
	{
		flag_10hz=1;

	//	FSM_Botones();
		
		paso=0;
	}
	
	tics++;		// 100 Hz
		
	TMR3CN&=0x7f;	//TF3H=0
}


#define LP_FILTER		32

void ADC0_handle() interrupt 10
{
	xdata int valor16;
	xdata float valorf;
	
	//valor16=ADC0H;
	//valor16=(valor16<<8)|ADC0L;
	valor16=ADC0;
	valorf=(14+4.75)/4.75*3.4f*valor16/1023.0f;

	valorf=((LP_FILTER-1)*v_bateria+valorf)/LP_FILTER;
	v_bateria = valorf;


	AD0INT=0;		// Clear interrupt flag

	AD0BUSY=1;		// Start new conversion
}