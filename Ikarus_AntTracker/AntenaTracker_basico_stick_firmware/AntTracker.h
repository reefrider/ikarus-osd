/* 
 * (c) 2011 Rafael Paz <rpaz@atc.us.es>
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
#include "c8051f340.h"

sfr16 ADC0		= 0xBD;
sfr16 TMR2RL	= 0xCA;
sfr16 PCA0CP1	= 0xE9;
sfr16 PCA0CP2	= 0xEB;
sfr16 PCA0CP3	= 0xED;
sfr16 PCA0		= 0xF9;
sfr16 PCA0CP0	= 0xFB;
sfr16 PCA0CP4	= 0xFD;

#define CLR_WDT()		PCA0CPH4=0x55

sbit LED = P2^2;

sbit SERVO0 = P1^3;
sbit SERVO1 = P1^4;

sbit BUTTON0_RMT = P1^2;
sbit BUTTON1_RMT = P1^5;

/*sbit AUX1 = P2^2;
sbit AUX2 = P2^1;
sbit AUX3 = P2^3;		// Analogica?
sbit AUX4 = P2^4;		// Analogica?

sbit AUXX = P3^0;*/

void BuclePrincipal();


struct GradosServo
{
	float grados;
	int servo;
};

struct StoredConfig
{
	char useInternalGPS;
    float AltitudSinGPS;

    char useInternalCompas;
    float offsetCompas;
    char enableCompasOverride;
    float speedCompasOverride;

    char useLipo;
    char numCellsLipo;
    float Vmin;
    float Vmax;
    float Valarm;

    float offsetPan;

    char decodeTelemetry;

	char useServo360;

	struct GradosServo PANmin2;
	struct GradosServo PANmin;
	struct GradosServo PANcenter;
	struct GradosServo PANmax;
	struct GradosServo PANmax2;

	struct GradosServo TILTmin;
	struct GradosServo TILTcenter;
	struct GradosServo TILTmax;

};

struct DatosAntena
{
    char tieneGPS;
	char tienePOS;
    float lon;
    float lat;
    int alt;

	float v_bateria;
	float v_bateria_porcentaje;
	char v_bateria_alarm;

};

struct DatosAvion
{
    float lon;
    float lat;
    int alt;

    float home_lon;
    float home_lat;
    int home_alt;
};

struct Debug
{
	char EnableDebug;
	unsigned int pan;
	unsigned int tilt;
	float grados_pan;
	float grados_tilt;
};
