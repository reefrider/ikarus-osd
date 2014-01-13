/*
 *		http://www.movable-type.co.uk/scripts/latlong.html
 *
 *
 *
 */

#include <math.h>

#define PI 3.14159265

#define EARTH_METROS 6378137.0 		// Radio en metros
#define EARTH_MILLAS 3443.9184		// Radio en millas

#define R 				EARTH_METROS

#define DEG2RAD(a)	((a)*PI/180.0)
#define RAD2DEG(a)	((a)*180.0/PI)

float calcDistance(float lon1, float lat1, float lon2, float lat2) large
{
	float sindlat=sin(DEG2RAD(lat1-lat2)/2.0);
	float sindlon=sin(DEG2RAD(lon1-lon2)/2.0);
	float d=2.0*asin(sqrt(sindlat*sindlat + sindlon*sindlon* cos(DEG2RAD(lat1))*cos(DEG2RAD(lat2))));
	return d*EARTH_METROS;
}

float calcBearing(float lon1,float lat1, float lon2, float lat2) large
{
	float dlon_grad=DEG2RAD(lon2-lon1);
	float lat1_grad=DEG2RAD(lat1);
	float lat2_grad=DEG2RAD(lat2);
	float y = sin(dlon_grad) * cos(lat2_grad);
	float x = cos(lat1_grad)*sin(lat2_grad) - sin(lat1_grad)*cos(lat2_grad)*cos(dlon_grad);
	if(y==0&&x==0)
		return 0;
	else
		return RAD2DEG(atan2(y,x));
}

/*
float calcBearing(float lon1,float lat1, float lon2, float lat2) large
{
	float y = sin(DEG2RAD(lon2-lon1)) * cos(DEG2RAD(lat2));
	float x = cos(DEG2RAD(lat1))*sin(DEG2RAD(lat2)) - sin(DEG2RAD(lat1))*cos(DEG2RAD(lat2))*cos(DEG2RAD(lon2-lon1));
	if(y==0&&x==0)
		return 0;
	else
		return RAD2DEG(atan2(y,x));
}
*/

/*
 * Midpoint
 *
 *	This is the half-way point along a great circle path between the two points.
 */

void MidPoint(float lon1, float lat1, float lon2, float lat2, float *plon3, float *plat3)
{
	float dLon_grad=DEG2RAD(lon2-lon1);
	float lat1_grad=DEG2RAD(lat1);
	float lat2_grad=DEG2RAD(lat2);

	float Bx = cos(lat2_grad) * cos(dLon_grad);
	float By = cos(lat2_grad) * sin(dLon_grad);
	float lat3 = atan2(sin(lat1_grad)+sin(lat2_grad), sqrt((cos(lat1_grad)+Bx)*(cos(lat1_grad)+Bx) + By*By)); 
	float lon3 = lon1 + atan2(By, cos(lat1_grad) + Bx);
	
	*plon3 =  RAD2DEG(lon3);
	*plat3 = RAD2DEG(lat3);
}
/*
 * Cross-track distance
 *
 * Distance of a point from a great-circle path (sometimes called cross track error).
 *
 * Formula: 	dxt = asin(sin(d13/R)*sin(brng13-brng12)) * R
 * 	where 	d13 is distance from start point to third point
 *			brng13 is (initial) bearing from start point to third point
 *			brng12 is (initial) bearing from start point to end point
 * 			R is the earth’s radius
 */
void DestPoint(float lon1, float lat1, float brng, float d, float *plon2, float *plat2)
{
	float lat2 = asin(sin(lat1)*cos(d/R) + cos(lat1)*sin(d/R)*cos(brng));
	float lon2 = lon1 + atan2(sin(brng)*sin(d/R)*cos(lat1), cos(d/R)-sin(lat1)*sin(lat2));


	*plon2 =  RAD2DEG(lon2);
	*plat2 = RAD2DEG(lat2);
}

/*
 * Along-track distance
 *
 *	
 */
float CrossTrackDist(float d13, float brng13, float brng12)
{

	float dXt = asin(sin(d13/R)*sin(brng13-brng12)) * R;
	return dXt;
}

float AlongTrackDist(float d13, float brng13, float brng12)
{
	float dXt = CrossTrackDist(d13, brng13, brng12);
	float dAt = acos(cos(d13/R)/cos(dXt/R)) * R;
	return dAt;
}