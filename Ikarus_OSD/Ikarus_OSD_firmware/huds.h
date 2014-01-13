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

#define ROWS 15
#define COLS 28

void logoIkarus();
void ChangeHUD(int estado) large;
void MuestraHUD(int estado) large;
void MuestraInstrumento(int id) large;
//void PantallaResumen() large;


struct ElementoOSD
{
	char tipo;
	char fila;			
	char col;			 
	char param;
};

struct Screen
{
	struct ElementoOSD Altimetro;	// Altimetro	(x2)
	struct ElementoOSD Autopilot;	// Autopilot
	struct ElementoOSD Bearing;		// Bearing	(x3.5)
	struct ElementoOSD Compas;		// Compas		(x2.5)
	struct ElementoOSD Dist_Home;	// Distancia casa
	struct ElementoOSD Dist_Wpt;	// Distancia wpt
	struct ElementoOSD Hora;		// ?? Hora
	struct ElementoOSD HorizonteArtificial;
	struct ElementoOSD I;			// I (A)
	struct ElementoOSD mAh;			// Consumo (consumido o restante - mAh)
	struct ElementoOSD Lat;			// LON&LAT
	struct ElementoOSD Lon;
	struct ElementoOSD NombreHUD;
	struct ElementoOSD NombrePiloto;
	struct ElementoOSD WptName;		// WPT NAME
	struct ElementoOSD NumSats;		// Num Satelites GPS
	struct ElementoOSD AntTrack;	// Tracker Antena
	struct ElementoOSD AntTrackV;	// Tracker Antena vertical
	struct ElementoOSD Consumo_km_ah;	
	struct ElementoOSD RSSI;		// RSSI
	struct ElementoOSD Tasa_planeo;
	struct ElementoOSD TiempoVuelo;	// ?? Tiempo vuelo
	struct ElementoOSD Variometro;	// Variometro	(x2)
	struct ElementoOSD Velocimetro;	// Velocimetro (x2)
	struct ElementoOSD V1_text;			// V1 (x2 simultaneos)
	struct ElementoOSD V1_bar;	
	struct ElementoOSD V2_text;			// V2 (x2 simultaneos)
	struct ElementoOSD V2_bar;	

	struct ElementoOSD MaxAlt;		// Altitud máxima
	struct ElementoOSD MaxDist;		// Distancia casa máxima
	struct ElementoOSD MaxVelo;		// Velocidad máxima
	struct ElementoOSD TotalDist;	// Distancia recorrida
	struct ElementoOSD BadRX;
	struct ElementoOSD Auxiliary;
	
	char strNombreHUD[16];
};

enum Instrumentos{eAltimetro, eAutopilot, eBearing, eCompas, eDist_Home, eDist_Wpt, eHora,
		eHorizonteArtificial, eI, emAh, eLat, eLon, eNombreHUD, eNombrePiloto, eWptName, 
		eNumSats, eAntTrack, eAntTrackV, eConsumo_km_ah, eRSSI, eTasa_planeo, eTiempoVuelo,  
		eVariometro, eVelocimetro, eV1_text, eV1_bar, eV2_text, eV2_bar, eMaxAlt, eMaxDist, 
		eMaxVelo, eTotalDist, eBadRX, eAuxiliary, };


#define CH_MPH		0xE0
#define CH_KMH		0xE1
#define CH_MI		0xE2
#define CH_KM		0xE3
#define CH_DEG		0xE4
#define CH_BEAR		0xE5
#define CH_ALT		0xE6
#define CH_DHOM		0xE7
#define CH_DWPT		0xE8
#define CH_INTA		0xE9
#define CH_MOTV		0xEA
#define CH_VIDV		0xEB
#define CH_FEET		0xEC

#define CH_ANTT		0xD6
#define CH_RSSI		0xD7
#define CH_NSAT		0xD8
#define CH_ANTTV	0xD9			

#define CH_ROWL		0x97
#define CH_ROWR		0x96
#define CH_ROWU		0x95