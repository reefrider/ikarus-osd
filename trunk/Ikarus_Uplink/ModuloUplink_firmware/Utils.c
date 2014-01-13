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

#include <absacc.h>
#include "Uplink.h"

xdata char pageRAM[512];
xdata int pageRAMaddr=0;


//extern code const struct StoredConfig storedConfig;

/*
extern xdata unsigned long tics;

void wait_for(int t)
{
	long now=tics;
	while(tics<t+now);
}

int NumOfCells(float v) large
{
	if(v>12.7)
		return 4;
	else if (v>8.6)
		return 3;
	else if(v>4.5)
		return 2;
	else if (v>2.0)
		return 1; 
	else 
		return 0;
}
*/
void flash_clear(char volatile * pos) large
{
	EA=0;
	FLKEY=0xA5;
	FLKEY=0xF1;
	PSCTL=0x03; //PSEE=1 & PSWE =1;
	XBYTE [(int)pos]=0xff;
	PSCTL=0;
	EA=1;
}

void flash_write(char volatile * pos, char dato) large
{
	EA=0;
	PFE0CN&=0xFE;			// ponia |. creo q estaba mal. comprobar. rafa
	PSCTL=0x01; //PSEE=0 & PSWE =1;

	FLKEY=0xA5;
	FLKEY=0xF1;
	XBYTE[(int)pos]=dato;	// Writes clear full page
	PSCTL=0;

	EA=1;
}

char flash_read(char volatile * pos) large
{
	return CBYTE[(int)pos];
}
			
void ram2flash(char volatile *dst, char *src, int len) large
{
	int i;
	for(i=0;i<len;i++)
		flash_write(dst+i,*(src+i));
}

void flash2ram(char *dst, char volatile *src, int len) large
{
	int i;
	for(i=0;i<len;i++)
		*(dst+i)=flash_read(src+i);
}

void flush_rom()
{
	if(pageRAMaddr!=0)
	{
		flash_clear((void*)pageRAMaddr);
		ram2flash((void*)pageRAMaddr, pageRAM,512);
		pageRAMaddr=0;
	}
}

void write_rom(char volatile *dst, char dato)
{
	int page =(int)dst&0xfe00;
	int offset=(int)dst&0x1ff;

	if (page&0xf000!=0xf000)
		return;

	if(page!=pageRAMaddr)
	{
		flush_rom();
		flash2ram(pageRAM, (void*)page,512);
		pageRAMaddr=page;
	}
	pageRAM[offset]=dato;
}
/*
char read_rom(char volatile *dst)
{
	char volatile *page =(long)dst&0xfffe00;
	int offset=(int)dst&0x1ff;
	char dato;

	if(page!=pageRAMaddr)
		dato=flash_read(dst);
	else
		dato=pageRAM[offset];

	return dato;
}
*/
void ram2rom(char volatile *dst, char *src, int len) large
{
	int i;
	for(i=0;i<len;i++)
		write_rom(dst+i,*(src+i));
}
/*
void rom2ram(char *dst, char volatile *src, int len) large
{
	int i;
	for(i=0;i<len;i++)
		*(dst+i)=read_rom(src+i);
}
*/
void ram2ram(char *dst, char *src, int len) large
{
	int i;
	for(i=0;i<len;i++)
		*(dst+i)=*(src+i);
}
