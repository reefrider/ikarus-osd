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

#include "c8051f340.h"


//#define IMU_UM6
#define SECURITY_FW 		// Proteccion codigo


// Funcion externa en USB_Interface
void USB_Connection();

#define MNU_ENTER 		0x00 	//2
#define MNU_NEXT	 	0x01	//1



#define FS_DLY_MAX	2 //10

#define PPM_HOLD_COUNT_MAX	20
#define RFLOST_MAX			20
#define FS_POS_MAX			20
#define MODEM_LOST_MAX		20
#define MODEM_CRC_MAX		20
#define UPLINK_CMD_MAX		30

#define DELAY_MENU			50


enum Pantallas{SC_Clear, SC_hud1, SC_hud2, SC_hud3, SC_FailSafe, SC_Resumen};
enum ADC_NAMES{ADC_V2=0,ADC_I=1,ADC_V1, ADC_RSSI, ADC_temp, ADC_IR_X, ADC_IR_Y, ADC_IR_Z}; 
enum MEZCLAS{MEZCLA_INH, MEZCLA_ELEVON, MEZCLA_VTAIL};
enum AUX_MODE{AUX_AUTOPLT, AUX_WPT, AUX_TILT, AUX_AIL2};

enum NAVIGATE{NAV_HOME, NAV_RUTA, NAV_HOLD, NAV_HOLD_FS, NAV_MODEM};
enum SERVOS{SERVO_COPY, SERVO_REPLACE, SERVO_MIX, SERVO_BITMASKS};

enum MODOS{MODO_SW2, MODO_SW3_BASIC, MODO_SW3, MODO_RUEDA, MODO_MIX223, MODO_AJUSTE223, MODO_MIX224, MODO_MODEM, MODO_SW33};
enum COMANDOS{PG_IKARUSCFG, PG_AUTOPILOTCFG, PG_RUTA, PG_SCREEN, PG_GPSINFO, PG_IKARUSINFO, PG_CHARSET, PG_ADCVALUES, PG_SERVOS, PG_SERVOS_RAW};

enum AUTOPILOT_TYPE{AP_DISABLED, AP_FIX_ALT, AP_DIST_ALT, AP_WPT_SAFE, AP_WPT_UNSAFE};
enum AUTOPILOT_MODE{APLT_DISABLED, APLT_ESTAB, APLT_FULL};
enum FAILSAFE_STATUS{FSS_NORMAL, FSS_HOLD, FSS_STAB, FSS_NOSTAB, FSS_AUTO, FSS_NOAUTO};
enum IR_SENSOR{IR_XY, IR_XYZ, IR_NONE};


//#define SFR16(name, addr)      sfr16 name = addr

sfr16 ADC0		= 0xBD;
sfr16 TMR2RL	= 0xCA;
sfr16 PCA0CP1	= 0xE9;
sfr16 PCA0CP2	= 0xEB;
sfr16 PCA0CP3	= 0xED;
sfr16 PCA0		= 0xF9;
sfr16 PCA0CP0	= 0xFB;
sfr16 PCA0CP4	= 0xFD;

#define CLR_WDT()		PCA0CPH4=0x55

sbit NSS=P0^3;

sbit HSYNC = P0^6;
sbit VSYNC = P0^7;
sbit TELEMETRY = P1^0;


sbit CAM_SEL=P1^7;
sbit SERVOIN=P1^4;

sbit SERVOAIL=P2^0;
sbit SERVOELE=P2^1;
sbit SERVOTHR=P2^2;
sbit SERVOTAIL=P2^3;
sbit SERVOPAN=P2^4;
sbit SERVOAUX=P2^5;

/////////////////////

struct StoredConfig
{
	// OSD
	char Video_PAL; 	// 1 -> PAL, 0 -> NTSC, 0xff-> Eprom empty
	char offsetX;
	char offsetY;

	// Baterias
	char cellsBatt1;
	float offset_sensorV1;
	float gain_sensorV1;

	char cellsBatt2;
	float offset_sensorV2;
	float gain_sensorV2;

	float total_mAh;
	float offset_sensorI;			// mV
	float gain_sensorI;				// mV/A
	

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
	float min_rssi;
	float max_rssi;

	unsigned char Modo_PPM;
	unsigned char Canal_PPM;
	unsigned char NumCanales_PPM;
	unsigned char Modo_Failsafe;
	unsigned char Retraso_Failsafe;

	unsigned char LeftBand;

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
//	char rev;
};

struct AutoPilotConfig
{
	char servos_cfg[7*7];	//struct ServoInfo servos_cfg[7];		

	char AutopilotMode;
	float baseCruiseAltitude;
	float distanceAltitude;

	// Canales PPM
	char ch_ail;
	char ch_ele;
	char ch_thr;
	char ch_tail;
	char ch_pan;
	char ch_aux;

	float pan_gain;
	char tipo_mezcla;		// Normal, Elevon, V-Tail
	char rev_mezcla;
		
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

	char chAux_mode;
	int MotorSafeAlt;

	// Copilot ganancias
	char copilot_use_PID_gains;
	float copilot_pitch_P;
	float copilot_roll_P;

	// Autopilot mezclado
	char mezclaAutopilot_roll_on;
	float mezclaAutopilot_roll_value;

	char ServoMotorEnableAlt;
	int ServoMotorSafeAlt;
};

struct IkarusInfo
{
	float v1;
	float v2;
	float currI;
	float consumidos_mAh;

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
	char modem_wptid;

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
	float AutoPilot_actitud_pitch;
	float AutoPilot_actitud_roll;

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

	float lastAltitude;
	float lastHeading;
	char cambioWpt;

	char failsafe;
	char servoDriver; 
	char navigateTo;

	float verticalSpeed;

	char ppm_hold;

	float failsafe_countdown;
	unsigned char uplink_cmd_received;
	unsigned char uplink_cmd_countdown;
	float uplink_cmd_param;

	char servos_bitmasks;
};
