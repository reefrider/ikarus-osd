/* 
 * (c) 2011 Rafael Paz <rpaz@atc.us.es>
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

#include <Math.h>

#include "AntTracker.h"
#include "Calculate.h"
#include "Utils.h"

extern code struct StoredConfig storedConfig;
extern xdata struct DatosAvion datosAvion;
extern xdata struct DatosAntena datosAntena;

extern xdata float v_bateria;

//extern xdata float heading;
extern xdata float pan, tilt;

xdata float bearing, distance;
/*

void CalculateHeading () large
{
	if(storedConfig.useInternalGPS&&storedConfig.useInternalCompas)
	{
		if(gpsinfo.velo>storedConfig.speedCompasOverride && storedConfig.enableCompasOverride)
			heading = gpsinfo.rumbo;
		else
			heading = read_heading() - storedConfig.offsetCompas + gpsinfo.mag_desv;
	}
	else if(storedConfig.useInternalGPS)
		heading = gpsinfo.rumbo;
	else if(storedConfig.useInternalCompas)
		heading = read_heading()- storedConfig.offsetCompas;
	else 
		heading = 0;

	heading = Adjust360(heading);
//	heading = 0;
}
*/

void CalculaPanTilt() large
{

	float lonCasa, latCasa, altCasa;
	float lonPlane, latPlane, altPlane;

	lonCasa=datosAvion.home_lon;
	latCasa=datosAvion.home_lat;
	altCasa=datosAvion.home_alt;

	lonPlane=datosAvion.lon;
	latPlane=datosAvion.lat;
	altPlane=datosAvion.alt;

	bearing=calcBearing(lonCasa, latCasa, lonPlane, latPlane);
	distance = calcDistance(lonCasa, latCasa, lonPlane, latPlane);
	if(distance==0)
		tilt=90.0f;
	else 
		tilt=RAD2DEG(atan2(altPlane-altCasa, distance));


	pan = Adjust180(bearing +storedConfig.offsetPan );	//- heading
}

void Copy2DatosAntena() large
{
	float proportion;
	datosAntena.v_bateria= v_bateria;
	
	proportion= (datosAntena.v_bateria-storedConfig.Vmin)/(storedConfig.Vmax-storedConfig.Vmin);
	if(proportion >1)
		datosAntena.v_bateria_porcentaje = 100.0f;
	else if (proportion <0)
		datosAntena.v_bateria_porcentaje = 0.0f;
	else
		datosAntena.v_bateria_porcentaje = 100*proportion;

	if(datosAntena.v_bateria<storedConfig.Valarm)
		datosAntena.v_bateria_alarm = 1;
	else
		datosAntena.v_bateria_alarm = 0;

	datosAntena.tieneGPS=0;

}

float calcDistance(float lon1, float lat1, float lon2, float lat2) large
{
	float sindlat=sin(DEG2RAD(lat1-lat2)/2.0);
	float sindlon=sin(DEG2RAD(lon1-lon2)/2.0);
	float d=2.0*asin(sqrt(sindlat*sindlat + sindlon*sindlon* cos(DEG2RAD(lat1))*cos(DEG2RAD(lat2))));
	//if(storedConfig.MetricsImperial==0)
		return d*EARTH_METROS;
	//else
	//	return d*EARTH_MILLAS;
}

float calcBearing(float lon1,float lat1, float lon2, float lat2) large
{
	float dlon_grad=DEG2RAD(lon2-lon1);
	float lat1_grad=DEG2RAD(lat1);
	float lat2_grad=DEG2RAD(lat2);
	float y = sin(dlon_grad) * cos(lat2_grad);
	float x = cos(lat1_grad)*sin(lat2_grad) - sin(lat1_grad)*cos(lat2_grad)*cos(dlon_grad);
	if(y==0&&x==0)
		return 0;
	else
		return RAD2DEG(atan2(y,x));
}

/*
float calcBearing(float lon1,float lat1, float lon2, float lat2) large
{
	float y = sin(DEG2RAD(lon2-lon1)) * cos(DEG2RAD(lat2));
	float x = cos(DEG2RAD(lat1))*sin(DEG2RAD(lat2)) - sin(DEG2RAD(lat1))*cos(DEG2RAD(lat2))*cos(DEG2RAD(lon2-lon1));
	if(y==0&&x==0)
		return 0;
	else
		return RAD2DEG(atan2(y,x));
}
*/