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

#include "Servos.h"
#include "Modem.h"
#include "Ikarus.h"
#include "PID.h"

#define MAX_PACKET_ID	0x0F

extern xdata struct PID elevatorPID;
extern xdata struct PID aileronsPID;
extern xdata struct PID tailPID;
extern xdata struct PID motorPID;

extern xdata float modem_irxoff, modem_iryoff, modem_irmax, modem_ail, modem_ele;

code const unsigned char bin2gray5[32]= {
    0x00, 0x01, 0x03, 0x02, 0x06, 0x07, 0x05, 0x04,
    0x0C, 0x0D, 0x0F, 0x0E, 0x0A, 0x0B, 0x09, 0x08,
    0x18, 0x19, 0x1B, 0x1A, 0x1E, 0x1F, 0x1D, 0x1C,
    0x14, 0x15, 0x17, 0x16, 0x12, 0x13, 0x11, 0x10};

code const unsigned char gray2bin5[32]={
    0x00, 0x01, 0x03, 0x02, 0x07, 0x06, 0x04, 0x05,
    0x0F, 0x0E, 0x0C, 0x0D, 0x08, 0x09, 0x0B, 0x0A,
    0x1F, 0x1E, 0x1C, 0x1D, 0x18, 0x19, 0x1B, 0x1A,
    0x10, 0x11, 0x13, 0x12, 0x17, 0x16, 0x14, 0x15};

code const char pkt_lens[MAX_PACKET_ID+1]={1,2,4,4,/**/0,0,0,0,/**/0,0,0,0,/**/0,0,0,5};

extern xdata struct IkarusInfo ikarusInfo;
extern xdata unsigned char rflost;
xdata char data_frame[16];	// STA ID DATA[1,2,4 o 8] STO
xdata char last_clk;
xdata char i_frame;

void modem_init()
{
	int i;
	for(i=0;i<16;i++)
		data_frame[i]=0;
	i_frame=-1;
	last_clk=0;
}

void modem_rx(int rx) large
{
	const char bits=5;	
	int valores = 1<<bits;
	static char offset=0;

	float rango=(2000-1000)/(valores-1.0);
	float v=rx-1000-offset+rango/2;	// aparece 8us mas amplio de lo q le mandamos

	char dato=(char)(v/rango);

	static char midato;
	static char pkt_len;
	static unsigned char crc4;

	char clk=(dato&0x10)>>4;

	//if(dato<0)
	if(rx>750&&rx<850)
	{
		last_clk=0;
		i_frame=0;
		offset=rx-800;
		crc4=0;
	}
	else if(i_frame==0 && clk==0)
	{
		dato = bin2gray5[dato&0x1f] & 0x0f;
		data_frame[0]=dato;
		pkt_len=pkt_lens[dato];
		calc_crc4(dato,&crc4);
		last_clk=clk;				
		i_frame++;
	}
	else if(clk!=last_clk&&i_frame>0)
	{
		rflost=0;
		dato = bin2gray5[dato&0x1f] & 0x0f;
		if(clk==1)
		{
			if(i_frame>pkt_len) // i_frame==pkt_len+1
			{
				if(crc4==dato)
					modem_Analize();
				i_frame=-1;
			}
			else
				midato=dato<<4;
		}
		else
		{
			midato|=dato;
			data_frame[i_frame]=midato;
			i_frame++;
			if(i_frame>=16)
				i_frame=-1;
		}
		calc_crc4(dato,&crc4);				
		last_clk=clk;
	}
}

void debug(char id, float v)
{
	struct PID * pid;
	char con=(id>>3)&0x7;
	char var=id&0x7;

	switch(con)
	{
		case 0:
			pid = &elevatorPID;
			break;
		case 1:
			pid = &aileronsPID;
			break;
		case 2:
			pid = &tailPID;
			break;
		case 3:
			pid = &motorPID;
			break;
		default:
			break;
	}

	if(con==4) // otros
	{
		switch(var)
		{
			case 0:
				modem_irxoff=v;
				break;
			case 1:
				modem_iryoff=v;
				break;
			case 2:
				modem_irmax=v;
				break;
			case 3:
				modem_ail=v;
				break;
			case 4:
				modem_ele=v;
				break;
			default:
				break;
		}
	}
	else
	{
		switch(var)
		{
			case 0:
				pid->P=v;
				break;
			case 1:
				pid->I=v;
				break;
			case 2:
				pid->D=v;
				break;
			case 3:
				pid->ILimit=v;
				break;
			case 4:
				pid->DriveLimit=v;
				break;
			default:
				break;
		}
	}
}

void modem_Analize() large
{
	void *p_data=&data_frame[1];
	//float f;

	switch(data_frame[0])
	{
		case 0x0:	// Pos switches
					ikarusInfo.modem_sw=data_frame[1]&0x3f; // poner 3f para añadir 1 sw mas? (camsel)
					break;

		case 0x1:	// Altura
					ikarusInfo.modem_alt=*((int*)p_data);
					break;

		case 0x2:	// Lon
					ikarusInfo.modem_lon=*((float*)p_data);
					break;

		case 0x3:	// Lat
					ikarusInfo.modem_lat=*((float*)p_data);
					break;

		case 0xF:	//
					debug(*((char*)p_data),*((float*)((char*)p_data+1)));
	
		default:	
					break;
	}
}

#if 1
//#define calc_crc4(dato, crc) crc = CRC4Table[(crc^dato)&0x0f]

code const unsigned char CRC4Table[16] = {
	0x00, 0x03, 0x06, 0x05, 0x0C, 0x0F, 0x0A, 0x09, 
	0x0B, 0x08, 0x0D, 0x0E, 0x07, 0x04, 0x01, 0x02};

void calc_crc4(unsigned char dato, unsigned char *crc)
{
    *crc = CRC4Table[(*crc ^ dato) & 0xF];
}

#else
void calc_crc4(char dato, char *pcrc)
{
 	const unsigned char POLY = 0x3;// 0x8c;
	unsigned char j,crc;

	crc=*pcrc;
   	crc = crc ^ dato;
    for (j = 4; j > 0; j--)
    {
        crc = ((crc & 0x8) != 0) ? (crc<< 1) ^ POLY : crc << 1;
		crc= crc & 0x0f;
    }
	*pcrc=crc;
}
#endif