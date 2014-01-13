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

#include "ParserNMEA.h"

#include "c8051f340.h"
#include <stdio.h>
#include <string.h>
#include <math.h>
#include "Utils.h"
#include "ikarus.h"
#include "Telemetry.h"
#include "filtroSimple.h"

#define NUM_BAUDS 7

#ifndef IMU_UM6

code struct { char str[7],t1h,ckcon; } bauds[NUM_BAUDS] = 
	{{"4800",0x98,0x02},{"9600",0x30,0x00}, {"14400", 0x75, 0x00}, {"19200",0x95, 0x00}, 
	{"28800", 0x30, 0x01}, {"38400",0x64,0x01},{"57600",0x98,0x01}};

enum Estados
	{
	WaitS, WaitG, WaitP, WaitCmd0, RxG0, RxG1, IsGGA, RxR0, RxM1, IsRMC,
	GGA_hora, GGA_lat, GGA_N, GGA_lon, GGA_W, GGA_fix, GGA_nsats, GGA_hdop, GGA_alt,
	RMC_hora, RMC_valid, RMC_lat, RMC_N, RMC_lon, RMC_W, RMC_velo, RMC_rumbo,
	GGA_WaitX, GGA_CRC, RMC_WaitX, RMC_CRC, AUX_CRC};

xdata struct GGA gga;
xdata struct RMC rmc;

float gps_f;
char gps_char;
bit gps_sign;

unsigned char gps_tmp;
unsigned char gps_crc;

unsigned char recv_crc;
unsigned char calc_crc;
float gps_comma;

volatile char NMEA_estado=WaitS;

xdata char CheckedNMEA;
xdata char ReceivedUART;
#endif

xdata struct FiltroSimpleF fsAltitud, fsVelocidad;
xdata struct GPSInfo gpsinfo;

xdata unsigned char _selectedbps=2;

// Debe salir a otro fichero???
xdata char pkt_index=0;
extern xdata struct Telemetry_PKT pkt;

extern code struct StoredConfig storedConfig;
extern xdata struct IkarusInfo ikarusInfo;

#ifndef IMU_UM6
char GetSelectedBauds()
{
	return _selectedbps;
}

char * SelectBauds(unsigned char c)
{
	if(c<NUM_BAUDS)
	{
		TH1=bauds[c].t1h;
		CKCON=(CKCON&0xF4)|bauds[c].ckcon;
		_selectedbps=c;
	
		return bauds[c].str;
	}
	return "";
}

char * GetBaudStr(unsigned char c)
{
	if(c<NUM_BAUDS)
	{
		return bauds[c].str;
	}
	else
		return "";
}

#endif

float getRelAltitude()
{
	if(storedConfig.RelativeAltitude)
		return gpsinfo.alt_filter-storedConfig.HomeAltitude;
	else	
		return gpsinfo.alt_filter;
}

void initStruct()
{
	SelectBauds(storedConfig.BaudRate);
	gpsinfo.lat=storedConfig.HomeLat;
	gpsinfo.lon=storedConfig.HomeLon;
	gpsinfo.alt=storedConfig.HomeAltitude;
	gpsinfo.knots=0;
	gpsinfo.rumbo=0.0f;
	gpsinfo.pos_valid=0;
	gpsinfo.fix=0;
	gpsinfo.numsats=0;
	gpsinfo.altitudeMAX=0;
	gpsinfo.knotsMAX=0;
	gpsinfo.knots_filtered=0;
	gpsinfo.alt_filter=storedConfig.HomeAltitude;
	gpsinfo.en_movimiento=0;
	gpsinfo.bad_crc=0;

	init_filtroSimpleF(&fsAltitud,gpsinfo.alt);
	init_filtroSimpleF(&fsVelocidad,gpsinfo.knots);
}

void GPS_Calculate() large
{
	xdata float relAlt;

	gpsinfo.alt_filter=filtroSimpleF(&fsAltitud,gpsinfo.alt);
				
	relAlt=getRelAltitude();	
	if(relAlt>gpsinfo.altitudeMAX)
			gpsinfo.altitudeMAX=relAlt;

	gpsinfo.knots_filtered=filtroSimpleF(&fsVelocidad,gpsinfo.knots);
	
	if(gpsinfo.knots_filtered>gpsinfo.knotsMAX)
		gpsinfo.knotsMAX=gpsinfo.knots_filtered;		
}

#ifndef IMU_UM6			
float gps_adjust(float v)
{
	int deg=v/100;
	return deg+(v-deg*100)/60;
}


void UART0_handle() interrupt 4
{
	static bit skip=0;
	char uart_data;
	unsigned char i=0;
	while(RI0)		// Recive los datos del GPS
	{
		RI0=0;
		
		gpsinfo.conected=1;
		gpsinfo.uart_timeout=0;

		uart_data=SBUF0;
		
		if(uart_data=='$')
		{
			NMEA_estado=WaitG;
			gps_crc='$';		// El $ no se cuenta. '$' ^ '$' => 0
			calc_crc=0;
			recv_crc=0xff;
		}
		else if(uart_data=='*')			// comienza chksum
		{
			if(NMEA_estado==GGA_WaitX)
				NMEA_estado=GGA_CRC;
			else if(NMEA_estado==RMC_WaitX)
				NMEA_estado=RMC_CRC;
			else
				NMEA_estado=AUX_CRC;
			calc_crc=gps_crc;
			recv_crc=0;
			skip=0;
		}
		else if((uart_data==13||uart_data==10)&&skip==0)
		{
			skip=1;
			if(calc_crc==recv_crc)
			{
				gpsinfo.nmea_ok=1;
				gpsinfo.nmea_timeout=0;
				
				if(NMEA_estado==GGA_CRC)
				{
					if(gga.fix)
					{		
						gpsinfo.lon=gga.lon;
						gpsinfo.lat=gga.lat;
						gpsinfo.alt=gga.alt;			// Metrico (m)

						gpsinfo.pos_valid=1;
					}
					else
						gpsinfo.pos_valid=0;

					gpsinfo.hora=gga.hora;
					gpsinfo.fix=gga.fix;
					gpsinfo.numsats=gga.nsats;
					gpsinfo.hdop=gga.hdop;

					gpsinfo.GGA_received++;			
				}
				else if(NMEA_estado==RMC_CRC)
				{
					if(rmc.active=='A')
					{					
						gpsinfo.lon=rmc.lon;
						gpsinfo.lat=rmc.lat;
						gpsinfo.knots=rmc.knots;			

						gpsinfo.rumbo=rmc.rumbo;			
					
						gpsinfo.pos_valid=1;
					}
					else
						gpsinfo.pos_valid=0;

					gpsinfo.hora=rmc.hora;
					gpsinfo.act=rmc.active;

					gpsinfo.RMC_received++;
				}
				// Fix hora....
				for(i=0;i<3&&gpsinfo.hora>235959;i++)
					gpsinfo.hora/=10;
									
			}
			else
			{
				gpsinfo.bad_crc++;
				gpsinfo.nmea_ok=0;
			}
			NMEA_estado=WaitS;
		}
		else if(uart_data==',')
		{
			switch(NMEA_estado)
			{
				case IsGGA:
					NMEA_estado=GGA_hora;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					break;

				case IsRMC:
					NMEA_estado=RMC_hora;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;	
					break;
					
				case GGA_hora:
					gga.hora=gps_f;
					gps_f=0;
					gps_comma=0;
					NMEA_estado=GGA_lat;
					break;

				case GGA_lat:
					NMEA_estado=GGA_N;
					break;
				case GGA_N:
					if(gps_char=='S'||gps_char=='s')
						gps_f=-gps_f;

					gga.lat=gps_adjust(gps_f);
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_lon;
					break;
	
				case GGA_lon:
					NMEA_estado=GGA_W;
					break;
	
				case GGA_W:
					if(gps_char=='W'||gps_char=='w')
						gps_f=-gps_f;
					gga.lon=gps_adjust(gps_f);
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_fix;
					break;
			
				case GGA_fix:
					gga.fix=gps_char-'0';
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_nsats;
					break;

				case GGA_nsats:
					gga.nsats=(int)gps_f;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_hdop;
					break;
	
				case GGA_hdop:
					gga.hdop=gps_f;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_alt;
					break;
				
				case GGA_alt:
					gga.alt=gps_f;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=GGA_WaitX;
					break;

				case RMC_hora:
					rmc.hora=gps_f;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=RMC_valid;
					break;

				case RMC_valid:
					rmc.active=gps_char;
					gps_f=0;
					gps_comma=0;
					gps_sign=0;
					NMEA_estado=RMC_lat;
					break;


				case RMC_lat:
					NMEA_estado=RMC_N;
					break;

				case RMC_N:
					if(gps_char=='S'||gps_char=='s')
						gps_f=-gps_f;

					rmc.lat=gps_adjust(gps_f);
					gps_f=0;
					gps_comma=0;
					NMEA_estado=RMC_lon;
					break;
	
				case RMC_lon:
					NMEA_estado=RMC_W;
					break;
	
				case RMC_W:
					if(gps_char=='W'||gps_char=='w')
						gps_f=-gps_f;
					rmc.lon=gps_adjust(gps_f);
					gps_f=0;
					gps_comma=0;
					NMEA_estado=RMC_velo;
					break;

				case RMC_velo:
					rmc.knots=gps_f;
					gps_f=0;
					gps_comma=0;
					NMEA_estado=RMC_rumbo;
					break;

				case RMC_rumbo:
					rmc.rumbo=gps_f;
					gps_f=0;
					gps_comma=0;
					NMEA_estado=RMC_WaitX;
					break;

				default:
					break;
			}
		}
		else 
		{
			switch(NMEA_estado)
			{
				case WaitG:	
					if(uart_data=='G')
						NMEA_estado=WaitP;
					else
						NMEA_estado=WaitS;
					break;

				case WaitP:	
					if(uart_data=='P')
						NMEA_estado=WaitCmd0;
					else
						NMEA_estado=WaitS;
					break;
					
				case WaitCmd0:	
					if(uart_data=='G')
						NMEA_estado=RxG0;
					else if(uart_data=='R')
						NMEA_estado=RxR0;
					else
						NMEA_estado=WaitS;
					break;

				case RxG0:	
					if(uart_data=='G')
						NMEA_estado=RxG1;
					else
						NMEA_estado=WaitS;
					break;

				case RxR0:	
					if(uart_data=='M')
					{
						NMEA_estado=RxM1;
					}
					else
						NMEA_estado=WaitS;
					break;

				case RxG1:	
					if(uart_data=='A')
					{
						NMEA_estado=IsGGA;
					}
					else
						NMEA_estado=WaitS;
					break;

				case RxM1:	
					if(uart_data=='C')
					{
						NMEA_estado=IsRMC;
					}
					else
						NMEA_estado=WaitS;
					break;

				case IsRMC:
					break;

				case IsGGA:	
					break;

				case GGA_fix:
				case GGA_N:
				case GGA_W:
				case RMC_valid:
				case RMC_N:
				case RMC_W:
						gps_char=uart_data;
						break;
				case GGA_hora:
				case GGA_lat:
				case GGA_lon:
				case GGA_nsats:
				case GGA_hdop:
				case GGA_alt:
				case RMC_hora:
				case RMC_lat:
				case RMC_lon:
				case RMC_velo:
				case RMC_rumbo:
					if(uart_data=='-')
						gps_sign=1;
					else if(uart_data=='.')
						gps_comma=0.1f;
					else if(gps_comma==0)
					{
						if(gps_sign)
							gps_f=10*gps_f-(uart_data-'0');
						else
							gps_f=10*gps_f+uart_data-'0';
					}
					else
					{
						if(gps_sign)
							gps_f=gps_f-gps_comma*(uart_data-'0');
						else
							gps_f=gps_f+gps_comma*(uart_data-'0');
						gps_comma=gps_comma/10;
					}
					break;

				case GGA_CRC:
				case RMC_CRC:
				case AUX_CRC:
					recv_crc=recv_crc<<4;
					if(uart_data>='A'&&uart_data<='F')
						recv_crc|=uart_data - 'A' + 0x0A;
					else if(uart_data>='a'&&uart_data<='f')
						recv_crc|=uart_data - 'a' + 0x0A;
					if(uart_data>='0'&&uart_data<='9')
						recv_crc|=uart_data - '0';
					break;

				case GGA_WaitX:
				case RMC_WaitX:
				default:
					break;
			}
		}
		gps_crc^=uart_data;
		//RI0=0;
	}

	if(TI0)		// Voy a usar el TX para el modem XBEE
	{
		if(pkt_index>0&& pkt_index<(pkt.len+3))
			{
			char *ppkt=(char*)&pkt;
			if(pkt_index==0)
				SBUF0='A';
			else if(pkt_index==1)
				SBUF0='T';
			else if(pkt_index==2)
				SBUF0='C';
			else
				SBUF0=ppkt[pkt_index-3];
			pkt_index++;
			}
		TI0=0;
	}
}
#endif