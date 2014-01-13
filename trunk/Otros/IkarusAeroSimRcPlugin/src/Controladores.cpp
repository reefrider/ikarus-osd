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

#include "Ikarus.h"
#include "controladores.h"
#include "LibraryMAX7456.h"
#include "huds.h"
#include "ParserNMEA.h"
#include "Servos.h"
#include "Navigation.h"
#include "Utils.h"
#include "AutoPilot.h"
#include "MenuConfig.h"
#include "PID.h"
#include "modem.h"

#ifndef SIMULADOR
#include "C8051F340.h"
#include "config.h"
#include "USB_API.h"
#include "Telemetry.h"
#else
char CAM_SEL;
#endif


extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern xdata struct IkarusInfo ikarusInfo;
extern xdata struct GPSInfo gpsinfo;

extern bit failsafe;

extern code struct ServoInfo * servos_cfg;
extern xdata unsigned int servos_in[];
extern xdata unsigned int servos_out[];

xdata unsigned char rflost=1;

xdata unsigned char movingOK=0;
xdata unsigned char movingCounter=0;


char PantallaResumen()
{

	if(gpsinfo.velo_filter<=5)		// Si esta quieto
	{	
		if(movingOK==1)
		{
			if(movingCounter<100)
			{
				movingCounter++;
			}
			else
			{
				ikarusInfo.mostrarResumen=1;
			}
		}
	}
	else //if(gpsinfo.kmph>30) 
	{
		if(movingOK==0&&movingCounter<100)
			movingCounter++;
		else
		{
			movingCounter=0;
			movingOK=1;
		}
		ikarusInfo.mostrarResumen=0;
	}
	return ikarusInfo.mostrarResumen;
}

void Control_ServosIN() 
{
	int hud_num;

	if(failsafe==1)
	{
		hud_num=SC_FailSafe;
		SetToHome();

		if(autopilotCfg.AutopilotMode!=AP_DISABLED)
			ikarusInfo.AutoPilot_Enabled=1;
		else
			ikarusInfo.AutoPilot_Enabled=0;
	}
	else
	{
		if(ikarusInfo.ctrl_doruta)
			SetDoRoute();
		else
			SetToHome();

		if(ikarusInfo.ctrl_autopilot&&autopilotCfg.AutopilotMode!=AP_DISABLED)
			ikarusInfo.AutoPilot_Enabled=1;
		else
			ikarusInfo.AutoPilot_Enabled=0;

		if(storedConfig.DefaultHUD)
			hud_num=storedConfig.DefaultHUD;
		else
			hud_num=ikarusInfo.ctrl_hudnum&0x03;		// 0, 1 y 2 (sw 3 pos)		
	}	

	// Select video camera
	if(storedConfig.CamSel==2)
		CAM_SEL=0;
	else if(storedConfig.CamSel==1||failsafe==1)
		CAM_SEL=1;
	else if(storedConfig.ControlProportional==MODO_MODEM||storedConfig.ControlProportional==MODO_MIX224)	
		CAM_SEL=ikarusInfo.ctrl_camsel;
	else
		CAM_SEL=(hud_num!=SC_hud3);

	if(PantallaResumen())		// Cambia solo el hud, no los mandos
		hud_num=SC_Resumen;	
	
	ChangeHUD(hud_num);
}

void Control_ServosOUT()
{
	if(ikarusInfo.AutoPilot_Enabled)
	{
		TopLevelAutoPilot();
		LowLevelAutoPilot();
			
		if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON)
		{
			float mix1, mix2;
			float aileron=ikarusInfo.AutoPilot_ail;
			float elevator=ikarusInfo.AutoPilot_ele;
			mix1=(aileron+elevator)/2.0f;
			mix2=(aileron-elevator)/2.0f;
			set_servof(AIL, mix1);
			set_servof(ELE, mix2);
			set_servof(THR,ikarusInfo.AutoPilot_thr);
			set_servof(TAIL,ikarusInfo.AutoPilot_tail);				
		}
		else if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
		{
			float mix1, mix2;
			float elevator=ikarusInfo.AutoPilot_ele;
			float tail=ikarusInfo.AutoPilot_tail;
			mix1=(elevator+tail)/2.0f;
			mix2=(elevator-tail)/2.0f;
			set_servof(AIL, ikarusInfo.AutoPilot_ail);
			set_servof(ELE, mix1);
			set_servof(THR,ikarusInfo.AutoPilot_thr);
			set_servof(TAIL,mix2);

		}
		else	// sin mezcla
		{
			set_servof(AIL, ikarusInfo.AutoPilot_ail);
			set_servof(ELE, ikarusInfo.AutoPilot_ele);
			set_servof(THR, ikarusInfo.AutoPilot_thr);
			set_servof(TAIL,ikarusInfo.AutoPilot_tail);
		}
	}
/*
	else
	{
		// esta parte sobraria
		//servos_out[AIL]=servos_in[AIL];
		//servos_out[ELE]=servos_in[ELE];
		//servos_out[THR]=servos_in[THR];
		//servos_out[TAIL]=servos_in[TAIL];

		// Capturamos la posicion central de la cola
		last_center=servos_in[TAIL];
	}
*/
}

void demux_switch2(unsigned int valor) large
{
	static xdata unsigned char estado=0;
	static xdata unsigned int servo_changes=0;
	static xdata char servo_old;

	if(valor>SERVOS_THR)
	{
		servo_changes=(servo_old==0);
		servo_old=1;
	}
	else
	{
		servo_changes=(servo_old==1);
		servo_old=0;
	}

	switch_set(MNU_ENTER, servo_old);
	switch_set(MNU_NEXT, servos_in[TAIL]>SERVOS_THR);
	
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{			
		if(servo_changes&&servo_old)
			estado=(estado+1)&0x01;

		ikarusInfo.ctrl_doruta=estado;
		ikarusInfo.ctrl_autopilot=servo_old;
		ikarusInfo.ctrl_hudnum=SC_hud1;	// HUD #1
//		ikarusInfo.ctrl_camsel=0;
	}
	else
	{
		if(servo_changes&&servo_old)
			estado=(estado+1)&0x03;

		ikarusInfo.ctrl_doruta=servo_old;
		ikarusInfo.ctrl_autopilot=0;
		ikarusInfo.ctrl_hudnum=estado;
//		ikarusInfo.ctrl_camsel=0;
	}
}

void demux_switch3(unsigned int valor) large
{
	
	static xdata unsigned char estado_hud=0, estado_ruta=0;
	static xdata char servo_old;

	if(valor<1300)
	{
		if(servo_old!=0)
		{
			estado_ruta=!estado_ruta;
		}
		servo_old=0;
		switch_set(MNU_ENTER, 1);
		switch_set(MNU_NEXT, 0);
	}
	else if(valor<1700)
	{
		servo_old=1;
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 0);
	}
	else
	{
		if(servo_old!=2)
		{
			if(estado_hud<3)
				estado_hud++;
			else
				estado_hud=0;
		}
		servo_old=2;
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 1);
	}

	ikarusInfo.ctrl_doruta=estado_ruta;
	ikarusInfo.ctrl_hudnum=estado_hud;	
//	ikarusInfo.ctrl_camsel=0;
	
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{			
		ikarusInfo.ctrl_autopilot=(servo_old==2)||(servo_old==0);
	}
	else
	{
		ikarusInfo.ctrl_autopilot=0;
	}
	
}

void demux_rueda(unsigned int valor) large
{
	unsigned char opt;
	unsigned int min, max;

	min=1100;
	max=1900;

	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		opt=slider_pos(valor,min,max, 4);

		ikarusInfo.ctrl_doruta=(opt==1||opt==2);
		ikarusInfo.ctrl_autopilot=(opt<2);
		ikarusInfo.ctrl_hudnum=1;		// HUD #1
//		ikarusInfo.ctrl_camsel=0;
	}
	else
	{
		code const unsigned char adapt[]={3, 2, 1, 0, 0, 1, 2, 3};
		opt=slider_pos(valor, min,max, 8);

		ikarusInfo.ctrl_doruta=(opt<4);	// Ruta o casa
		ikarusInfo.ctrl_autopilot=0;	// Modo piloto automatico off (esta disabled)
		ikarusInfo.ctrl_hudnum=adapt[opt];
//		ikarusInfo.ctrl_camsel=0;
	}

	if(valor<1300)
	{
		switch_set(MNU_ENTER, 1);
		switch_set(MNU_NEXT, 0);
	}
	else if(valor<1700)
	{
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 0);
	}
	else
	{
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 1);
	}
}


	
void demux_mix223(unsigned int v) large
{
	const int valores=12;	// 2*2*3
	
	int value;
	int rango; 
	char sw_a, sw_b, sw_c;

	rango=(servos_cfg[0].max-servos_cfg[0].min)/(valores-1);
	value=v-servos_cfg[0].min+rango/2;
	
	if(value>6*rango)
	{
		sw_a=1;
		value=value-6*rango;
	}
	else
		sw_a=0;
	
	if(value>3*rango)
	{
		sw_b=1;
		value=value-3*rango;
	}		
	else
		sw_b=0;

	if(value>(2*rango))
		sw_c=2;
	else if(value>rango)
		sw_c=1;
	else
		sw_c=0;


	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		ikarusInfo.ctrl_doruta = sw_a;
		ikarusInfo.ctrl_autopilot = sw_b;
		ikarusInfo.ctrl_hudnum = sw_c+1;
//		ikarusInfo.ctrl_camsel=0;
	}
	else
	{
		ikarusInfo.ctrl_doruta = sw_a;
		ikarusInfo.ctrl_autopilot = 0;
		ikarusInfo.ctrl_hudnum = (sw_b)?sw_c+1:0;
//		ikarusInfo.ctrl_camsel=1;
	}
//	switch_set(MNU_ENTER, sw_b );
//	switch_set(MNU_NEXT, sw_a);

//	Modificacion para JAGA
	switch_set(MNU_ENTER, sw_c==2 );
	switch_set(MNU_NEXT, sw_c==0);

}


void demux_mix224(int i) large
{
	const int valores=16;	// 2*2*4
	static xdata char servo_old;
	static xdata char hud_num;
	char sw_a, sw_b, sw_c, sw_d;
	
	int value;
	int rango; 

	rango=(servos_cfg[0].max-servos_cfg[0].min)/(valores-1);
	value=i-servos_cfg[0].min+rango/2;
	
	if(value>8*rango)
	{
		sw_a=1;
		value=value-8*rango;
	}
	else
		sw_a=0;
	
	if(value>4*rango)
	{
		sw_b=1;
		value=value-4*rango;
	}		
	else
		sw_b=0;

	if(value>2*rango)
	{
		sw_c=1;
		value=value-2*rango;
	}
	else 
		sw_c=0;
	
	if(value>rango)
		sw_d=1;
	else
		sw_d=0;
	
	if(servo_old!=sw_d)
	{
		hud_num++;
		if(hud_num>3)
			hud_num=0;
		servo_old=sw_d;
	}
	ikarusInfo.ctrl_doruta = sw_a;
	ikarusInfo.ctrl_camsel = sw_c;
	ikarusInfo.ctrl_hudnum = hud_num; //sw_d;
		
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		ikarusInfo.ctrl_autopilot = sw_b;
	}
	else
	{
		ikarusInfo.ctrl_autopilot = 0;
	}

	switch_set(MNU_ENTER, sw_a );
	switch_set(MNU_NEXT, sw_b);
}


void demux_modem(unsigned int valor) large
{
	modem_rx(valor);

	switch_set(MNU_ENTER, ikarusInfo.modem_sw&0x08);
	switch_set(MNU_NEXT, ikarusInfo.modem_sw&0x04);

	ikarusInfo.ctrl_hudnum=ikarusInfo.modem_sw&0x03;
	ikarusInfo.ctrl_doruta=ikarusInfo.modem_sw&0x08;
	ikarusInfo.ctrl_camsel=(ikarusInfo.modem_sw&0x20)==0x20;

	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		ikarusInfo.ctrl_autopilot=ikarusInfo.modem_sw&0x04;
	}
	else
	{
		ikarusInfo.ctrl_autopilot=0;
	}
}

void ParseControl(unsigned int valor) large
{
	char modo=storedConfig.ControlProportional;

	switch(modo)
	{
		case MODO_SW2: // Interruptor
			demux_switch2(valor);
			break;

		case MODO_SW3: // Interruptor 3
			demux_switch3(valor);
			break;
		
		case MODO_RUEDA: // Rueda
		default:
			demux_rueda(valor);
			break;

		case MODO_MIX223: // multiplexado 223
			demux_mix223(valor);
			break;

		case MODO_MIX224: // multiplexado 224
			demux_mix224(valor);
			break;

		case MODO_MODEM:	//modem
			demux_modem(valor);	
			break;
	}

	if(storedConfig.ControlProportional!=MODO_MODEM)
	{
		rflost=0;
	
		if(valor>servos_cfg[0].max+100)
			failsafe=1;
		else
			failsafe=0;
	}
	else if(rflost<20)
		failsafe=0;
}

