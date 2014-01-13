/*
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  FPVOSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FPVOSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <stdio.h>

#include "C8051F340.h"
#include "config.h"
#include "LibraryMAX7456.h"
#include "huds.h"
#include "ParserNMEA.h"
#include "Servos.h"
#include "USB_API.h"
#include "Navigation.h"
#include "Telemetry.h"
#include "Ikarus.h"
#include "Utils.h"
#include "AutoPilot.h"
#include "MenuConfig.h"
#include "PID.h"
#include "modem.h"
#include "controladores.h"

extern xdata struct GPSInfo gpsinfo;

xdata struct IkarusInfo ikarusInfo;
xdata unsigned long tics=0;	// 100 hz

extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern code struct Ruta miRuta;

extern code struct Screen hud_text;
extern code struct Screen huds[5];

extern xdata unsigned char rflost;

extern code char lock_byte;


extern xdata float modem_irxoff, modem_iryoff, modem_irmax, modem_ail, modem_ele;
extern xdata struct PID elevatorPID;
extern xdata struct PID aileronsPID;
extern xdata struct PID tailPID;
extern xdata struct PID motorPID;


extern xdata unsigned char movingOK;
extern xdata unsigned char movingCounter;

extern xdata unsigned char updateHome;
extern xdata unsigned char saveToFlash;

xdata char cuenta_menu;
xdata char enable_cuenta;


bit trigger;

void CheckIkarusROM()
{
	#ifdef SECURITY_FW
	if(lock_byte==-1)
		flash_write(&lock_byte, -125);		// -120 dejar sin proteger datos. -125 todo		
	#endif

	ClrScr();

	if(miRuta.numwpt<0)
	{
		int zero=0;
		ram2rom((void*)&miRuta.numwpt, (void*)&zero, sizeof(int));
	}

	// Si no hay huds, cargamos el de por defecto
	if(huds[0].V1_text.tipo<0)
	{
		xdata struct Screen scr;
		scr=hud_text;	// copia de ROM a RAM
		
		sprintf(scr.strNombreHUD,"HUD 1");
		ram2rom((void*)&huds[0], (void*)&scr,sizeof(struct Screen));

		sprintf(scr.strNombreHUD,"HUD 2");
		ram2rom((void*)&huds[1], (void*)&scr,sizeof(struct Screen));

		sprintf(scr.strNombreHUD,"HUD 3");
		ram2rom((void*)&huds[2], (void*)&scr,sizeof(struct Screen));

		sprintf(scr.strNombreHUD,"FAILSAFE");
		ram2rom((void*)&huds[3], (void*)&scr,sizeof(struct Screen));

		sprintf(scr.strNombreHUD,"RESUME");
		ram2rom((void*)&huds[4], (void*)&scr,sizeof(struct Screen));

		flush_rom();
	}

	if(storedConfig.Video_PAL==-1)	// EEPROM BORRADA?
		{
		IkarusOsdAutoconfig(0);
		sendValue(0x2,storedConfig.offsetX);		//hor offset
		sendValue(0x3,storedConfig.offsetY); 		//ver offset
		}

	if(autopilotCfg.AutopilotMode==-1)	// EEPROM BORRADA?
		AutopilotAutoconfig(0);

}

void Load_Params_Flash()
{
	modem_irxoff=autopilotCfg.x_off;
	modem_iryoff=autopilotCfg.y_off;

	modem_irmax=autopilotCfg.IR_max;

	modem_ail=autopilotCfg.rumbo_ail;
	modem_ele=autopilotCfg.altura_ele;
}

void InitIkarusInfo()
{
	int i;
	ikarusInfo.mostrarResumen=0;
	ikarusInfo.currI=0;
	ikarusInfo.v1 = 0;
	ikarusInfo.v2 = 0;
	for(i=0;i<sizeof(struct IkarusInfo);i++)
		((char*)&ikarusInfo)[i]=0;

	ikarusInfo.hora_inicio=-1;
	ikarusInfo.consumidos_mAh =0;

	ikarusInfo.modem_lon=storedConfig.HomeLon;
	ikarusInfo.modem_lat=storedConfig.HomeLat;
	ikarusInfo.modem_alt=storedConfig.HomeAltitude;
	ikarusInfo.modem_sw=0;

	ikarusInfo.navigateTo=NAV_HOME;


	Load_Params_Flash();

	movingOK=0;
	movingCounter=0;
	updateHome=0;
	saveToFlash = 0;
	cuenta_menu = 0;

}



void initIkarus()
{	
	int i;

//	xdata float tmp_ail, tmp_ele, tmp_tail, tmp_thr;

	if(storedConfig.CamSel==2)
		CAM_SEL=0;
	else
		CAM_SEL=1;
	
	InitIkarusInfo();
	InitMAX7456();

	ikarusInfo.servoDriver = SERVO_COPY;
	ActivarServos();

	CheckIkarusROM();

	// Activamos varios modulos
	modem_init();
	initStruct();
	init_navigator();
		
//	ikarusInfo.modem_lon=storedConfig.HomeLon;
//	ikarusInfo.modem_lat=storedConfig.HomeLat;
//	ikarusInfo.modem_alt=storedConfig.HomeAltitude;
//	ikarusInfo.modem_sw=0;

//	movingOK=0;
//	movingCounter=0;
//	updateHome=0;
//	saveToFlash = 0;
//	cuenta_menu = 0;

	ClrScr();			
	logoIkarus();
	for(i=0;i<50&&ikarusInfo.v1<1.0;i++)
		wait_for(10);
	
	USB_Connection();

//	if((get_servo_in(CTRL)<900)||(get_servo_in(CTRL)<1200&&get_servo_in(TAIL)<1200))
//		CalibrarServos();

	if((ikarusInfo.v1<1.0f))
	{
		WaitForGPSscreen(storedConfig.ControlProportional!=MODO_MODEM);
	}

	ikarusInfo.navigateTo=NAV_HOME;
	InitAutoPilot();

	ikarusInfo.consumidos_mAh=0;
}

void main (void) large
{
	int i;
	static int last_id=0;
	static char half=0;


	Init_Device();

	// ES NECESARIO VMON PARA PODER ESCRIBIR EN LA FLASH
 	VDM0CN    = 0x80;
    for (i = 0; i < 350; i++);  // Wait 100us for initialization
    RSTSRC    = 0x02;

	PCA0CPL4 = 0xff;	// 65ms
	PCA0MD|=0x60;		// 0x40 Activa el Watchdog!!!! 0x60 LOCK
	CLR_WDT();

	NSS=1;	


	initIkarus();

	ClrScr();
	while(1)
	{
		if(ikarusInfo.hora_inicio<0&&gpsinfo.pos_valid==1)
		{
			ikarusInfo.hora_inicio=gpsinfo.hora;
		}

	#ifndef IMU_UM6
		if(autopilotCfg.IR_Z_enabled==IR_XY||autopilotCfg.IR_Z_enabled==IR_XYZ)
		{
			Sensor_IR();
		}
		else
		{
			Sensor_DUMMY();
		}
	#endif
		half=!half;
		if(half)
			UpdateNavigator();		// Actualizamos el navegador
		
		Control_ServosIN();
		Control_ServosOUT(); 	// LowLevelAutoPilot() y TopLevelAutopilot() se llaman aqui

		// Preparamos telemetria nueva
		if(storedConfig.TelemetryMode==1)
			BuildTelemetryPKT();
		else if((storedConfig.TelemetryMode==2)&&((tics%50)==0))
		{
			BuildTelemetryPKT();
			InitXBEE_tx();
		}

		// Actualizamos el HUD....
		while(!trigger)
		{
			last_id++;
			if(last_id<0||last_id>eAuxiliary)		// eNombreHUD deberia ser el nº del ultimo instrumento
				last_id=0;
			
			MuestraInstrumento(last_id);
		}
		trigger=0;

		if(storedConfig.ControlProportional!=MODO_MODEM)
		{
			if((leer_servo(THR)<-0.8f) && (leer_servo(AIL)>0.8f)&& (leer_servo(TAIL)<-0.8f) && (leer_servo(ELE)>0.8f))// && gpsinfo.knots_filtered==0)
			{
				if(cuenta_menu>=DELAY_MENU)		// 
				{
					WaitForGPSscreen(1);
					InitIkarusInfo();
					//ikarusInfo.hora_inicio=-1;
					ClrScr();
				}
				else
					enable_cuenta = 1;
			}
			else
			{
				enable_cuenta = 0;
				cuenta_menu = 0;
			}
		}
		else //if(gpsinfo.knots_filtered<10)
		{
			if(updateHome)
			{
				updateHome=0;
				FixHome();
			}
			else if(saveToFlash)
			{
				ram2rom((void*)&autopilotCfg.rumbo_ail, (void*)&modem_ail, sizeof(float));
				ram2rom((void*)&autopilotCfg.altura_ele, (void*)&modem_ele, sizeof(float));
				
				ram2rom((void*)&autopilotCfg.x_off, (void*)&modem_irxoff, sizeof(float));
				ram2rom((void*)&autopilotCfg.y_off, (void*)&modem_iryoff, sizeof(float));
				ram2rom((void*)&autopilotCfg.IR_max, (void*)&modem_irmax, sizeof(float));

				ram2rom((void*)&autopilotCfg.pid_pitch.P, (void*)&elevatorPID.P, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_pitch.I, (void*)&elevatorPID.I, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_pitch.D, (void*)&elevatorPID.D, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_pitch.IL, (void*)&elevatorPID.ILimit, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_pitch.DL, (void*)&elevatorPID.DriveLimit, sizeof(float));

				ram2rom((void*)&autopilotCfg.pid_roll.P, (void*)&aileronsPID.P, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_roll.I, (void*)&aileronsPID.I, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_roll.D, (void*)&aileronsPID.D, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_roll.IL, (void*)&aileronsPID.ILimit, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_roll.DL, (void*)&aileronsPID.DriveLimit, sizeof(float));

				ram2rom((void*)&autopilotCfg.pid_motor.P, (void*)&motorPID.P, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_motor.I, (void*)&motorPID.I, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_motor.D, (void*)&motorPID.D, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_motor.IL, (void*)&motorPID.ILimit, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_motor.DL, (void*)&motorPID.DriveLimit, sizeof(float));

				ram2rom((void*)&autopilotCfg.pid_tail.P, (void*)&tailPID.P, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_tail.I, (void*)&tailPID.I, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_tail.D, (void*)&tailPID.D, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_tail.IL, (void*)&tailPID.ILimit, sizeof(float));
				ram2rom((void*)&autopilotCfg.pid_tail.DL, (void*)&tailPID.DriveLimit, sizeof(float));

				flush_rom();

				saveToFlash=0;

			}
		}
	}
}


void Timer2_handle () interrupt 5
{
	static unsigned char counter;
	CLR_WDT();
	
	counter++;
	if(counter>=100/10)
	{
		counter=0;
		trigger=1;
		if(enable_cuenta&&cuenta_menu<DELAY_MENU)
		{
			enable_cuenta = 0;
			cuenta_menu++;
		}

		if(rflost<254)
			rflost++;


		if(gpsinfo.uart_timeout<10)
			gpsinfo.uart_timeout++;
		else
			gpsinfo.conected=0;
		
		if(gpsinfo.nmea_timeout<10)
			gpsinfo.nmea_timeout++;
		else
			gpsinfo.nmea_ok=0;
		
		if(ikarusInfo.uplink_cmd_countdown >0)
			ikarusInfo.uplink_cmd_countdown--;
	}

	tics++;		// 100 Hz
	
	AD0BUSY=1;	
	
	TF2H=0;
}

