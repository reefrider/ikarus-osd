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

struct PID
{
	float P;
	float I;
	float D;
	
	float ILimit;
	float DriveLimit;

	float set_point;

	float last_error;	// Used to compute Derivative
	float acum_error;	// Used to compute Integral
	// float input;
	float resultado;
};

void InitPID(struct PID *pid, float P, float I, float D);

float ControlPID(struct PID *pid, float y) large;
float ControlP(float gainP, float set_point, float DriveLimit, float y) large;
float limit(float value, float min, float max) large;