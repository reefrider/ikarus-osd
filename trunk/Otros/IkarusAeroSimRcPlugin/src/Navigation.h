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
#include "Portable.h"

#define MAX_WPTS	31

struct WayPoint
{
	char Name[20];
	float lon;
	float lat;
	float alt;
};

struct Ruta
{
	int numwpt;
	int attribs;
	struct WayPoint wpts[MAX_WPTS];
};

float calcDistance(float lon1, float lat1, float lon2, float lat2) large;
float calcBearing(float lon1,float lat1, float lon2, float lat2) large;

void init_navigator();
void UpdateNavigator()large;

unsigned char GetIndex();
float GetDistDst();
float GetRumboDst();
char *GetNameDst();
float GetLonDst() large;
float GetLatDst() large;
float GetAltDst() large;

void ChangeNextIfReached();
void SetToHome();
void SetDoRoute();


