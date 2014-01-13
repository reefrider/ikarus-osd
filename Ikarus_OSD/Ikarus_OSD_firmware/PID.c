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

#include "pid.h"

#define DT 0.1f

void InitPID(struct PID *pid, float P, float I, float D)
{
	pid->P=P;
	pid->I=I;
	pid->D=D;
	pid->ILimit=1.0f;
	pid->DriveLimit=1.0f;
	pid->set_point=0.0f;
	pid->last_error=0.0f;
	pid->acum_error=0.0f;	
	pid->resultado=0.0f;
}


float ControlP(float gainP, float set_point, float DriveLimit, float y) large
{
	xdata float drive, pTerm;

    pTerm = (set_point - y) * gainP;
	drive = limit(pTerm, -DriveLimit, DriveLimit); // + offset;  
    return drive;
}

float ControlPID(struct PID * pid, float y) large
{
    xdata float drive, error, pTerm, iTerm, dTerm;

    error = pid->set_point - y;

	pid->acum_error = limit(pid->acum_error + error*DT, -pid->ILimit, pid->ILimit);

    pTerm = error * pid->P;

	// Anti windup
    if (pTerm > pid->DriveLimit || pTerm < -pid->DriveLimit)
     pid->acum_error = 0.0f;

    iTerm = pid->acum_error*pid->I;
    dTerm = pid->D * (error - pid->last_error)/DT;

    drive = pTerm + iTerm + dTerm;

	pid->resultado = drive;    
    pid->last_error = error;

    return limit(drive, -pid->DriveLimit, pid->DriveLimit);
}

float limit(float value, float min, float max) large
{
	if (value < min)
		return min;
	else if (value > max)
		return max;
	else
		return value;
}

