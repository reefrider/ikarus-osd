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

#include <stdio.h>
#include <math.h>
#include <string.h>

#include "c8051f340.h"
#include "LibraryMax7456.h"
#include "ParserNMEA.h"
#include "Navigation.h"
#include "Utils.h"
#include "Servos.h"
#include "huds.h"
#include "Ikarus.h"
#include "MenuConfig.h"
#include "Modem.h"

#include "PID.h"

extern xdata struct PID tailPID;

extern xdata struct GPSInfo gpsinfo;
extern xdata struct IkarusInfo ikarusInfo;
extern code struct StoredConfig storedConfig;
extern code struct AutoPilotConfig autopilotCfg;
extern xdata unsigned int servos_out[MAX_CH_OUT];		// for debug only

extern xdata char estado;
extern bit modechanged;

extern xdata unsigned int rflost;

extern xdata unsigned long tics;
extern xdata char CheckedNMEA;
extern xdata char cuenta_menu;

extern code struct Screen huds[5];

xdata struct Screen hud;

extern xdata float tail, motor;

#define BUILD_DATE "Build: " __DATE2__ "-" __TIME__

void logoIkarus()
{
	ClrScr();
	printCenteredAtStr(2, "W E L C O M E");

	printCenteredAtStr(5, "FPV IKARUS OSD");
	printCenteredAtStr(7, "(ATC-US)");
	printCenteredAtStr(11, BUILD_DATE);

	printCenteredAtStr((storedConfig.Video_PAL==1)?14:12,"(c) Ikarus Sevilla 2010");
}


code struct Screen hud_text=
	{{1,0,22,0},				// Altimetro	(x2)
	{0,0,0,0},				// Autopilot
	{2,0,13,0},				// Bearing	(x3.5)
	{1,0,15,0},				// Compas		(x2.5)
	{0,0,0,0},				// Distancia casa
	{1,0,5,0},				// Distancia wpt
	{0,0,0,0},				// ?? Hora
	{0,0,0,0},				// Horizonte Artificial
	{1,-1,14,0},			// I (A)
	{0,0,0,0},				// Consumo (consumido o restante - mAh)
	{0,0,0,0},				// LON&LAT
	{0,0,0,0},	
	{1,3,10,0},				// NombreHUD
	{0,0,0,0},				// NombrePiloto
	{0,0,0,0},				// WPT NAME
	{1,-1,25,0},			// Num Satelites GPS
	{0,0,0,0},				// AntTrack
	{0,0,0,0},				// AntTrackV
	{0,0,0,0},				// consumo_km_ah
	{0,0,0,0},				// RSSI
	{0,0,0,0},				// Tasa planeo
	{0,0,0,0},				// ?? Tiempo vuelo
	{1,0,20,0},				// Variometro	(x2)
	{1,0,0,0},				// Velocimetro (x2)	
	{1,-1,0,0},				// V1 (Text)
	{0,0,0,10},				// V1 (Bar)
	{1,-1,7,0},				// V2 (Text)
	{0,0,0,0},				// V2 (Bar)
	{0,0,0,0},				// Altitud máxima
	{0,0,0,0},				// Distancia casa máxima
	{0,0,0,0},				// Velocidad máxima
	{0,0,0,0},				// Distancia recorrida
	{1,6,12,0},				// Bad RX
	{0,0,0,0},				// Auxiliary

	"Ikarus OSD"			// strNombreHUD
	};	

void ChangeHUD(int estado) large
{
	static int last_estado=-1;

	if(estado!=last_estado)
	{
		if(estado&0x80)
			estado=estado&0x7f;
		if(estado==0)
		{
			memset(&hud,0,sizeof(hud));
		}
		else
		{
			hud=huds[estado-1];		
		}
		last_estado=estado;		
		ClrScr();
	}
}

void MuestraHUD(int estado) large
{
	static int last_id;

	ChangeHUD(estado);

	for(last_id=0;last_id<=eAuxiliary;last_id++)
		MuestraInstrumento(last_id);
}

void MuestraInstrumento(int id) large
{
	char cad[50];
	float var;
	int len;
	
	switch (id)
	{
		case eV1_text:	
			// Voltage V1
			if(ikarusInfo.v1<storedConfig.cellsBatt1*storedConfig.cellAlarm)
				CharAttrBlink();
			else
				CharAttrNoBlink();
			if(hud.V1_text.tipo==1)
			{
				len=sprintf(cad,"%4.1f",ikarusInfo.v1);
				printAtStr(hud.V1_text.fila,hud.V1_text.col,cad);		
				writeAtChr(hud.V1_text.fila,hud.V1_text.col+len,CH_MOTV);
			}
			CharAttrNoBlink();
			break;

		case eV1_bar:
			if(ikarusInfo.v1<storedConfig.cellsBatt1*storedConfig.cellAlarm)
				CharAttrBlink();
			else
				CharAttrNoBlink();
			if(hud.V1_bar.tipo==1)
			{
				Bar(hud.V1_bar.fila,hud.V1_bar.col,hud.V1_bar.param,(ikarusInfo.v1/storedConfig.cellsBatt1-3.2)*255);
			}
			CharAttrNoBlink();
			break;

		case eV2_text:
			// Voltage V2
			if(ikarusInfo.v2<storedConfig.cellsBatt2*storedConfig.cellAlarm)
				CharAttrBlink();
			else
				CharAttrNoBlink();
			if(hud.V2_text.tipo==1)
			{
				len=sprintf(cad,"%4.1f",ikarusInfo.v2);
				printAtStr(hud.V2_text.fila,hud.V2_text.col,cad);		
				writeAtChr(hud.V2_text.fila,hud.V2_text.col+len,CH_VIDV);
			}
			CharAttrNoBlink();
			break;

		case eV2_bar:
			if(ikarusInfo.v2<storedConfig.cellsBatt2*storedConfig.cellAlarm)
				CharAttrBlink();
			else
				CharAttrNoBlink();
			if(hud.V2_bar.tipo==1)
			{
				Bar(hud.V2_bar.fila,hud.V2_bar.col,hud.V2_bar.param,(ikarusInfo.v2/storedConfig.cellsBatt2-3.2)*255);
			}
			CharAttrNoBlink();
			break;

		case eI:
			// Intensidad (A)
			if(hud.I.tipo==1)
			{	
				len=sprintf(cad,"%5.1f",ikarusInfo.currI);
				printAtStr(hud.I.fila,hud.I.col,cad);
				writeAtChr(hud.I.fila,hud.I.col+len,CH_INTA);
			}
			break;

		case emAh:
			// Consumo (mAh?)
			if(hud.mAh.tipo==1)
			{
				sprintf(cad,"%5.0fmAh",ikarusInfo.consumidos_mAh);	
				printAtStr(hud.mAh.fila,hud.mAh.col,cad);
			}
			else if(hud.mAh.tipo==2)
			{
				Bar(hud.mAh.fila,hud.mAh.col,hud.mAh.param,(1-ikarusInfo.consumidos_mAh/storedConfig.total_mAh)*255);
			}
			break;

		case eRSSI:
			// RSSI
			if(hud.RSSI.tipo==1)
			{	
				len=sprintf(cad,"%3.0f",ikarusInfo.RSSI);
				printAtStr(hud.RSSI.fila,hud.RSSI.col,cad);
				writeAtChr(hud.RSSI.fila,hud.RSSI.col+len,CH_RSSI);
			}
			break;

		case eAutopilot:
			// Autopilot
			if(hud.Autopilot.tipo==1)
			{	
				if(ikarusInfo.AutoPilot_Enabled==APLT_ESTAB)
					sprintf(cad,"STAB");
				else if(ikarusInfo.AutoPilot_Enabled==APLT_FULL)
					sprintf(cad,"AUTO");				
				else
					sprintf(cad,"    ");
				printAtStr(hud.Autopilot.fila,hud.Autopilot.col,cad);
			}
			break;

		case eWptName:
			// Nombre destino
			if(hud.WptName.tipo==1)
			{	
				sprintf(cad,"%s        ",GetNameDst());
				printAtStr2(hud.WptName.fila,hud.WptName.col,cad,8);
			}	
			break;

		case eDist_Home:
			//  DISTANCIA a CASA
			if(hud.Dist_Home.tipo==1)
			{
				var=ikarusInfo.distance_home;
				if(var>storedConfig.distanceAlarm)
					CharAttrBlink();
				else
					CharAttrNoBlink();
			
				if(storedConfig.MetricsImperial == 0)
				{
					len=sprintf(cad, "%5.0f", var);		// metrico (metros)
				}
				else
				{
					len=sprintf(cad, "%5.2f", var/1852.0f);		// Imperial (millas)
				}

				printAtStr(hud.Dist_Home.fila,hud.Dist_Home.col,cad);
				writeAtChr(hud.Dist_Home.fila,hud.Dist_Home.col+len,CH_DHOM);
				CharAttrNoBlink();
			}
			else if(hud.Dist_Home.tipo==2)
			{
				var=ikarusInfo.distance_home;

				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad, "Distancia Casa %5.0fm", var);
					printAtStr2(hud.Dist_Home.fila,hud.Dist_Home.col,cad,21);
				}
				else
				{
					len=sprintf(cad, "Distancia Casa %5.2f", var/1852.0f);
					printAtStr2(hud.Dist_Home.fila,hud.Dist_Home.col,cad,21);
					writeAtChr(hud.Dist_Home.fila,hud.Dist_Home.col+len,CH_MI);
				}
			}
			break;

		case eDist_Wpt:
			// DISTANCIA a WPT
			if(hud.Dist_Wpt.tipo==1)
			{
				var=ikarusInfo.distance_wpt; // en metros
			
				if(storedConfig.MetricsImperial == 0)
				{
					len=sprintf(cad, "%5.0f", var);
			
				}
				else
				{
					len=sprintf(cad, "%5.2f", var/1852.0f);
				}
				printAtStr(hud.Dist_Wpt.fila,hud.Dist_Wpt.col,cad);
				writeAtChr(hud.Dist_Wpt.fila,hud.Dist_Wpt.col+len,CH_DWPT);
			
			}
			
			break;

		case eBearing:
			// BEARING
			var=(int)ikarusInfo.navigator_rel_bearing;
				
			if(hud.Bearing.tipo==1)	// Relative bearing
			{
				len=sprintf(cad, "%4.0f", var);
				printAtStr(hud.Bearing.fila,hud.Bearing.col,cad);
				writeAtChr(hud.Bearing.fila,hud.Bearing.col+len,CH_BEAR);
			}
			else if(hud.Bearing.tipo==2)
			{
				COMPAS_chr(hud.Bearing.fila,hud.Bearing.col,var);
			}
			else if(hud.Bearing.tipo==3)
			{
				COMPAS_grp(hud.Bearing.fila,hud.Bearing.col,var);
			}
			else if (hud.Bearing.tipo==4) // Absolute bearing
			{
				var = ikarusInfo.navigator_bearing;
				if(var<0)
					var+=360.0f;
				len=sprintf(cad, "%3.0f", var);
				printAtStr(hud.Bearing.fila,hud.Bearing.col,cad);
				writeAtChr(hud.Bearing.fila,hud.Bearing.col+len,CH_BEAR);
			}
			break;

		case eNumSats:
			// Número satelites
			if(hud.NumSats.tipo==1)
			{	
				if(!gpsinfo.conected)
					printAtStr(hud.NumSats.fila,hud.NumSats.col,"BRK");
				else if(!gpsinfo.nmea_ok)
					printAtStr(hud.NumSats.fila,hud.NumSats.col,"CRC");
				else if(!gpsinfo.pos_valid)
				{
					if(gpsinfo.numsats>9)
						printAtStr(hud.NumSats.fila,hud.NumSats.col,"NP9");
					else
					{
						sprintf(cad, "NP%d",gpsinfo.numsats);
						printAtStr(hud.NumSats.fila,hud.NumSats.col,cad);
					}
				}
				else if(gpsinfo.numsats>=0&&gpsinfo.numsats<20)
				{
					sprintf(cad,"%2d",gpsinfo.numsats);
					printAtStr(hud.NumSats.fila,hud.NumSats.col,cad);
					writeAtChr(hud.NumSats.fila,hud.NumSats.col+2,CH_NSAT);
				}
				else
					printAtStr(hud.NumSats.fila,hud.NumSats.col,"ERR");
			}
			break;

		case eAntTrack:
			if(hud.AntTrack.tipo==1)
			{
				var=ikarusInfo.AntTracker;
				if(var<0)
					var+=360.0f;
				len=sprintf(cad, "%3.0f", var);
				printAtStr(hud.AntTrack.fila,hud.AntTrack.col,cad);
				writeAtChr(hud.AntTrack.fila,hud.AntTrack.col+len,CH_ANTT);
			}
			else if(hud.AntTrack.tipo==2)
			{
				var=ikarusInfo.AntTracker;
				if(var>=180)
					var-=360.0f;
				len=sprintf(cad, "%4.0f", var);
				printAtStr(hud.AntTrack.fila,hud.AntTrack.col,cad);
				writeAtChr(hud.AntTrack.fila,hud.AntTrack.col+len,CH_ANTT);
			}

			break;

		case eAntTrackV:
			if(hud.AntTrackV.tipo==1)
			{
				var=ikarusInfo.AntTrackerV;
				len=sprintf(cad, "%3.0f", var);
				printAtStr(hud.AntTrackV.fila,hud.AntTrackV.col,cad);
				writeAtChr(hud.AntTrackV.fila,hud.AntTrackV.col+len,CH_ANTTV);
			}
			break;

		case eLon:
			// LON & LAT
			if(hud.Lon.tipo==1)
			{	
				sprintf(cad, "Lon:%10.5f", gpsinfo.lon);
				printAtStr(hud.Lon.fila,hud.Lon.col,cad);
			}	
			break;

		case eLat:
			if(hud.Lat.tipo==1)
			{	
				sprintf(cad, "Lat:%9.5f", gpsinfo.lat);
				printAtStr(hud.Lat.fila,hud.Lat.col,cad);
			}	
			break;

		case eAltimetro:
			// DIBUJA ALTIMETRO
			if(hud.Altimetro.tipo==1)
			{
				var=getRelAltitude() ;	
				if(var<storedConfig.altitudeAlarm)
					CharAttrBlink();
				else
					CharAttrNoBlink();
				len=sprintf(cad,"%5.0f",(storedConfig.MetricsImperial == 0)?var:var*3.28f);
				printAtStr(hud.Altimetro.fila,hud.Altimetro.col,cad);
				writeAtChr(hud.Altimetro.fila,hud.Altimetro.col+len,CH_ALT);
				CharAttrNoBlink();
			}
			else if(hud.Altimetro.tipo==2)		
			{
				var=getRelAltitude();
				
				// DIBUJA ALTIMETRO FIGHTER
				Altimetro(hud.Altimetro.fila,hud.Altimetro.col+4,hud.Altimetro.param,(storedConfig.MetricsImperial == 0)?var:var*3.28f);
				if(var<storedConfig.altitudeAlarm)
					CharAttrBlink();
				else
					CharAttrNoBlink();
				sprintf(cad,"%4.0f",(storedConfig.MetricsImperial == 0)?var:var*3.28f);
				printAtStr2(hud.Altimetro.fila+hud.Altimetro.param/2,hud.Altimetro.col,cad,4);
	
				if(storedConfig.MetricsImperial == 0)
					printAtChr(hud.Altimetro.fila+hud.Altimetro.param/2+1,hud.Altimetro.col+2,'m');
				else
					writeAtChr(hud.Altimetro.fila+hud.Altimetro.param/2+1,hud.Altimetro.col+2,CH_FEET);

				CharAttrNoBlink();
			}
			break;

		case eVelocimetro:
			// DIBUJA VELOCIMETRO
			if(hud.Velocimetro.tipo==1)
			{
				var=gpsinfo.knots_filtered;
				
				if(var<storedConfig.lowSpeedAlarm)
					CharAttrBlink();
				else
					CharAttrNoBlink();

				sprintf(cad,"%3.0f",(storedConfig.MetricsImperial == 0)?var*1.852f:var);
				printAtStr2(hud.Velocimetro.fila,hud.Velocimetro.col,cad,3);	
				writeAtChr(hud.Velocimetro.fila,hud.Velocimetro.col+3,(storedConfig.MetricsImperial == 0)?CH_KMH:CH_MPH);

				CharAttrNoBlink();
			}
			else if(hud.Velocimetro.tipo==2)
			{
				// DIBUJA VELOCIMETRO FIGHTER
				CharAttrNoBlink();
				
				var=gpsinfo.knots_filtered;
				
				Velocimetro(hud.Velocimetro.fila,hud.Velocimetro.col,hud.Velocimetro.param,(storedConfig.MetricsImperial == 0)?var*1.852f:var);
				if(var<storedConfig.lowSpeedAlarm)
					CharAttrBlink();
				else
					CharAttrNoBlink();
				sprintf(cad,"%3.0f",(storedConfig.MetricsImperial == 0)?var*1.852f:var);
				printAtStr2(hud.Velocimetro.fila+hud.Velocimetro.param/2,hud.Velocimetro.col+1,cad,3);
				printAtStr(hud.Velocimetro.fila+hud.Velocimetro.param/2+1,hud.Velocimetro.col+1,(storedConfig.MetricsImperial == 0)?"kmh":"mph");		
	
				CharAttrNoBlink();
			}
			break;

		case eCompas:
			// DIBUJA COMPAS
			if(hud.Compas.tipo==1)
			{
				sprintf(cad,"%3.0f",gpsinfo.rumbo);
				printAtStr(hud.Compas.fila,hud.Compas.col,cad);
				writeAtChr(hud.Compas.fila,hud.Compas.col+3,CH_DEG);
			}
			else if(hud.Compas.tipo==2)
			{
				Compas(hud.Compas.fila,hud.Compas.col,hud.Compas.param,gpsinfo.rumbo,ikarusInfo.navigator_bearing);	// Compas&bearing fighter
			}
			break;

		case eVariometro:
			// DIBUJA VARIOMETRO
			if(hud.Variometro.tipo==1)
			{
				Variometro1(hud.Variometro.fila,hud.Variometro.col,ikarusInfo.verticalSpeed);
			}
			else if(hud.Variometro.tipo==2)
			{
				Variometro2(hud.Variometro.fila,hud.Variometro.param, hud.Variometro.col,(int)ikarusInfo.verticalSpeed);
			}
			break;

		case eHorizonteArtificial:
			if(hud.HorizonteArtificial.tipo==1||hud.HorizonteArtificial.tipo==2)
			{
				HorizonteArtificial(hud.HorizonteArtificial.fila, hud.HorizonteArtificial.col, 
					hud.HorizonteArtificial.param, ikarusInfo.Pitch, ikarusInfo.Roll,
					hud.HorizonteArtificial.tipo-1); 
			}			
			break;

		case eMaxAlt:
			if(hud.MaxAlt.tipo==1)
			{	
				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad, "Altitud MAX:%5.0fm ", gpsinfo.altitudeMAX);
					printAtStr(hud.MaxAlt.fila,hud.MaxAlt.col,cad);
				}
				else
				{
					sprintf(cad, "Altitud MAX:%5.0fft ", gpsinfo.altitudeMAX*3.28f);
					printAtStr(hud.MaxAlt.fila,hud.MaxAlt.col,cad);
				}
			}	
			break;

		case eMaxVelo:
			if(hud.MaxVelo.tipo==1)
			{	
				len=sprintf(cad, "Velocidad MAX:%3.0f", (storedConfig.MetricsImperial == 0)?gpsinfo.knotsMAX*1.852f:gpsinfo.knotsMAX);
				printAtStr(hud.MaxVelo.fila,hud.MaxVelo.col,cad);
				printAtStr(hud.MaxVelo.fila,hud.MaxVelo.col+len,(storedConfig.MetricsImperial == 0)?"Km/h":"Mph");
			}	
			break;

		case eMaxDist:
			if(hud.MaxDist.tipo==1)
			{	
				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad, "Distancia Max: %4.1fKm", ikarusInfo.max_distance_home/1000.0f);
					printAtStr(hud.MaxDist.fila,hud.MaxDist.col,cad);
				}
				else
				{
					sprintf(cad, "Distancia Max: %4.1fMi", ikarusInfo.max_distance_home/1852.0f);
					printAtStr(hud.MaxDist.fila,hud.MaxDist.col,cad);
				}
			}	
			break;

		case eTotalDist:
			if(hud.TotalDist.tipo==1)
			{	
				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad, "%4.1fKm", ikarusInfo.distancia_recorrida/1000.0f);
					printAtStr(hud.TotalDist.fila,hud.TotalDist.col,cad);
				}
				else
				{
					sprintf(cad, "%4.1fMi", ikarusInfo.distancia_recorrida/1852.0f);
					printAtStr(hud.TotalDist.fila,hud.TotalDist.col,cad);
				}
			}
			else if(hud.TotalDist.tipo==2)
			{	
				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad, "Dist. Recorrida: %4.1fKm", ikarusInfo.distancia_recorrida/1000.0f);
					printAtStr(hud.TotalDist.fila,hud.TotalDist.col,cad);
				}
				else
				{
					sprintf(cad, "Dist. Recorrida: %4.1fMi", ikarusInfo.distancia_recorrida/1852.0f);
					printAtStr(hud.TotalDist.fila,hud.TotalDist.col,cad);
				}
			}
			break;

		case eHora:
			if(hud.Hora.tipo==1)
			{
				long tmp=gpsinfo.hora;
				sprintf(cad,"%2d:%2.2d", (int)(get_hora_zona(tmp,storedConfig.TimeZone)), (int)get_mins(tmp));
				printAtStr(hud.Hora.fila,hud.Hora.col,cad);
			}
			else if(hud.Hora.tipo==2)
			{
				long tmp=gpsinfo.hora;
				sprintf(cad,"%2d:%2.2d:%2.2d", (int)(get_hora_zona(tmp,storedConfig.TimeZone)), (int)get_mins(tmp), (int)get_secs(tmp));
				printAtStr(hud.Hora.fila,hud.Hora.col,cad);
			}
			break;

		case eTiempoVuelo:
			if(hud.TiempoVuelo.tipo==1)
			{
				long tmp;
				tmp=get_timesecs((long)ikarusInfo.hora_inicio);
				tmp=get_timesecs((long)gpsinfo.hora)-tmp;
				sprintf(cad,"%2d:%2.2d ", (int)secs2hora(tmp), (int)secs2mins(tmp));
				printAtStr(hud.TiempoVuelo.fila,hud.TiempoVuelo.col,cad);
			}
			else if(hud.TiempoVuelo.tipo==2)
			{
				long tmp;
				tmp=get_timesecs((long)ikarusInfo.hora_inicio);
				tmp=get_timesecs((long)gpsinfo.hora)-tmp;
				sprintf(cad,"%2d:%2.2d:%2.2d ", (int)secs2hora(tmp),(int)secs2mins(tmp), (int)secs2secs(tmp));
				printAtStr(hud.TiempoVuelo.fila,hud.TiempoVuelo.col,cad);
			}
			else if(hud.TiempoVuelo.tipo==3)
			{
				long tmp;
				tmp=get_timesecs((long)ikarusInfo.hora_inicio);
				tmp=get_timesecs((long)gpsinfo.hora)-tmp;
				sprintf(cad,"Tiempo Vuelo: %2d:%2.2d:%2.2d ", (int)secs2hora(tmp),(int)secs2mins(tmp), (int)secs2secs(tmp));
				printAtStr(hud.TiempoVuelo.fila,hud.TiempoVuelo.col,cad);
			}
			break;

		case eTasa_planeo:
			// tasa de planeo
			if(hud.Tasa_planeo.tipo==1)
			{
				int tmp =(int)ikarusInfo.tasa_planeo;
				sprintf(cad,"%2d:1 ", tmp);
				printAtStr(hud.Tasa_planeo.fila,hud.Tasa_planeo.col,cad);
			}
			break;

		case eConsumo_km_ah:
			// consumo km/Ah o m/mAh (es lo mismo)
			if(hud.Consumo_km_ah.tipo==1)
			{
				if(storedConfig.MetricsImperial == 0)
				{
					sprintf(cad,"%2.1fKm/Ah ", ikarusInfo.coste_km_Ah);
					printAtStr(hud.Consumo_km_ah.fila,hud.Consumo_km_ah.col,cad);
				}
				else
				{
					sprintf(cad,"%2.1fMi/Ah ", ikarusInfo.coste_km_Ah/1.852f);
					printAtStr(hud.Consumo_km_ah.fila,hud.Consumo_km_ah.col,cad);
				}
			}
			break;

		case eNombrePiloto:
			// Nombre piloto
			if(hud.NombrePiloto.tipo==1)
			{	
				printAtStr2(hud.NombrePiloto.fila,hud.NombrePiloto.col,storedConfig.NombrePiloto,20);
			}
			break;

		case eNombreHUD:
			// Cadena Usuario
			if(hud.NombreHUD.tipo==1)
			{	
				printAtStr2(hud.NombreHUD.fila,hud.NombreHUD.col,hud.strNombreHUD,16);
			}
			break;

		case eBadRX:
			// Cadena Usuario
			if(hud.BadRX.tipo==1)
			{	
				if(ikarusInfo.failsafe == FSS_STAB || ikarusInfo.failsafe == FSS_NOSTAB)
					sprintf(cad,"BAD_RX %2.0f",ikarusInfo.failsafe_countdown);
				else if(ikarusInfo.uplink_cmd_countdown>0)
				{
					MensajeUplink(cad);
					//sprintf(cad,"UPLINK CMD OK   ");
				}
				else if(cuenta_menu > 10)
					sprintf(cad,"MENU IN %2.0f",(DELAY_MENU-cuenta_menu)/10.0);
				else
					sprintf(cad,"                ");
				printAtStr2(hud.BadRX.fila,hud.BadRX.col,cad,16);
				
			}
			break;		

		case eAuxiliary:
			if(hud.Auxiliary.tipo==1)
			{
				char fila=hud.Auxiliary.fila;
				char col = hud.Auxiliary.col;
				float bear= ikarusInfo.navigator_bearing;
				int tmp;

				if(bear<0.0f)
					bear+=360.0f;

				if(hud.Auxiliary.param&2)
					CharAttrBackGr();
				
				if(storedConfig.ControlProportional==MODO_AJUSTE223 && autopilotCfg.AutopilotMode!=AP_DISABLED)
				{
					HUD_Debug(fila, col, hud.Auxiliary.param&4);
					fila++;
				}
			
				tmp = sprintf(cad,"ALT %4.0f/%4.0f ", getRelAltitude(),	ikarusInfo.navigator_altitude);
				sprintf(cad+tmp,"PITCH %3.0f/%3.0f ", ikarusInfo.Pitch, ikarusInfo.AutoPilot_actitud_pitch);
				printAtStr(fila++, col, cad);

				tmp = sprintf(cad,"HDG %4.0f/%4.0f ", gpsinfo.rumbo,	bear);
				sprintf(cad+tmp,"ROLL  %3.0f/%3.0f ", ikarusInfo.Roll, ikarusInfo.AutoPilot_actitud_roll);
				printAtStr(fila++, col, cad);
				
				if(hud.Auxiliary.param&1)
				{
					tmp = sprintf(cad,"A %4d E %4d ", servos_out[AIL], servos_out[ELE]);
					sprintf(cad+tmp,"M %4d T %4d ", servos_out[THR], servos_out[TAIL]);
					printAtStr(fila++, col, cad);
				}
				else
				{
					tmp = sprintf(cad,"A%5.2f E%5.2f ",	ikarusInfo.AutoPilot_ail, ikarusInfo.AutoPilot_ele);
					sprintf(cad+tmp,"M%5.2f T%5.2f ", ikarusInfo.AutoPilot_thr, ikarusInfo.AutoPilot_tail);
					printAtStr(fila++, col, cad);
				}
				
				CharAttrNoBackGr();
						
			}
			
			break;

		default:	//
			break;
	}
}