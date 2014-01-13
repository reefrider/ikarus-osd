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

extern code struct Ruta miRuta;

//xdata struct Ruta miRuta;
unsigned char wpt_index=0;

bit go_home=0;
bit invertir_ruta=0;
extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;

extern xdata struct IkarusInfo ikarusInfo;
extern xdata struct GPSInfo gpsinfo;


void init_navigator()
{
	wpt_index=0;		// El 0 quizas debiera ser el punto de partida o el 1???
	go_home=1;
	invertir_ruta=0;
}

void UpdateNavigator() large
{
	static float lastLon=0;
	static float lastLat=0;
	static char has_previous=0;
	static unsigned int iTics=0;

	float dist;
	float deriva;

	// Altualizar campos gpsinfo
	GPS_Calculate();
		
	// Actualizamos informacion de casa
	ikarusInfo.distance_home=calcDistance(gpsinfo.lon, gpsinfo.lat, storedConfig.HomeLon, storedConfig.HomeLat);
	if(ikarusInfo.distance_home>ikarusInfo.max_distance_home)
		ikarusInfo.max_distance_home=ikarusInfo.distance_home;

	// Altualizamos datos destino
	ikarusInfo.navigator_bearing=GetRumboDst();
	ikarusInfo.distance_wpt=GetDistDst();

	// Orientación de la antena de tierra
	ikarusInfo.AntTracker=calcBearing(storedConfig.HomeLon, storedConfig.HomeLat, gpsinfo.lon, gpsinfo.lat);

	if(ikarusInfo.distance_home==0)
		ikarusInfo.AntTrackerV=90.0f;
	else if(storedConfig.MetricsImperial==0)
		ikarusInfo.AntTrackerV=RAD2DEG(atan2(gpsinfo.alt_filter-storedConfig.HomeAltitude, ikarusInfo.distance_home));
	else
		ikarusInfo.AntTrackerV=RAD2DEG(atan2(gpsinfo.alt_filter-storedConfig.HomeAltitude, 6076.1155f*ikarusInfo.distance_home));	//millas -> pies

	// Actualizamos ruta de waypoints
	ChangeNextIfReached();
	
	// ... altitud....
	ikarusInfo.navigator_altitude=GetAltDst();

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
		if(storedConfig.MetricsImperial==0)
			tasa=tmp/(last_altitud-gpsinfo.alt_filter);
		else
			tasa=6076.1155*tmp/(last_altitud-gpsinfo.alt_filter); //millas -> pies 

		// Calculamos la tasa de planeo
		
		if(tasa>99)
			tasa=99;
		else if(tasa<-99)
			tasa=-99;
				
		ikarusInfo.tasa_planeo=tasa;

		// Calculamos el coste en Km/Ah o Mi/Ah
		if(storedConfig.MetricsImperial==0)
			ikarusInfo.coste_km_Ah=tmp/(ikarusInfo.consumidos_mAh-last_consumidos);
		else
			ikarusInfo.coste_km_Ah=1000*tmp/(ikarusInfo.consumidos_mAh-last_consumidos);

		last_distancia=ikarusInfo.distancia_recorrida;
		last_consumidos=ikarusInfo.consumidos_mAh;
		last_altitud=gpsinfo.alt_filter;
		iTics=0;
	}
}

unsigned char GetIndex()
{
	if((storedConfig.ControlProportional==MODO_MODEM)&&(ikarusInfo.modem_sw&0x10))
		return 0xfe;
	else if(go_home||wpt_index>=miRuta.numwpt)
		return 0xff;		
	else
		return wpt_index;
}

float GetDistDst()
{
	float d;
	d=calcDistance(gpsinfo.lon, gpsinfo.lat, GetLonDst(), GetLatDst());
	return d;
}

float GetRumboDst() 
{
	float b;
	b=calcBearing(gpsinfo.lon, gpsinfo.lat, GetLonDst(), GetLatDst());
	return b;
}

char *GetNameDst()
{
	if((storedConfig.ControlProportional==MODO_MODEM)&&(ikarusInfo.modem_sw&0x10))
		return "UPLINK";
	else if(go_home)
		return "HOME";
	else if(miRuta.numwpt==0)
		return "SIN RUTA";
	else if(wpt_index>=miRuta.numwpt)
		return "HOME";
	else 
		return miRuta.wpts[wpt_index].Name;
}

float GetLonDst() large
{
	if((storedConfig.ControlProportional==MODO_MODEM)&&(ikarusInfo.modem_sw&0x10))
		return ikarusInfo.modem_lon;
	else if(go_home||wpt_index>=miRuta.numwpt)
		return storedConfig.HomeLon;
	else
		return miRuta.wpts[wpt_index].lon;
}

float GetLatDst() large
{
	if((storedConfig.ControlProportional==MODO_MODEM)&&(ikarusInfo.modem_sw&0x10))
		return ikarusInfo.modem_lat;
	else if(go_home||wpt_index>=miRuta.numwpt)
		return storedConfig.HomeLat;
	else
		return miRuta.wpts[wpt_index].lat;
}

float GetAltDst() large
{
	float alt;
	float wptAlt;

	if((storedConfig.ControlProportional==MODO_MODEM)&&(ikarusInfo.modem_sw&0x10))
		wptAlt=ikarusInfo.modem_alt;
	else if(go_home||wpt_index>=miRuta.numwpt)
		wptAlt=autopilotCfg.baseCruiseAltitude;		
	else
		wptAlt=miRuta.wpts[wpt_index].alt;
	
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
				alt+=ikarusInfo.distance_home*autopilotCfg.distanceAltitude/1000.0f;
			break;

		case AP_WPT_SAFE:
			alt=autopilotCfg.baseCruiseAltitude;
			if(ikarusInfo.distance_home>0)
				alt+=ikarusInfo.distance_home*autopilotCfg.distanceAltitude/1000.0f;
			if(wptAlt>alt)
				alt=wptAlt;
			break;

		case AP_WPT_UNSAFE:
			alt=wptAlt;
			break;
	}
	return alt;
}

void ChangeNextIfReached() 
{
	static float last_d;
	float d=ikarusInfo.distance_wpt;
	
	if(go_home)
		return;

	// Cambiamos wpt_index si es necesario, en funcion del modo de rutas
	if(d<storedConfig.wptRange&&last_d<d)
	{
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
	last_d=d;
}

void SetToHome()
{
	go_home=1;
}

void SetDoRoute()
{
//	if(wpt_index>=miRuta.numwpt)
//		go_home=1;
//	else
		go_home=0;
}

float calcDistance(float lon1, float lat1, float lon2, float lat2) large
{
	float sindlat=sin(DEG2RAD(lat1-lat2)/2.0);
	float sindlon=sin(DEG2RAD(lon1-lon2)/2.0);
	float d=2.0*asin(sqrt(sindlat*sindlat + sindlon*sindlon* cos(DEG2RAD(lat1))*cos(DEG2RAD(lat2))));
	if(storedConfig.MetricsImperial==0)
		return d*EARTH_METROS;
	else
		return d*EARTH_MILLAS;
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