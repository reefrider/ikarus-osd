
/* (c) 2009 Rafael Paz <rpaz@atc.us.es>
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

#include <stdio.h>
#include <string.h>
#include <absacc.h>

#include "c8051f340.h"
#include "USB_API.h"
#include "LibraryMax7456.h"
#include "ParserNMEA.h"
#include "Utils.h"
#include "Navigation.h"
#include "Ikarus.h"
#include "Servos.h"
#include "huds.h"
#include "MenuConfig.h"

/*** [BEGIN] USB Descriptor Information [BEGIN] ***/
code const UINT USB_VID = 0x10C4;
code const UINT USB_PID = 0xEA61;
code const BYTE USB_MfrStr[] = {0x0E,0x03,'A',0,'T',0,'C',0,' ',0,'U',0,'S',0};                       // Manufacturer String
code const BYTE USB_ProductStr[] = {0x16,0x03,'I',0,'K',0,'A',0,'R',0,'U',0,'S',0,' ',0,'O',0,'S',0,'D',0}; // Product Desc. String
code const BYTE USB_SerialStr[] = {0x0A,0x03,'9',0,'0',0,'0',0,'0',0};
code const BYTE USB_MaxPower = 15;            // Max current = 30 mA (15 * 2)
code const BYTE USB_PwAttributes = 0xC0;// <- selfpowered 0x80;      // Bus-powered, remote wakeup not supported
code const UINT USB_bcdDevice = 0x0100;       // Device release number 1.00
/*** [ END ] USB Descriptor Information [ END ] ***/

							
extern xdata float adc_values[]; // V2, I, V1, RSSI, temp , Co_X, Co_Y, Co_Z	
extern xdata unsigned int servos_in[];
extern xdata unsigned int servos_stream[];
extern xdata struct GPSInfo gpsinfo;
extern xdata struct IkarusInfo ikarusInfo;

xdata BYTE In_Packet[64];
xdata BYTE Out_Packet[64];

code struct StoredConfig storedConfig _at_ 0xF000;
code struct AutoPilotConfig autopilotCfg _at_ 0xF100;
code struct Ruta miRuta _at_ 0xF200;
code struct Screen huds[5] _at_ 0xF600;

code char lock_byte _at_ 0xFBFF;

void atc_parser() large;

enum Tipos{Type_RAM, Type_ROM, Type_Ext};

void Load_Params_Flash();

#define ACK_OUTPACKET() 	Block_Write(In_Packet, 64)
#define CLEAR_OUTPACKET()	memset(Out_Packet,'\0', 8)

void USB_Connection() 
{
	if(REG0CN&0x40)
	{
		CLEAR_OUTPACKET();

		USB_Clock_Start();                     // Init USB clock *before* calling USB_Init
	    USB_Init(USB_VID,USB_PID,USB_MfrStr,USB_ProductStr,USB_SerialStr,USB_MaxPower,USB_PwAttributes,USB_bcdDevice);
	   	USB_Int_Enable();

		ClrScr();
	
		printAtChr(0,0,'X');
		printAtChr(0,27,'X');
		if(storedConfig.Video_PAL==1)
		{
			printAtChr(14,0,'X');
			printAtChr(14,27,'X');
			printAtChr(15,28,'X');
		}
		else
		{
			printAtChr(12,0,'X');
			printAtChr(12,27,'X');
		}
		
		printCenteredAtStr(4,"USB attached");

		printCenteredAtStr(6,"WAITING FLIGHT PLAN");
		printCenteredAtStr(7,"FROM PC");

		while(REG0CN&0x40)
		{
			atc_parser();
		}

		Load_Params_Flash();
		USB_Int_Disable();
		USB_Disable();
		ClrScr();
	}
}
//enum Comandos{IkarusConfig, AutoPilotConfig, Ruta, Screen, gpsinfo, ikarusinfo, MAX7456};
void remote_cmds()
{
	char interactive=0;

	if(Out_Packet[4]==0x00)
	{
		MuestraHUD((Out_Packet[5]+1)|0x80);
	}
	else if(Out_Packet[4]==0x01)
	{
		sendValue(0x2,storedConfig.offsetX);		//hor offset
		sendValue(0x3,storedConfig.offsetY); 		//ver offset
		AutoDetectNTSC_PAL();
	}
	else if(Out_Packet[4]==0x02)
	{
		ClrScr();
	}
	else if(Out_Packet[4]==0x03)
	{
		printAtStr2(Out_Packet[5], Out_Packet[6], &Out_Packet[7],30);
	}
//	else if(Out_Packet[4]==0x04)
//	{
//		IkarusOsdAutoconfig(interactive);
//	}
//	else if(Out_Packet[4]==0x05)
//	{
//		AutopilotAutoconfig(interactive);	
//	}
	else if(Out_Packet[4]==0x06)
	{
		ACK_OUTPACKET();			// Damos el ACK antes de que no podamos
		wait_for(100);	//1seg
		USB_Int_Disable();
		USB_Disable();				// Desactivamos el USB
		
		MenuReflash();
	}
}

void atc_parser() large
{
	int i;
	bit rw;
	char dev;
	unsigned char id;
	unsigned char offset;
	unsigned char len;
	unsigned char reg_size;
	char tipo;

	char *pram;
	char volatile *prom;
	
	if(Out_Packet[0]=='A'&&Out_Packet[1]=='T'&&Out_Packet[2]=='C')
	{
		if(Out_Packet[3]==0xff)	// deberia ser otro remote comand
		{
			flush_rom();
			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xfe)
		{
			remote_cmds();
			ACK_OUTPACKET();
		}
		else
		{

			rw=(Out_Packet[3]&0x10)!=0;
			dev=Out_Packet[3]&0x0F;
			id=Out_Packet[4];
			offset=Out_Packet[5];
			len=Out_Packet[6];

			switch(dev)
			{
				case PG_IKARUSCFG: 	//IkarusConfig
					prom=(void*)&storedConfig;
					reg_size=sizeof(storedConfig);
					tipo=Type_ROM;
					break;

				case PG_AUTOPILOTCFG: 	// AutopilotConfig
					prom=(void*)&autopilotCfg;
					reg_size=sizeof(autopilotCfg);
					tipo=Type_ROM;
				
					break;

				case PG_RUTA:		// Ruta
					if(id==0xff)
					{
						prom=(void*)&miRuta.numwpt;
						reg_size=sizeof(int);
					}
					else
					{
						prom=(void*)&miRuta.wpts[id];
						reg_size=sizeof(struct WayPoint);
					}
					tipo=Type_ROM;
					break;		
			
				case PG_SCREEN: 	// Screens
					prom=(void*)&huds[id];
					reg_size=sizeof(struct Screen);
					tipo=Type_ROM;					
					break;

				case PG_GPSINFO:	//gpsinfo
					pram=(void*)&gpsinfo;
					reg_size=sizeof(gpsinfo);
					tipo=Type_RAM;
					break;

				case PG_IKARUSINFO:	
					pram=(void*)&ikarusInfo;
					reg_size=sizeof(ikarusInfo);
					tipo=Type_RAM;
					break;

				case PG_ADCVALUES:
					pram = (void*)&adc_values;
					reg_size=8*sizeof(float);
					tipo=Type_RAM;
					break;

				case PG_SERVOS:
					pram = (void*)&servos_in;
					reg_size=MAX_CH_IN*sizeof(int);
					tipo=Type_RAM;
					break;

				case PG_SERVOS_RAW:
					pram = (void*)&servos_stream;
					reg_size=MAX_CH_STRM_IN*sizeof(int);
					tipo=Type_RAM;
					break;

				case PG_CHARSET:
					tipo=Type_Ext;
					break;
			}

			
			if(offset+len>reg_size)
				len=reg_size-offset;

			prom+=offset;
			pram+=offset;

			if(rw==1)	//Write
			{
				switch(tipo)
				{
					case Type_ROM:
						if(len<=64-7)
							ram2rom(prom, &Out_Packet[7],len);
						break;
					
					case Type_RAM:
						if(len<=64-7)
							ram2ram(pram, &Out_Packet[7],len);
						break;

					case Type_Ext:
						WriteCharMemory2 (id,54, &Out_Packet[7]);
						break;
				}
			}
			else
			{
				switch(tipo)
				{
					case Type_ROM:
						for(i=0;i<len;i+=64)
						{
							if(len-i>64)
								rom2ram(In_Packet, prom+i,64);
							else
								rom2ram(In_Packet, prom+i,len-i);
							Block_Write(In_Packet, 64);	
						}
						break;
			
					case Type_RAM:
						for(i=0;i<len;i+=64)
						{
							if(len-i>64)
								ram2ram(In_Packet, pram+i,64);
							else
								ram2ram(In_Packet, pram+i,len-i);
							Block_Write(In_Packet, 64);	
						}
						break;

					case Type_Ext:
						ReadCharMemory (id,In_Packet);
						Block_Write(In_Packet, 64);	
						break;
				}
			}
			
			if(rw==1)
			{
				ACK_OUTPACKET();
			}
		}
		CLEAR_OUTPACKET();
	}
}

void  USB_API_TEST_ISR(void) interrupt 17
{
   BYTE INTVAL = Get_Interrupt_Source();

   if (INTVAL & RX_COMPLETE)
   {
      Block_Read(Out_Packet, 64);
	  // data_available=1;
   }
}