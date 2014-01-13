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
#include <stdio.h>

#include "Servos.h"
#include "Modem.h"
#include "Ikarus.h"
#include "PID.h"
#include "Navigation.h"
#include "Utils.h"
#include "MenuConfig.h"

#define MAX_PACKET_ID	0x0F

#define MAX			2000
#define MIN			1000
#define STA			 900

extern xdata struct PID elevatorPID;
extern xdata struct PID aileronsPID;
extern xdata struct PID tailPID;
extern xdata struct PID motorPID;

extern xdata float modem_irxoff, modem_iryoff, modem_irmax, modem_ail, modem_ele;
extern xdata float adc_values[];
extern code struct AutoPilotConfig autopilotCfg;

xdata unsigned char updateHome;
xdata unsigned char saveToFlash;


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

code const char pkt_lens[MAX_PACKET_ID+1]={1,2,4,4,/**/1,1,0,0,/**/0,0,0,0,/**/0,0,0,5};

extern xdata struct IkarusInfo ikarusInfo;
extern code struct Ruta miRuta;
extern unsigned char wpt_index;

xdata unsigned char modem_lost, modem_badcrc;

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
	modem_lost=0;
	modem_badcrc=0;

}

void modem_rx(int rx) large
{
	const char bits=5;	
	int valores = 1<<bits;
	static char offset=0;

	float rango=(MAX-MIN)/(valores-1.0);
	float v=rx-MIN-offset+rango/2;	// aparece 8us mas amplio de lo q le mandamos

	char dato=(char)(v/rango);

	static char midato;
	static char pkt_len;
	static unsigned char crc4;

	char clk=(dato&0x10)>>4;

	if(modem_lost<254)
		modem_lost++;

	//if(rx>750&&rx<850)
	if(rx>STA-25 && rx < STA+25)
	{
		last_clk=0;
		i_frame=0;
		offset=rx - STA;
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
	else if(clk!=last_clk && i_frame>0)
	{
		modem_lost=0;
		dato = bin2gray5[dato&0x1f] & 0x0f;
		if(clk==1)
		{
			if(i_frame>pkt_len) // i_frame==pkt_len+1
			{
				if(crc4==dato)
				{
					modem_Analize();
					modem_badcrc=0;
				}
				else if(modem_badcrc<254)
					modem_badcrc++;

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
	else
	{
		i_frame = -1;	// Descartamos trama
	}
}

void debug(char id, float v)
{
	struct PID * pid;
	char con=(id>>3)&0x7;
	char var=id&0x7;



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


void DecodeModemCmd(char id)
{
	static float irx_max, iry_max;
	static char irmax_load=0;
	float tmp;

	switch(id & 0x03)
	{
		case 0:		// Set IR Gain
			irx_max = adc_values[ADC_IR_X];
			iry_max = adc_values[ADC_IR_Y];
			irmax_load = 1;

			tmp=irx_max-modem_irxoff;
			if(tmp<0)
				tmp=-tmp;
			modem_irmax=tmp;

			tmp=iry_max-modem_iryoff;
			if(tmp<0)
				tmp=-tmp;

			modem_irmax+=tmp;
			break;
	
		case 1: // Set IR Center 
			modem_irxoff=adc_values[ADC_IR_X];
			modem_iryoff=adc_values[ADC_IR_Y];
			if(irmax_load)
			{
				//irmax_load = 0;
				tmp=irx_max-modem_irxoff;
				if(tmp<0)
					tmp=-tmp;
				modem_irmax=tmp;

				tmp=iry_max-modem_iryoff;
				if(tmp<0)
					tmp=-tmp;

				modem_irmax+=tmp;
			}

			break;

		case 2: //  Set Home GPS
			updateHome= 1;
			break;

		case 3:	// Save To Flash
			saveToFlash=1;
			//modem_irzoff=adc_values[ADC_IR_Z];
			//ram2rom((void*)&autopilotCfg.IR_max, (void*)&modem_irmax, sizeof(float));
			//ram2rom((void*)&autopilotCfg.x_off, (void*)&adc_values[ADC_IR_X], 3*sizeof(float));
			//flush_rom();

			break;
	}
}

void modem_Analize() large
{
	void *p_data=&data_frame[1];
	static unsigned char codseq=0xff;

	switch(data_frame[0])
	{
		case 0x0:	// Pos switches
					ikarusInfo.modem_sw=data_frame[1]&0x7f; 
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
		case 0x4:	// Wpt ID
					if( codseq!=data_frame[1])
					{
						codseq=data_frame[1];
						ikarusInfo.modem_wptid=data_frame[1]&0x1f;				
						if(ikarusInfo.modem_wptid<miRuta.numwpt)
							wpt_index = ikarusInfo.modem_wptid;
				//		else
				//			wpt_index = 0;
					}

					break;

		case 0x5:	if(codseq!=data_frame[1])
					{
						codseq=data_frame[1];
						DecodeModemCmd(data_frame[1]);
						ikarusInfo.uplink_cmd_received=(data_frame[1]&0x3F);		// refinaremos mas
						ikarusInfo.uplink_cmd_countdown=UPLINK_CMD_MAX;

					}
					break;

		case 0xF:	if(codseq!=data_frame[1])
					{
						codseq=data_frame[1];
						debug(*((char*)p_data),*((float*)(p_data+1)));
						ikarusInfo.uplink_cmd_received=0x80 | (data_frame[1]&0x3F);		// refinaremos mas
						ikarusInfo.uplink_cmd_param = *((float*)(p_data+1));
						ikarusInfo.uplink_cmd_countdown=UPLINK_CMD_MAX;
					}
					break;
	
		default:	
					break;
	}
}

#if 1
//#define calc_crc4(dato, crc) crc = CRC4Table[(crc^dato)&0x0f]

code const unsigned char CRC4Table[16] = {
	0x00, 0x03, 0x06, 0x05, 0x0C, 0x0F, 0x0A, 0x09, 
	0x0B, 0x08, 0x0D, 0x0E, 0x07, 0x04, 0x01, 0x02};

void calc_crc4(char dato, char *crc)
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

void MensajeUplink(char *cad) large
{
	code const char msg [5][5][6]={
		{"PI P ","PI I ","PI D ","PI IL","PI DL"},
		{"RL P ","RL I ","RL D ","RL IL","RL DL"},
		{"TL P ","TL I ","TL D ","EL IL","TL DL"},
		{"TH P ","TH I ","TH D ","TH IL","TH DL"},
		{"IR X ","IR Y ","IR MX","M AIL","M ELE"}};

	if(ikarusInfo.uplink_cmd_received&0x80)		// Es un parametro
	{
		char con=(ikarusInfo.uplink_cmd_received>>3)&0x7;
		char var=ikarusInfo.uplink_cmd_received&0x7;

		if(con<5 && var<5)
			sprintf(cad," <%s %6.3f> ",msg[con][var],ikarusInfo.uplink_cmd_param);
		else
			sprintf(cad,"<<UNKNOW PARAM>>");

	}
	else	// Es un comando
	{
		switch(ikarusInfo.uplink_cmd_received&0x3F)
		{
			case 0:	// IR Max
				sprintf(cad," <IR MAX %4.2f>  ", modem_irmax);
				break;
		
			case 1: // IR Center
				sprintf(cad," <IR %4.2f %4.2f> ", modem_irxoff, modem_iryoff);
				break;

			case 2: // Home
				sprintf(cad," <HOME UPDATED> ");
				break;

			case 3: // Flash
				sprintf(cad,"<SAVED TO FLASH>");
				break;

			default:
				sprintf(cad," <<UNKNOW CMD>> ");
				break;
		}
	}

}