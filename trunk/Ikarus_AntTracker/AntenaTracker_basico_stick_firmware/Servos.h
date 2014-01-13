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

#define MAX_CH_OUT	 3

#define SERVOS_MAX  2800
#define SERVOS_MIN	 400

#define SERVOS_THR	1500

#define PERIODO_SERVOS 20000

enum Sevos{SERVO_PAN, SERVO_TILT, SERVO_AUX};

void set_servo_tilt(float angle) large;
void set_servo_pan(float angle) large;

void set_servo(char ch, int valor) large;

void SoundOn(int freq);
void SoundOff();