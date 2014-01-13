#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
#include <windows.h>
#include <stdio.h>
#include <direct.h>
#include <string.h>

#include "plugin.h"

#include "ParserNMEA.h"
#include "ikarus.h"
#include "Navigation.h"
#include "huds.h"
#include "AutoPilot.h"
#include "Servos.h"
#include "MenuConfig.h"
#include "LibraryMAX7456.h"
#include "Utils.h"
#include "Controladores.h"


struct IkarusInfo ikarusInfo;
struct StoredConfig storedConfig;
struct AutoPilotConfig autopilotCfg;

struct Ruta miRuta;

extern struct Screen hud_text;
struct Screen huds[5];

extern struct GPSInfo gpsinfo;
extern bool go_home;

extern char strDefaultPATH[];
extern char strOutputFolder[MAX_PATH];

extern xdata unsigned int servos_in[MAX_CH_IN];

const TDataFromAeroSimRC *ptDataFromAeroSimRC;
TDataToAeroSimRC *ptDataToAeroSimRC;


enum Estados{ST_LOGO, ST_MENU, ST_VOLANDO};

int Ikarus_estado= ST_LOGO;
float integrationTime=0.0f;
extern bool bMenuOverride;
void Ikarus(const TDataFromAeroSimRC *ptDataFromAeroSim, TDataToAeroSimRC *ptDataToAeroSim)
{

	ptDataFromAeroSimRC = ptDataFromAeroSim;
	ptDataToAeroSimRC = ptDataToAeroSim;

	SimulaSERVO();
	if(!bMenuOverride)
		ParseControl(servos_in[CTRL]);
	else
	{
		Ikarus_estado=ST_VOLANDO;
	}

	switch(Ikarus_estado)
	{
		case ST_LOGO:
			logoIkarus();
			integrationTime+=ptDataFromAeroSimRC->Simulation_fIntegrationTimeStep;
			if(integrationTime>2)
			{
				Ikarus_estado=ST_MENU;
				ClrScr();
			}
			break;

		case ST_MENU:
			if(WaitForGPSscreen())
				Ikarus_estado=ST_VOLANDO;
			break;

		case ST_VOLANDO:
	
			SimulaADC();
			SimulaGPS();
			SimulaIR();
			UpdateNavigator();
			Control_ServosIN();
			Control_ServosOUT();

			if(ikarusInfo.AutoPilot_Enabled)
			{
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_AILERON]=1;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_ELEVATOR]=1;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_THROTTLE]=1;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_RUDDER]=1;
			}
			else
			{
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_AILERON]=0;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_ELEVATOR]=0;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_THROTTLE]=0;
				ptDataToAeroSimRC->Channel_abOverride_TX[CH_RUDDER]=0;
			}

//			MuestraHUD(SC_Debug);
			MuestraHUD();
			break;
	}
}

void InitIkarus(const TDataFromAeroSimRC *ptDataFromAeroSim)
{
	ptDataFromAeroSimRC = ptDataFromAeroSim;

	bMenuOverride=0;
	Ikarus_estado= ST_LOGO;
	integrationTime=0.0f;

	LoadStoredConfig();
	LoadAutopilotConfig();

	ikarusInfo.ctrl_hudnum = 0;

	gpsinfo.numsats=6;

	miRuta.numwpt=4;

	if(strlen(ptDataFromAeroSimRC->Scenario_strWPA_Description)>0)
		strcpy(miRuta.wpts[0].Name,ptDataFromAeroSimRC->Scenario_strWPA_Description);
	else
		strcpy(miRuta.wpts[0].Name,"Wpt 0");

	miRuta.wpts[0].lon = ptDataFromAeroSimRC->Scenario_fWPA_Long;
	miRuta.wpts[0].lat = ptDataFromAeroSimRC->Scenario_fWPA_Lat;

	if(strlen(ptDataFromAeroSimRC->Scenario_strWPB_Description)>0)
		strcpy(miRuta.wpts[1].Name,ptDataFromAeroSimRC->Scenario_strWPB_Description);
	else
		strcpy(miRuta.wpts[1].Name,"Wpt 1");	
	miRuta.wpts[1].lon = ptDataFromAeroSimRC->Scenario_fWPB_Long;
	miRuta.wpts[1].lat = ptDataFromAeroSimRC->Scenario_fWPB_Lat;

	if(strlen(ptDataFromAeroSimRC->Scenario_strWPC_Description)>0)
		strcpy(miRuta.wpts[2].Name,ptDataFromAeroSimRC->Scenario_strWPC_Description);
	else
		strcpy(miRuta.wpts[2].Name,"Wpt 2");	
	miRuta.wpts[2].lon = ptDataFromAeroSimRC->Scenario_fWPC_Long;
	miRuta.wpts[2].lat = ptDataFromAeroSimRC->Scenario_fWPC_Lat;

	if(strlen(ptDataFromAeroSimRC->Scenario_strWPD_Description)>0)
		strcpy(miRuta.wpts[3].Name,ptDataFromAeroSimRC->Scenario_strWPD_Description);
	else
		strcpy(miRuta.wpts[3].Name,"Wpt 3");	
	miRuta.wpts[3].lon = ptDataFromAeroSimRC->Scenario_fWPD_Long;
	miRuta.wpts[3].lat = ptDataFromAeroSimRC->Scenario_fWPD_Lat;

	go_home=false;

	InitAutoPilot();
}

void SaveStoredConfig()
{

	char strFileName[MAX_PATH];
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\StoredConfig.bin");
	FILE *pFile = fopen(strFileName, "wb");

	if(pFile)
	{
		fwrite(&storedConfig,sizeof(struct StoredConfig),1,pFile);
		fclose(pFile);
	}
}

void LoadStoredConfig()
{
	int readOK=0;
	char strFileName[MAX_PATH];
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\StoredConfig.bin");
	FILE *pFile = fopen(strFileName, "rb");

	if(pFile)
	{
		readOK=fread(&storedConfig,sizeof(struct StoredConfig),1,pFile);
		fclose(pFile);
	}

	if(!readOK)
	{
		memset(&storedConfig, 0, sizeof(struct StoredConfig));
		storedConfig.wptRange =100;
		storedConfig.cellsBatt1=NumOfCells(ptDataFromAeroSimRC->Model_fBatteryVoltage);
		storedConfig.cellsBatt2=storedConfig.cellsBatt1;
		storedConfig.total_mAh = ptDataFromAeroSimRC->Model_fBatteryCapacity*1000;

		storedConfig.cellAlarm=3.5f;
		storedConfig.distanceAlarm=600;
		storedConfig.altitudeAlarm = 50;
		storedConfig.lowSpeedAlarm = 20;

		storedConfig.BaudRate=4;
		storedConfig.ControlProportional=MODO_SW3;

		// Dummy, but cool XDD
		storedConfig.Video_PAL=1;
		storedConfig.offsetX =44;
		storedConfig.offsetY = 22;
		storedConfig.line_telemetry= 24;
		storedConfig.min_rssi = 0.8f;
		storedConfig.max_rssi = 2.2f;
	}
}

void SaveAutopilotConfig()
{
	char strFileName[MAX_PATH];
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\AutopilotConfig.bin");
	FILE *pFile = fopen(strFileName, "wb");

	if(pFile)
	{
		fwrite(&autopilotCfg,sizeof(struct AutoPilotConfig),1,pFile);
		fclose(pFile);
	}
}

void LoadAutopilotConfig()
{
	int readOK=0;
	char strFileName[MAX_PATH];
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\AutopilotConfig.bin");
	FILE *pFile = fopen(strFileName, "rb");

	if(pFile)
	{
		readOK=fread(&autopilotCfg,sizeof(struct AutoPilotConfig),1,pFile);
		fclose(pFile);
	}

	if(!readOK)
	{
		memset(&autopilotCfg,0, sizeof(struct AutoPilotConfig));
		for(int i=0;i<5;i++)
		{
			autopilotCfg.servos_cfg[i].max=2000;
			autopilotCfg.servos_cfg[i].min=1000;
			autopilotCfg.servos_cfg[i].center=1500;
		}

		autopilotCfg.tipo_mezcla = MEZCLA_INH;

		autopilotCfg.pid_pitch.P=0.02f;
		autopilotCfg.pid_pitch.DL = 1.0f;
		autopilotCfg.pid_pitch.rev = 1;

		autopilotCfg.pid_roll.P=0.02f;
		autopilotCfg.pid_roll.DL = 1.0f;
		autopilotCfg.pid_roll.rev = 0;

		autopilotCfg.pid_tail.P=0.02f;
		autopilotCfg.pid_tail.DL = 1.0f;
		autopilotCfg.pid_tail.rev = 1;

		autopilotCfg.rumbo_ail = 20;

		autopilotCfg.pid_motor.P=0.02f;
		autopilotCfg.pid_motor.DL = 1.0f;
		autopilotCfg.pid_motor.rev = 0;

		autopilotCfg.altura_ele = 10;
		autopilotCfg.AutopilotMode = AP_FIX_ALT;

		autopilotCfg.baseCruiseAltitude = 100;
	}
}