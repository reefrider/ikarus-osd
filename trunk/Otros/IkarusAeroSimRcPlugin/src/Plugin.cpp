//-----------------------------------------------------------------------------
//  PluginDLL.cpp
//-----------------------------------------------------------------------------
//
//  Plugin example for AeroSIM RC
//
//  This plugin example ilustrates:
//    - how to define a custom menu
//    - how to change model position (useful to repeat tests from same position, velocity, attitude, etc.)
//    - a simple aileron  autopilot that takes the roll  angle and move the ailerons to keep roll  at 0º
//    - a simple elevator autopilot that takes the pitch angle and move the elevator to keep pitch at 0º
//    - a OSD with a simplified MAX7456 emulation
//
//  Please contact us at info@aerosimrc.com if you need support
//
//-----------------------------------------------------------------------------
#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
#include <windows.h>
#include <stdio.h>
#include <direct.h>

#include "Ikarus.h"
#include "huds.h"

#include "Plugin.h"
#include "MAX7456.h"

//-----------------------------------
//-----------------------------------
bool bFirstRun = true;          // Used to initialise menu checkboxes showing OSD with HUD
char strDebugInfo[10000] = "";  // static to remain in memory when function exits

char strDefaultPATH[MAX_PATH];
char strOutputFolder[MAX_PATH];

extern bool go_home;
extern struct Screen huds[5];
extern struct Screen hud_text;
extern struct IkarusInfo ikarusInfo;

//-----------------------------------------------------------------------------
// 
//-----------------------------------------------------------------------------
BOOL APIENTRY DllMain( HANDLE hModule, 
					  DWORD  ul_reason_for_call, 
					  LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		// Free here any resources used by the plugin
		break;
	}
	return TRUE;
}



//-----------------------------------------------------------------------------
//
// This function is called once, just after the dll is loaded
//  - to initialise the top bar menu with your custom menu items
//  - to initialise any other resources (e.g. character map for OSD)
//
//-----------------------------------------------------------------------------
AeroSIMRC_DLL_EXPORT void AeroSIMRC_Plugin_Init(TPluginInit *ptPluginInit)
{
	//--------------------------------------------
	// Initialise character map for OSD
	//--------------------------------------------
	char strFullFileName[MAX_PATH];
	strcpy(strOutputFolder, ptPluginInit->strOutputFolder);
	strcpy(strDefaultPATH, ptPluginInit->strPluginFolder);
	strcpy(strFullFileName, ptPluginInit->strPluginFolder);
	strcat(strFullFileName, "\\Ikarus.MCM");
	MAX7456_LoadCharacterMemory(strFullFileName); // Load character set for OSD

	//--------------------------------------------
	// Init custom Menu
	//--------------------------------------------
	ptPluginInit->strMenuTitle = "Ikarus OSD";
	
#define MASK_MENU_ITEM__COMMAND_RESET             (1 << 0)
	ptPluginInit->atMenuItem[0].eType   = MIT_COMMAND;
	ptPluginInit->atMenuItem[0].strName = "Reset";

#define MASK_MENU_ITEM__COMMAND_RESET_CONFIG      (1 << 1)
	ptPluginInit->atMenuItem[1].eType   = MIT_COMMAND;
	ptPluginInit->atMenuItem[1].strName = "Reset Config";

#define MASK_MENU_ITEM__CHECKBOX_OSD_TEXTS        (1 << 2)
	ptPluginInit->atMenuItem[2].eType   = MIT_CHECKBOX| MIT_SEPARATOR;
	ptPluginInit->atMenuItem[2].strName = "Enable Ikarus";

#define MASK_MENU_ITEM__CHECKBOX_OVERRIDE          (1 << 3)
	ptPluginInit->atMenuItem[3].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[3].strName = "Override Ctrl";

#define MASK_MENU_ITEM__CHECKBOX_GO_HOME          (1 << 4)
	ptPluginInit->atMenuItem[4].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[4].strName = "GO HOME";

#define MASK_MENU_ITEM__CHECKBOX_AUTOPILOT          (1 << 5)
	ptPluginInit->atMenuItem[5].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[5].strName = "Enable AutoPilot";

#define MASK_MENU_ITEM__CHECKBOX_HUD0          (1 << 6)
	ptPluginInit->atMenuItem[6].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[6].strName = "HUD 0";
#define MASK_MENU_ITEM__CHECKBOX_HUD1          (1 << 7)
	ptPluginInit->atMenuItem[7].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[7].strName = "HUD 1";
#define MASK_MENU_ITEM__CHECKBOX_HUD2          (1 << 8)
	ptPluginInit->atMenuItem[8].eType   = MIT_CHECKBOX;
	ptPluginInit->atMenuItem[8].strName = "HUD 2";

	for(int i=0;i<5;i++)
	{
		FILE * handle;
		char strFileName[MAX_PATH];

		sprintf(strFileName,"%s\\HUD%d.bin",strDefaultPATH,i);
		handle=fopen(strFileName,"rb");
		if(handle)
		{
			fread(&huds[i],sizeof(char),sizeof(struct Screen),handle);
			fclose(handle);
		}
		else
		{
			huds[i]=hud_text;
			switch(i)
			{
				case 0:
				case 1:
				case 2:
					sprintf(huds[i].strNombreHUD,"Pantalla %d",i);
					break;
				case 3:
					sprintf(huds[i].strNombreHUD,"FailSafe",i);
					break;
				case 4:
					sprintf(huds[i].strNombreHUD,"Resumen",i);
					break;
			}
		}
	}
}


//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
void Run_Command_Reset(const TDataFromAeroSimRC  *ptDataFromAeroSimRC, TDataToAeroSimRC *ptDataToAeroSimRC)
{
	char strFileName[MAX_PATH];
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\StoredConfig.bin");
	remove(strFileName);
	strcpy(strFileName, strOutputFolder);
	strcat(strFileName, "\\AutopilotConfig.bin");
	remove(strFileName);
	
	InitIkarus(ptDataFromAeroSimRC);
}



//-----------------------------------------------------------------------------
// OSD
//-----------------------------------------------------------------------------
bool bMenuOverride;
void Run_OSD(const TDataFromAeroSimRC  *ptDataFromAeroSimRC, TDataToAeroSimRC *ptDataToAeroSimRC)
{
	static bool bOldCheckBox_HUD0, bOldCheckBox_HUD1, bOldCheckBox_HUD2;

	bool bMenuReset  = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__COMMAND_RESET) != 0;
	bool bMenuResetConfig  = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__COMMAND_RESET_CONFIG) != 0;

	bMenuOverride  = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_OVERRIDE) != 0;
	
	bool bMenuGoHome  = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_GO_HOME) != 0;
	bool bMenuAutoPilot  = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_AUTOPILOT) != 0;

	bool bCheckBox_OSD_Texts        = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_OSD_TEXTS       ) != 0;

	bool bCheckBox_HUD0        = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_HUD0) != 0;
	bool bCheckBox_HUD1        = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_HUD1) != 0;
	bool bCheckBox_HUD2        = (ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status & MASK_MENU_ITEM__CHECKBOX_HUD2) != 0;

	if(bMenuReset)
		InitIkarus(ptDataFromAeroSimRC);

	if(bMenuResetConfig)
		Run_Command_Reset(ptDataFromAeroSimRC,ptDataToAeroSimRC);

	OSD_Clear(ptDataFromAeroSimRC->OSD_pVideoBuffer);
	ptDataToAeroSimRC->OSD_nWindow_DX  = MAX7456_OSD_PIXEL_WIDTH;
	ptDataToAeroSimRC->OSD_nWindow_DY  = MAX7456_OSD_PIXEL_HEIGHT;
	ptDataToAeroSimRC->OSD_fScale      = 0.9f;  // leaves a small gap between the OSD and the edges of the window
	ptDataToAeroSimRC->OSD_bHasChanged = true;

	if(bCheckBox_OSD_Texts)
	{
		ptDataToAeroSimRC->OSD_bShow = true;
	}

	//---------------------------------------------------
	// Run CheckBox OSD Texts
	//---------------------------------------------------
	if(bCheckBox_OSD_Texts)
	{
		// Update data displayed every 0.4 seconds
		#define OSD_UPDATE_PERIOD 0.4f
		static float fTimeSinceLastOSDUpdate = 0.0f;

		bool bReprintDisplay = false;

		fTimeSinceLastOSDUpdate += ptDataFromAeroSimRC->Simulation_fIntegrationTimeStep;
		if(fTimeSinceLastOSDUpdate > OSD_UPDATE_PERIOD)
		{
			fTimeSinceLastOSDUpdate = 0.0f;
			bReprintDisplay = true;
		}

		if(bReprintDisplay||true)
		{
			//go_home = bMenuGoHome;
			//ikarusInfo.AutoPilot_Enabled = bMenuAutoPilot;			
			if(bMenuOverride)
			{
				ikarusInfo.ctrl_autopilot = (ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status & MASK_MENU_ITEM__CHECKBOX_AUTOPILOT)!=0;
				ikarusInfo.ctrl_doruta = !((ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status & MASK_MENU_ITEM__CHECKBOX_GO_HOME)!=0);
		
				if(bCheckBox_HUD0!=bOldCheckBox_HUD0)
				{
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~(MASK_MENU_ITEM__CHECKBOX_HUD0|MASK_MENU_ITEM__CHECKBOX_HUD1|MASK_MENU_ITEM__CHECKBOX_HUD2);
					if(bCheckBox_HUD0)
					{
						ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD0;
						ikarusInfo.ctrl_hudnum = 1;
						bCheckBox_HUD1=bCheckBox_HUD2=0;
					}
					else
					{
						ikarusInfo.ctrl_hudnum = 0;
					}
				}
				else if(bCheckBox_HUD1!=bOldCheckBox_HUD1)
				{
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~(MASK_MENU_ITEM__CHECKBOX_HUD0|MASK_MENU_ITEM__CHECKBOX_HUD1|MASK_MENU_ITEM__CHECKBOX_HUD2);
					if(bCheckBox_HUD1)
					{
						ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD1;
						ikarusInfo.ctrl_hudnum = 2;
						bCheckBox_HUD0=bCheckBox_HUD2=0;
					}
					else
					{
						ikarusInfo.ctrl_hudnum = 0;
					}
				}
				else if(bCheckBox_HUD2!=bOldCheckBox_HUD2)
				{
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~(MASK_MENU_ITEM__CHECKBOX_HUD0|MASK_MENU_ITEM__CHECKBOX_HUD1|MASK_MENU_ITEM__CHECKBOX_HUD2);
					if(bCheckBox_HUD2)
					{
						ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD2;
						ikarusInfo.ctrl_hudnum = 3;
						bCheckBox_HUD0=bCheckBox_HUD1=0;
					}
					else
					{
						ikarusInfo.ctrl_hudnum = 0;
					}
				}
				bOldCheckBox_HUD0=bCheckBox_HUD0;
				bOldCheckBox_HUD1=bCheckBox_HUD1;
				bOldCheckBox_HUD2=bCheckBox_HUD2;
				
			}

			Ikarus(ptDataFromAeroSimRC, ptDataToAeroSimRC);
			
			if(!bMenuOverride)
			{
				if(!ikarusInfo.ctrl_doruta)
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_GO_HOME;
				else
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~MASK_MENU_ITEM__CHECKBOX_GO_HOME;

				if(ikarusInfo.AutoPilot_Enabled)
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_AUTOPILOT;
				else
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~MASK_MENU_ITEM__CHECKBOX_AUTOPILOT;

				ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status &= ~(MASK_MENU_ITEM__CHECKBOX_HUD0|MASK_MENU_ITEM__CHECKBOX_HUD1|MASK_MENU_ITEM__CHECKBOX_HUD2);

				if(ikarusInfo.ctrl_hudnum==1)
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD0;
				else if(ikarusInfo.ctrl_hudnum==2)
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD1;
				else if(ikarusInfo.ctrl_hudnum==3)
					ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status |= MASK_MENU_ITEM__CHECKBOX_HUD2;
			}

			MAX7456_Render(ptDataFromAeroSimRC->OSD_pVideoBuffer, ptDataFromAeroSimRC->Simulation_fIntegrationTimeStep);
		}
	}
}



//-----------------------------------------------------------------------------
// The string returned in pucOnScreenDebugInfoText will be displayed in AeroSIMRC
// In this example, the all data received from AeroSIMRC is displayed on the screen
//-----------------------------------------------------------------------------
void InfoText(const TDataFromAeroSimRC *ptDataFromAeroSimRC, TDataToAeroSimRC *ptDataToAeroSimRC)
{
	int rafa=sizeof(struct Screen); //148
  sprintf(strDebugInfo,
    "----------------------------------------------------------------------\n"
    "nStructSize = %d\n"
    "\n" // Simulation Data
    "fIntegrationTimeStep = %f\n"
	"Rafa = %d\n"
    "\n" // Model data
    "fPosX,Y,Z    = (% 8.2f, % 8.2f, % 8.2f)\n"
    "fVelX,Y,Z    = (% 8.2f, % 8.2f, % 8.2f)\n"
    "fAngVelX,Y,Z = (% 8.5f, % 8.5f, % 8.5f)\n"
    "fAccelX,Y,Z  = (% 8.5f, % 8.5f, % 8.5f)\n"
    "\n"
    "Lat, Long   = % 10.5f, % 10.5f\n"
    "\n"
    "fHeightAboveTerrain = %f\n"
    "\n"
    "fHeading = % 6.4f   fPitch = % 6.4f   fRoll = % 6.4f\n"
    "\n"
    "TX Aileron  = % 4.2f\n"
    "TX Elevator = % 4.2f\n"
    "TX Throttle = % 4.2f\n"
    "TX Rudder   = % 4.2f\n"
	"TX CH 5   = % 4.2f\n"
	"TX CH 6   = % 4.2f\n"
	"TX CH 7   = % 4.2f\n"

    "\n"
    "RX Aileron  = % 4.2f\n"
    "RX Elevator = % 4.2f\n"
    "RX Throttle = % 4.2f\n"
    "RX Rudder   = % 4.2f\n"
    "RX CH 5   = % 4.2f\n"
    "RX CH 6   = % 4.2f\n"
    "RX CH 7   = % 4.2f\n"

    "\n"
    "MenuItems = %X\n"
    "\n"
    "Model Initial Pos (%f, %f, %f)\n"
    "Model Initial HPR (%f, %f, %f)\n"
    "\n"
    "WP Home X,Y = (% 8.2f, % 8.2f)  Lat, Long = (% 10.5f, % 10.5f)  Description %s\n"
    "WP A    X,Y = (% 8.2f, % 8.2f)  Lat, Long = (% 10.5f, % 10.5f)  Description %s\n"
    "WP B    X,Y = (% 8.2f, % 8.2f)  Lat, Long = (% 10.5f, % 10.5f)  Description %s\n"
    "WP C    X,Y = (% 8.2f, % 8.2f)  Lat, Long = (% 10.5f, % 10.5f)  Description %s\n"
    "WP D    X,Y = (% 8.2f, % 8.2f)  Lat, Long = (% 10.5f, % 10.5f)  Description %s\n"
    "\n"
    "Engine1 = % 5.0f RPM   Engine2 = % 5.0f RPM   Engine3 = % 5.0f RPM   Engine4 = % 5.0f RPM\n"
    "\n"
    "Wind = (% 8.2f, % 8.2f, % 8.2f)\n"
    "\n"
    "Battery % 2.1f V  % 2.1f A   Consumed = % 2.3f Ah   Capacity = % 2.3f Ah\n"
    "Fuel Consumed = % 2.3f l    Tank Capacity = % 2.3f l\n"
    "----------------------------------------------------------------------\n"
    ,
    ptDataFromAeroSimRC->nStructSize,
    
    // Simulation Data
    ptDataFromAeroSimRC->Simulation_fIntegrationTimeStep,
	rafa,
    // Model data
    ptDataFromAeroSimRC->Model_fPosX,     ptDataFromAeroSimRC->Model_fPosY,     ptDataFromAeroSimRC->Model_fPosZ,
    ptDataFromAeroSimRC->Model_fVelX,     ptDataFromAeroSimRC->Model_fVelY,     ptDataFromAeroSimRC->Model_fVelZ,
    ptDataFromAeroSimRC->Model_fAngVelX,  ptDataFromAeroSimRC->Model_fAngVelY,  ptDataFromAeroSimRC->Model_fAngVelZ,
    ptDataFromAeroSimRC->Model_fAccelX,   ptDataFromAeroSimRC->Model_fAccelY,   ptDataFromAeroSimRC->Model_fAccelZ,

    ptDataFromAeroSimRC->Model_fLatitude, ptDataFromAeroSimRC->Model_fLongitude,
    
    ptDataFromAeroSimRC->Model_fHeightAboveTerrain,

    ptDataFromAeroSimRC->Model_fHeading, ptDataFromAeroSimRC->Model_fPitch, ptDataFromAeroSimRC->Model_fRoll,

    ptDataFromAeroSimRC->Channel_afValue_TX[CH_AILERON],
    ptDataFromAeroSimRC->Channel_afValue_TX[CH_ELEVATOR],
    ptDataFromAeroSimRC->Channel_afValue_TX[CH_THROTTLE],
    ptDataFromAeroSimRC->Channel_afValue_TX[CH_RUDDER],
	ptDataFromAeroSimRC->Channel_afValue_TX[CH_5],
	ptDataFromAeroSimRC->Channel_afValue_TX[CH_6],
	ptDataFromAeroSimRC->Channel_afValue_TX[CH_7],


    ptDataFromAeroSimRC->Channel_afValue_RX[CH_AILERON],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_ELEVATOR],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_THROTTLE],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_RUDDER],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_5],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_6],
    ptDataFromAeroSimRC->Channel_afValue_RX[CH_7],

    ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status,

    ptDataFromAeroSimRC->Scenario_fInitialModelPosX,    ptDataFromAeroSimRC->Scenario_fInitialModelPosY,  ptDataFromAeroSimRC->Scenario_fInitialModelPosZ,
    ptDataFromAeroSimRC->Scenario_fInitialModelHeading, ptDataFromAeroSimRC->Scenario_fInitialModelPitch, ptDataFromAeroSimRC->Scenario_fInitialModelRoll,

    ptDataFromAeroSimRC->Scenario_fWPHome_X, ptDataFromAeroSimRC->Scenario_fWPHome_Y,  ptDataFromAeroSimRC->Scenario_fWPHome_Lat, ptDataFromAeroSimRC->Scenario_fWPHome_Long, ptDataFromAeroSimRC->Scenario_strWPHome_Description,
    ptDataFromAeroSimRC->Scenario_fWPA_X,    ptDataFromAeroSimRC->Scenario_fWPA_Y,     ptDataFromAeroSimRC->Scenario_fWPA_Lat,    ptDataFromAeroSimRC->Scenario_fWPA_Long,    ptDataFromAeroSimRC->Scenario_strWPA_Description,
    ptDataFromAeroSimRC->Scenario_fWPB_X,    ptDataFromAeroSimRC->Scenario_fWPB_Y,     ptDataFromAeroSimRC->Scenario_fWPB_Lat,    ptDataFromAeroSimRC->Scenario_fWPB_Long,    ptDataFromAeroSimRC->Scenario_strWPB_Description,
    ptDataFromAeroSimRC->Scenario_fWPC_X,    ptDataFromAeroSimRC->Scenario_fWPC_Y,     ptDataFromAeroSimRC->Scenario_fWPC_Lat,    ptDataFromAeroSimRC->Scenario_fWPC_Long,    ptDataFromAeroSimRC->Scenario_strWPC_Description,
    ptDataFromAeroSimRC->Scenario_fWPD_X,    ptDataFromAeroSimRC->Scenario_fWPD_Y,     ptDataFromAeroSimRC->Scenario_fWPD_Lat,    ptDataFromAeroSimRC->Scenario_fWPD_Long,    ptDataFromAeroSimRC->Scenario_strWPD_Description,

    ptDataFromAeroSimRC->Model_fEngine1_RPM, ptDataFromAeroSimRC->Model_fEngine2_RPM, ptDataFromAeroSimRC->Model_fEngine3_RPM, ptDataFromAeroSimRC->Model_fEngine4_RPM,

    ptDataFromAeroSimRC->Model_fWindVelX, ptDataFromAeroSimRC->Model_fWindVelY, ptDataFromAeroSimRC->Model_fWindVelZ,

    ptDataFromAeroSimRC->Model_fBatteryVoltage, ptDataFromAeroSimRC->Model_fBatteryCurrent, ptDataFromAeroSimRC->Model_fBatteryConsumedCharge, ptDataFromAeroSimRC->Model_fBatteryCapacity,

    ptDataFromAeroSimRC->Model_fFuelConsumed, ptDataFromAeroSimRC->Model_fFuelTankCapacity
  );

}


//-----------------------------------------------------------------------------
// This function is called at each program cycle from AeroSIM RC
//-----------------------------------------------------------------------------
AeroSIMRC_DLL_EXPORT void AeroSIMRC_Plugin_Run(const TDataFromAeroSimRC  *ptDataFromAeroSimRC,
											   TDataToAeroSimRC    *ptDataToAeroSimRC)
{
	//-----------------------------------
	// debug info is shown on the screen
	//-----------------------------------
	ptDataToAeroSimRC->Debug_pucOnScreenInfoText = strDebugInfo;

	InfoText(ptDataFromAeroSimRC, ptDataToAeroSimRC);

	//-----------------------------------
	// By default do not change the Menu Items of type CheckBox
	//-----------------------------------
	ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status = ptDataFromAeroSimRC->Menu_nFlags_MenuItem_Status;

	//-----------------------------------
	// Tick checkboxes to show OSD with HUD
	//-----------------------------------
	if(bFirstRun)
	{
		InitIkarus(ptDataFromAeroSimRC);
		ptDataToAeroSimRC->Menu_nFlags_MenuItem_New_CheckBox_Status = MASK_MENU_ITEM__CHECKBOX_GO_HOME | MASK_MENU_ITEM__CHECKBOX_OSD_TEXTS;
	}

	//---------------------------------------------------
	// OSD
	//---------------------------------------------------
	Run_OSD(ptDataFromAeroSimRC, ptDataToAeroSimRC);

	//-----------------------------------
	//-----------------------------------
	bFirstRun = false;
}





