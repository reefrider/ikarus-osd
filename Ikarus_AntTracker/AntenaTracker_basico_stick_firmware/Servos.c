/* 
 * (c) 2011 Rafael Paz <rpaz@atc.us.es>
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

#include "c8051f340.h"
#include "Servos.h"
#include "AntTracker.h"
#include "Utils.h"


extern code struct StoredConfig storedConfig;
xdata unsigned int servos[MAX_CH_OUT]={1500, 1500, 1500};


float interpolar(float angle, float grados_center, float grados_minmax, int servo_center, servo_minmax)
{
	return (angle-grados_center)/(grados_minmax-grados_center)*(servo_minmax-servo_center)+servo_center;
}

void set_servo(char ch, int valor) large
{
	if(ch<3 && valor>300 && valor<2800)
		servos[ch]=valor;		
}

void set_servo_tilt(float angle) large
{
	float grados_max, grados_min, grados_center;
	int servos_max, servos_min, servos_center;

	float valor;

	grados_max=storedConfig.TILTmax.grados;
	grados_min=storedConfig.TILTmin.grados;
	grados_center=storedConfig.TILTcenter.grados;

	servos_max=storedConfig.TILTmax.servo;
	servos_min=storedConfig.TILTmin.servo;
	servos_center=storedConfig.TILTcenter.servo;

	angle=Adjust180(angle);

	if(angle>grados_max)
		valor = servos_max;
	else if(angle<grados_min)
		valor = servos_min;
	else if(angle>=grados_center)
		valor=(angle-grados_center)/(grados_max-grados_center)*(servos_max-servos_center)+servos_center;
	else
		valor=(angle-grados_center)/(grados_min-grados_center)*(servos_min-servos_center)+servos_center;

	servos[SERVO_TILT] = valor;
}

void set_servo_pan(float angle) large
{
	static float last_angle=0;
	float valor, angle2;

	angle=Adjust180(angle);

	if(storedConfig.useServo360==0)
	{
		if(angle  >storedConfig.PANmax.grados)
			valor = storedConfig.PANmax.servo;
		else if(angle > storedConfig.PANcenter.grados)
			valor=interpolar(angle, storedConfig.PANcenter.grados, storedConfig.PANmax.grados, storedConfig.PANcenter.servo, storedConfig.PANmax.servo);
		else if(angle > storedConfig.PANmin.grados)
			valor=interpolar(angle, storedConfig.PANcenter.grados, storedConfig.PANmin.grados, storedConfig.PANcenter.servo, storedConfig.PANmin.servo);
		else 
			valor = storedConfig.PANmin.servo;
	}
	else
	{
		if(last_angle>storedConfig.PANcenter.grados)
		{
			angle2=angle+360.0f;
			if(angle2>=storedConfig.PANmax.grados && angle2 <=storedConfig.PANmax2.grados)
			{
				angle=angle2;
			}				
		}
		else if(last_angle<storedConfig.PANcenter.grados)
		{
			angle2=angle-360.0f;
			if(angle2>=storedConfig.PANmin2.grados && angle2 <=storedConfig.PANmin.grados)
			{
				angle=angle2;
			}	
		}
		last_angle= angle;

		if(angle > storedConfig.PANmax2.grados)
			valor = storedConfig.PANmax2.servo;
		else if(angle > storedConfig.PANmax.grados)
		{
			valor = interpolar(angle, storedConfig.PANmax.grados, storedConfig.PANmax2.grados, storedConfig.PANmax.servo, storedConfig.PANmax2.servo);
		}
		else if(angle > storedConfig.PANcenter.grados)
		{
			valor=interpolar(angle, storedConfig.PANcenter.grados, storedConfig.PANmax.grados, storedConfig.PANcenter.servo, storedConfig.PANmax.servo);
		}
		else if(angle > storedConfig.PANmin.grados)
		{
			valor=interpolar(angle, storedConfig.PANcenter.grados, storedConfig.PANmin.grados, storedConfig.PANcenter.servo, storedConfig.PANmin.servo);
		}
		else if(angle > storedConfig.PANmin2.grados)
		{
			valor=interpolar(angle, storedConfig.PANmin.grados, storedConfig.PANmin2.grados, storedConfig.PANmin.servo, storedConfig.PANmin2.servo);
		}
		else
			valor = storedConfig.PANmin2.servo;
		

	}
	servos[SERVO_PAN] = valor;
}


void PCA_handle() interrupt 11
{		
	unsigned int ts;

	if(CCF0)
	{
		ts=PCA0CP0;

		if(SERVO0)
		{
			ts+=servos[0];
		}
		else
		{
			ts+=PERIODO_SERVOS-servos[0];
		}
			
		PCA0CPL0=ts&0xff;
		PCA0CPH0=(ts>>8)&0xff;
		
		CCF0=0;
	}

	if(CCF1)
	{
		ts=PCA0CP1;

		if(SERVO1)
		{
			ts+=servos[1];
		}
		else
		{
			ts+=PERIODO_SERVOS-servos[1];
		}
			
		PCA0CPL1=ts&0xff;
		PCA0CPH1=(ts>>8)&0xff;
		
		CCF1=0;
	}
}
