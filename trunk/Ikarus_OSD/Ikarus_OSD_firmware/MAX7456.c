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

#include "c8051f340.h"
#include "Ikarus.h"

////////////////////////////////////////////////////////////////////////////////////////
//
// Low level SPI functions
//
xdata unsigned char regDMM=0x00;

char sendByte(char byte){

  while(!TXBMT);  //wait for xmit buffer empty
  SPI0DAT=byte;

  while(!SPIF); //wait for end of transmition
  SPIF=0;  //clear SPIF
  
  return SPI0DAT;
}


void sendValue(unsigned char dir, unsigned char value){
	NSS=0; //Transferimos los 3 bytes.
	sendByte(dir);
	sendByte(value);
	NSS=1;	
}
/*
void sendValue16(unsigned char dir, unsigned char msb, unsigned char lsb){
	NSS=0; //Transferimos los 3 bytes.
	sendByte(dir);
	sendByte(msb);
	sendByte(lsb);
	NSS=1;	
}
*/
char readValue(unsigned char dir)
{
	char tmp;
	NSS=0;
	sendByte(dir);
	tmp=sendByte(0x00);
	NSS=1;
	return tmp;
}

////////////////////////////////////////////////////////////////////////////////////////
//
// High level MAX7456 OSD functions
//
/*

void ClrScr()
{
	sendValue(0x4,readValue(0x84)|0x4);	
}

void CharColorWhite()
{
	regDMM &= ~0x08;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

void CharColorBlack()
{
	regDMM |= 0x08;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}
*/
void CharAttrBlink()
{
	regDMM |= 0x10;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

void CharAttrNoBlink()
{
	regDMM &= ~0x10;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

void CharAttrBackGr()
{
	regDMM |= 0x20;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

void CharAttrNoBackGr()
{
	regDMM &= ~0x20;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

/*
void CharAttrAutoInc()
{
	regDMM |= 0x01;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}

void CharAttrNoAutoInc()
{
	regDMM &= ~0x01;
	sendValue(0x4,regDMM);		// +8->invert, +20->background, +10->blink
}
*/

void WriteCharMemory2(unsigned char c, unsigned char len, unsigned char buff[64])large
{
	xdata int i;
	xdata unsigned char r;

	sendValue(0x00,0x00);		// OSD off
	sendValue(0x09,c);

	for(i=0;i<len;i++)
	{
		sendValue(0x0A,i);
		sendValue(0x0B,buff[i]);
	}
	
	sendValue(0x08,0xAA);		// Magic number. Flush to flash
	do
	{
	r=readValue(0xA0);
	} while(r&0x20);

	sendValue(0x00, 0x48);		// OSD on + PAL

}

/*
void WriteCharMemory(unsigned char c, unsigned char buff[64])large
{
	WriteCharMemory2(c,64,buff);

}
*/
void ReadCharMemory(unsigned char c, unsigned char buff[64])large
{
	xdata int i;

	sendValue(0x00,0x00);		// OSD off
	sendValue(0x09,c);			//CMAH

	sendValue(0x08,0x55);		// Magic number. Flush to flash
	
	for(i=0;i<64;i++)
	{
		sendValue(0x0A,i);
		buff[i]=readValue(0xCF);
	}
	
	sendValue(0x00, 0x48);		// OSD on + PAL
}
