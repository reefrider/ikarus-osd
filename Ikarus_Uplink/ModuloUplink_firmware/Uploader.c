//#pragma src
/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
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
#include "c8051f340.h"
#include "uplink.h"


#define POLY	0x31
#define LFSR_INIT0	0x11
#define LFSR_INIT1	0x22
#define LFSR_INIT2	0x33
#define LFSR_INIT3	0x44

void AutoUpdateSerie() 
{
#ifndef	DUMMY
	unsigned char lfsr[4];
	unsigned char dato;
	unsigned char crc, rcrc;
	unsigned char paginas;
	unsigned char c,d,tmp;
	unsigned char lfsr_enable;
	int i, j, k;
	int dir;

	LED_RED=1;
	LED_GREEN=0;
	//115200 bps
//	TH1=0x30;
//	CKCON=(CKCON&0xF4)|0x09;

	EA=0;
		
	SBUF0='U';
	TI0=0;
	while(1)		// Ya no hay salida
	{
		i=0;
		while(i<4)
		{
			
			while(RI0==0)
			{
				if(j==0)
				{
					LED_RED=!LED_RED;
					LED_GREEN=!LED_GREEN;
				}
				j++;
				
				CLR_WDT();
			}
			dato=SBUF0;
			if(dato=='$')
				i=1;
			else if((dato=='A'&&i==1)||(dato=='T'&&i==2)||(dato=='C'&&i==3))
				i++;
			else
				i=0;
			RI0=0;
		}

		SBUF0='A';
		TI0=0;

		while(RI0==0)
			CLR_WDT();
		paginas=SBUF0;	
		RI0=0;
		lfsr_enable=0;

		if(paginas<126)		
		{
		SBUF0='E';		
		TI0=0;
			LED_RED=1;
			LED_GREEN=0;

			for(i=0;i<paginas;i++)
			{
				CLR_WDT();
				FLKEY=0xA5;
				FLKEY=0xF1;
				PSCTL=0x03; //PSEE=1 & PSWE =1;
				XBYTE [i<<9]=0xff;
				PSCTL=0;
			}

			LED_RED = 0;
			SBUF0='D';
			TI0=0;

			crc=0;
			
			lfsr[3]=LFSR_INIT0;
			lfsr[2]=LFSR_INIT1;
			lfsr[1]=LFSR_INIT2;
			lfsr[0]=LFSR_INIT3;

			dir=0;	
			for(i=0;i<paginas;i++)
			{
				LED_GREEN=!LED_GREEN;
				for(j=0;j<512;j++)
				{
					while(RI0==0)
						CLR_WDT();
					dato=SBUF0;
					RI0=0;
				
			    	crc = crc ^ dato;
				    for (k = 8; k > 0; k--)
				    {
				        crc = ((crc & 0x80) != 0) ? (crc<< 1) ^ POLY : crc << 1;
				    }
					
					CLR_WDT();
					
					/* taps: 32 31 29 1; characteristic polynomial: x^32 + x^31 + x^29 + x + 1 */
 					//lfsr = (lfsr >> 1) ^ (unsigned long)(0 - (lfsr & 1u) & 0xd0000001u); 
 				
					tmp=lfsr[0]&1;		// 0 -> lsb, 3 -> msb
					c=lfsr[3]&1;
					lfsr[3]>>=1;
					lfsr[3]^=tmp?0xd0:0;
					d=lfsr[2]&1;
					lfsr[2]>>=1;
					lfsr[2]|=c?0x80:0;
					c=lfsr[1]&1;
					lfsr[1]>>=1;
					lfsr[1]|=d?0x80:0;
					d=lfsr[0]&1;
					lfsr[0]>>=1;
					lfsr[0]|=c?0x80:0;
					lfsr[0]^=tmp?0x1:0;
					
					dato = dato ^ lfsr[0];
					
					CLR_WDT();

					PFE0CN&=0xFE;		
					PSCTL=0x01; //PSEE=0 & PSWE =1;	
					FLKEY=0xA5;
					FLKEY=0xF1;
					XBYTE[dir]=dato;	// Writes clear full page
					PSCTL=0;
					dir++;
				}
			}
			SBUF0='X';
			TI0=0;

			while(RI0==0)
				CLR_WDT();
			rcrc=SBUF0;	
			RI0=0;
			if(crc==rcrc)
			{
				SBUF0='R';
				TI0=0;
				RSTSRC|=0x10;		// Software Reset
			}
			SBUF0='B';
			TI0=0;
		}
	} 
#endif
}
