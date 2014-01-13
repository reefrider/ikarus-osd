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

struct Telemetry_PKT
{
	unsigned char len;
	float lon;
	float lat;
	int alt;	
	unsigned char rumbo; 
	unsigned char knots;
	char vertSpeed;
	unsigned char wptID;
	float homeLon;
	float homeLat;
	unsigned char v1;
	unsigned char v2;
	char pitch;
	char roll;
	unsigned char RSSI;
	unsigned char CRC;
};

void InitXBEE_tx() large;
void BuildTelemetryPKT();
void ComputeTelemetryCRC(char *ppkt);
