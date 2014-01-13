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

#define MAX_CH_STRM_IN	11
#define MAX_CH_IN	 7
#define MAX_CH_OUT	 7

#define SERVOS_MAX  2800
#define SERVOS_MIN	 400

#define SERVOS_THR	1500
#define HIST		50

struct ServoStreamParams
{
	unsigned char canales;
	unsigned int periodo;
	unsigned int preamble;
	unsigned char lvl_idle;
//	unsigned int servos[MAX_CH];
};

enum Servos
	{
	CTRL=0,
	AIL=1,
	ELE=2, 	
	THR=3, 
	TAIL=4,
	PAN=5,
	TILT=6 
	};

char switch_pos(unsigned char ch);
int switch_changed(unsigned char ch);
void switch_set(unsigned char ch, char value);

void set_servof(unsigned char ch, float valor);

//int get_servo(unsigned char ch) large reentrant;
//void set_servo(unsigned char ch, int valor) large reentrant;
//float get_servof(unsigned char ch) large reentrant;
//void set_servofe(unsigned char ch, float v, int min, int center, int max) large;

void SimulaSERVO();