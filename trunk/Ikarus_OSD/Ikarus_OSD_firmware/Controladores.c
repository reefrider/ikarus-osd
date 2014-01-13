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
#include "controladores.h"
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
#include "filtroSimple.h"

enum FAILSAFE_STATE{FS_NORMAL, FS_PRE, FS_STAB, FS_AUTOPLT};



extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern xdata struct IkarusInfo ikarusInfo;
extern xdata struct GPSInfo gpsinfo;
extern code struct Ruta miRuta;

extern unsigned char wpt_index;

extern code struct ServoInfo * servos_cfg;
extern xdata struct ServoStreamParams s_in;


xdata unsigned char rflost=1;
extern xdata unsigned char modem_lost, modem_badcrc;

xdata unsigned char movingOK=0;
xdata unsigned char movingCounter=0;

void ControladorCopilot() large;
void ControladorAutopilot() large;

char CheckFailSafes() large
{
	static unsigned char ppmhold_count=0;
	static unsigned int contador_dly=0;
	static unsigned char fs_fsm_state = FS_NORMAL;

	char fs_position, fs_hold, fs_rflost, fs_modem;
	char failsafe;

	// "Temporiza" rf_lost
	fs_rflost = rflost>RFLOST_MAX;

	
	// Temporiza ppm_hold
	if(ikarusInfo.ppm_hold)
	{
		if(ppmhold_count<PPM_HOLD_COUNT_MAX)
		{
			ppmhold_count++;
			fs_hold = 0;
		}
		else
		{
			fs_hold = 1;
		}
	}
	else
	{
		fs_hold = 0;
		ppmhold_count = 0;
	}

	if(storedConfig.ControlProportional!=MODO_MODEM)
	{
		fs_position=(get_servo_in(CTRL)>servos_cfg[CTRL].max+100)||(get_servo_in(THR)<servos_cfg[THR].min-50);
		fs_modem = 0;
	}
	else
	{
		fs_position=get_servo_in(THR)<servos_cfg[THR].min-50; 
		fs_modem = modem_badcrc>MODEM_CRC_MAX || modem_lost> MODEM_LOST_MAX;
	}

	failsafe = fs_hold || fs_rflost || fs_position ||fs_modem;
	
	switch(fs_fsm_state)
	{
		case FS_NORMAL:
				contador_dly = 0;
				ikarusInfo.failsafe = FSS_NORMAL;
				if(failsafe)
					fs_fsm_state = FS_PRE;
				break;

		case FS_PRE:
				if(!failsafe)
					fs_fsm_state = FS_NORMAL;
				else if(contador_dly < FS_DLY_MAX)
					contador_dly ++;
				else
				{
					fs_fsm_state = FS_STAB;
					ikarusInfo.failsafe_countdown = storedConfig.Retraso_Failsafe;
				}

				if(fs_hold || fs_rflost)
					ikarusInfo.failsafe = FSS_HOLD;
				else
					ikarusInfo.failsafe = FSS_NORMAL;
				
				break;

		case FS_STAB:
				if(!failsafe)
					fs_fsm_state = FS_NORMAL;
				else if(ikarusInfo.failsafe_countdown >=0)
					ikarusInfo.failsafe_countdown-=0.1f;
				else
					fs_fsm_state = FS_AUTOPLT;
			
				if(fs_hold || fs_rflost || fs_position)
					ikarusInfo.failsafe = FSS_STAB;
				else // si es por el modem
					ikarusInfo.failsafe = FSS_NOSTAB;
				
				break;

		case FS_AUTOPLT:
				if(!failsafe)
					fs_fsm_state = FS_NORMAL;

				if(fs_hold || fs_rflost || fs_position )
					ikarusInfo.failsafe = FSS_AUTO;
				else if (get_servo_in(CTRL)<SERVOS_THR)	// si es por el modem
					ikarusInfo.failsafe = FSS_AUTO;
				else
					ikarusInfo.failsafe = FSS_NOAUTO;
				break;

		default:
				fs_fsm_state = FS_NORMAL;
				break;
	}
	return failsafe;
}

char PantallaResumen()
{
	if(gpsinfo.knots_filtered<=5)		// Si esta quieto
	{	
		if(movingOK==1)
		{
			if(movingCounter<100)
			{
				movingCounter++;
			}
			else if(gpsinfo.alt-storedConfig.HomeAltitude<autopilotCfg.MotorSafeAlt)
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
	static int old_hud_num=0;

	CheckFailSafes();

	if(ikarusInfo.failsafe==FSS_AUTO || ikarusInfo.failsafe == FSS_NOAUTO)
	{
		hud_num=SC_FailSafe;

		if(storedConfig.Modo_Failsafe==0 || miRuta.numwpt<=0)
			ikarusInfo.navigateTo=NAV_HOME;
		else
			ikarusInfo.navigateTo=NAV_RUTA;
	

		if(autopilotCfg.AutopilotMode==AP_DISABLED)
			ikarusInfo.AutoPilot_Enabled=APLT_DISABLED;
		else
		{ 
			if(ikarusInfo.failsafe == FSS_NOAUTO)
				ikarusInfo.AutoPilot_Enabled=APLT_DISABLED;		
			else
				ikarusInfo.AutoPilot_Enabled=APLT_FULL;		
		}
			if(storedConfig.CamSel==2)
				CAM_SEL=0;
			else
				CAM_SEL = 1;
	}
	else
	{
		if(ikarusInfo.ctrl_doruta==NAV_RUTA && miRuta.numwpt==0)
			ikarusInfo.navigateTo = NAV_HOLD;
		else if(ikarusInfo.ctrl_doruta == NAV_RUTA && wpt_index >=miRuta.numwpt)
			ikarusInfo.navigateTo = NAV_HOME;
		else 
			ikarusInfo.navigateTo = ikarusInfo.ctrl_doruta;

		if(autopilotCfg.AutopilotMode!=AP_DISABLED)
		{
			if(ikarusInfo.failsafe == FSS_STAB && ikarusInfo.ctrl_autopilot == APLT_DISABLED)
			{
				//ikarusInfo.AutoPilot_Enabled = APLT_ESTAB;
				ikarusInfo.navigateTo = NAV_HOLD_FS;
				ikarusInfo.AutoPilot_Enabled = APLT_FULL;
			}
			else
				ikarusInfo.AutoPilot_Enabled=ikarusInfo.ctrl_autopilot;
		}
		else
			ikarusInfo.AutoPilot_Enabled=APLT_DISABLED;

		if(storedConfig.DefaultHUD)
			hud_num=storedConfig.DefaultHUD;
		else
			hud_num=ikarusInfo.ctrl_hudnum&0x03;		// 0, 1 y 2 (sw 3 pos)	
			
		// Select video camera
		if(storedConfig.CamSel==2)
			CAM_SEL=0;
		else if(storedConfig.CamSel==1)
			CAM_SEL=1;
		else 
			CAM_SEL=ikarusInfo.ctrl_camsel;	
	}	



	if(PantallaResumen())		// Cambia solo el hud, no los mandos
	{
		ChangeHUD(SC_Resumen);
		if(old_hud_num!=hud_num)
		{
			movingOK=0;
			movingCounter = 0;
			ikarusInfo.mostrarResumen = 0;
		}	
	}
	else
	{
		ChangeHUD(hud_num);
	}
	
	old_hud_num = hud_num;
}



void ProcesarMezclas() large
{
	if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON) 
	{
		float mix1, mix2;
		if(autopilotCfg.rev_mezcla)
		{
			mix1=(ikarusInfo.AutoPilot_ail+ikarusInfo.AutoPilot_ele)/2.0f;
			mix2=(ikarusInfo.AutoPilot_ail-ikarusInfo.AutoPilot_ele)/2.0f;
		}
		else
		{
			mix1=(ikarusInfo.AutoPilot_ail-ikarusInfo.AutoPilot_ele)/2.0f;
			mix2=(ikarusInfo.AutoPilot_ail+ikarusInfo.AutoPilot_ele)/2.0f;
		}

		set_servof(AIL, mix1);
		set_servof(ELE, mix2);
		set_servof(THR,ikarusInfo.AutoPilot_thr);
		set_servof(TAIL,ikarusInfo.AutoPilot_tail);				
	}
	else if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
	{
		float mix1, mix2;
		if(autopilotCfg.rev_mezcla)
		{
			mix1=(ikarusInfo.AutoPilot_ele+ikarusInfo.AutoPilot_tail)/2.0f;
			mix2=(ikarusInfo.AutoPilot_ele-ikarusInfo.AutoPilot_tail)/2.0f;
		}
		else
		{
			mix1=(ikarusInfo.AutoPilot_ele-ikarusInfo.AutoPilot_tail)/2.0f;
			mix2=(ikarusInfo.AutoPilot_ele+ikarusInfo.AutoPilot_tail)/2.0f;
		}
		set_servof(AIL, ikarusInfo.AutoPilot_ail);
		set_servof(ELE, mix1);
		set_servof(THR,ikarusInfo.AutoPilot_thr);
		set_servof(TAIL,mix2);
		if(autopilotCfg.chAux_mode==AUX_AIL2)
			set_servof(AUX, ikarusInfo.AutoPilot_ail);

	}
	else	// sin mezcla
	{
		set_servof(AIL, ikarusInfo.AutoPilot_ail);
		set_servof(ELE, ikarusInfo.AutoPilot_ele);
		set_servof(THR,ikarusInfo.AutoPilot_thr);
		set_servof(TAIL,ikarusInfo.AutoPilot_tail);
		if(autopilotCfg.chAux_mode==AUX_AIL2)
			set_servof(AUX, ikarusInfo.AutoPilot_ail);
	}
}

void Control_ServosOUT()
{
//	static int last_center=1500;

	if(ikarusInfo.AutoPilot_Enabled==APLT_DISABLED)	
	{
		if(autopilotCfg.chAux_mode==AUX_AUTOPLT)
			set_servof(AUX, -1.0f);
		ikarusInfo.servoDriver = SERVO_COPY;
	}
	else if(ikarusInfo.AutoPilot_Enabled==APLT_ESTAB)
	{
		Copilot();
		
		ikarusInfo.AutoPilot_tail=0; 
		ikarusInfo.AutoPilot_thr=0;
		
		ProcesarMezclas();
		if(autopilotCfg.chAux_mode==AUX_AUTOPLT) // AUX_TILT, AUX_AIL2, AUX_AUTOPLT, AUX_WPT
			set_servof(AUX, 0.0f);
			
		ikarusInfo.servoDriver = SERVO_MIX;
	}
	else	// if(ikarusInfo.AutoPilot_Enabled==APLT_FULL)
	{
		FullAutoPilot();
		ProcesarMezclas();
		if(autopilotCfg.chAux_mode==AUX_AUTOPLT) // AUX_TILT, AUX_AIL2, AUX_AUTOPLT, AUX_WPT
			set_servof(AUX, 1.0f);

		ikarusInfo.servoDriver = SERVO_REPLACE;
	}

	if (autopilotCfg.chAux_mode==AUX_WPT)
		set_servof(AUX, ikarusInfo.cambioWpt?1.0f:-1.0f);
}


/*
void ControladorCopilot() large
{
	if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON)
	{
		float ail, ele, mix1, mix2;
		mix1 = get_servof(AIL);
		mix2 = get_servof(ELE);

		ail = mix1 + mix2;
		ele = mix1 - mix2;

		if(autopilotCfg.pid_roll.rev)
			ail=-ail;

		if(autopilotCfg.pid_pitch.rev)
			ele = -ele;

		DriveByWire(ele, ail);

		mix1=(ikarusInfo.AutoPilot_ail+ikarusInfo.AutoPilot_ele)/2.0f;
		mix2=(ikarusInfo.AutoPilot_ail-ikarusInfo.AutoPilot_ele)/2.0f;

		set_servof(AIL, mix1);
		set_servof(ELE, mix2);
		
		servos_out[THR]=servos_in[THR];
		servos_out[TAIL]=servos_in[TAIL];
	}
	else if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
	{
		float ail, tail, ele, mix1, mix2;
		mix1 = get_servof(ELE);
		mix2 = get_servof(TAIL);

		ele = mix1 + mix2;
		tail = mix1 - mix2;

		if(autopilotCfg.pid_pitch.rev)
			ele = -ele;

		if(autopilotCfg.pid_tail.rev)
			tail = -tail;

		if(autopilotCfg.chAux_mode==AUX_AIL2)
			ail = (get_servof(AIL) + get_servof(AUX))/2.0f;
		else
			ail = get_servof(AIL);	

		if(autopilotCfg.pid_roll.rev)
			ail=-ail;
					
		DriveByWire(ele, ail);

		mix1=(ikarusInfo.AutoPilot_ele+tail)/2.0f;
		mix2=(ikarusInfo.AutoPilot_ele-tail)/2.0f;

		set_servof(AIL, ikarusInfo.AutoPilot_ail);
		set_servof(ELE, mix1);
		set_servof(TAIL, mix2);
		
		if(autopilotCfg.chAux_mode==AUX_AIL2)
			set_servof(AUX, ikarusInfo.AutoPilot_ail);

		servos_out[THR]=servos_in[THR];
	}
	else	// sin mezcla
	{
		float ail, ele;
		if(autopilotCfg.chAux_mode==AUX_AIL2)
			ail = (get_servof(AIL) + get_servof(AUX))/2.0f;
		else
			ail = get_servof(AIL);

		if(autopilotCfg.pid_roll.rev)
			ail=-ail;

		ele = get_servof(ELE);


		if(autopilotCfg.pid_roll.rev)
			ail=-ail;

		if(autopilotCfg.pid_pitch.rev)
			ele = -ele;

		DriveByWire(ele, ail);

		set_servof(AIL, ikarusInfo.AutoPilot_ail);
		set_servof(ELE, ikarusInfo.AutoPilot_ele);
		
		if(autopilotCfg.chAux_mode==AUX_AIL2)
			set_servof(AUX, ikarusInfo.AutoPilot_ail);

		servos_out[THR]=servos_in[THR];
		servos_out[TAIL]=servos_in[TAIL];
	}

}
*/

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
	switch_set(MNU_NEXT, get_servo_in(TAIL)>SERVOS_THR);
	
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{			
		if(servo_changes&&servo_old)
			estado=(estado+1)&0x01;

		ikarusInfo.ctrl_doruta=(estado) ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot=(servo_old)?APLT_FULL:APLT_DISABLED;
		ikarusInfo.ctrl_hudnum=SC_hud1;	// HUD #1
	}
	else
	{
		if(servo_changes&&servo_old)
			estado=(estado+1)&0x03;

		ikarusInfo.ctrl_doruta=(servo_old) ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;
		ikarusInfo.ctrl_hudnum=estado;
	}
	ikarusInfo.ctrl_camsel=(ikarusInfo.ctrl_hudnum!=SC_hud3);
}

void demux_switch3_basic(unsigned int valor) large
{
	xdata char servo_pos;

	if(valor<1300)
	{
		servo_pos = 0;
		switch_set(MNU_ENTER, 1);
		switch_set(MNU_NEXT, 0);
	}
	else if(valor<1700)
	{
		servo_pos = 1;
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 0);
	}
	else
	{
		servo_pos=2;
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 1);
	}

			
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{			
		ikarusInfo.ctrl_doruta=NAV_HOME;
		ikarusInfo.ctrl_hudnum=SC_hud1;	
		ikarusInfo.ctrl_camsel=1;
	
		switch(servo_pos)
		{
			case 0: ikarusInfo.ctrl_autopilot=APLT_DISABLED;
					break;
		
			case 1: ikarusInfo.ctrl_autopilot=APLT_ESTAB;
					break;
		
			case 2: ikarusInfo.ctrl_autopilot=APLT_FULL;
					break;
		}
	}
	else
	{
		ikarusInfo.ctrl_hudnum= servo_pos + 1;	
		ikarusInfo.ctrl_doruta=(ikarusInfo.ctrl_hudnum!=SC_hud3) ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_camsel=(ikarusInfo.ctrl_hudnum!=SC_hud1);
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;
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

	ikarusInfo.ctrl_doruta=(estado_ruta) ? NAV_RUTA : NAV_HOME;
	ikarusInfo.ctrl_hudnum=estado_hud;	
	ikarusInfo.ctrl_camsel=(ikarusInfo.ctrl_hudnum!=SC_hud3);
			
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{			

		if((servo_old==2)||(servo_old==0))
			ikarusInfo.ctrl_autopilot=APLT_FULL;
		else if(ikarusInfo.ctrl_doruta==NAV_RUTA && miRuta.numwpt==0)
		{
			ikarusInfo.ctrl_doruta = NAV_HOME;
			ikarusInfo.ctrl_autopilot=APLT_ESTAB;
		}
		else
			ikarusInfo.ctrl_autopilot=APLT_DISABLED;

	}
	else
	{
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;
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

		ikarusInfo.ctrl_doruta=(opt==1||opt==2) ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot=(opt<2) ? APLT_FULL : APLT_DISABLED;
		ikarusInfo.ctrl_hudnum=1;		// HUD #1
	}
	else
	{
		code const unsigned char adapt[]={3, 2, 1, 0, 0, 1, 2, 3};
		opt=slider_pos(valor, min,max, 8);

		ikarusInfo.ctrl_doruta=(opt<4) ? NAV_RUTA : NAV_HOME;	// Ruta o casa
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;	// Modo piloto automatico off (esta disabled)
		ikarusInfo.ctrl_hudnum=adapt[opt];
	}
	ikarusInfo.ctrl_camsel=(ikarusInfo.ctrl_hudnum!=SC_hud3);

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
	static xdata unsigned char estado_hud=0;
	static xdata char servo_old;
	static struct FiltroIgualC filtroPantalla;

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

	sw_b = filtroIgual(&filtroPantalla, sw_b);
	if(servo_old!=sw_b && sw_b)
	{
		if(estado_hud<3)
			estado_hud++;
		else
			estado_hud=0;
	}
	servo_old=sw_b;

	if(autopilotCfg.AutopilotMode==AP_DISABLED)
	{
		ikarusInfo.ctrl_doruta = sw_a ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot = APLT_DISABLED;
		ikarusInfo.ctrl_hudnum = sw_c+1;
		ikarusInfo.ctrl_camsel = sw_b;
	}
	else if(storedConfig.ControlProportional==MODO_AJUSTE223)
	{
		ikarusInfo.ctrl_doruta = sw_a ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot = sw_b?APLT_FULL:APLT_DISABLED;
		ikarusInfo.ctrl_hudnum = SC_hud1;
		ikarusInfo.ctrl_camsel=1;
	}
	else // if(storedConfig.ControlProportional==MODO_MIX223)
	{
		ikarusInfo.ctrl_doruta = sw_a ? NAV_RUTA : NAV_HOME;
		ikarusInfo.ctrl_autopilot = sw_c;
		ikarusInfo.ctrl_hudnum = estado_hud;
		ikarusInfo.ctrl_camsel=(ikarusInfo.ctrl_hudnum!=SC_hud3);
	}
		
	switch_set(MNU_ENTER, sw_c==2 );
	switch_set(MNU_NEXT, sw_c==0);
}

void demux_mix224(int i) large
{
	const int valores=16;	// 2*2*4
//	static xdata char servo_old;
//	static xdata char hud_num;
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
	
	ikarusInfo.ctrl_doruta = sw_a ? NAV_RUTA : NAV_HOME;
	ikarusInfo.ctrl_hudnum = sw_d+1;	// hud_num; 
		
	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		if(sw_b)
			ikarusInfo.ctrl_autopilot = APLT_DISABLED;
		else if(sw_c)
			ikarusInfo.ctrl_autopilot = APLT_ESTAB;
		else
			ikarusInfo.ctrl_autopilot = APLT_FULL;

		ikarusInfo.ctrl_camsel = sw_d;
	
	}
	else
	{
		ikarusInfo.ctrl_autopilot = APLT_DISABLED;
		ikarusInfo.ctrl_camsel = sw_c;
	
	}

	switch_set(MNU_ENTER, sw_a );
	switch_set(MNU_NEXT, sw_b);
}

void demux_modem(unsigned int valor) large
{
	modem_rx(valor);

	//switch_set(MNU_ENTER, ikarusInfo.modem_sw&0x08);
	//switch_set(MNU_NEXT, ikarusInfo.modem_sw&0x04);

	ikarusInfo.ctrl_hudnum=ikarusInfo.modem_sw&0x03;
	ikarusInfo.ctrl_doruta=(ikarusInfo.modem_sw>>4)&0x03;
	ikarusInfo.ctrl_camsel=(ikarusInfo.modem_sw&0x40)==0x40;

	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		ikarusInfo.ctrl_autopilot=(ikarusInfo.modem_sw>>2)&0x03;
	}
	else
	{
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;
	}
}

/*
void demux_switch33(unsigned int valor, unsigned int valor2)
{
	char valor_a, valor_b;
	static xdata char valor_a_old;
	static xdata unsigned char estado_hud=0, estado_ruta=0;

	if(valor<1300)
	{
		switch_set(MNU_ENTER, 1);
		switch_set(MNU_NEXT, 0);
		valor_a=0;
		if(valor_a_old!=0)
		{
			if(estado_hud<3)
				estado_hud++;
			else
				estado_hud=0;

		}
	}
	else if(valor<1700)
	{
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 0);
		valor_a=1;
	}
	else
	{
		switch_set(MNU_ENTER, 0);
		switch_set(MNU_NEXT, 1);
		valor_a=2;
		if(valor_a_old!=2)
		{
			if(estado_ruta<2)
				estado_ruta++;
			else
				estado_ruta=0;
		}
	}
	valor_a_old=valor_a;
	
	if(valor2<1300)
	{
		valor_b=0;
	}
	else if(valor2<1700)
	{
		valor_b=1;
	}
	else
	{
		valor_b=2;
	}

	if(autopilotCfg.AutopilotMode!=AP_DISABLED)
	{
		ikarusInfo.ctrl_hudnum=estado_hud;
		ikarusInfo.ctrl_doruta=estado_ruta;
		//ikarusInfo.ctrl_camsel=(ikarusInfo.modem_sw&0x40)==0x40;
		ikarusInfo.ctrl_autopilot=valor_b;
	}
	else
	{
		ikarusInfo.ctrl_hudnum=valor_a;
		ikarusInfo.ctrl_doruta=valor_b;
		//ikarusInfo.ctrl_camsel=(ikarusInfo.modem_sw&0x40)==0x40;
		ikarusInfo.ctrl_autopilot=APLT_DISABLED;
	}


}
*/
void ParseControl(unsigned int valor) large
{
	char modo=storedConfig.ControlProportional;
	switch(modo)
	{
		case MODO_SW2: // Interruptor
			demux_switch2(valor);
			break;

		case MODO_SW3_BASIC:
			demux_switch3_basic(valor);
			break;

		case MODO_SW3: // Interruptor 3
			demux_switch3(valor);
			break;
		
		case MODO_RUEDA: // Rueda
		default:
			demux_rueda(valor);
			break;

		case MODO_MIX223: // multiplexado 223
		case MODO_AJUSTE223:
			demux_mix223(valor);
			break;

		case MODO_MIX224: // multiplexado 224
			demux_mix224(valor);
			break;

		case MODO_MODEM:	//modem
			demux_modem(valor);	
			break;

//		case MODO_SW33:
//			demux_switch33(valor,get_servo_in(storedConfig.Canal_PPM+1));
	}

	rflost=0;
}

