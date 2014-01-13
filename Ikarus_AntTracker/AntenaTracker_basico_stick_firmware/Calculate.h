/* 
 * (c) 2011 Rafael Paz <rpaz@atc.us.es>
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

#define PI 3.14159265

#define EARTH_METROS 6378137.0 		// Radio en metros
#define EARTH_MILLAS 3443.9184		// Radio en millas

#define DEG2RAD(a)	((a)*PI/180.0)
#define RAD2DEG(a)	((a)*180.0/PI)

 void CalculateHeading () large;
 void CalculaPanTilt() large;
 void Copy2DatosAntena() large;

 float calcDistance(float lon1, float lat1, float lon2, float lat2) large;
 float calcBearing(float lon1,float lat1, float lon2, float lat2) large;

