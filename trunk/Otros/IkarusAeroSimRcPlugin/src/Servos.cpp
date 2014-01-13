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

#include "Ikarus.h"
#include "Servos.h"
#include "Modem.h"
#include "Utils.h"
#include "Controladores.h"
//#include "filtroSimple.h"


extern code struct AutoPilotConfig autopilotCfg;
code struct ServoInfo * servos_cfg=autopilotCfg.servos_cfg;

xdata unsigned int servos_in[MAX_CH_IN]={1500, 1500, 1500, 1500, 1500,   1500, 1500};
xdata unsigned int servos_out[MAX_CH_OUT]={0, 1500, 1500, 1500, 1500,   1500, 1500};

bit failsafe;

unsigned char switch_changes[2]={0,0};
unsigned char switch_old[2]={0,0};	


extern const TDataFromAeroSimRC *ptDataFromAeroSimRC;
extern TDataToAeroSimRC *ptDataToAeroSimRC;

char switch_pos(unsigned char ch)
{
	if(ch==0||ch==1)
		return switch_old[ch]; 
	else
		return 0;
}

int switch_changed(unsigned char ch)
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

void switch_set(unsigned char ch, char value) 
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

void SimulaSERVO()
{	/*
	switch(ch)
	{
		case AIL: 
			channel = CH_AILERON;
			break;

		case ELE:
			channel = CH_ELEVATOR;
			break;

		case THR:
			channel = CH_THROTTLE;
			break;

		case TAIL:
			channel = CH_RUDDER;
			break;
	}
	*/
	servos_in[CTRL]=1500-500*ptDataFromAeroSimRC->Channel_afValue_TX[CH_PLUGIN_1];
	servos_in[AIL]=1500+500*ptDataFromAeroSimRC->Channel_afValue_TX[CH_AILERON];
	servos_in[ELE]=1500+500*ptDataFromAeroSimRC->Channel_afValue_TX[CH_ELEVATOR];
	servos_in[THR]=1500+500*ptDataFromAeroSimRC->Channel_afValue_TX[CH_THROTTLE];
	servos_in[TAIL]=1500+500*ptDataFromAeroSimRC->Channel_afValue_TX[CH_RUDDER];
}

/*
int get_servo(unsigned char ch) large reentrant
{
	if(ch>=0&&ch<MAX_CH_IN)
		return servos_in[ch]; 
	else 
		return 0;
}

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
void set_servof(unsigned char ch, float v) 
{
	int channel;
	switch(ch)
	{
		case AIL: 
			channel = CH_AILERON;
			break;

		case ELE:
			channel = CH_ELEVATOR;
			break;

		case THR:
			channel = CH_THROTTLE;
			break;

		case TAIL:
			channel = CH_RUDDER;
			break;
	}
	ptDataToAeroSimRC->Channel_afNewValue_TX[channel]=v;
}

/*
float get_servof(unsigned char ch) large reentrant 
{
	int v=get_servo(ch);
	struct ServoInfo serv;
	float t;
	if(ch<0||ch>2)
		return 0.0;
	serv=servos_cfg[ch];
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
*/
