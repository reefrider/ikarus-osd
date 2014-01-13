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
#include "Portable.h"
#include "plugin.h"

//#define DEBUG				// Opciones de depuracion

#ifndef DEBUG

#define SECURITY_FW 		// Proteccion codigo

#endif

// Opciones que SI deben estar activas
//#define	NEW_DIVISOR_V		// Divisor de las nuevas placas

// Opciones que NO deben estar activas por defecto

//#define DUMMY				// No actualizable
//#define OLD_BOARD_I		// Primeras pcb tenian mal el divisor de I		
//#define NO_FILTER 		// Desactiva filtrado (no se recomienda)


// Funcion externa en USB_Interface
void USB_Connection();

#define MNU_ENTER 		0x00 	//2
#define MNU_NEXT	 	0x01	//1

enum Pantallas{SC_Clear, SC_hud1, SC_hud2, SC_hud3, SC_FailSafe, SC_Resumen, SC_Debug};
enum ADC_NAMES{ADC_V2=0,ADC_I=1,ADC_V1, ADC_RSSI, ADC_temp, ADC_IR_X, ADC_IR_Y, ADC_IR_Z}; 
enum MEZCLAS{MEZCLA_INH, MEZCLA_ELEVON, MEZCLA_VTAIL};
enum AUTOPILOT_MODE{AP_DISABLED, AP_FIX_ALT, AP_DIST_ALT, AP_WPT_SAFE, AP_WPT_UNSAFE};
enum MODOS{MODO_SW2, MODO_SW3, MODO_RUEDA, MODO_MIX223, MODO_MIX224, MODO_MODEM};
enum COMANDOS{PG_IKARUSCFG, PG_AUTOPILOTCFG, PG_RUTA, PG_SCREEN, PG_GPSINFO, PG_IKARUSINFO, PG_CHARSET, PG_ADCVALUES, PG_SERVOS};

struct StoredConfig
{
	// OSD
	char Video_PAL; 	// 1 -> PAL, 0 -> NTSC, 0xff-> Eprom empty
	char offsetX;
	char offsetY;

	// Baterias
	char cellsBatt1;
	char cellsBatt2;
	float total_mAh;

	// alarms
	float cellAlarm;
	float distanceAlarm;
	float altitudeAlarm;
	float lowSpeedAlarm;

	// GPS
	float HomeLon;
	float HomeLat;
	float HomeAltitude;

	// otros
	float wptRange;
	char DefaultHUD;
	char TelemetryMode;
	char ControlProportional;
	char RelativeAltitude;
	char BaudRate;
	
	char MetricsImperial;			// Vamos a usar el sistema metrico o imperial

	char TimeZone;
	char CamSel;
	char modelo_ruta;				// Despues del ultimo waypoint...
	unsigned char line_telemetry;	// Por defecto deberia ser 20
	float offset_sensorI;			// mV
	float gain_sensorI;				// mV/A
	float min_rssi;
	float max_rssi;

	unsigned char Modo_PPM;
	unsigned char Canal_PPM;

	char NombrePiloto[16];
	};



struct ServoInfo
{
	int min;
	int max;
	int center;
	//int pos;
	unsigned char reverse;
};


struct PID_Setup
{
	float P;
	float I;
	float D;
	float IL;
	float DL;
	char rev;
};

struct AutoPilotConfig
{
#ifdef SIMULADOR
	struct ServoInfo servos_cfg[5];
#else
	char servos_cfg[5*7];	//struct ServoInfo servos_cfg[5];		
#endif
	char AutopilotMode;
	float baseCruiseAltitude;
	float distanceAltitude;

	// Canales PPM
	char ch_ail;
	char ch_ele;
	char ch_thr;
	char ch_tail;
	char ch_pan;
	char ch_tilt;

	float pantilt_gain;
	char tipo_mezcla;		// Normal, Elevon, V-Tail
		
	// Sersor copilot
	char IR_reverse_pitch;
	char IR_reverse_roll;
	char IR_crossed;
	char IR_reverse_cross;

    char IR_Z_enabled;
    float x_off, y_off, z_off;
    float IR_max;

	// Ganancias PID
	struct PID_Setup pid_pitch;
	struct PID_Setup pid_roll;
	struct PID_Setup pid_motor;
	struct PID_Setup pid_tail;

	// Grado mezcla rumbo y altura con ail y ele
	float rumbo_ail;
	float altura_ele;
};

struct IkarusInfo
{
	float v1;
	float v2;
	float currI;
	float consumidos_mAh;
	float mv_currI;

	char AutoPilot_Enabled;

	float navigator_bearing;			// Rumbo a destino
	float navigator_altitude;
	float navigator_rel_bearing;		// Error de rumbo

	float AntTracker;
	float AntTrackerV;

	float distance_home;
	float distance_wpt;

	char modem_sw;						// datos subidos desde la emisora
	int modem_alt;
	float modem_lon;
	float modem_lat;

	// Obtenido desde los switches de la emisora
	char ctrl_hudnum;
	char ctrl_autopilot;
	char ctrl_doruta;
	char ctrl_camsel;

	// Sensor Copilot/IMU
	float Pitch;
	float Roll;

	// Salida del piloto automatico
	float AutoPilot_ail;
	float AutoPilot_ele;
	float AutoPilot_thr;
	float AutoPilot_tail;

	// Salida piloto alto nivel
	float AutoPilot_pitch;
	float AutoPilot_roll;

	// Calculados
	float distancia_recorrida;			
	float max_distance_home;
	float hora_inicio;
	float tasa_planeo;
	float coste_km_Ah;		
	float RSSI;

	// Otros
	char mostrarResumen;

	float tempC;
};


// cosas temporales

void InitIkarus(const TDataFromAeroSimRC *ptDataFromAeroSimRC);
void Ikarus(const TDataFromAeroSimRC *ptDataFromAeroSimRC, TDataToAeroSimRC *ptDataToAeroSimRC);


void LoadStoredConfig();
void LoadAutopilotConfig();
void SaveStoredConfig();
void SaveAutopilotConfig();
	