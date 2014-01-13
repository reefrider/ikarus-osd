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
#include "USB_API.h"
#include "Uplink.h"
#include "modem.h"
#include "Utils.h"
#include "Servos.h"


/*** [BEGIN] USB Descriptor Information [BEGIN] ***/
code const UINT USB_VID = 0x10C4;
code const UINT USB_PID = 0xEA61;
code const BYTE USB_MfrStr[] = {0x0E,0x03,'A',0,'T',0,'C',0,' ',0,'U',0,'S',0};                       // Manufacturer String
code const BYTE USB_ProductStr[] = {0x1C,0x03,'I',0,'K',0,'A',0,'R',0,'U',0,'S',0,' ',0,'U',0,'P',0,'L',0,'I',0,'N',0,'K',0}; // Product Desc. String
code const BYTE USB_SerialStr[] =  {0x0A,0x03,'9',0,'1',0,'0',0,'0',0};
code const BYTE USB_MaxPower = 15;            // Max current = 30 mA (15 * 2)
code const BYTE USB_PwAttributes = 0x80;      // Bus-powered, remote wakeup not supported
code const UINT USB_bcdDevice = 0x0100;       // Device release number 1.00
/*** [ END ] USB Descriptor Information [ END ] ***/


#define ACK_OUTPACKET() 	Block_Write(In_Packet, 64)
#define CLEAR_OUTPACKET()	memset(Out_Packet,'\0', 8)

extern xdata struct ServoStream s_in;
extern xdata struct ServoStream s_pc;
extern xdata struct Packet slots[MAX_MODEM_SLOTS];
extern xdata struct Sequence seq;

extern code struct StoredConfig storedConfig;

extern xdata char ServoMask[MAX_CH];

extern xdata float vsupply;

xdata BYTE In_Packet[64];
xdata BYTE Out_Packet[64];

xdata int timeout_usb;

void AutoUpdateSerie();

void InitUSB()
{
	
	int i;
	for(i=0;i<64;i++)
	{
		Out_Packet[i]=0;
		In_Packet[i]=0;
	}
	USB_Clock_Start();                     // Init USB clock *before* calling USB_Init
    USB_Init(USB_VID,USB_PID,USB_MfrStr,USB_ProductStr,USB_SerialStr,USB_MaxPower,USB_PwAttributes,USB_bcdDevice);
   	USB_Int_Enable();
}

void Silabs_Config()
{
	USB_Int_Disable();
	USB_Disable();

	// Puertos
	XBR0      = 0x01;
    XBR1      = 0x40;
	P0SKIP 	  = 0x00;
    P0MDOUT   = 0x20;

	// Timer 1
	TCON      = 0x51;
    TMOD      = 0x22;
    CKCON     = 0x0C;
    TH0       = 0xD0;
    TH1       = 0x30;
    TMR2CN    = 0x04;
    TMR2RLL   = 0xC0;
    TMR2RLH   = 0x63;
    TMR2L     = 0xC0;
    TMR2H     = 0x63;

	// Enable UART
	SCON0     = 0x10;

	
}

void atc_parser()
{
	int i;
	char ch=-1;
	bit rw;
	char dev;
	unsigned char id;
	unsigned char offset;
	unsigned char len;
	char *pram;

	if(Out_Packet[0]=='A'&&Out_Packet[1]=='T'&&Out_Packet[2]=='C')
	{
		timeout_usb = TIMEOUT_USB;

		if(Out_Packet[3]==0xff)
		{
			for(i=0;i<MAX_CH;i++)
			{
				ServoMask[i]=Out_Packet[4+i];
			}
			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xfe)
		{
			len=Out_Packet[4];
			if(len<8)
			{
				for(i=0;i<len;i++)
					s_pc.servos[Out_Packet[3*i+5]]=*(int*)&Out_Packet[3*i+6];
			}
			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xfd)	// Obsoleto
		{

			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xfc)
		{
	
			ram2rom((char volatile*) &storedConfig, (char*) &Out_Packet[4], sizeof(struct StoredConfig));
			flush_rom();
			
			s_pc.canales=storedConfig.canales;
			s_pc.periodo=storedConfig.periodo;
			s_pc.preamble=storedConfig.preamble;
			s_pc.lvl_idle=storedConfig.lvl_idle;

			ACK_OUTPACKET();
		
		}
		else if(Out_Packet[3]==0xfb)
		{
			ram2ram(In_Packet, (char*)&vsupply,sizeof(float));
			Block_Write(In_Packet, 64);		

		}
		else if(Out_Packet[3]==0xfa)		// Obsoleto
		{

			ACK_OUTPACKET();
		}
		else if(Out_Packet[3]==0xF0)
		{
			ACK_OUTPACKET();
			Silabs_Config();
			AutoUpdateSerie(); 
		}
		else
		{

			rw=(Out_Packet[3]&0x10)!=0;
			dev=Out_Packet[3]&0x0F;
			id=Out_Packet[4];
			offset=Out_Packet[5];
			len=Out_Packet[6];
			
			if(dev<4)	// Paginas en RAM
			{
				if(dev==0)	//gpsinfo
					pram=(void*)&s_pc;
				else if(dev==1)
					pram=(void*)&s_in;
				else if(dev==2)
					pram=(void*)&seq;
				else if(dev==3)
					pram=(void*)&slots[id];

				pram+=offset;

				if(rw==1)	//Write
				{
					for(i=0;i<len;i++)
						*(pram+i)=Out_Packet[7+i];
				}
				else
				{
					for(i=0;i<len;i+=64)
					{
						if(len-i>64)
							ram2ram(In_Packet, pram+i,64);
						else
							ram2ram(In_Packet, pram+i,len-i);

						Block_Write(In_Packet, 64);		

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