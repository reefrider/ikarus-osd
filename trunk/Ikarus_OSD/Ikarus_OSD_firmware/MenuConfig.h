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
 
void AutopilotAutoconfig(char interactive) large;
void IkarusOsdAutoconfig(char interactive) large;
void IkarusOsdconfig() large;
void FixHome() large;
void SaveBauds(char bauds) large;

void WaitForGPSscreen(char interactive) large;

void MenuConfig(struct StoredConfig *cfg/*, struct AutoPilotConfig *apcfg*/) large;
void MenuReflash() large;
void MenuRX_config(struct StoredConfig *cfg) large;
void MenuMAX7456_config(struct StoredConfig *cfg) large;
void MenuBattery_config(struct StoredConfig *cfg) large;
void MenuAlarms_config(struct StoredConfig *cfg) large;
void MenuIkarus_config(struct StoredConfig *cfg) large;
void MenuAutopilot_config(struct AutoPilotConfig *apcfg) large;
//void MenuAutopilotGains_config(struct PID_Setup *pidsetup, char title[]) large;
//void MenuServos_config(struct ServoInfo *servos_cfg) large;
void MenuPPM_config(struct AutoPilotConfig *apcfg) large;
void MenuIR_config(struct AutoPilotConfig *apcfg) large;


int Selection(int fila, int col, char curr, char max, char cad[][15]) large;
float NumericUpDown(int fila, int col, float curr, float min, float max, float inc, char delay, char *fmt) large;
//float NumericMulDiv(int fila, int col, float curr, float min, float max, float inc, char delay, char *fmt) large;

void TestHardware();
void CalibrarServos() large;
void CalibrarCopilot() large;

void HUD_Debug(int fila, int columna, unsigned char param) large;
