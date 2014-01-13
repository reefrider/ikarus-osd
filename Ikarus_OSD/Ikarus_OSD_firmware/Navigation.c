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

#include <String.h>
#include <Math.h>
#include "ParserNMEA.h"
#include "Utils.h"
#include "Navigation.h"
#include "Ikarus.h"

#define PI 3.14159265

#define EARTH_METROS 6378137.0 		// Radio en metros
#define EARTH_MILLAS 3443.9184		// Radio en millas

#define DEG2RAD(a)	((a)*PI/180.0)
#define RAD2DEG(a)	((a)*180.0/PI)

#define FILTRO_VARIOMETRO	8

extern code struct Ruta miRuta;

//xdata struct Ruta miRuta;
unsigned char wpt_index=0;

bit invertir_ruta=0;
extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;

extern xdata struct IkarusInfo ikarusInfo;
extern xdata struct GPSInfo gpsinfo;


void init_navigator()
{
	wpt_index=0;		// El 0 quizas debiera ser el punto de partida o el 1???
	ikarusInfo.navigateTo=NAV_HOME;
	ikarusInfo.verticalSpeed = 0;
	invertir_ruta=0;
}

void UpdateNavigator() large
{
	static float lastLon=0;
	static float lastLat=0;
	static float lastAlt=0;
	
	static char has_previous=0;
	static unsigned int iTics=0;
	static unsigned int countDown=0;
	float dist;
	float deriva;

	float diffAlt;
		
		diffAlt=(gpsinfo.alt_filter-lastAlt)*5;		//m/s
		lastAlt = gpsinfo.alt_filter;

		#ifdef FILTRO_VARIOMETRO
			diffAlt = (ikarusInfo.verticalSpeed*(FILTRO_VARIOMETRO-1)+diffAlt)/FILTRO_VARIOMETRO;
		#endif

		ikarusInfo.verticalSpeed = diffAlt;

	
	// Altualizar campos gpsinfo
	GPS_Calculate();
		
	// Actualizamos informacion de casa
	ikarusInfo.distance_home=calcDistance(gpsinfo.lon, gpsinfo.lat, storedConfig.HomeLon, storedConfig.HomeLat);
	if(ikarusInfo.distance_home>ikarusInfo.max_distance_home)
		ikarusInfo.max_distance_home=ikarusInfo.distance_home;

	// Altualizamos datos destino
	ikarusInfo.navigator_bearing=GetRumboDst();
	ikarusInfo.distance_wpt=GetDistDst();

		// ... altitud....
	ikarusInfo.navigator_altitude=GetAltDst();


	// Orientación de la antena de tierra
	ikarusInfo.AntTracker=calcBearing(storedConfig.HomeLon, storedConfig.HomeLat, gpsinfo.lon, gpsinfo.lat);

	if(ikarusInfo.distance_home==0)
		ikarusInfo.AntTrackerV=90.0f;
	else 
		ikarusInfo.AntTrackerV=RAD2DEG(atan2(gpsinfo.alt_filter-storedConfig.HomeAltitude, ikarusInfo.distance_home));

	// Actualizamos ruta de waypoints
	if(ikarusInfo.cambioWpt)
	{
		if(countDown>0)
			countDown--;
		else
		{
			ikarusInfo.cambioWpt = 0;
			ChangeNext();
		}
	}
	else if(WayPointReached())
	{
		ikarusInfo.cambioWpt = 1;
		if(autopilotCfg.chAux_mode==AUX_WPT)
			countDown=10;
		else
			countDown=0;
	}

	// .. deriva relativa
	deriva= ikarusInfo.navigator_bearing-gpsinfo.rumbo;

    if (deriva > 180.0)
        deriva -= 360.0f;
    else if (deriva < -180.0)
        deriva += 360.0f;

	ikarusInfo.navigator_rel_bearing=deriva;

	// Calculamos distancia recorrida
	if(has_previous==0)
	{
		if(gpsinfo.pos_valid)
		{
			lastLon=gpsinfo.lon;
			lastLat=gpsinfo.lat;
			has_previous=1;
		}
	}
	else
	{
		dist=calcDistance(gpsinfo.lon, gpsinfo.lat, lastLon, lastLat);
		ikarusInfo.distancia_recorrida+=dist;
		lastLon=gpsinfo.lon;
		lastLat=gpsinfo.lat;
	}

	if(iTics<100)	// 10s/0.1s
		iTics++;
	else
	{
		static float last_distancia, last_consumidos, last_altitud;
		float tmp=ikarusInfo.distancia_recorrida-last_distancia;
		float tasa;

		tasa=tmp/(last_altitud-gpsinfo.alt_filter);
	
		// Calculamos la tasa de planeo
		if(tasa>99)
			tasa=99;
		else if(tasa<-99)
			tasa=-99;
				
		ikarusInfo.tasa_planeo=tasa;

		//ikarusInfo.coste_km_Ah=tmp/(ikarusInfo.consumidos_mAh-last_consumidos);

		ikarusInfo.coste_km_Ah=ikarusInfo.distancia_recorrida/ikarusInfo.consumidos_mAh;


		last_distancia=ikarusInfo.distancia_recorrida;
		last_consumidos=ikarusInfo.consumidos_mAh;
		last_altitud=gpsinfo.alt_filter;

		iTics=0;
	}
}

/*
unsigned char GetIndex()
{
	if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		return 0xfd;
	else if(ikarusInfo.navigateTo==NAV_MODEM)
		return 0xfe;
	else if(ikarusInfo.navigateTo==NAV_HOME)
		return 0xff;		
	else
		return wpt_index;
}
*/

unsigned char GetIndex()
{
	unsigned char dato;

	if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		dato = 0x1d;
	else if(ikarusInfo.navigateTo==NAV_MODEM)
		dato = 0x1e;
	else if(ikarusInfo.navigateTo==NAV_HOME)
		dato = 0x1f;		
	else
		dato = wpt_index;

	if(gpsinfo.pos_valid)
		dato |= 0x80;

	if (ikarusInfo.failsafe != FSS_NORMAL)
		dato |= 0x60;
	else if(ikarusInfo.AutoPilot_Enabled==APLT_ESTAB)
		dato |= 0x20;
	else if(ikarusInfo.AutoPilot_Enabled==APLT_FULL)
		dato |= 0x40;				

	return dato;
}


float GetDistDst()
{
	float d;
	if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		d=0.0f;
	else
		d=calcDistance(gpsinfo.lon, gpsinfo.lat, GetLonDst(), GetLatDst());
	return d;
}

float GetRumboDst() 
{
	float b;
	if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		b=ikarusInfo.lastHeading;
	else
	{
		b=calcBearing(gpsinfo.lon, gpsinfo.lat, GetLonDst(), GetLatDst());
		ikarusInfo.lastHeading=gpsinfo.rumbo;
	}
	return b;
}

char *GetNameDst()
{
	if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		return "HOLD    ";
	else if(ikarusInfo.navigateTo==NAV_MODEM)
		return "UPLINK  ";
	else if(ikarusInfo.navigateTo==NAV_HOME)
		return "HOME    ";
	else 
		return miRuta.wpts[wpt_index].Name;
}

float GetLonDst() large
{
	if(ikarusInfo.navigateTo==NAV_MODEM)
		return ikarusInfo.modem_lon;
	else if(ikarusInfo.navigateTo==NAV_HOME)
		return storedConfig.HomeLon;
	else if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		return ikarusInfo.lastHeading;
	else
		return miRuta.wpts[wpt_index].lon;
}

float GetLatDst() large
{
	if(ikarusInfo.navigateTo==NAV_MODEM)
		return ikarusInfo.modem_lat;
	else if(ikarusInfo.navigateTo==NAV_HOME)
		return storedConfig.HomeLat;
	else if(ikarusInfo.navigateTo==NAV_HOLD||ikarusInfo.navigateTo==NAV_HOLD_FS)
		return 0.0f;
	else
		return miRuta.wpts[wpt_index].lat;
}

float GetAltDst() large
{
	float alt;
	float wptAlt;



	if(ikarusInfo.navigateTo==NAV_HOLD)
	{
		wptAlt=ikarusInfo.lastAltitude;
	}
	if(ikarusInfo.navigateTo==NAV_HOLD_FS)
	{
		wptAlt=ikarusInfo.lastAltitude;
		return wptAlt;
	}
	else if(ikarusInfo.navigateTo==NAV_MODEM)
	{
		wptAlt=ikarusInfo.modem_alt;
		ikarusInfo.lastAltitude=getRelAltitude();
	}	
	else
	{
		ikarusInfo.lastAltitude=getRelAltitude();

		if(ikarusInfo.navigateTo==NAV_HOME)
			wptAlt=autopilotCfg.baseCruiseAltitude;		
		else
			wptAlt=miRuta.wpts[wpt_index].alt;
	
	}

	switch(autopilotCfg.AutopilotMode)
	{
		case AP_DISABLED:
		case AP_FIX_ALT:
		default:
			alt=autopilotCfg.baseCruiseAltitude;
			break;

		case AP_DIST_ALT:
			alt=autopilotCfg.baseCruiseAltitude;
			if(ikarusInfo.distance_home>0)
				alt+=ikarusInfo.distance_home*autopilotCfg.distanceAltitude;
			break;

		case AP_WPT_SAFE:
			alt=autopilotCfg.baseCruiseAltitude;
			if(ikarusInfo.distance_home>0)
				alt+=ikarusInfo.distance_home*autopilotCfg.distanceAltitude;
			if(wptAlt>alt)
				alt=wptAlt;
			break;

		case AP_WPT_UNSAFE:
			alt=wptAlt;
			break;
	}
	return alt;
}

void ChangeNext() large
{
	if(ikarusInfo.navigateTo==NAV_HOME)
		return;

	if(invertir_ruta==0)
	{
		if(wpt_index>=miRuta.numwpt-1)
		{
			if(storedConfig.modelo_ruta==0)
			{
				wpt_index=0xff;		// Volver a casa
			}
			else if(storedConfig.modelo_ruta==1)
			{
				wpt_index=0;		// repetir
			}
			else 	// invertir	
			{
				invertir_ruta=1;
				if(wpt_index>0)
					wpt_index--;
			}
		}
		else
			wpt_index++;
	}
	else
	{
		if(wpt_index==0)
		{
			if(storedConfig.modelo_ruta==2)
			{
				wpt_index=0xff;		// Volver a casa
			}
			else
			{
				invertir_ruta=0;
				if(wpt_index<miRuta.numwpt-1)
					wpt_index++;
			}
		}
		else
			wpt_index--;

	}
}

char WayPointReached()  large
{
	static float last_d;
	float d=ikarusInfo.distance_wpt;
	
	if(ikarusInfo.navigateTo==NAV_HOME)
		return 0;

	// Cambiamos wpt_index si es necesario, en funcion del modo de rutas
	if(d<storedConfig.wptRange&&last_d<d)
	{
		return 1;
	}
	last_d=d;
	return 0;
}

float calcDistance(float lon1, float lat1, float lon2, float lat2) large
{
	float sindlat=sin(DEG2RAD(lat1-lat2)/2.0);
	float sindlon=sin(DEG2RAD(lon1-lon2)/2.0);
	float d=2.0*asin(sqrt(sindlat*sindlat + sindlon*sindlon* cos(DEG2RAD(lat1))*cos(DEG2RAD(lat2))));
	return d*EARTH_METROS;
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