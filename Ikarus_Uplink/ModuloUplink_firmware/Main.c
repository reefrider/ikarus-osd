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
#include "Uplink.h"

#include "modem.h"
#include "Servos.h"
#include "Utils.h"

#define VOLTAGE_ALARM 3.4f
#define TOP_DIVIDER	100

code char lock_byte _at_ 0xFBFF;

xdata float vsupply;

xdata int timeout_s_in, timeout_s_aux;

extern code struct StoredConfig storedConfig;

extern xdata struct Sequence seq;
extern xdata int pageRAMaddr;
extern xdata int timeout_usb;


void main()
{
	Init_Device();
	pageRAMaddr=0;

	#ifdef SECURITY_FW
	if(lock_byte==-1)
		flash_write(&lock_byte, -125);		// -120 dejar sin proteger datos. -125 todo		
	#else
	#warning Codigo desprotegido!!! Debe activarse la proteccion de codigo

	#endif

	InitUSB();

	modem_init();
	InitServos();

	while(1)
	{
		atc_parser();
		CLR_WDT();
	}
}

void Timer2_handler() interrupt 5
{	
	static int divider=0;
	int max_divider = TOP_DIVIDER;

	if(vsupply<storedConfig.lowVoltage && divider >max_divider/2)
	{
		LED_RED=0;
		LED_GREEN=0;
	}
	else if(timeout_s_in>0 && timeout_s_aux>0)
	{
		LED_RED=1;
		LED_GREEN=1;
	}
	else if(timeout_s_in>0)
	{
		LED_RED=0;
		LED_GREEN=1;
	}
	else
	{
		LED_RED=1;
		LED_GREEN=0;
	}

	if(timeout_s_in>0)
		timeout_s_in--;

	if(timeout_s_aux>0)
		timeout_s_aux--;

	if(timeout_usb>0)
		timeout_usb--;

	if(divider<max_divider)
		divider++;
	else
		divider=0;


	CLR_WDT();
   	TF2H=0;
}

void ADC_handler() interrupt 10
{

	if(AD0INT)
	{
		vsupply=(14+4.7)/4.7*3.3f*ADC0H/255.0f;
		AD0INT=0;
	}

}