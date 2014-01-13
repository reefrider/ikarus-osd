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

#include "Plugin.h"

#include "ParserNMEA.h"

#include <stdio.h>
#include <string.h>
#include <math.h>
#include "Utils.h"
#include "ikarus.h"

struct GPSInfo gpsinfo;

extern struct StoredConfig storedConfig;
extern struct IkarusInfo ikarusInfo;

char str_bauds_rates[6][8]={"4800","9600", "14400", "28800", "38400", "57600"};
	
char GetSelectedBauds()
{
	return storedConfig.BaudRate;
}

char * SelectBauds(unsigned char c)
{
	return str_bauds_rates[c];
}

char * GetBaudStr(unsigned char c)
{
	return str_bauds_rates[c];
}

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
	gpsinfo.velo=0;
	gpsinfo.rumbo=0.0f;
	gpsinfo.pos_valid=0;
	gpsinfo.fix=0;
	gpsinfo.numsats=0;
	gpsinfo.verticalSpeed=0;
	gpsinfo.altitudeMAX=0;
	gpsinfo.veloMAX=0;
	gpsinfo.velo_filter=0;
	gpsinfo.alt_filter=storedConfig.HomeAltitude;
	gpsinfo.en_movimiento=0;
	gpsinfo.bad_crc=0;
}

void GPS_Calculate() 
{
	float lastAltitude=gpsinfo.alt_filter;

	gpsinfo.alt_filter=gpsinfo.alt;

	// warning: la precision de esta medida depende de DT=0.1 -> (tmp/DT)*60/100 => 6*tmp
	if(storedConfig.MetricsImperial==0)
		gpsinfo.verticalSpeed=6*(gpsinfo.alt_filter-lastAltitude)*3.28084;
	else
		gpsinfo.verticalSpeed=6*(gpsinfo.alt_filter-lastAltitude);
					
	if(getRelAltitude()>gpsinfo.altitudeMAX)
			gpsinfo.altitudeMAX=getRelAltitude();

	gpsinfo.velo_filter=gpsinfo.velo;
	
	if(gpsinfo.velo_filter>gpsinfo.veloMAX)
		gpsinfo.veloMAX=gpsinfo.velo_filter;		
}

			
float gps_adjust(float v)
{
	int deg=v/100;
	return deg+(v-deg*100)/60;
}

extern const TDataFromAeroSimRC  *ptDataFromAeroSimRC;

void SimulaADC()
{
	ikarusInfo.v1 = ptDataFromAeroSimRC->Model_fBatteryVoltage;
	ikarusInfo.v2 = ptDataFromAeroSimRC->Model_fBatteryVoltage;
	ikarusInfo.consumidos_mAh = ptDataFromAeroSimRC->Model_fBatteryConsumedCharge*1000;
	ikarusInfo.currI = ptDataFromAeroSimRC->Model_fBatteryCurrent;	
}

void SimulaGPS() 
{
	storedConfig.HomeLon = ptDataFromAeroSimRC->Scenario_fWPHome_Long;
	storedConfig.HomeLat = ptDataFromAeroSimRC->Scenario_fWPHome_Lat;
	storedConfig.HomeAltitude = 0.0f;

	gpsinfo.conected=1;
	gpsinfo.uart_timeout=0;

	gpsinfo.nmea_ok=1;
	gpsinfo.nmea_timeout=0;

	// GGA
	gpsinfo.lon=ptDataFromAeroSimRC->Model_fLongitude;
	gpsinfo.lat=ptDataFromAeroSimRC->Model_fLatitude;

	if(storedConfig.MetricsImperial==0)
		gpsinfo.alt=ptDataFromAeroSimRC->Model_fHeightAboveTerrain;			// Metrico (m)
	else
		gpsinfo.alt=ptDataFromAeroSimRC->Model_fHeightAboveTerrain*3.28084f;	// Imperial (ft)

	gpsinfo.pos_valid=1;

	gpsinfo.hora=123222;
	gpsinfo.fix=2;		// 0->No, 1 ->2D, 2->3D??
	gpsinfo.numsats=6;
	gpsinfo.hdop=1.5f;

	float velo=(float)sqrt(pow(ptDataFromAeroSimRC->Model_fVelX,2)+
		pow(ptDataFromAeroSimRC->Model_fVelY,2)+
		pow(ptDataFromAeroSimRC->Model_fVelZ,2))*3.6f/1.852f;

	if(storedConfig.MetricsImperial==0)
		gpsinfo.velo=velo*1.852f;	// Metrico (kmh)
	else
		gpsinfo.velo=velo;			// Imperial (knots)

	gpsinfo.rumbo=ptDataFromAeroSimRC->Model_fHeading*180/3.1415f;			
	if(gpsinfo.rumbo<0)
		gpsinfo.rumbo+=360.0f;

	gpsinfo.pos_valid=1;

	gpsinfo.act=1;
}