/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
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


#include <stdio.h>
#include <math.h>

#include "c8051f340.h"

#include "LibraryMax7456.h"
#include "ParserNMEA.h"
#include "Navigation.h"
#include "Utils.h"
#include "Servos.h"
#include "huds.h"
#include "Ikarus.h"
#include "MenuConfig.h"
#include "PID.h"
#include "Controladores.h"

extern xdata struct IkarusInfo ikarusInfo;
extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern xdata struct GPSInfo gpsinfo;

extern bit vsync;
//extern xdata unsigned char rflost;
extern xdata unsigned long tics;

extern xdata float adc_values[];


extern xdata struct PID elevatorPID;
extern xdata struct PID aileronsPID;
extern xdata struct PID tailPID;
extern xdata struct PID motorPID;
extern xdata float modem_ail, modem_ele;

void Load_Params_Flash();

#define _COLS 11

char enter_pressed()
{
	return switch_changed(MNU_ENTER)&&switch_pos(MNU_ENTER);
}

char enter_down()
{
	return switch_pos(MNU_ENTER);
}

char next_pressed()
{
	return switch_changed(MNU_NEXT)&&switch_pos(MNU_NEXT);
}

char next_down()
{
	return switch_pos(MNU_NEXT);	
}

void HUD_Debug(int fila, int col, unsigned char param) large
{	
	code const char MAX_idx=14; 
	code char cad[][_COLS]={
		"Pitch P   ", "Pitch I   ", "Pitch ILim", 
		"Roll P    ", "Roll I    ", "Roll ILim ", 
		"Tail P    ", "Tail I    ", "Tail ILim ",
		"Motor P   ", "Motor I   ", "Motor ILim",
		"ALTIT. ELE", "RUMBO AIL "};
	
	static char estado = -1;
	static char addmult;
	static float *pValue;
	static float Inc, Min, Max;

	xdata char text[20];

	char next = next_pressed();
	char prev = enter_pressed();

	float sf = get_servof(PAN);

	if((estado<0)||next)//||prev)//next_pressed())
	{
		//if(prev)
		//{
		//	if(estado <= 0)
		//		estado = MAX_idx - 1;
		//	else
		//		estado = estado - 1;
		//}
		//else
		//{
			estado = (estado+1)%MAX_idx;
		//}

		// Por defecto
		addmult=1;	
		Inc = 1.1f;
		Min = 0.0001f;
		Max = 0.5f;

		switch (estado)
		{
			case 0:
				pValue = &elevatorPID.P;
				break;
			case 1:
				pValue = &elevatorPID.I;
				break;
			case 2:
				pValue = &elevatorPID.ILimit;
				Max=100.0f;
				break;

			case 3:
				pValue = &aileronsPID.P;
				break;
			case 4:
				pValue = &aileronsPID.I;
				break;
			case 5:
				pValue = &aileronsPID.ILimit;
				Max=100.0f;
				break;


			case 6:
				pValue = &tailPID.P;
				break;
			case 7:
				pValue = &tailPID.I;
				break;
			case 8:
				pValue = &tailPID.ILimit;
				Max=100.0f;
				break;

			case 9:
				pValue = &motorPID.P;
				break;
			case 10:
				pValue = &motorPID.I;
				break;
			case 11:
				pValue = &motorPID.ILimit;
				Max=100.0f;
				break;

			case 12:
				pValue = &modem_ele;
				addmult=0;
				Inc=0.2f;
				Min=0.0f;
				Max=autopilotCfg.altura_ele;
				break;

			case 13:
				pValue = &modem_ail;
				addmult=0;
				Inc=0.2f;
				Min=0.0f;
				Max=autopilotCfg.rumbo_ail;
				break;
		}
	}

	if(param)	// Pos -> Valor absoluto
	{
		if(enter_down())
		{	
			*pValue=Min+(Max-Min)*(sf+0.9)/1.8;
		
			if (*pValue < Min)
				*pValue = Min;
			else if(*pValue >Max)
				*pValue = Max;
		}
	}
	else // Pos -> Incremento log
	{
		if((sf>0.1 || sf <-0.1)&&enter_down())//!enter_down()&&!next_down()))
		{	
			*pValue*=1+sf/20;
			if(*pValue == 0)
				*pValue = 0.0001;
			else if (*pValue < Min)
				*pValue = Min;
			else if(*pValue >Max)
				*pValue = Max;
		
		}
	}

	/*
	if(enter_pressed())
	{
		if(addmult==0)
			Inc=-Inc;
		else
			Inc=1/Inc;
	}
	
	if(enter_down())
	{
		if(addmult==0)
		{	// Lineal
			*pValue +=Inc;
		}
		else
		{	// Logaritmico
			*pValue *=Inc;
		}

		if(*pValue<Min)
			*pValue = Min;
		else if(*pValue>Max)
			*pValue = Max;
	}
	*/
	sprintf(text,"%s  %.4f ",cad[estado],*pValue);
	printAtStr(fila,col,text);

}

void CalibrarCopilot() large
{
	char cad[50];
	float max;
	float co_x, co_y;
	ClrScr();
	printCenteredAtStr(1, "Calibrando IR");

	printCenteredAtStr(3, "Set horizontal (OK)");
	while(!enter_pressed())
	{
		sprintf(cad,"P: %.4f R:%.4f",adc_values[ADC_IR_X], adc_values[ADC_IR_Y]);
		printAtStr(4,3,cad);
	}
	ram2rom((void*)&autopilotCfg.x_off, (void*)&adc_values[ADC_IR_X], 3*sizeof(float));
//	ram2rom((void*)&autopilotCfg.y_off, (void*)&adc_values[ADC_IR_Y], sizeof(float));
//	ram2rom((void*)&autopilotCfg.z_off, (void*)&adc_values[ADC_IR_Z], sizeof(float));


	printCenteredAtStr(6, "Set vertical (OK)");
	while(!enter_pressed())
	{
		co_x=adc_values[ADC_IR_X]-autopilotCfg.x_off;
		if(co_x<0)
			co_x=-co_x;

		co_y=adc_values[ADC_IR_Y]-autopilotCfg.y_off;
		if(co_y<0)
			co_y=-co_y;

		max=co_x+co_y;

		sprintf(cad,"Max: %.4f ",max);
		printAtStr(7,3,cad);
	}

	if(max<0)
		max=-max;

	ram2rom((void*)&autopilotCfg.IR_max, (void*)&max, sizeof(float));
	flush_rom();

	Load_Params_Flash();
	

}

void CalibrarGananciaCopilot() large
{
	
	char cad[50];
	float max;
	float co_x, co_y;
	ClrScr();
	printCenteredAtStr(1, "Ganancia IR");

	printCenteredAtStr(3, "Set AIL 45º (OK)");
	while(!enter_pressed())
	{
		co_x=adc_values[ADC_IR_X]-autopilotCfg.x_off;
		if(co_x<0)
			co_x=-co_x;

		co_y=adc_values[ADC_IR_Y]-autopilotCfg.y_off;
		if(co_y<0)
			co_y=-co_y;

		max=co_x+co_y;

		max*=2; // por eso de los 45º

		sprintf(cad,"Max: %.4f ",max);
		printAtStr(5,3,cad);
	}

	if(max<0)
		max=-max;

	ram2rom((void*)&autopilotCfg.IR_max, (void*)&max, sizeof(float));
	flush_rom();

	Load_Params_Flash();
}

/*
void CalibrarRSSI() large
{
	char cad[50];
	float min=3.3f;
	float max=0.0f;
	float value;
	char wait=0;

	ClrScr();
	printCenteredAtStr(1, "Calibrando RSSI");

	while(!enter_pressed())
	{
		value=adc_values[ADC_RSSI];
		if(value>max)
			max=value;
		else if(value<min)
			min=value;
		sprintf(cad,"MIN: %.2f MAX:%.2f",min, max);
		printAtStr(4,3,cad);
	}

	if((value-min)<(max-value))	// invertir
	{
		value=min;
		min=max;
		max=value;
	}

	ram2rom((void*)&storedConfig.min_rssi, (void*)&min, sizeof(float));
	ram2rom((void*)&storedConfig.max_rssi, (void*)&max, sizeof(float));
	flush_rom();
}

void CalibrarSensorI() large
{
	char cad[50];
	float read0, read1;
	float offset, gain;
	float imax=10.0f;

	ClrScr();
	printCenteredAtStr(1, "Calibrando I");

	printCenteredAtStr(3, "Stop Motor (OK)");
	while(!enter_pressed())
	{
		read0=adc_values[ADC_I];		
		sprintf(cad,"Read 0A: %.0f",read0);
		printAtStr(4,3,cad);
	}
	
	printAtStr(6,3,"I MAX");
	imax=NumericUpDown(6,14,(float)imax,0.0f, 100.0f,1.0f, 20, "%.0f ");

	sprintf(cad,"%.0f ",imax);
	printAtStr(6,14,cad);
	
	printAtStr(8,3,"Full Throttle (OK)");
	
	while(!enter_pressed())
	{
		read1=adc_values[ADC_I];				
		sprintf(cad,"Read %dA: %.3f",imax,read1);
		printAtStr(9,3,cad);
	}
	
	gain=(read1-read0)/imax;
	offset = gain*read0;

	printAtStr(11,3,"Calibration done");

	ram2rom((void*)&storedConfig.offset_sensorI, (void*)&offset, sizeof(float));
	ram2rom((void*)&storedConfig.gain_sensorI, (void*)&gain, sizeof(float));
	flush_rom();

	wait_for(100);
}
*/

void CentrarServos() large
{
	xdata struct ServoInfo si[7];
	code const char names[MAX_CH_IN][6]={"AIL","ELE", "THR", "TAIL","PAN", "AUX"}; 
	code const unsigned int range = 50;
	
	xdata char buff[10];
		
	char ch;
	char j;
	char ns;
	unsigned int pos_ms;	

	if(!storedConfig.Modo_PPM)
		ns=2;
	else if(autopilotCfg.chAux_mode == AUX_TILT || autopilotCfg.chAux_mode == AUX_AIL2)
		ns=6;
	else
		ns=5;

	rom2ram((void*)&si, (void*)&autopilotCfg.servos_cfg, sizeof(struct ServoInfo)*7);

	ClrScr();
	printCenteredAtStr(2,"Servo Center");	
	

	while(!enter_pressed())
	{
		for(j=0;j<ns;j++)
		{
			ch = j+1;
			pos_ms = get_servo_in(ch);
			
			if((pos_ms < si[ch].min+range) || (pos_ms > si[ch].max-range))
				CharAttrBlink();				

			printAtStr(4+j,3,names[j]);
			sprintf(buff,"%4d ",pos_ms);
			printAtStr(4+j,8,buff);
			CharAttrNoBlink();
		}
	}

	for(j=0;j<ns;j++)
	{
		ch = j+1;
		pos_ms = get_servo_in(ch);
			
		if((pos_ms >= si[ch].min+range) && (pos_ms <= si[ch].max-range))
			si[ch].center=get_servo_in(ch);

	}
	ram2rom((void*)&autopilotCfg.servos_cfg, (void*)&si, sizeof(struct ServoInfo)*7);
	flush_rom();
}

void CalibrarServos() large
{
	code const char names[MAX_CH_IN][7]={"CTRL", "AIL","ELE", "THR", "TAIL","PAN", "AUX"}; 
	xdata struct ServoInfo si[7];
	xdata char buff[10];
	int i,j,k;
	long now=-1;
	char ns;
	if(storedConfig.Modo_PPM)
		ns=7;
	else
		ns=3;

	ClrScr();
	printCenteredAtStr(2,"Calibrando canales");	
	
	for(j=0;j<7;j++)
		si[j]=*(((struct ServoInfo *)autopilotCfg.servos_cfg)+j);

	for(j=0;j<7;j++)		// tenia puesto 5??!!
	{
		si[j].min=1500;
		si[j].max=1500;
		si[j].center=1500;
	}

	while(si[CTRL].min==1500||si[CTRL].max==1500||now+1000>=tics)
	{
		for(j=0;j<ns;j++)
		{
			char ch;
			if(j==0)
			{
				ch=CTRL;
				k=0;
			}
			else if(ns==3)
			{
				ch=j+2;
				k=j+2;
			}
		//	else if(j==5)
		//	{
		//		ch=j+1;
		//		k=j+1;
		//	}
			else
			{
				ch=j;
				k=j;
			}
			i=get_servo_in(ch);
			
			if(i<si[k].min&&i>SERVOS_MIN)
				{
				si[k].min=i;
				now=tics;
				}
			else if(i>si[k].max&&i<SERVOS_MAX)
				{
				si[k].max=i;
				now=tics;
				}

			if(k==AIL||k==ELE||k==TAIL||k==AUX)
				si[k].center=i;		// Fijamos el centro con la ultima pos
			else
				si[k].center=(si[k].max+si[k].min)/2;	// El centro es la media
			
			if(now+600<tics)
				CharAttrBlink();
			else
				CharAttrNoBlink();

			printAtStr(5+j,2,names[k]);
				
			Bar(5+j,12, 8, (i-1500)/4+128);
			
			sprintf(buff,"%4d ",si[k].min);
			printAtStr(5+j,7,buff);

			sprintf(buff,"%4d ",si[k].max);
			printAtStr(5+j,21,buff);
		}
	
	}
	CharAttrNoBlink();
	ram2rom((void*)&autopilotCfg.servos_cfg, (void*)&si, sizeof(struct ServoInfo)*7);
	flush_rom();
}

void AutopilotAutoconfig(char interactive) large
{

	code struct PID_Setup pid_default={0.02f, 0,0, 0, 1};
	code struct ServoInfo servo_default={1100, 1900, 1500,0};	 
	xdata struct AutoPilotConfig apc;
	xdata struct ServoInfo * servoin=&apc.servos_cfg;
	char i;
	
	ClrScr();			

	for(i=0;i<7;i++)
	{
		servoin[i]=servo_default;
	}

	apc.AutopilotMode=AP_FIX_ALT;
	apc.baseCruiseAltitude=125.0f;
	apc.distanceAltitude=100.0f;
	apc.ch_ail=0;
	apc.ch_ele=1;
	apc.ch_thr=2;
	apc.ch_tail=3;
	apc.ch_pan=5;
	apc.ch_aux=6;

	apc.pan_gain=1.8;
	apc.tipo_mezcla=MEZCLA_INH;		// Normal, Elevon, V-Tail
		
	// Sersor copilot
	apc.IR_reverse_pitch=0;
	apc.IR_reverse_roll=0;
	apc.IR_crossed=0;
	apc.IR_reverse_cross=0;

    apc.IR_Z_enabled=IR_NONE;
    apc.x_off=1.66f; 
	apc.y_off=1.66f;
	apc.z_off=1.66f;
    apc.IR_max=2;

	// Ganancias PID
	apc.pid_pitch=pid_default;
	apc.pid_roll=pid_default;
	apc.pid_motor=pid_default;
	apc.pid_tail=pid_default;

	apc.rumbo_ail=20;
	apc.altura_ele=10;

	apc.chAux_mode = 0;
	apc.MotorSafeAlt = 20.0f;

	apc.copilot_use_PID_gains = 1;;
	apc.copilot_pitch_P=0.02f;
	apc.copilot_roll_P=0.02f;

	// Autopilot mezclado
	apc.mezclaAutopilot_roll_on = 1;
	apc.mezclaAutopilot_roll_value = 0.9f;

	apc.ServoMotorEnableAlt=1;
	apc.ServoMotorSafeAlt=1000;

	ram2rom((void*)&autopilotCfg, (void*)&apc, sizeof(struct AutoPilotConfig));
	flush_rom();
	if(interactive)
	{
		CalibrarServos();
	}	
}

void IkarusOsdAutoconfig(char interactive) large
{
	xdata struct StoredConfig tmp;
	ClrScr();		
	
	interactive=0;
	
//	rom2ram((void*)&tmp,(void*)&storedConfig, sizeof(struct StoredConfig));

//	if(interactive)
//	{
//		TestHardware(); // lenta ... >30 segundos
//		tmp.Video_PAL=(AutoDetectNTSC_PAL()&0x02)==0; 	// 1 -> PAL, 0 -> NTSC, 0xff-> Eprom empty
//		tmp.cellsBatt1=NumOfCells(ikarusInfo.v1);
//		tmp.cellsBatt2=NumOfCells(ikarusInfo.v2);
//		tmp.BaudRate=GetSelectedBauds();
//	}
//	else
//	{
		tmp.Video_PAL=1; 	// 1 -> PAL, 0 -> NTSC, 0xff-> Eprom empty
		tmp.cellsBatt1=3;

		tmp.cellsBatt2=3;
	
		tmp.BaudRate=5; 
//	}

	tmp.offsetX=40;
	tmp.offsetY=21;
	
	tmp.offset_sensorV1 = (33+4.7)/4.7;
	tmp.gain_sensorV1 = 0;
	tmp.offset_sensorV2 = (14+4.7)/4.7;
	tmp.gain_sensorV2 = 0;

	tmp.total_mAh=2200.0f;


	tmp.cellAlarm=3.6f;
	tmp.distanceAlarm=400.0f;
	tmp.altitudeAlarm=100.0f;
	tmp.lowSpeedAlarm=20.0f;
	tmp.HomeLat=gpsinfo.lat;
	tmp.HomeLon=gpsinfo.lon;
	tmp.HomeAltitude=gpsinfo.alt;
	tmp.wptRange=100.0f;
	tmp.DefaultHUD=1;
	tmp.TelemetryMode=1;
	tmp.ControlProportional=MODO_SW3_BASIC;
	tmp.RelativeAltitude=0;
	tmp.modelo_ruta=0;
	tmp.line_telemetry=((tmp.Video_PAL)?20:16);	// 20 PAL, 16 NTSC
	tmp.offset_sensorI=600.0f;			// mV
	tmp.gain_sensorI=60.0f;				// mV/A
	tmp.min_rssi=0;
	tmp.max_rssi=3.3f;
	tmp.Modo_PPM=0;
	tmp.Canal_PPM=4;
	tmp.TimeZone=0;

	tmp.MetricsImperial = 0;
	tmp.CamSel = 1;


	sprintf(tmp.NombrePiloto,"IkarusOSD_plt");

	// se guarda en flash
	ram2rom((void*)&storedConfig, (void*)&tmp, sizeof(struct StoredConfig));
	flush_rom();

}

void MenuCalibrar() large
{
	code char smenus[5][10+3] ={"Exit", "Sensor IR", "Gain IR", "Servos", "Servo Center"};//, "RSSI", "Sensor I"};
	code const char MAX_idx=5; 
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			wait_for(50);
			if(next_pressed())
			{
				sel_idx=(sel_idx+1)%MAX_idx;
			}
			printCenteredAtStr(1, "Calibrar Ikarus");


			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,3,CH_ROWR);
				else
					printAtChr(3+i,3,' ');
				printAtStr(3+i,5,smenus[i]);
			}
		}while(!enter_pressed());
		
		switch(sel_idx)
		{
			case 0: 
				break;
			
			case 1:
				CalibrarCopilot();
				break;

			case 2:
				CalibrarGananciaCopilot();
				break;

			case 3:
				CalibrarServos();
				break;
	
			case 4:
				CentrarServos();
				break;

//			case 3:
//				CalibrarRSSI();
//				break;
				
//			case 4:
//				CalibrarSensorI();
//				break;
		}
	} while(sel_idx!=0);
}

void WaitForGPSscreen(char interactive) large
{
	code const char MAX_idx=4; 
	long timeout;
	int len;

	char sel_idx=1;
	char cad[30];
	char i;
	
	bit is_ready2fly=0;
	bit movetailwhenpos=1;

	do
	{
		timeout=tics;
		wait_for(50);
		ClrScr();
		do
		{
			wait_for(50);
			USB_Connection();
			if(next_pressed())
			{
				sel_idx=(sel_idx+1)%MAX_idx;

				if(sel_idx==0&&!is_ready2fly)
					sel_idx=1;
			}

			printCenteredAtStr(1, "Welcome to Ikarus OSD");

			sprintf(cad,"Piloto: %s",storedConfig.NombrePiloto);
			printAtStr(3,3,cad);

			
			if(autopilotCfg.IR_Z_enabled==IR_XY)
			{
				sprintf(cad,"IR X: %4.2f  Y: %4.2f  ", adc_values[ADC_IR_X], adc_values[ADC_IR_Y]);
				printAtStr(5,3,cad);
			}
			else if(autopilotCfg.IR_Z_enabled==IR_XYZ)
			{
				len = sprintf(cad,"IR X:%4.2f Y:%4.2f ", adc_values[ADC_IR_X], adc_values[ADC_IR_Y]);
				printAtStr(5,3,cad);
				sprintf(cad, "Z:%4.2f ", adc_values[ADC_IR_Z]);
				printAtStr(5,3+len,cad);
			}
			else //if(autopilotCfg.IR_Z_enabled==IR_NONE)
			{
				printAtStr(5,3,"NO IR");
			}
			
			if(!gpsinfo.conected)
			{
				printAtStr(7,3,"GPS broken           ");
				is_ready2fly=0;
				timeout=tics;
			}
			else if(!gpsinfo.nmea_ok)
			{
				if((timeout+150)<tics)
				{
					char baud=GetSelectedBauds();
					baud=(baud+1)%7;
					SelectBauds(baud);
					timeout=tics;
				}
				sprintf(cad,"GPS bad nmea (%s)     ",GetBaudStr(GetSelectedBauds()));
				
				printAtStr(7,3,cad);
				is_ready2fly=0;
			}
			else
			{
				timeout=tics;
				sprintf(cad,"GPS Sats (%s): %d (%.2f)     ",GetBaudStr(GetSelectedBauds()),gpsinfo.numsats, gpsinfo.hdop);
				printAtStr(7,3,cad);
				if(gpsinfo.numsats>4) // &&gpsinfo.hdop < xx)
				{
					is_ready2fly=1;
				}
				else
					is_ready2fly=0;
			}

			if(interactive)
			{
				if(is_ready2fly)
					printAtStr(10,3,"Ready to FLY!");
				else
					printAtStr(10,3,"             ");

				
				if(!CheckFailSafes())
					printAtStr(8,3,"TX OK ");
				else
					printAtStr(8,3,"TX OFF");

				printAtStr(11,3,"Configurar");
				printAtStr(12,3,"Calibrar IR/otros");
				printAtStr(13,3,"Exit");
			
				for(i=0;i<MAX_idx;i++)
				{
					if(i==sel_idx)
						writeAtChr(10+i,1,CH_ROWR);
					else
						printAtChr(10+i,1,' ');
				}
			}
			else
			{
				CheckFailSafes();
				if(ikarusInfo.failsafe==FSS_NORMAL)
					printCenteredAtStr(9,"UPLINK OK");
				else
					printCenteredAtStr(9,"NO UPLINK");


			}

			if(is_ready2fly&&movetailwhenpos)
			{
				if(autopilotCfg.chAux_mode==AUX_AIL2)
					ikarusInfo.servos_bitmasks = 0x29;	//101001
				else
					ikarusInfo.servos_bitmasks = 0x09;	//1001
				ikarusInfo.servoDriver = SERVO_BITMASKS;
				for(i=0;i<2;i++)
				{
					set_servof(TAIL, 1.0f);
					set_servof(AIL, 1.0f);
					if(autopilotCfg.chAux_mode==AUX_AIL2)
						set_servof(AUX, -1.0f);
					
					//servos_out[TAIL]=1700;
					wait_for(20);
					set_servof(TAIL, -1.0f);
					set_servof(AIL, -1.0f);
					if(autopilotCfg.chAux_mode==AUX_AIL2)
						set_servof(AUX, 1.0f);
					
					//servos_out[TAIL]=1300;
					wait_for(20);
				}
				ikarusInfo.servos_bitmasks = 0x00;
				ikarusInfo.servoDriver = SERVO_COPY;

				movetailwhenpos=0;
			}
	
		}while((interactive&&!enter_pressed())||(!interactive&&!is_ready2fly&&ikarusInfo.failsafe!=FSS_NORMAL));
		
		if(!interactive&&is_ready2fly)
			sel_idx=0;

		switch(sel_idx)
		{
			case 0: 
				ClrScr();
				printCenteredAtStr(2, "Saving home...");
				// Guardar vel bauds
				SaveBauds(GetSelectedBauds());
				// Guardar home
				FixHome();
				wait_for(200);
				break;
			
			case 1:
				IkarusOsdconfig();
				break;

			case 2:
				MenuCalibrar();
				break;
				
			case 3:
				ClrScr();
				printCenteredAtStr(2, "Using stored home...");
				wait_for(100);
				break;
	
		}
	} while(sel_idx!=0&&sel_idx!=3);
}

void IkarusOsdconfig() large
{
	xdata struct StoredConfig tmp;
//	xdata struct AutoPilotConfig apcfg;
	
	rom2ram((void*)&tmp,(void*)&storedConfig, sizeof(struct StoredConfig));
	//rom2ram((void*)&apcfg,(void*)&autopilotCfg, sizeof(struct AutoPilotConfig));

	MenuConfig(&tmp/*, &apcfg*/);

	ram2rom((void*)&storedConfig, (void*)&tmp, sizeof(struct StoredConfig));
	//ram2rom((void*)&autopilotCfg, (void*)&apcfg, sizeof(struct AutoPilotConfig));
	flush_rom();
}

void FixHome() large
{
	ram2rom((void*)&storedConfig.HomeLat, (void*)&gpsinfo.lat, sizeof(float));
	ram2rom((void*)&storedConfig.HomeLon, (void*)&gpsinfo.lon, sizeof(float));
	ram2rom((void*)&storedConfig.HomeAltitude, (void*)&gpsinfo.alt, sizeof(float));
	flush_rom();
}

void SaveBauds(char bauds) large
{
	ram2rom((void*)&storedConfig.BaudRate, (void*)&bauds, sizeof(char));
	//flush_rom();
}

void MenuConfig(struct StoredConfig *cfg/*, struct AutoPilotConfig *apcfg*/) large
{
	code const char MAX_idx=4; 
	code char smenus[4][16] ={"Exit", /*"OSD Config", "Rx Config",*/ "Battery", "Alarm"/*,"Ikarus","Autopilot"*/, "Update firmware"};
	char sel_idx=0;
	char i;
									
	do
	{
		wait_for(50);
		ClrScr();
		while(!enter_pressed())
		{	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			
			printCenteredAtStr(1, "Ikarus OSD config");
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,3,CH_ROWR);
				else
					printAtChr(3+i,3,' ');
				printAtStr(3+i,5,smenus[i]);
			}
		}
		switch(sel_idx)
		{
			case 0: 
				break;
//			case 1:
//				MenuMAX7456_config(cfg); 
//				break;
//			case 2:
//				MenuRX_config(cfg); 
//				break;
			case 1:
				MenuBattery_config(cfg); 
				break;
			case 2:
				MenuAlarms_config(cfg); 
				break;
//			case 4:
//				MenuIkarus_config(cfg); 
//				break;
//			case 6:
//				MenuAutopilot_config(apcfg);
//				break;
			case 3:
				MenuReflash();
				break;
		}
	} while(sel_idx!=0);
}

void MenuReflash() large
{
#ifdef	SECURITY_FW
	ClrScr();
	printAtStr(3,0, "Reflash 115200 bps");
	printAtStr(5,0, "-No interrumpa el proceso");
	printAtStr(6,0, "-Utilice una bateria cargada");
	printAtStr(7,0, "-No toque los cables");



	DesactivarServos();			// Desactivamos los servos

	AutoUpdateSerie();
#else
	ClrScr();
	printAtStr(3,2, "Developement version");
	printAtStr(5,2, "No upgradeable!");
	while(1);
//	AutoUpdateSerie();
#endif
}

/*
void MenuRX_config(struct StoredConfig *cfg) large
{
	code const char str_modo[2][_COLS]={"NORMAL", "PPM   "};
	code const char str_canal[7][_COLS]={"5 ", "6 ", "7 ", "8 ", "9 ","10","11"};

	code const char MAX_idx=5; 
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"RX Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1:
						sprintf(cad,"Modo RX:        %s",str_modo[cfg->Modo_PPM]);
						break;

					case 2:
						sprintf(cad,"Canal:          %s",str_canal[cfg->Canal_PPM-4]);
						break;

					case 3:
						sprintf(cad,"RSSI Vmin:      %.3f",cfg->min_rssi);
						break;

					case 4:
						sprintf(cad,"RSSI Vmax:      %.3f",cfg->max_rssi);
						break;


				}
				printAtStr(3+i,2,cad);
			}
			//Ayuda
			//sprintf(cad, "Sensor RSSI= %.0fmV   ",ikarusInfo.mv_currI);
			//printAtStr(-1,2,cad);
				
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;
			
			case 1: 
				cfg->Modo_PPM=Selection(3+sel_idx,18,cfg->Modo_PPM,2,str_modo);
				break;

			case 2: 
				cfg->Canal_PPM=4+Selection(3+sel_idx,18,cfg->Canal_PPM-4,7,str_canal);
				break;
		
			case 3: 
				cfg->min_rssi=NumericUpDown(3+sel_idx,18,(float)cfg->min_rssi,0.0f, 3.3f,0.01f, 20, "%.3f");
				break;
			case 4: 
				cfg->max_rssi=NumericUpDown(3+sel_idx,18,(float)cfg->max_rssi,0.0f, 3.3f,0.01f, 20, "%.3f");
				break;


		}
	} while(sel_idx!=0);
}

void MenuMAX7456_config(struct StoredConfig *cfg) large
{
	code const char MAX_idx=5; 
	code const char str_video[2][_COLS]={"NTSC", "PAL "};
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"OSD Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"Video System:     %s",str_video[cfg->Video_PAL]);
						break;

					case 2: 
						sprintf(cad,"Offset X:         %d",(int)cfg->offsetX);
						break;

					case 3: 
						sprintf(cad,"Offset Y:         %d",(int)cfg->offsetY);
						break;

					case 4: 
						sprintf(cad,"Telemetry #:      %d",(int)cfg->line_telemetry);
						break;

				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				cfg->Video_PAL=Selection(3+sel_idx,20,cfg->Video_PAL,2,str_video);
				break;

			case 2: 
				cfg->offsetX=(char)NumericUpDown(3+sel_idx,20,(float)cfg->offsetX,0.0f, 63.0f, 1.0f, 50, "%.0f");
				break;
			case 3: 
				cfg->offsetY=(char)NumericUpDown(3+sel_idx,20,(float)cfg->offsetY,0.0f, 31.0f, 1.0f, 50, "%.0f");
				break;
			case 4: 
				cfg->line_telemetry=(char)NumericUpDown(3+sel_idx,20,(float)cfg->line_telemetry,5.0f, 55.0f, 1.0f, 50, "%.0f");
				break;
		}
	} while(sel_idx!=0);
}
*/

void MenuBattery_config(struct StoredConfig *cfg) large
{
	code const char MAX_idx=4; 
	code const char str_cells[5][_COLS]={"2", "3", "4", "5", "6"};
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"Battery Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"Motor Cells (B1): %d",(int)cfg->cellsBatt1);
						break;

					case 2: 
						sprintf(cad,"Video Cells (B2): %d",(int)cfg->cellsBatt2);
						break;

					case 3: 
						sprintf(cad,"Capacity mAh(B1): %.0f",cfg->total_mAh);
						break;

//					case 4:
//						sprintf(cad,"Sensor I Offset(mV): %d",(int)cfg->offset_sensorI);
//						break;

//					case 5:
//						sprintf(cad,"Sensor I Gain(mV/A): %d",(int)cfg->gain_sensorI);
//						break;


				}
				printAtStr(3+i,2,cad);
			}
			// Ayuda
			//sprintf(cad, "Sensor I= %.0fmV    (%.2f A)",ikarusInfo.mv_currI, 
			//	(ikarusInfo.mv_currI-cfg->offset_sensorI)/cfg->gain_sensorI);
			//printAtStr(-1,2,cad);
				
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				cfg->cellsBatt1=(char)(Selection(3+sel_idx,20,cfg->cellsBatt1-2,5,str_cells)+2);
				break;

			case 2: 
				cfg->cellsBatt2=(char)(Selection(3+sel_idx,20,cfg->cellsBatt2-2,2,str_cells)+2);
				break;
			case 3: 
				cfg->total_mAh=NumericUpDown(3+sel_idx,20,(float)cfg->total_mAh,800.0f, 10000.0f,50.0f, 20, "%.0f ");
				break;

//			case 4: 
//				cfg->offset_sensorI=NumericUpDown(3+sel_idx,23,(float)cfg->offset_sensorI,0.0f, 3000.0f,5.0f, 20, "%.0f ");
//				break;

//			case 5: 
//				cfg->gain_sensorI=NumericUpDown(3+sel_idx,23,(float)cfg->gain_sensorI,0.0f, 1000.0f,1.0f, 20, "%.0f ");
//				break;


		}
	} while(sel_idx!=0);
}

void MenuAlarms_config(struct StoredConfig *cfg) large
{
	code const char MAX_idx=5; 
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"Alarms Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"Cell Volt:        %.2f",cfg->cellAlarm);
						break;

					case 2: 
						sprintf(cad,"Distance:         %.0f",cfg->distanceAlarm);
						break;

					case 3: 
						sprintf(cad,"Low Altitude:     %.0f",cfg->altitudeAlarm);
						break;

					case 4: 
						sprintf(cad,"Low Speed:        %.0f",cfg->lowSpeedAlarm);
						break;

				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				cfg->cellAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->cellAlarm,2.8f, 4.2f, 0.1f,50, "%.2f");
				break;
			case 2: 
				cfg->distanceAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->distanceAlarm,0.0f, 10000.0f, 100.0f,20, "%.0f");
				break;
			case 3: 
				cfg->altitudeAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->altitudeAlarm,0.0f, 10000.0f,100.0f, 20, "%.0f");
				break;
			case 4: 
				cfg->lowSpeedAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->lowSpeedAlarm,10.0f, 200.0f, 1.0f, 20, "%.0f");
				break;
		}
	} while(sel_idx!=0);
}
/*
void MenuIkarus_config(struct StoredConfig *cfg) large
{
	code const char MAX_idx=9; 
	code const char str_telemetry[3][_COLS]={"DISABLE","VIDEO  ","XBEE   "};
	code const char str_bauds[6][_COLS]={"4800  ","9600  ", "14400 ", "28800 ", "38400 ", "57600 "};
	code const char str_osd[4][_COLS]={"TX SELEC","TEXTO   ","GRAPHICS", "FIGHTER "};
	code const char str_canal[7][_COLS]={"SWITCH 2 ", "SWITCH 3 ", "RUEDA    ","MEZCLA223", "AJUSTE223","MEZCLA224","UPLINK   "};
	code const char str_altitude[2][_COLS]={"ABSOLUTE","RELATIVE"};
	code const char str_finruta[4][_COLS]={"IR CASA", "REPETIR","INVERTIR","INV+REP"};

	
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"Ikarus Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"GPS baudrate:   %s",str_bauds[cfg->BaudRate]);
						break;

					case 2: 
						sprintf(cad,"OSD screen:     %s",str_osd[cfg->DefaultHUD]);
						break;

					case 3: 
						sprintf(cad,"Telemetry:      %s",str_telemetry[cfg->TelemetryMode]);
						break;

					case 4:
						sprintf(cad,"Canal control:  %s",str_canal[cfg->ControlProportional]);
						break;

					case 5:
						sprintf(cad,"WayPoint range: %.0f",cfg->wptRange);
						break;

					case 6:
						sprintf(cad,"Rel. Altitude:  %s",str_altitude[cfg->RelativeAltitude]);
						break;

					case 7:
						sprintf(cad,"Ruta ends:      %s",str_finruta[cfg->modelo_ruta]);
						break;
					
					case 8:
						sprintf(cad,"Zona Horaria:   %.0f",(float)cfg->TimeZone);
						break;
				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				cfg->BaudRate=(char)Selection(3+sel_idx,18,cfg->BaudRate,5,str_bauds);
				break;
			case 2: 
				cfg->DefaultHUD=(char)Selection(3+sel_idx,18,cfg->DefaultHUD,4,str_osd);
				break;
			case 3: 
				cfg->TelemetryMode=(char)Selection(3+sel_idx,18,cfg->TelemetryMode,3,str_telemetry);
				break;
			case 4: 
				cfg->ControlProportional=(char)Selection(3+sel_idx,18,cfg->ControlProportional,7,str_canal);
				break;

			case 5: 
				cfg->wptRange=NumericUpDown(3+sel_idx,18,(float)cfg->wptRange,3.0f, 200.0f,1.0f, 20, "%.0f");
				break;

			case 6: 
				cfg->RelativeAltitude=(char)Selection(3+sel_idx,18,cfg->RelativeAltitude,2,str_altitude);
				break;
			
			case 7: 
				cfg->modelo_ruta=(char)Selection(3+sel_idx,18,cfg->modelo_ruta,4,str_finruta);
				break;

			case 8: 
				cfg->TimeZone=(char)NumericUpDown(3+sel_idx,18,(float)cfg->TimeZone, -12.0f, 12.0f, 1.0f, 50,"%.0f  ");
				break;


		}
	} while(sel_idx!=0);
}

void MenuAutopilot_config(struct AutoPilotConfig *apcfg) large
{
	code const char MAX_idx=11; 
	code const char str_apmode[5][_COLS]=
		{	
		"DISABLE   ",
		"FIXED ALT ",
		"DIST ALT  ",
		"WPT (SAFE)",
		"WPT-UNSAFE"
		};
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"Autopilot Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"Mode:    %s",str_apmode[apcfg->AutopilotMode]);
						break;

					case 2: 
						sprintf(cad,"Cruise Alt.:    %.0f",apcfg->baseCruiseAltitude);
						break;

					case 3: 
						sprintf(cad,"Distance Alt.:  %.0f",apcfg->distanceAltitude);
						break;

					case 4: 
						sprintf(cad,"Sensor IR");
						break;

					case 5: 
						sprintf(cad,"Canales PPM");
						break;
								
					case 6: 
						sprintf(cad,"Servos config");
						break;
			
					case 7: 
						sprintf(cad,"Pitch PID Gains");
						break;

					case 8: 
						sprintf(cad,"Roll PID Gains");
						break;

					case 9: 
						sprintf(cad,"Motor PID Gains");
						break;

					case 10: 
						sprintf(cad,"Tail PID Gains");
						break;

				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				apcfg->AutopilotMode=Selection(3+sel_idx,11,apcfg->AutopilotMode,5,str_apmode);
				break;

			case 2: 
				apcfg->baseCruiseAltitude=NumericUpDown(3+sel_idx,18,apcfg->baseCruiseAltitude,0.0f, 99999.0f, 100.0f, 20, "%.0f");
				break;

			case 3: 
				apcfg->distanceAltitude=NumericUpDown(3+sel_idx,18,apcfg->distanceAltitude,0.0f, 1000.0f, 10.0f, 20, "%.0f");
				break;

			case 4: 
				MenuIR_config(apcfg);
				break;

			case 5: 
				MenuPPM_config(apcfg);
				break;

			case 6: 
				MenuServos_config((struct ServoInfo *)&apcfg->servos_cfg);
				break;

			case 7: 
				MenuAutopilotGains_config((struct PID_Setup *)&apcfg->pid_pitch, "Pitch PID Gains");
				break;

			case 8: 
				MenuAutopilotGains_config((struct PID_Setup *)&apcfg->pid_roll, "Roll PID Gains");
				break;

			case 9: 
				MenuAutopilotGains_config((struct PID_Setup *)&apcfg->pid_motor, "Motor PID Gains");
				break;

			case 10: 
				MenuAutopilotGains_config((struct PID_Setup *)&apcfg->pid_tail, "Tail PID Gains");
				break;
		}
	} while(sel_idx!=0);
}
*/
int Selection(int fila, int col, char curr, char max, char cad[][_COLS]) large
{
	char val=curr;

	if(curr<0)
		curr=0;
	else if(curr>max)
		curr=max;

	wait_for(50);
	do
	{

		if(next_pressed())
			val=(val+1)%max;
		
		CharAttrBlink();
		printAtStr(fila,col,cad[val]);
		CharAttrNoBlink();
			
		
	}while(!enter_pressed()); 
	return val;
}

float NumericUpDown(int fila, int col, float curr, float min, float max, float inc, char delay, char *fmt) large
{
	bit up=0;
	char cad[20];
	float val=curr;

	wait_for(50);
	do
	{
		if(next_down())
		{
			if(next_pressed())
				up=!up;
	
			if(up)
				val+=inc;
			else
				val-=inc;
			if(val>max)
				val=max;
			else if(val<min)
				val=min;		
		}
		sprintf(cad,fmt,val);

		CharAttrBlink();
		printAtStr(fila,col,cad);
		CharAttrNoBlink();
			
		wait_for(delay);
		
	}while(!enter_pressed());
	return val;
}

/*
void TestHardware()
{
	xdata char cad[50];
	xdata char i,j;	
	ClrScr();
	printCenteredAtStr(0,"Welcome Ikarus");
	printAtStr(2,0,"Autoconfig...");

	printAtStr(4,0,"Battery 1 (Motor)");
	sprintf(cad,"%3.1fv (%dS)",ikarusInfo.v1,NumOfCells(ikarusInfo.v1));
	printAtStr(4,18,cad);

	printAtStr(5,0,"Battery 2 (Video)");
	sprintf(cad,"%3.1fv (%dS)",ikarusInfo.v2,NumOfCells(ikarusInfo.v2));
	printAtStr(5,18,cad);

	printAtStr(7,0,"Detecting GPS...");

	j=0;
	do
	{
		i=gpsinfo.conected;
		if(i)
			printAtStr(7,18,"OK");
		else
			printAtStr(7,18,"NO");

		wait_for(100);
		j++;
	} while(i==0&&j<20);
	
	if(i!=0)
	{
		printAtStr(8,0,"Autodetect bps...");
		do
		{
			for(i=0;i<5;i++)
			{
				CharAttrBlink();
				printAtStr(8,18,"        ");
				printAtStr(8,18, SelectBauds(i));
				gpsinfo.nmea_ok=0;
				wait_for(100);
				if(gpsinfo.nmea_ok)
					break;
			} 
		} while (i==5);

		CharAttrNoBlink();

		printAtStr(8,0,"GPS bps set to:");
		printAtStr(8,18, GetBaudStr(i));

	}
	else
	{
		i=2;
		SelectBauds(i);
		CharAttrNoBlink();

		printAtStr(8,0,"GPS lost. Set to:");
		printAtStr(8,18, GetBaudStr(i));

	
	}

	j=0;
	printAtStr(10,0,"Autodetect video: ");
	do
	{
		i=AutoDetectNTSC_PAL();
		if(i==0)
			printAtStr(10,18,"NONE");
		else if(i&0x01)	// PAL
			printAtStr(10,18,"PAL ");
		else if(i&0x02)	// NTSC
			printAtStr(10,18,"NTSC");
		wait_for(50);
		j++;

	} while(i==0&&j<20);

	if(i==0)
	{
		printAtStr(11,0,"No video detected");
	}
	printAtStr(12,0, "TX Transmiter...");
	rflost=100;
	wait_for(100);
	if(rflost==100)
		printAtStr(12,18, "LOST");

	j=0;
	while(rflost==100&&j<20)
	{	
		wait_for(50);
		j++;
	}

	if(rflost==100)
		printAtStr(12,18,"FAIL");
	else
		printAtStr(12,18, "OK  ");
	
	wait_for(200);
}
*/

/*
void ConfigurarCopilotIR() large
{
	const char LVL=3;
	char cad[50];
	float center_x, center_y, pitch_x, pitch_y, roll_x,roll_y;
	
	char ir_cross, ir_cross_rev, ir_x_rev, ir_y_rev;

	ClrScr();
	printCenteredAtStr(1, "Configurando IR");

	printCenteredAtStr(3, "Libere IR (OK)");
	while(!enter_pressed())
	{
		center_x=adc_values[ADC_IR_X];
		center_y=adc_values[ADC_IR_Y];
		sprintf(cad,"X: %.4f Y:%.4f",center_x, center_y);
		printAtStr(4,3,cad);
	}
	
	printCenteredAtStr(6, "Tapar frontal (OK)");
	while(!enter_pressed())
	{
		pitch_x=adc_values[ADC_IR_X]-center_x;
		pitch_y=adc_values[ADC_IR_Y]-center_y;
		sprintf(cad,"X: %.4f Y:%.4f",pitch_x, pitch_y);
		printAtStr(7,3,cad);
	}

	printCenteredAtStr(3, "Tapar derecho (OK)");
	while(!enter_pressed())
	{
		roll_x=adc_values[ADC_IR_X]-center_x;
		roll_y=adc_values[ADC_IR_Y]-center_y;
		sprintf(cad,"X: %.4f Y:%.4f",roll_x, roll_y);
		printAtStr(4,3,cad);
	}

	if(abs(pitch_x)>LVL*abs(pitch_y)&&abs(roll_y)>LVL*abs(roll_x))
	{	// Pitch en X
		ir_cross=0;
		ir_cross_rev=0;
		ir_x_rev=(pitch_x>0)?1:0;
		ir_y_rev=(pitch_y>0)?1:0;
	}
	else if(LVL*abs(pitch_x)<abs(pitch_y)&&LVL*abs(roll_y)<abs(roll_x))
	{	// Pitch en Y
		ir_cross=0;
		ir_cross_rev=1;
		ir_x_rev=(pitch_x>0)?1:0;
		ir_y_rev=(pitch_y>0)?1:0;
	}
	else if(pitch_x>0&&pitch_y<0)
	{	// Cruz normal
		ir_cross=1;
		ir_cross_rev=0;
		ir_x_rev=(pitch_x>0)?1:0;
		ir_y_rev=(pitch_y>0)?1:0;
	}
	else
	{
		ir_cross=1;
		ir_cross_rev=1;
		ir_x_rev=(pitch_x>0)?1:0;
		ir_y_rev=(pitch_y>0)?1:0;
	}

	ram2rom((void*)&autopilotCfg.IR_crossed, (void*)&ir_cross, sizeof(char));
	ram2rom((void*)&autopilotCfg.IR_reverse_cross, (void*)&ir_cross_rev, sizeof(char));
	ram2rom((void*)&autopilotCfg.IR_reverse_pitch, (void*)&ir_x_rev, sizeof(char));
	ram2rom((void*)&autopilotCfg.IR_reverse_roll, (void*)&ir_y_rev, sizeof(char));
	flush_rom();
}

void MenuPPM_config(struct AutoPilotConfig *apcfg) large
{
	code const char str_mezcla[3][_COLS]={"NORMAL", "ELEVON", "V-TAIL"};
	code const char str_canalC[6][_COLS]={"1 ", "2 ", "3 ", "4 ", "5 ","6 "};
	code const char str_canalP[6][_COLS]={"5 ", "6 ", "7 ", "8 ", "9 ","10"};
	code const char str_canalT[6][_COLS]={"6 ", "7 ", "8 ", "9 ", "10","11"};


	code const char MAX_idx=9; 
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"PPM Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1:
						sprintf(cad,"Ailerons:       %s",str_canalC[apcfg->ch_ail]);
						break;

					case 2:
						sprintf(cad,"Elevator:       %s",str_canalC[apcfg->ch_ele]);
						break;

					case 3:
						sprintf(cad,"Thrust:         %s",str_canalC[apcfg->ch_thr]);
						break;

					case 4:
						sprintf(cad,"Tail:           %s",str_canalC[apcfg->ch_tail]);
						break;

					case 5:
						sprintf(cad,"Pan:            %s",str_canalP[apcfg->ch_pan-4]);
						break;

					case 6:
						sprintf(cad,"Tilt:           %s",str_canalT[apcfg->ch_tilt-5]);
						break;

					case 7:
						sprintf(cad,"Mezcla:         %s",str_mezcla[apcfg->tipo_mezcla]);
						break;

					case 8:
						sprintf(cad,"Pan Gain:       %3.1f",apcfg->pan_gain);
						break;


				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;
			
			case 1: 	
				apcfg->ch_ail=Selection(3+sel_idx,18,apcfg->ch_ail,6,str_canalC);
				break;

			case 2: 	
				apcfg->ch_ele=Selection(3+sel_idx,18,apcfg->ch_ele,6,str_canalC);
				break;

			case 3: 	
				apcfg->ch_thr=Selection(3+sel_idx,18,apcfg->ch_thr,6,str_canalC);
				break;

			case 4: 	
				apcfg->ch_tail=Selection(3+sel_idx,18,apcfg->ch_tail,6,str_canalC);
				break;

			case 5: 	
				apcfg->ch_pan=Selection(3+sel_idx,18,apcfg->ch_pan-4,6,str_canalP)+4;
				break;

			case 6: 	
				apcfg->ch_tilt=Selection(3+sel_idx,18,apcfg->ch_tilt-5,6,str_canalT)+5;
				break;

			case 7:
				apcfg->tipo_mezcla=Selection(3+sel_idx,18,apcfg->tipo_mezcla,3,str_mezcla);
				break;
		
			case 8: 
				apcfg->pan_gain=NumericUpDown(3+sel_idx,18,(float)apcfg->pan_gain,1.0f, 3.0f,0.1f, 20, "%3.1f");
				break;
		}
	} while(sel_idx!=0);
}


void MenuIR_config(struct AutoPilotConfig *apcfg) large
{
	code const char str_nosi[2][_COLS]={"NO", "SI"};
	code const char str_sensor[3][_COLS]={"IR  ", "IR+Z","IMU "};

	code const char MAX_idx=9; 
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"IR Sensor Config");
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1:
						sprintf(cad,"Pitch REV:      %s",str_nosi[apcfg->IR_reverse_pitch]);
						break;

					case 2:
						sprintf(cad,"Roll REV:       %s",str_nosi[apcfg->IR_reverse_roll]);
						break;

					case 3:
						sprintf(cad,"Cross Sensor:   %s",str_nosi[apcfg->IR_crossed]);
						break;

					case 4:
						sprintf(cad,"Cross REV:      %s",str_nosi[apcfg->IR_reverse_cross]);
						break;

					case 5:
						sprintf(cad,"Sensor:         %s",str_sensor[apcfg->IR_Z_enabled]);
						break;

					case 6:
						sprintf(cad,"X Off:          %4.2f",apcfg->x_off);
						break;

					case 7:
						sprintf(cad,"Y Off:          %4.2f",apcfg->y_off);
						break;

					case 8:
						if(apcfg->IR_Z_enabled)
							sprintf(cad,"Z Off:          %4.2f",apcfg->z_off);
						else
							sprintf(cad,"IR_MAX:         %4.2f",apcfg->IR_max);
					
					
						break;
				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;
			
			case 1:
				apcfg->IR_reverse_pitch=Selection(3+sel_idx,18,apcfg->IR_reverse_pitch,2,str_nosi);
				break;

			case 2: 	
				apcfg->IR_reverse_roll=Selection(3+sel_idx,18,apcfg->IR_reverse_roll,2,str_nosi);
				break;

			case 3: 	
				apcfg->IR_crossed=Selection(3+sel_idx,18,apcfg->IR_crossed,2,str_nosi);
				break;

			case 4: 	
				apcfg->IR_reverse_cross=Selection(3+sel_idx,18,apcfg->IR_reverse_cross,2,str_nosi);
				break;

			case 5: 	
				apcfg->IR_Z_enabled=Selection(3+sel_idx,18,apcfg->IR_Z_enabled,3,str_sensor);
				break;

			case 6: 	
				apcfg->x_off=NumericUpDown(3+sel_idx,18,(float)apcfg->x_off,1.0f, 2.0f,0.01f, 20, "%4.2f");
				break;

			case 7:
				apcfg->y_off=NumericUpDown(3+sel_idx,18,(float)apcfg->y_off,1.0f, 2.0f,0.01f, 20, "%4.2f");
				break;
		
			case 8: 
				if(apcfg->IR_Z_enabled)
					apcfg->z_off=NumericUpDown(3+sel_idx,18,(float)apcfg->z_off,1.0f, 2.0f,0.01f, 20, "%4.2f");
				else
					apcfg->IR_max=NumericUpDown(3+sel_idx,18,(float)apcfg->IR_max,1.0f, 2.0f,0.01f, 20, "%4.2f");

				break;
		}
	} while(sel_idx!=0);
}

void MenuAutopilotGains_config(struct PID_Setup *pidsetup, char title[]) large
{
	code const char str_nosi[2][_COLS]={"NO", "SI"};
	code const char MAX_idx=7; 
	char cad[50];
	char sel_idx=0;
	char i;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,title);
	
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,1,CH_ROWR);
				else
					printAtChr(3+i,1,' ');

				switch(i)
				{
					case 0:
						sprintf(cad,"Exit");
						break;

					case 1: 
						sprintf(cad,"P Gain:     %.6f",pidsetup->P);
						break;

					case 2: 
						sprintf(cad,"I Gain:     %.6f",pidsetup->I);
						break;
			
					case 3: 
						sprintf(cad,"D Gain:     %.6f",pidsetup->D);
						break;

					case 4: 
						sprintf(cad,"I Limit:    %.6f",pidsetup->IL);
						break;

					case 5: 
						sprintf(cad,"Drive Lim:  %.6f",pidsetup->DL);
						break;

					case 6: 
						sprintf(cad,"Reverse:    %s",str_nosi[pidsetup->rev]);
						break;


				}
				printAtStr(3+i,2,cad);
			}
		}while(!enter_pressed());
	
		switch(sel_idx)
		{
			case 0: //return;
				break;

			case 1: 
				pidsetup->P=NumericMulDiv(3+sel_idx,14,pidsetup->P,0.0f, 1.0f, 1.1f, 20, "%.6f");
				break;

			case 2: 
				pidsetup->I=NumericMulDiv(3+sel_idx,14,pidsetup->I,0.0f, 1.0f, 1.1f, 20, "%.6f");
				break;

			case 3: 
				pidsetup->D=NumericMulDiv(3+sel_idx,14,pidsetup->D,0.0f, 1.0f, 1.1f, 20, "%.6f");
				break;

			case 4: 
				pidsetup->IL=NumericMulDiv(3+sel_idx,14,pidsetup->IL,0.0f, 1.0f, 1.1f, 20, "%.6f");
				break;

			case 5: 
				pidsetup->DL=NumericMulDiv(3+sel_idx,14,pidsetup->DL,0.0f, 1.0f, 1.1f, 20, "%.6f");
				break;
			
			case 6: 
				pidsetup->rev=Selection(3+sel_idx,18,pidsetup->rev,2,str_nosi);
		
		}
	} while(sel_idx!=0);
}

void MenuServos_config(struct ServoInfo *pscfg) large
{
	code const char str_reverse[2][_COLS]={"NOR","REV"};
	code const char str_servo[5][6]={"CTRL:","AIL: ","ELE: ","TRH:","TAIL:"};

	code const char MAX_idx=21; 
	char cad[50];
	char sel_idx=0;
	unsigned char i=0,j=0;

	do
	{
		wait_for(50);
		ClrScr();
		do
		{
			printCenteredAtStr(1,"Servo Config");
	
			printAtStr(3,2,"Exit");
			for(i=0;i<5;i++)
			{
				printAtStr(4+i,1,str_servo[i]);
				sprintf(cad,"%4.d",pscfg[i].min);
				printAtStr(4+i,8,cad);
				sprintf(cad,"%4.d",pscfg[i].center);
				printAtStr(4+i,13,cad);
				sprintf(cad,"%4.d",pscfg[i].max);
				printAtStr(4+i,18,cad);
				printAtStr(4+i,23,str_reverse[pscfg[i].reverse]);
			}
			
			if(next_pressed())
			{
				if(sel_idx==0)
					printAtChr(3,1,' ');
				else
				{
					i=sel_idx-1;
					j=(i%4)*5+7;
					i=i/4+4;
					printAtChr(i,j,' ');
				}
				sel_idx=(sel_idx+1)%MAX_idx;
			
			}
			if(sel_idx==0)
				writeAtChr(3,1,CH_ROWR);
			else
			{
				i=sel_idx-1;
				j=(i%4)*5+7;
				i=i/4+4;
				writeAtChr(i,j,CH_ROWR);
			}
			wait_for(20);				
		
		}while(!enter_pressed());
	
		if(sel_idx!=0)
		{
			i=sel_idx-1;
			j=i%4;
			i=i/4;
			switch(j)
			{
				case 0:
					pscfg[i].min=NumericUpDown(i+4,j*5+8,pscfg[i].min,500.0f, 2500.0f, 5.0f, 1, "%4.0f");
					break;
				case 1:
					pscfg[i].center=NumericUpDown(i+4,j*5+8,pscfg[i].center,500.0f, 2500.0f, 5.0f, 1, "%4.0f");
					break;
				case 2:
					pscfg[i].max=NumericUpDown(i+4,j*5+8,pscfg[i].max,500.0f, 2500.0f, 5.0f, 1, "%4.0f");
					break;
				case 3:
					pscfg[i].reverse=Selection(i+4,j*5+8,pscfg[i].reverse,2,str_reverse);
					break;
			}	
		}

	} while(sel_idx!=0);
}

float NumericMulDiv(int fila, int col, float curr, float min, float max, float inc, char delay, char *fmt) large
{
	bit up=0;
	char cad[20];
	float val=curr;

	wait_for(50);
	do
	{
		if(next_down())
		{
			if(next_pressed())
				up=!up;

			if(up)
				{
				if(val==0.0f) val=0.000001f;
				val=val*inc;
				}
			else
				val=val/inc;
			if(val>max)
				val=max;
			else if(val<min)
				val=min;		
		}
		sprintf(cad,fmt,val);

		
		CharAttrBlink();
		printAtStr(fila,col,cad);
		CharAttrNoBlink();
			
		wait_for(delay);
		
	}while(!enter_pressed()); 
	return val;
}
*/
