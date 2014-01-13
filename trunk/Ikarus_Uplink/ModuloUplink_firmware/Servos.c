#include "compiler_defs.h"
#include "C8051f340_defs.h"
#include "Uplink.h"
#include "modem.h"
/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
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

#include "Servos.h"
#include "Utils.h"

#define ENFASAR 	5000
#define MAX_DFASE	10

code const struct StoredConfig storedConfig _at_ 0xf000;

code const struct StoredConfig defaultConfig=
{
	8,				// Numero Canales
	22622, //	22127,			// Periodo
	400,			// Preambulo
	1,				// Polaridad PPM
	0xff,			// PAN_IN
	0xff,			// PAN_OUT
	0xff,			// TILT_IN
	0xff,			// TILT_OUT
	10.8f,			// Low Level
	1,				// Usar emisora
	0,
	0
};

xdata struct ServoStream s_in;
xdata struct ServoStream s_aux;
xdata struct ServoStream s_pc;
xdata char ServoMask[MAX_CH];

extern xdata int timeout_s_in, timeout_s_aux;
extern xdata int timeout_usb;


unsigned int last_sync_o;

unsigned int last_sync_i;
bit valid_sync_i=0;

xdata unsigned int servo_pan, servo_tilt;

void InitServos()
{
	int i;

	if(storedConfig.canales<4 || storedConfig.canales>MAX_CH)
	{
		struct StoredConfig tmp;
		tmp=defaultConfig;
		ram2rom((volatile char*)&storedConfig, (char*)&tmp, sizeof(struct StoredConfig));
		flush_rom();
	}
	

	InitServoStruct(&s_in);
	InitServoStruct(&s_pc);
	InitServoStruct(&s_aux);

	for(i=0;i<MAX_CH;i++)
		ServoMask[i] = MASK_SERVO_IN;

	if(storedConfig.usarPAN && storedConfig.pan_out<MAX_CH)
		ServoMask[storedConfig.pan_out] = MASK_PAN;

	if(storedConfig.usarTILT && storedConfig.tilt_out<MAX_CH)
		ServoMask[storedConfig.tilt_out] = MASK_TILT;

	timeout_s_in = 0;
	timeout_s_aux = 0;
	timeout_usb = 0;
}

void InitServoStruct(struct ServoStream *ss)
{
	int i;
	ss->canales=storedConfig.canales;
	ss->periodo=storedConfig.periodo;
	ss->preamble=storedConfig.preamble;
	ss->lvl_idle=storedConfig.lvl_idle;
	for (i=0;i<MAX_CH;i++)
	{
		ss->servos[i]=1500;
	}
} 

void IgualarStreams()
{
	s_pc.canales=s_in.canales;
	s_pc.periodo=s_in.periodo;
	s_pc.preamble=s_in.preamble;
	s_pc.lvl_idle=s_in.lvl_idle;
}


void PCA_handler() interrupt 11
{
	if(CCF0)
	{	
		static unsigned char i=0;
		static unsigned int total=0;
		if(i<s_pc.canales)
		{
			if(SStreamOUT!=s_pc.lvl_idle)
				PCA0CP0+=s_pc.preamble;
			else
			{
				unsigned int valor;

				if(timeout_usb==0)
				{
					valor=s_in.servos[i];
				}
				else
				{
					switch(ServoMask[i])
					{
						case 0:
						default:
							valor=s_in.servos[i];
							break;
			
						case 1:
							valor=s_pc.servos[i];
							break;

						case 2:
							valor=modem_get();
							break;
			
						case 3:
							valor=servo_pan;
							break;

						case 4:
							valor=servo_tilt;
							break;
					}
				}		
			
				if(valor<500||valor>2500)
				{
					valor=1500;
				}
				
				PCA0CP0+=(valor-s_pc.preamble);
				total+=valor;
				i++;
			}
		}
		else
		{
			if(SStreamOUT!=s_pc.lvl_idle) // Añadimos un pulso mas si fuera necesario
			{
				PCA0CP0+=s_pc.preamble;
				total+=s_pc.preamble;
			}
			else
			{	
				int r=0;

				if(storedConfig.usarEmisora && valid_sync_i)
				{
					r=last_sync_i-last_sync_o+ENFASAR;	// desfase. + si entrada adelantada
					if(r<-MAX_DFASE)
						r=-MAX_DFASE;
					else if(r>MAX_DFASE)
						r=MAX_DFASE;
					valid_sync_i=0;
		
					IgualarStreams();
		
				}
				else
				{
					r=0;
				} 

				PCA0CP0+=(s_pc.periodo-total)+r;
				last_sync_o=PCA0CP0;
				total=0;
				i=0;

				modem_update();
			}
		}
		CCF0=0;
	}
		
	if(CCF1) // Captura SERVO_IN!!!
	{	
		static unsigned int old_ts=0;
		static unsigned int total=0;
		static unsigned int last_preamble;
		static unsigned int i=0;
		unsigned int ts=PCA0CP1;
		unsigned int diff;

		diff=ts-old_ts;
		old_ts=ts;
	
		total+=diff;
		if(diff>5000)
		{
			if(i>=4 && i<MAX_CH)
			{
				s_in.canales=i;
				s_in.lvl_idle=!SStreamIN;
				s_in.periodo=total;

				timeout_s_in = TIMEOUT_S_IN;
			}
			i=0;
			total=0;
			last_sync_i=ts;
			valid_sync_i=1;
		}
		else
		{
			if(SStreamIN!=s_in.lvl_idle)
			{
				unsigned int servo=diff+last_preamble;
				if(servo>500&&servo<2500&&i<MAX_CH)
					s_in.servos[i]=servo;
				else
					i=0xFF;		// lock. Esperamos next sync
				if(i<MAX_CH)
					i++;
			}
			else
			{
				last_preamble=diff;
				s_in.preamble=(7*s_in.preamble+diff)/8;
			}
		}
		CCF1=0;
	}

	if(CCF2)		// Captura del pan&tilt
	{
		
		static unsigned int old_ts=0;
		static unsigned int total=0;
		static unsigned int last_preamble;
		static unsigned int i=0;
		unsigned int ts=PCA0CP2;
		unsigned int diff;

		diff=ts-old_ts;
		old_ts=ts;
	
		total+=diff;
		if(diff>5000)
		{
			if(i!=0xff)
			{
				s_aux.canales=i;
				s_aux.lvl_idle=!SStreamAUX;
				s_aux.periodo=total;
				/*
				if(storedConfig.pan_in<MAX_CH)
					servo_pan=s_aux.servos[storedConfig.pan_in];
				if(storedConfig.tilt_in<MAX_CH)
					servo_tilt=s_aux.servos[storedConfig.tilt_in];
				*/
				timeout_s_aux = TIMEOUT_S_AUX;
			}
			i=0;
			total=0;
			//last_sync_i=ts;
			//valid_sync_i=1;
		}
		else
		{
			if(SStreamAUX!=s_in.lvl_idle)
			{
				unsigned int servo=diff+last_preamble;
				if(servo>500&&servo<2500&&i<MAX_CH)
				{
					s_aux.servos[i]=servo;
					if(i==storedConfig.pan_in)
						servo_pan=s_aux.servos[storedConfig.pan_in];
					if(i==storedConfig.tilt_in)
						servo_pan=s_aux.servos[storedConfig.tilt_in];
						
				}
				else
					i=0xFF;		// lock. Esperamos next sync
				if(i<MAX_CH)
					i++;
			}
			else
			{
				last_preamble=diff;
				s_aux.preamble=(7*s_in.preamble+diff)/8;
			}
		}
		
		CCF2=0;
	}
}