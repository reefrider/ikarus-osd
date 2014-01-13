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
 
#include "compiler_defs.h"
#include "C8051f340_defs.h"

/// Debe estar activado
#define SECURITY_FW




sbit LED_RED = P1^4;
sbit LED_GREEN = P1^6;

sbit SStreamOUT = P0^5;
sbit SStreamIN = P1^7;
sbit SStreamAUX= P2^0;


void Init_Device(void);
void InitUSB();
void atc_parser();

#define CLR_WDT()		PCA0CPH4=0x55
#define TIMEOUT_USB		200

#define TIMEOUT_S_AUX	20
#define TIMEOUT_S_IN	20


enum MASKS_VALUES{MASK_SERVO_IN, MASK_SERVO_PC, MASK_MODEM, MASK_PAN, MASK_TILT};

struct StoredConfig
{
	unsigned char canales;
	unsigned int periodo;
	unsigned int preamble;
	unsigned char lvl_idle;

	char pan_in;
	char pan_out;
	char tilt_in;
	char tilt_out;

	float lowVoltage;

	char usarEmisora;
	char usarPAN;
	char usarTILT;
};

