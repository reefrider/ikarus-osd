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

#include "LibraryMax7456.h"
#include "ParserNMEA.h"
#include "Navigation.h"
#include "Utils.h"
#include "Servos.h"
#include "huds.h"
#include "Ikarus.h"
#include "MenuConfig.h"
#include "PID.h"

extern struct IkarusInfo ikarusInfo;
extern struct StoredConfig storedConfig;
extern struct AutoPilotConfig autopilotCfg;
extern struct GPSInfo gpsinfo;

extern unsigned char rflost;
extern unsigned long tics;
extern unsigned int servos_in[];
extern float adc_values[];


extern xdata struct PID elevatorPID;
extern xdata struct PID aileronsPID;
extern xdata struct PID tailPID;
extern xdata struct PID motorPID;
extern xdata float modem_ail, modem_ele;


struct StoredConfig tmp_storedConfig;
struct AutoPilotConfig tmp_autopilotCfg;


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

float NumericUpDown(int fila, int col, float valor, float min, float max, float inc, float delay, char *fmt, bool * salir) 
{
	static bool up=0;
	char cad[20];
	float val=valor;

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
			
	*salir=enter_pressed();
	return val;
}

char Selection(int fila, int col, char valor, char max, char cad[][_COLS], bool *result) 
{
	char val=valor;

	if(val<0)
		val=0;
	else if(val>max)
		val=max;

	if(next_pressed())
		val=(val+1)%max;
	
	CharAttrBlink();
	printAtStr(fila,col,cad[val]);
	CharAttrNoBlink();
	
	*result=enter_pressed(); 
	return val;
}


void HUD_Debug() large
{	
	code const char MAX_idx=14; 
	code char cad[][_COLS]={
		"Pitch P   ", "Pitch I   ", "Pitch ILim", 
		"Roll P    ", "Roll I    ", "Roll ILim ", 
		"Tail P    ", "Tail I    ", "Tail ILim ",
		"Motor P   ", "Motor I   ", "Motor ILim",
		"ALT PITCH ", "RUMBO TAIL"};
	
	xdata static char estado = -1;
	xdata static float *pValue;
	xdata static char updown;
	xdata static char addmult;
	xdata static float Inc, Min, Max;

	xdata int i;
	xdata char text[20];

	if(next_pressed()||(estado<0))
	{
		estado = (estado+1)%MAX_idx;
		
		// Por defecto
		addmult=1;	
		Inc = 1.1f;
		Min = 0.0001f;
		Max = 1.0f;

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
				Inc=1.0f;
				Min=0.0f;
				Max=10.0f;
				break;

			case 13:
				pValue = &modem_ail;
				addmult=0;
				Inc=1.0f;
				Min=0.0f;
				Max=10.0f;
				break;
		}
	}

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

	printAtStr(3,2,cad[estado]);
	sprintf(text,"%.4f ",*pValue);
	printAtStr(3,14,text);

}

bool MenuAlarmConfig()
{
	bool resultado=false;
	enum Estado{ST_MENU};
	static int estado = ST_MENU;
	char cad[50];
	const char MAX_idx=5; 
	static char sel_idx=0;
	int i;
	struct StoredConfig * cfg = &tmp_storedConfig;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
			case 1: 
				cfg->cellAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->cellAlarm,2.8f, 4.2f, 0.1f,50, "%.2f ", &resultado);
				break;

			case 2: 
				cfg->distanceAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->distanceAlarm,0.0f, 10000.0f, 10.0f,20, "%.0f ",&resultado);
				break;

			case 3: 
				cfg->altitudeAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->altitudeAlarm,0.0f, 2000.0f,10.0f, 20, "%.0f ",&resultado);
				break;

			case 4: 
				cfg->lowSpeedAlarm=NumericUpDown(3+sel_idx,20,(float)cfg->lowSpeedAlarm,10.0f, 200.0f, 1.0f, 20, "%.0f ",&resultado);
				break;

		default:
			resultado=true;
			break;
	}

	if(resultado)
		estado=ST_MENU;

	return false;
}


bool MenuRX_config()
{
	char str_modo[2][_COLS]={"NORMAL", "PPM   "};
	char str_canal[7][_COLS]={"5 ", "6 ", "7 ", "8 ", "9 ","10","11"};

	bool resultado=false;
	enum Estado{ST_MENU};
	static int estado = ST_MENU;
	char cad[50];
	const char MAX_idx=5; 
	static char sel_idx=0;
	int i;
	struct StoredConfig * cfg = &tmp_storedConfig;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
			
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
						sprintf(cad,"Canal:          %s",str_canal[cfg->Canal_PPM]);
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
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
			
			case 1: 
				cfg->Modo_PPM=Selection(3+sel_idx,18,cfg->Modo_PPM,2,str_modo, &resultado);
				break;

			case 2: 
				cfg->Canal_PPM=Selection(3+sel_idx,18,cfg->Canal_PPM,7,str_canal, &resultado);
				break;
		
			case 3: 
				cfg->min_rssi=NumericUpDown(3+sel_idx,18,(float)cfg->min_rssi,0.0f, 3.3f,0.01f, 20, "%.3f", &resultado);
				break;
			case 4: 
				cfg->max_rssi=NumericUpDown(3+sel_idx,18,(float)cfg->max_rssi,0.0f, 3.3f,0.01f, 20, "%.3f", &resultado);
				break;
	
		default:
			resultado=true;
			break;
	}

	if(resultado)
		estado=ST_MENU;

	return false;
}


bool MenuBattery_config()
{
	bool resultado=false;
	enum Estado{ST_MENU};
	static int estado = ST_MENU;
	char cad[50];
	const char MAX_idx=6; 
	static char sel_idx=0;
	int i;
	struct StoredConfig * cfg = &tmp_storedConfig;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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

					case 4:
						sprintf(cad,"Sensor I Offset(mV): %d",(int)cfg->offset_sensorI);
						break;

					case 5:
						sprintf(cad,"Sensor I Gain(mV/A): %d",(int)cfg->gain_sensorI);
						break;


				}
				printAtStr(3+i,2,cad);
			}
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
			case 1: 
				cfg->cellsBatt1=(char)NumericUpDown(3+sel_idx,20,(float)cfg->cellsBatt1,2.0f, 6.0f, 1.0f,50, "%.0f", &resultado);
				break;

			case 2: 
				cfg->cellsBatt2=(char)NumericUpDown(3+sel_idx,20,(float)cfg->cellsBatt2,2.0f, 6.0f, 1.0f,50, "%.0f", &resultado);
				break;
			case 3: 
				cfg->total_mAh=NumericUpDown(3+sel_idx,20,(float)cfg->total_mAh,800.0f, 10000.0f,50.0f, 20, "%.0f", &resultado);
				break;

			case 4: 
				cfg->offset_sensorI=NumericUpDown(3+sel_idx,23,(float)cfg->offset_sensorI,0.0f, 3000.0f,5.0f, 20, "%.0f", &resultado);
				break;

			case 5: 
				cfg->gain_sensorI=NumericUpDown(3+sel_idx,23,(float)cfg->gain_sensorI,0.0f, 1000.0f,1.0f, 20, "%.0f", &resultado);
				break;
		default:
			resultado=true;
			break;
	}

	if(resultado)
		estado=ST_MENU;

	return false;
}

bool MenuMAX7456config()
{
	bool resultado=false;
	enum Estados{ST_MENU};
	static int estado = ST_MENU;
	char cad[50];
	char str_video[2][_COLS]={"NTSC", "PAL "};
	const char MAX_idx=5; 
	static char sel_idx=0;
	int i;
	struct StoredConfig * cfg = &tmp_storedConfig;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
		case 1:
			cfg->Video_PAL=Selection(3+sel_idx,20,cfg->Video_PAL,2,str_video, &resultado);
			break;
		case 2:
				cfg->offsetX=NumericUpDown(3+sel_idx, 20,cfg->offsetX, 0.0f, 63.0f, 1.0f, 000, "%.0f ", &resultado);
				break;
		case 3:
				cfg->offsetY=NumericUpDown(3+sel_idx, 20,cfg->offsetY, 0.0f, 63.0f, 1.0f, 000, "%.0f ", &resultado);
				break;
		case 4:
				cfg->line_telemetry=NumericUpDown(3+sel_idx, 20,cfg->line_telemetry, 0.0f, 63.0f, 1.0f, 000, "%.0f ", &resultado);
				break;

		default:
			resultado=true;
			break;
	}
	if(resultado)
		estado=ST_MENU;

	return false;
}


bool MenuIkarusConfig()
{
	bool resultado=false;
	enum Estados{ST_MENU};
	static int estado = ST_MENU;
	char cad[50];
	char MAX_idx=9; 
	char str_telemetry[3][_COLS]={"DISABLE","VIDEO  ","XBEE   "};
	char str_bauds[6][_COLS]={"4800  ","9600  ", "14400 ", "28800 ", "38400 ", "57600 "};
	char str_osd[4][_COLS]={"TX SELEC","TEXTO   ","GRAPHICS", "FIGHTER "};
	char str_canal[6][_COLS]={"SWITCH 2 ", "SWITCH 3 ", "RUEDA    ","MEZCLA223", "MEZCLA224","UPLINK   "};
	char str_altitude[2][_COLS]={"ABSOLUTE","RELATIVE"};
	char str_finruta[4][_COLS]={"IR CASA", "REPETIR","INVERTIR","INV+REP"};
	
	static char sel_idx=0;
	int i;
	struct StoredConfig * cfg = &tmp_storedConfig;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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
		// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
			case 1: 
				cfg->BaudRate=(char)Selection(3+sel_idx,18,cfg->BaudRate,5,str_bauds, &resultado);
				break;
			case 2: 
				cfg->DefaultHUD=(char)Selection(3+sel_idx,18,cfg->DefaultHUD,4,str_osd, &resultado);
				break;
			case 3: 
				cfg->TelemetryMode=(char)Selection(3+sel_idx,18,cfg->TelemetryMode,3,str_telemetry, &resultado);
				break;
			case 4: 
				cfg->ControlProportional=(char)Selection(3+sel_idx,18,cfg->ControlProportional,6,str_canal, &resultado);
				break;

			case 5: 
				cfg->wptRange=NumericUpDown(3+sel_idx,18,(float)cfg->wptRange,3.0f, 200.0f,1.0f, 20, "%.0f", &resultado);
				break;

			case 6: 
				cfg->RelativeAltitude=(char)Selection(3+sel_idx,18,cfg->RelativeAltitude,2,str_altitude, &resultado);
				break;
			
			case 7: 
				cfg->modelo_ruta=(char)Selection(3+sel_idx,18,cfg->modelo_ruta,4,str_finruta, &resultado);
				break;

			case 8: 
				cfg->TimeZone=(char)NumericUpDown(3+sel_idx,18,(float)cfg->TimeZone, -12.0f, 12.0f, 1.0f, 50,"%.0f  ", &resultado);
				break;


		default:
			resultado=true;
			break;
	}
	if(resultado)
		estado=ST_MENU;

	return false;
}


bool MenuIrConfig()
{
	bool resultado=false;
	enum Estados{ST_MENU=0, ST_FIRST=-1};
	static int estado = ST_MENU;
	char cad[50];
	char str_nosi[2][_COLS]={"NO", "SI"};
	char str_sensor[3][_COLS]={"IR  ", "IR+Z","IMU "};
	char MAX_idx=9; 
	
	static char sel_idx=0;
	int i;
	struct AutoPilotConfig * apcfg = &tmp_autopilotCfg;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
					estado=sel_idx;
			}
			break;
			// Submenus
			case 1:
				apcfg->IR_reverse_pitch=Selection(3+sel_idx,18,apcfg->IR_reverse_pitch,2,str_nosi, &resultado);
				break;

			case 2: 	
				apcfg->IR_reverse_roll=Selection(3+sel_idx,18,apcfg->IR_reverse_roll,2,str_nosi, &resultado);
				break;

			case 3: 	
				apcfg->IR_crossed=Selection(3+sel_idx,18,apcfg->IR_crossed,2,str_nosi, &resultado);
				break;

			case 4: 	
				apcfg->IR_reverse_cross=Selection(3+sel_idx,18,apcfg->IR_reverse_cross,2,str_nosi, &resultado);
				break;

			case 5: 	
				apcfg->IR_Z_enabled=Selection(3+sel_idx,18,apcfg->IR_Z_enabled,3,str_sensor, &resultado);
				break;

			case 6: 	
				apcfg->x_off=NumericUpDown(3+sel_idx,18,(float)apcfg->x_off,1.0f, 2.0f,0.01f, 20, "%4.2f", &resultado);
				break;

			case 7:
				apcfg->y_off=NumericUpDown(3+sel_idx,18,(float)apcfg->y_off,1.0f, 2.0f,0.01f, 20, "%4.2f", &resultado);
				break;
		
			case 8: 
				if(apcfg->IR_Z_enabled)
					apcfg->z_off=NumericUpDown(3+sel_idx,18,(float)apcfg->z_off,1.0f, 2.0f,0.01f, 20, "%4.2f", &resultado);
				else
					apcfg->IR_max=NumericUpDown(3+sel_idx,18,(float)apcfg->IR_max,1.0f, 2.0f,0.01f, 20, "%4.2f", &resultado);
				break;

		default:
			resultado=true;
			break;
	}
	if(resultado)
		estado=ST_MENU;

	return false;
}

bool MenuAutopilotConfig()
{
	bool resultado=false;
	enum Estados{ST_MENU=0, ST_FIRST=-1};
	static int estado = ST_MENU;
	char cad[50];
	char MAX_idx=5; 
	char str_apmode[5][_COLS]=
		{	
		"DISABLE   ",
		"FIXED ALT ",
		"DIST ALT  ",
		"WPT (SAFE)",
		"WPT-UNSAFE"
		};
	
	static char sel_idx=0;
	int i;
	struct AutoPilotConfig * apcfg = &tmp_autopilotCfg;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
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
				}
				printAtStr(3+i,2,cad);
			}
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else
				{
					if(sel_idx==4)
						ClrScr();
					estado=sel_idx;
				}
			}
			break;
			// Submenus
			
			case 1: 
				apcfg->AutopilotMode=Selection(3+sel_idx,11,apcfg->AutopilotMode,5,str_apmode, &resultado);
				break;

			case 2: 
				apcfg->baseCruiseAltitude=NumericUpDown(3+sel_idx,18,apcfg->baseCruiseAltitude,0.0f, 99999.0f, 100.0f, 20, "%.0f", &resultado);
				break;

			case 3: 
				apcfg->distanceAltitude=NumericUpDown(3+sel_idx,18,apcfg->distanceAltitude,0.0f, 1000.0f, 10.0f, 20, "%.0f", &resultado);
				break;

			case 4: 
				resultado=MenuIrConfig();
				break;


		default:
			resultado=true;
			break;
	}
	if(resultado)
		estado=ST_MENU;

	return false;
}

bool ActualizarFirmware()
{
	static int estado=-1;
	if(estado<0)
	{
		ClrScr();	
	printAtStr(3,2, "Simulador de Ikarus OSD.");
	printAtStr(5,2, "Nada que actualizar!");
	printAtStr(8,2, "Gracias por probar Ikarus.");
	}
	estado++;
	if(estado>100)
	{
		ClrScr();	
		estado=-1;
		return true;
	}
	else
		return false;
}

bool MenuConfig()
{
	enum Estados{ST_MENU};
	static int estado = ST_MENU;
	bool resultado=false;

	const char MAX_idx=8; 
	static char sel_idx=0;
	char smenus[8][16] ={"Exit", "OSD Config", "Rx Config", "Battery", "Alarm","Ikarus","Autopilot", "Update firmware"};
	char i;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;
			// Pintamos menu
			printCenteredAtStr(1, "Ikarus OSD config");
			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,3,CH_ROWR);
				else
					printAtChr(3+i,3,' ');
				printAtStr(3+i,5,smenus[i]);
			}
			// Transicion
			if(enter_pressed())
			{
				if(sel_idx==0)
				{
					ClrScr();
					return true;
				}
				else 
				{
					ClrScr();
					estado=sel_idx;
				}
			}
			break;
			// Submenus
		case 1:
			resultado=MenuMAX7456config();
			break;
		case 2:
			resultado = MenuRX_config();
			break;
		case 3:
			resultado=MenuBattery_config();
			break;
		case 4:
			resultado=MenuAlarmConfig();
			break;
		case 5:
			resultado = MenuIkarusConfig();
			break;
		case 6:
			resultado = MenuAutopilotConfig();
			break;
		case 7:
			resultado=ActualizarFirmware();
			break;
		default:
			resultado=true;
			break;
	}

	if(resultado)
		estado=ST_MENU;

	return false;
}

bool MenuCalibrar()
{
	enum Estados{ST_MENU};
	static int estado = ST_MENU;
	
	const char MAX_idx=5; 
	static char sel_idx=0;
	char smenus[5][10] ={"Exit", "Sensor IR", "Servos", "RSSI", "Sensor I"};
	char i;

	switch(estado)
	{
		// Estamos en el menu
		case ST_MENU:
			if(next_pressed())
				sel_idx=(sel_idx+1)%MAX_idx;

			printCenteredAtStr(1, "Calibrar Ikarus");

			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(3+i,3,CH_ROWR);
				else
					printAtChr(3+i,3,' ');
				printAtStr(3+i,5,smenus[i]);
			}
			// Transicion
			if(enter_pressed())
			{
				switch(sel_idx)
				{
				case 0:	
					ClrScr();
					return true;
					break;
				
				default: 
					break;
				}
			}
			break;
		default:
			break;
	}
	return false;
}


bool Volar()
{
	static int estado=-1;
	if(estado<0)
	{
		ClrScr();	
		printCenteredAtStr(1, "Saving HOME....");
	}
	estado++;
	if(estado>10)
	{
		estado=-1;
		ClrScr();
		return true;
	}
	else
		return false;
}

bool Volar_NoSave()
{
	static int estado=-1;
	if(estado<0)
	{
		ClrScr();	
		printCenteredAtStr(1, "Exiting....");
	}
	estado++;
	if(estado>10)
	{
		ClrScr();	
		estado=-1;
		return true;
	}
	else
		return false;
}
bool WaitForGPSscreen()
{
	enum Estados{ST_MENU, ST_FLY, ST_EXIT, ST_CONFIG, ST_CALIBRAR};
	static int estado = ST_MENU;

	const char MAX_idx=4; 
	static char sel_idx=1;
	char cad[50];
	int i;

	switch(estado)
	{
		case ST_MENU:
			if(next_pressed())
			{
				sel_idx=(sel_idx+1)%MAX_idx;
			}

			printCenteredAtStr(1, "Wellcome to Ikarus OSD");
			sprintf(cad,"Piloto: %s","AerosimRC");
			printAtStr(3,3,cad);
			sprintf(cad,"GPS Sats (%s): %d           ",GetBaudStr(GetSelectedBauds()),gpsinfo.numsats);
			printAtStr(5,3,cad);

			printAtStr(7,3,"Ready to FLY!");
			printAtStr(8,3,"Configurar");
			printAtStr(9,3,"Calibrar IR/otros");
			printAtStr(10,3,"Exit");	

			for(i=0;i<MAX_idx;i++)
			{
				if(i==sel_idx)
					writeAtChr(7+i,1,CH_ROWR);
				else
					printAtChr(7+i,1,' ');
			}

			if(enter_pressed())
			{
				switch(sel_idx)
				{
				case 0:	
					ClrScr();
					estado=ST_FLY;
					break;
				
				case 1: 
					ClrScr();
					estado = ST_CONFIG;
					tmp_storedConfig=storedConfig;
					tmp_autopilotCfg = autopilotCfg;
					break;

				case 2: 
					ClrScr();
					estado = ST_CALIBRAR;
					break;

				case 3: 
					ClrScr();
					estado=ST_EXIT;
					break;

				default: 
					break;
				}
			}
			break;

		case ST_FLY:
			if(Volar())
			{
				estado=ST_MENU;
				sel_idx=1;
				return true;
			}
			else
				return false;
			break;

		case  ST_EXIT:
			if(Volar_NoSave())
			{
				estado=ST_MENU;
				sel_idx=1;
				return true;
			}
			else
				return false;
			break;

		case ST_CONFIG:
			if(MenuConfig())
			{
				estado=ST_MENU;
				storedConfig=tmp_storedConfig;
				autopilotCfg=tmp_autopilotCfg;
				SaveStoredConfig();
				SaveAutopilotConfig();
			}
			break;

		case ST_CALIBRAR:
			if(MenuCalibrar())
			{
				estado=ST_MENU;
			}
			break;

	}
	return false;
}
