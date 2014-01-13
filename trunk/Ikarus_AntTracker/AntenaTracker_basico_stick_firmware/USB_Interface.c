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

#include <string.h>
#include "c8051f340.h"
#include "USB_API.h"
#include "AntTracker.h"
#include "Utils.h"

/*** [BEGIN] USB Descriptor Information [BEGIN] ***/
code const UINT USB_VID = 0x10C4;
code const UINT USB_PID = 0xEA61;
code const BYTE USB_MfrStr[] = {0x0E,0x03,'A',0,'T',0,'C',0,' ',0,'U',0,'S',0};                       // Manufacturer String
code const BYTE USB_ProductStr[] = {0x20,0x03,'I',0,'K',0,'A',0,'R',0,'U',0,'S',0,' ',0,'A',0,'N',0,'T',0,'T',0,'R',0,'A',0,'C',0,'K',0}; // Product Desc. String
code const BYTE USB_SerialStr[] =  {0x0A,0x03,'9',0,'2',0,'0',0,'0',0};
code const BYTE USB_MaxPower = 150;            // Max current = 30 mA (15 * 2)
code const BYTE USB_PwAttributes = 0x80;      // Bus-powered, remote wakeup not supported
code const UINT USB_bcdDevice = 0x0100;       // Device release number 1.00
/*** [ END ] USB Descriptor Information [ END ] ***/

enum Tipos{Type_RAM, Type_ROM, Type_Ext};
enum COMANDOS{PG_ANTTRACKERCFG, PG_DATOSAVION, PG_DATOSANTENA, PG_TELEMETRY, PG_DEBUG};


#define ACK_OUTPACKET() 	Block_Write(In_Packet, 64)
#define CLEAR_OUTPACKET()	memset(Out_Packet,'\0', 8)

xdata BYTE In_Packet[64];
xdata BYTE Out_Packet[64];

extern xdata struct Debug debugInfo;

extern xdata struct DatosAntena datosAntena;
extern xdata struct DatosAvion datosAvion;
extern xdata char UpdatedDatosAvion;

//xdata struct Telemetry_PKT pkt_unchecked;
code struct StoredConfig storedConfig _at_ 0xF000;

void InitUSB()
{
	CLEAR_OUTPACKET();
	USB_Clock_Start();                     // Init USB clock *before* calling USB_Init
    USB_Init(USB_VID,USB_PID,USB_MfrStr,USB_ProductStr,USB_SerialStr,USB_MaxPower,USB_PwAttributes,USB_bcdDevice);
   	USB_Int_Enable();
}

void Silabs_Config()
{
	CLEAR_OUTPACKET();
	USB_Int_Disable();
	USB_Disable();
	XBR1&=0xF8;		// = 0x40
	AutoUpdateSerie(); 
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

	char invalid = 0;


	if(Out_Packet[0]=='A'&&Out_Packet[1]=='T'&&Out_Packet[2]=='C')
	{
		if(Out_Packet[3]==0xff)	// deberia ser otro remote comand
		{
			flush_rom();
			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xf0)
		{
			ACK_OUTPACKET();
			Silabs_Config();
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
				case PG_ANTTRACKERCFG: 	//IkarusConfig
					prom=(void*)&storedConfig;
					reg_size=sizeof(storedConfig);
					tipo=Type_ROM;
					break;

				case PG_DATOSAVION: 	
					pram=(void*)&datosAvion;
					reg_size=sizeof(struct DatosAvion);
					tipo=Type_RAM;
					UpdatedDatosAvion=1;					
					break;

				case PG_DATOSANTENA:	
					pram=(void*)&datosAntena;
					reg_size=sizeof(struct DatosAntena);
					tipo=Type_RAM;
					break;
/*		
				case PG_TELEMETRY:	
					pram=(void*)&pkt_unchecked;
					reg_size=sizeof(struct Telemetry_PKT);
					tipo=Type_RAM;
					break;
*/
				case PG_DEBUG:
					pram = (void*)&debugInfo;
					reg_size=sizeof(struct Debug);
					tipo=Type_RAM;
					break;

				default:
					invalid = 1;
					break;
			}

			if(!invalid)
			{
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
					}
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