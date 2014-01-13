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

#include <string.h>

#include "c8051f340.h"
#include "Ikarus.h"
#include "Servos.h"
#include "Modem.h"
#include "Utils.h"
#include "Controladores.h"
#include "filtroSimple.h"


#define PERIODO_SERVOS 20000
#define PREAMBLE_PWM	400

extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern xdata struct IkarusInfo ikarusInfo;

code const unsigned char mbit[]={0x01,0x02,0x04,0x08,0x10,0x20,0x40,0x80};


code const struct ServoStreamParams s_default=
	{
	8,
	20000,
	400,
	0
	};

xdata struct ServoStreamParams s_in;

xdata unsigned int servos_stream[MAX_CH_STRM_IN]={1500,1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500};
//xdata unsigned int servos_stream_out[MAX_CH_STRM_IN]={1500,1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500};

xdata unsigned int servos_in[MAX_CH_IN]={1500, 1500, 1500, 1500, 1500,   1500, 1500};
xdata unsigned int servos_lastin[MAX_CH_IN]={1500, 1500, 1500, 1500, 1500,   1500, 1500};

xdata volatile unsigned int servos_out[MAX_CH_OUT]={0, 1500, 1500, 1500, 1500,   1500, 1500};

//xdata unsigned int imu_ppm[4];

code struct ServoInfo * servos_cfg=&autopilotCfg.servos_cfg; // Sobra & o falta [0]

unsigned char switch_changes[2]={0,0};
unsigned char switch_old[2]={0,0};	

unsigned int sync_ts;
bit sync_ok;
unsigned int sync_ts2;

void ActivarServos()
{
	if(autopilotCfg.ServoMotorEnableAlt)
		servos_in[THR]=autopilotCfg.ServoMotorSafeAlt;
	else
		servos_in[THR]=servos_cfg[THR].min;

	if(storedConfig.Modo_PPM==1)
	{
		SERVOAIL=0;
		SERVOELE=0;
		SERVOTHR=0;
		SERVOTAIL=0;
		SERVOAUX=0;
		SERVOPAN=0;
 	
		P2MDOUT=0x3F;				// 1-> salidas
		PCA0CPM1  = 0x4D;			// PCA1 -> High speed output
		PCA0CPM2 = 0x21;			// PCA2 -> Capture on positive edge
	
		
		XBR1=(XBR1&0xF8)|0x02;		// Activamos PCA 1 y 2 
		//XBR1=(XBR1&0xF8)|0x03;		// Activamos PCA 1, 2 y 3 Creo q era para IMU fallida
	
	}
	else	// legacy
	{
		SERVOAIL=1;		// entradas
		SERVOELE=1;		// entradas
		SERVOTHR=0;
		SERVOTAIL=0;
		SERVOAUX=0;
		SERVOPAN=0;

		P2MDOUT=0x2C;
		PCA0CPM1 = 0x31;			// Compare on both edges
		PCA0CPM2 = 0x31;
		PCA0CPM3 = 0x4D;

		XBR1=(XBR1&0xF8)|0x04;
	}
}

void DesactivarServos()
{
	XBR1 = XBR1&0xF8;
}

char switch_pos(unsigned char ch) large
{
	if(ch==0||ch==1)
		return switch_old[ch]; 
	else
		return 0;
}

int switch_changed(unsigned char ch) large
{
	int result;

	if(ch==0||ch==1)
	{
		result=switch_changes[ch];
		switch_changes[ch]=0;
	}
	else
		result=0;

	return result;
}

void switch_set(unsigned char ch, char value) large
{
	if(ch==0||ch==1)
	{
		if(switch_old[ch]!=value)
		{
			switch_changes[ch]=1;
			switch_old[ch]=value;
		}
	}
}


int get_servo_in(unsigned char ch) reentrant
{
	if(ch>=MAX_CH_IN)
		return 1500;
	else if(ikarusInfo.ppm_hold==0 && ikarusInfo.failsafe != FSS_HOLD)	
		return servos_in[ch]; 
	else 
		return servos_lastin[ch];
}

/*
void set_servo(unsigned char ch, int valor) large reentrant
{
	if(ch>=0&&ch<MAX_CH_OUT)
	{
		if(valor<servos_cfg[ch].min)
			valor=servos_cfg[ch].min;
		else if(valor>servos_cfg[ch].max)
			valor=servos_cfg[ch].max;
		servos_out[ch]=valor;
	}
}

void set_servofe(unsigned char ch, float v, int min, int center, int max) large
{
	xdata int valor;
	if(ch>=0&&ch<MAX_CH_OUT)
	{
		if(v>1.0f)
			v=1.0f;
		else if(v<-1.0f)
			v=-1.0f;
	
		if(v>=0)
			valor=(int)((max-center)*v+center);
		else
			valor=(int)((center-min)*v+center);

		set_servo(ch,valor);
	}
}
*/

int servo_mix(unsigned char ch) large 
{
	int value;

	value = get_servo_in(ch)+servos_out[ch]-servos_cfg[ch].center;
	if(value > servos_cfg[ch].max)
		return servos_cfg[ch].max;
	else if(value < servos_cfg[ch].min)
		return servos_cfg[ch].min;
	else
		return value;
}

void set_servof(unsigned char ch, float v) large
{
	xdata int valor;
	int max, min, center;

	if(ch>=MAX_CH_OUT)
		return;
	
	if(ch!=PAN)
	{
		max=servos_cfg[ch].max;
		min=servos_cfg[ch].min;
		center=servos_cfg[ch].center;
	}
	else
	{
		max=SERVOS_MAX;
		min=SERVOS_MIN;
		center=1500;
	}	

	if(ch == THR && v<-1.5f &&autopilotCfg.ServoMotorEnableAlt)
	{
		servos_out[THR] = autopilotCfg.ServoMotorSafeAlt;
	}
	else
	{
		if(v>1.0f)
			v=1.0f;
		else if(v<-1.0f)
			v=-1.0f;

		if(servos_cfg[ch].reverse)
			v=-v;

		if(v>=0)
			valor=(int)((max-center)*v+center);
		else
			valor=(int)((center-min)*v+center);

		servos_out[ch]=valor;
	}
}

float get_servof(unsigned char ch) large  
{
	int v;
	float t;
	if(ch>=MAX_CH_OUT)
		return 0.0;
	v=servos_in[ch];
	if(v>servos_cfg[ch].max)
		t=1.0;
	else if (v>servos_cfg[ch].center)
		t= ((float)v-servos_cfg[ch].center)/(servos_cfg[ch].max-servos_cfg[ch].center);
	else if (v>servos_cfg[ch].min)
		t= ((float)v-servos_cfg[ch].center)/(servos_cfg[ch].center-servos_cfg[ch].min);
	else
		t= -1.0f;
	if(servos_cfg[ch].reverse)
		t=-t;
	return t;
}

#if 1
float leer_servo(unsigned char ch) large
{
	switch(ch)
	{
		case AIL:
			if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON)
				return get_servof(AIL) + get_servof(ELE);
			else
				return get_servof(AIL);

		case ELE:
			if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON)
			{
				if(autopilotCfg.rev_mezcla)
				{
					return get_servof(AIL) - get_servof(ELE);
				}
				else
				{
					return get_servof(ELE) - get_servof(AIL);
				}
			}
			else if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
				return get_servof(ELE) + get_servof(TAIL);
			else
				return get_servof(ELE);

		case TAIL:
			if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
			{
				if(autopilotCfg.rev_mezcla)
				{
					return get_servof(ELE) - get_servof(TAIL);
				}
				else
				{
					return get_servof(TAIL) - get_servof(ELE);
				}
			}
			else
				return get_servof(TAIL);

		default:
			return get_servof(ch);

	}
}
#else
float leer_servo(unsigned char ch) large
{
	if(autopilotCfg.tipo_mezcla==MEZCLA_ELEVON) 
	{
		float s_ail, s_ele;
		float mix1, mix2;
		mix1 = get_servof(AIL);
		mix2 = get_servof(ELE);

		if(autopilotCfg.rev_mezcla)
		{
			s_ail = mix1+mix2;
			s_ele = mix1-mix2;
		}
		else
		{
			s_ail = mix1+mix2;
			s_ele = mix2-mix1;
		}

		switch(ch)
		{			
			case AIL: return s_ail;
			case ELE: return s_ele;
			case THR: return get_servof(THR);
			case TAIL: return get_servof(TAIL);
		}
	}
	else if(autopilotCfg.tipo_mezcla==MEZCLA_VTAIL)
	{
		float s_ele, s_tail;
		float mix1, mix2;
		mix1 = get_servof(ELE);
		mix2 = get_servof(TAIL);

		if(autopilotCfg.rev_mezcla)
		{
			s_ele = mix1 + mix2;
			s_tail = mix1 - mix2;
		}
		else
		{
			s_ele = mix1 + mix2;
			s_tail = mix2 - mix1;
		}

		switch(ch)
		{
			case AIL: return get_servof(AIL);
			case ELE: return s_ele;
			case THR: return get_servof(THR);
			case TAIL: return s_tail;
		}

	}
	else	// sin mezcla
	{
		switch(ch)
		{
			case AIL: return get_servof(AIL);
			case ELE: return get_servof(ELE);
			case THR: return get_servof(THR);
			case TAIL: return get_servof(TAIL);
		}
	}
}
#endif



void PCA_handle() interrupt 11
{
	static unsigned int acum_period=0;
	static unsigned char curr_ch=0;

	unsigned int valor_servo;
			
	unsigned int ts;
	unsigned int diff;	

	if(CCF0)
	{
		static unsigned char index;
		static unsigned int last_ts0=0;
		static unsigned int total=0;
		static unsigned int last_preamble=0;
	
		ts=PCA0CP0;
		diff=ts-last_ts0;		
		total+=diff;
		
		if(storedConfig.Modo_PPM==1)
		{
			if(diff>3000)
			{
				if(index!=0xff)
				{
					s_in.canales=index;
					s_in.periodo=total;	
					
					if(storedConfig.NumCanales_PPM==255 || storedConfig.NumCanales_PPM == index)
					{	
						ParseControl(get_servo_in(CTRL));
						memcpy(servos_lastin, servos_in, MAX_CH_IN*sizeof(int));
						ikarusInfo.ppm_hold = 0;
					}
					else
					{
						ikarusInfo.ppm_hold = 1;
					}
				}
				else
				{
						ikarusInfo.ppm_hold = 1;
				}

				s_in.lvl_idle=!SERVOIN;
				total=0;
				index=0;
				// Sincronizar la salida
				sync_ok=1;
				sync_ts=ts;
			}
			else if(SERVOIN!=s_in.lvl_idle)		// canal
			{
				diff+=last_preamble;
				if(diff>SERVOS_MIN&&diff<SERVOS_MAX&&index<MAX_CH_STRM_IN)
				{
					if(index==storedConfig.Canal_PPM)
					{
						servos_in[CTRL]=diff;
					}
					else if(index==autopilotCfg.ch_ail)
					{
						servos_in[AIL]=diff;
					}
					else if(index==autopilotCfg.ch_ele)
					{
						servos_in[ELE]=diff;
					}
					else if(index==autopilotCfg.ch_thr)
					{
						servos_in[THR]=diff;
					}
					else if(index==autopilotCfg.ch_tail)
					{
						servos_in[TAIL]=diff;
					}
					else if(index==autopilotCfg.ch_pan)
					{
						servos_in[PAN]=diff;
					}
					else if(index==autopilotCfg.ch_aux)
					{
						servos_in[AUX]=diff;
					}
					
					servos_stream[index++]=diff;
				}
				else
				{
					index=0xFF;		// lock. Esperamos next sync
					ikarusInfo.ppm_hold = 1;
				}

			}
			else	// preambulo
			{
				if(diff<600&&index!=0xff)
				{
					last_preamble=diff;
					s_in.preamble=last_preamble; 
				}
				else
					index=0xff;
			}
		}
		else		// no PPM multiplexado
		{
			if(!SERVOIN&&diff>SERVOS_MIN&&diff<SERVOS_MAX)
			{
				servos_in[CTRL]=diff;
				servos_lastin[CTRL]=diff;
				ParseControl(diff);
				ikarusInfo.ppm_hold = 0;
			}
			else
				ikarusInfo.ppm_hold = 1;
		}
		last_ts0=ts;

		CCF0=0;
	}

	if(storedConfig.Modo_PPM==1)
	{

		if(CCF1)
		{
			char pin_status;
			char next_skip;
		
			ts=PCA0CP1;

			switch(curr_ch)
			{
				case 0: pin_status=SERVOAIL;
						next_skip=0xFD;		//0xFD;		//0x01;
						break;

				case 1: pin_status=SERVOELE;
						next_skip=0xFB; 	//0xFB;		//0x03;
						break;

				case 2: pin_status=SERVOTHR;
						next_skip=0xF7; 	//0xF7;		//0x07;
						break;

				case 3: pin_status=SERVOTAIL;
						next_skip=0xEF;		//0xEF;		//0x0F;
						break;

				case 4: pin_status=SERVOPAN;
						next_skip=0xDF;		//0xDF;		//0x1F;
						break;

				case 5: pin_status=SERVOAUX;
						next_skip=0xFE;		//0xFE;		//0x00;
						break;
			}

			if(pin_status)
			{
				if(curr_ch==PAN-1)
				{
					if(storedConfig.ControlProportional==MODO_AJUSTE223)
						valor_servo = servos_cfg[PAN].center; 
					else
					{
						servos_out[PAN]=(3*servos_out[PAN]+get_servo_in(PAN))/4;		// Filtro de velocidad
						valor_servo=autopilotCfg.pan_gain*((int)servos_out[PAN]-servos_cfg[PAN].center)+servos_cfg[PAN].center;
						
					}
				
				}
				// AUX_TILT, AUX_AIL2, AUX_AUTOPLT, AUX_WPT
				else if((curr_ch==AUX-1)&&(autopilotCfg.chAux_mode==AUX_TILT))
				{
					valor_servo=get_servo_in(curr_ch+1);
				}
				else if((curr_ch==AUX-1)&&((autopilotCfg.chAux_mode==AUX_AUTOPLT)||(autopilotCfg.chAux_mode==AUX_WPT)))
				{
					valor_servo=servos_out[curr_ch+1];					
				}
				else if(ikarusInfo.servoDriver==SERVO_REPLACE)
				{
					valor_servo=servos_out[curr_ch+1];
				}
				else if(ikarusInfo.servoDriver==SERVO_BITMASKS)
				{
					if(ikarusInfo.servos_bitmasks&mbit[curr_ch])
						valor_servo=servos_out[curr_ch+1];					
					else
						valor_servo=get_servo_in(curr_ch+1);
				}
				else if(ikarusInfo.servoDriver==SERVO_COPY||curr_ch==THR-1) // || curr_ch = TAIL-1)
				{
					valor_servo=get_servo_in(curr_ch+1);
				}
				else //if(ikarusInfo.servoDriver==SERVO_MIX)
				{
					valor_servo=servo_mix(curr_ch+1);
				}
				
					// Quitar si descomento abajo...
				if(valor_servo<SERVOS_MIN)
					valor_servo=SERVOS_MIN;
				else if(valor_servo>SERVOS_MAX)
					valor_servo=SERVOS_MAX;
			
				ts+=valor_servo;
				acum_period+=valor_servo;

			}
			else
			{
				curr_ch++;
				if(curr_ch>=6)					// Ha sido el ultimo
				{
					if(sync_ok)
					{
						int off = sync_ts2-sync_ts-6000;	// <0 atrasa salida respecto entrada
						if(off>100)
							off=100;
						else if(off<-100)
							off=-100;
							
						ts+=s_in.periodo-acum_period-off;
						sync_ok=0;			
					}
					else
						ts+=s_in.periodo-acum_period;
					
					acum_period=0;
					sync_ts2=ts;

					curr_ch=0;

			    	P2SKIP    = 0xFE;	//0xFE;		//0x00; //SKIP[curr_ch]; 
				}
				else
				{
					ts+=400;
					acum_period+=400;

			    	P2SKIP    = next_skip; //SKIP[curr_ch];		// revisar 
				}					
			}
		
			PCA0CPL1=ts&0xff;
			PCA0CPH1=(ts>>8)&0xff;
			CCF1=0;
		}
	}/* 
	else if(storedConfig.Modo_PPM==2)
	{
		if(CCF1)
		{
			char pin_status;
			char next_skip;
		
			ts=PCA0CP1;

			pin_status=SERVOAIL;
			next_skip=0xFD;		//0xFD;		//0x01;
			
			if(pin_status)
			{
			
				ts+=400;
				acum_period+=400;

			}
			else
			{
				curr_ch++;
				if(curr_ch>=s_in.canales)					// Ha sido el ultimo
				{
					if(sync_ok)
					{
						int off = sync_ts2-sync_ts-6000;	// <0 atrasa salida respecto entrada
						if(off>100)
							off=100;
						else if(off<-100)
							off=-100;
							
						ts+=s_in.periodo-acum_period-off;
						sync_ok=0;			
					}
					else
						ts+=s_in.periodo-acum_period;
					
					acum_period=0;
					sync_ts2=ts;

					curr_ch=0;
				}
				else
				{
					char curr_ctrl;
					char copiar;

					if(curr_ch==storedConfig.Canal_PPM)
					{
						curr_ctrl = CTRL;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_ail)
					{
						curr_ctrl = AIL;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_ele)
					{
						curr_ctrl = ELE;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_thr)
					{
						curr_ctrl = THR;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_tail)
					{
						curr_ctrl = TAIL;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_pan)
					{
						curr_ctrl = PAN;
						copiar = 0;
					}
					else if(curr_ch==autopilotCfg.ch_aux)
					{
						curr_ctrl = AUX;
						copiar = 0;
					}
					else
						copiar = 1;
					
					if(copiar)
						valor_servo=servos_stream[curr_ch];
					else
					{
						if(curr_ctrl==PAN)
						{
							valor_servo=autopilotCfg.pan_gain*((int)get_servo_in(PAN)-SERVOS_THR)+SERVOS_THR;
							// Quitar si descomento abajo...
							if(valor_servo<SERVOS_MIN)
								valor_servo=SERVOS_MIN;
							else if(valor_servo>SERVOS_MAX)
								valor_servo=SERVOS_MAX;
				
						}
						// AUX_TILT, AUX_AIL2, AUX_AUTOPLT, AUX_WPT
						else if((curr_ctrl==AUX)&&(autopilotCfg.chAux_mode==AUX_TILT))
						{
							valor_servo=get_servo_in(curr_ctrl);
						}
						else if((curr_ctrl==AUX)&&((autopilotCfg.chAux_mode==AUX_AUTOPLT)||(autopilotCfg.chAux_mode==AUX_WPT)))
						{
							valor_servo=servos_out[curr_ctrl];					
						}
						else if(ikarusInfo.servoDriver==SERVO_REPLACE)
						{
							valor_servo=servos_out[curr_ctrl];
						}
						else if(ikarusInfo.servoDriver==SERVO_BITMASKS)
						{
							if(ikarusInfo.servos_bitmasks&mbit[curr_ctrl-1])
								valor_servo=servos_out[curr_ctrl];					
							else
								valor_servo=get_servo_in(curr_ctrl);
						}
						else if(ikarusInfo.servoDriver==SERVO_COPY||curr_ctrl==THR) // || curr_ch = TAIL-1)
						{
							valor_servo=get_servo_in(curr_ctrl);
						}
						else //if(ikarusInfo.servoDriver==SERVO_MIX)
						{
							valor_servo=servo_mix(curr_ctrl);
						}	
					}
					

					valor_servo-=400;
					ts+=valor_servo;
					acum_period+=valor_servo;
	
				}					
			}
		
			PCA0CPL1=ts&0xff;
			PCA0CPH1=(ts>>8)&0xff;
			CCF1=0;
		}
	}/* */
	else	// legacy code
	{

		if(CCF1)
		{
			static unsigned int last_ts2=0;
			ts=PCA0CP1;
			if(SERVOAIL)		// Aqui es Entrada TAIL
				last_ts2=ts;
			else
			{
				diff=ts-last_ts2;
				if(diff>SERVOS_MIN&&diff<SERVOS_MAX)
				{
					servos_in[TAIL]=diff;
					servos_lastin[TAIL]=diff;
					ikarusInfo.ppm_hold = 0;
				}
				else
					ikarusInfo.ppm_hold = 1;
			}
			CCF1=0;
		}	

		if(CCF2)
		{	
			static unsigned int last_ts1=0;
			ts=PCA0CP2;
			if(SERVOELE)		// Aqui es entrada THR
				last_ts1=ts;
			else
			{
				diff=ts-last_ts1;
				if(diff>SERVOS_MIN&&diff<SERVOS_MAX)
				{
					servos_in[THR]=diff;
					servos_lastin[THR]=diff;
					ikarusInfo.ppm_hold = 0;
				}
				else
					ikarusInfo.ppm_hold = 1;
			}
			CCF2=0;
		}

		if(CCF3)
		{
			char pin_status;
			char next_skip;
		
			ts=PCA0CP3;

			switch(curr_ch)
			{

				case 0: pin_status=SERVOTHR;
						next_skip=0xF4; 	
						if(ikarusInfo.servoDriver==SERVO_REPLACE)
						{
							valor_servo=servos_out[THR];
						}
						else 
						{
							//valor_servo=get_servo_in(THR);
							valor_servo=servos_in[THR];
						}
						break;

				case 1: pin_status=SERVOTAIL;
						next_skip=0xDC;		
						if(ikarusInfo.servoDriver==SERVO_REPLACE)
						{
							valor_servo=servos_out[TAIL];
						}
						else if(ikarusInfo.servoDriver==SERVO_BITMASKS)
						{
							if(ikarusInfo.servos_bitmasks&mbit[TAIL-1])
								valor_servo=servos_out[TAIL];					
							else
								valor_servo=get_servo_in(TAIL);
						}else 
						{
							//valor_servo=get_servo_in(TAIL);
							valor_servo=servos_in[TAIL];
						}
						break;

				case 2: pin_status=SERVOAUX;
						next_skip=0xF8;		
						if((autopilotCfg.chAux_mode==AUX_AUTOPLT)||(autopilotCfg.chAux_mode==AUX_WPT))
						{
							valor_servo=servos_out[AUX];					
						}
						else
						{
							valor_servo = 1500;
						}
						break;
			}

			if(pin_status)
			{
				
				ts+=valor_servo;
				acum_period+=valor_servo;

			}
			else
			{
				curr_ch++;
				if(curr_ch>=3)					// Ha sido el ultimo
				{
					/*
					if(sync_ok)
					{
						int off = sync_ts2-sync_ts-6000;	// <0 atrasa salida respecto entrada
						if(off>100)
							off=100;
						else if(off<-100)
							off=-100;
							
						ts+=s_in.periodo-acum_period-off;
						sync_ok=0;			
					}
					else*/
					
					ts+=PERIODO_SERVOS-acum_period;
					
					acum_period=0;
					sync_ts2=ts;

					curr_ch=0;
			    	P2SKIP    = 0xF8;	
			
				}
				else
				{
					ts+=400;
					acum_period+=400;

			    	P2SKIP    = next_skip; //SKIP[curr_ch];		// revisar 
				}					
			}
		
			PCA0CPL3=ts&0xff;
			PCA0CPH3=(ts>>8)&0xff;
			CCF3=0;
		}
	} // end legacy
}
