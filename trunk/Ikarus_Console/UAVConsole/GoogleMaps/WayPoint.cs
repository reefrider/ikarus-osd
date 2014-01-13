/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of FPVOSD.
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
 *  along with FPVOSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    public class WayPoint
    {

        public String name;
        public double Longitude, Latitude;
        public float Altitude;
        public Bitmap icon = null;
        public Color color = Color.Red;
        public float heading = 0.0f;
        const float EARTH = 6378.137f;

        /** Creates a new instance of WayPoint */

        public WayPoint()
        {
            name = "NoName";
        }

        public WayPoint(String s)
        {
            name = s;
        }

        public WayPoint(String s, double lon, double lat)
        {
            name = s;
            this.Longitude = lon;
            this.Latitude = lat;
        }

        public WayPoint(String s, double lon, double lat, float alt)
        {
            name = s;
            this.Longitude = lon;
            this.Latitude = lat;
            this.Altitude = alt;
        }

        public WayPoint(String s, double lon, double lat, Bitmap icon)
        {
            name = s;
            this.Longitude = lon;
            this.Latitude = lat;            
            this.icon = icon;
        }

        public WayPoint(WayPoint wpt)
        {
            name = wpt.name;
            this.Longitude = wpt.Longitude;
            this.Latitude = wpt.Latitude;
            this.Altitude = wpt.Altitude;
            this.icon = wpt.icon;
        }
        public static WayPoint Buscar(String busqueda)
        {
            return null;
        }

        override public bool Equals(Object obj)
        {
            WayPoint tmp = (WayPoint)obj;
            return tmp.name.Equals(this.name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public static double toRadians(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public static double toDegrees(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        public float getDistance(WayPoint b)
        {
            return WayPoint.getDistance(this, b);
        }
        public static WayPoint Desplazar(WayPoint a, float azimut, float distance)
        {
            //distance = distance / 1.852f;
            double dlat = distance * Math.Cos(toRadians(azimut));
            //double lat = dlat / 60 + a.Latitude;
            double lat = dlat / toRadians(EARTH) + a.Latitude;
            double latm = (lat + a.Latitude) / 2;
            double apart = distance * Math.Sin(toRadians(azimut));
            double dlon = apart / Math.Cos(toRadians(latm));
            //double lon = dlon / 60 + a.Longitude;
            double lon = dlon / toRadians(EARTH) + a.Longitude;
            return new WayPoint(a.name + "2", lon, lat);
        }

        public WayPoint Desplazar(float azimut, float distance)
        {
            return Desplazar(this, azimut, distance);
        }

        public static float getDistance(WayPoint a, WayPoint b)
        {
            double lon1 = a.Longitude;
            double lat1 = a.Latitude;
            double lon2 = b.Longitude;
            double lat2 = b.Latitude;
            
            double sindlat = Math.Sin(toRadians(lat1 - lat2) / 2.0);
            double sindlon = Math.Sin(toRadians(lon1 - lon2) / 2.0);
            double d = 2.0 * Math.Asin(Math.Sqrt(sindlat * sindlat + sindlon * sindlon * Math.Cos(toRadians(lat1)) * Math.Cos(toRadians(lat2))));
            return (float)d * EARTH;
        }

        public float calcBearing(WayPoint b)
        {
            return calcBearing(this, b);
        }

        public static float calcBearing(WayPoint a, WayPoint b)
        {
                double lon1 = a.Longitude;
                double lat1 = a.Latitude;
                double lon2 = b.Longitude;
                double lat2 = b.Latitude;
                double y = Math.Sin(toRadians(lon2-lon1)) * Math.Cos(toRadians(lat2));
                double x = Math.Cos(toRadians(lat1)) * Math.Sin(toRadians(lat2)) - Math.Sin(toRadians(lat1)) * Math.Cos(toRadians(lat2)) * Math.Cos(toRadians(lon2 - lon1));
	            if(y==0&&x==0)
		            return 00.0f;
	            else
		            return (float)toDegrees(Math.Atan2(y,x));
        }

        /* 
        public double distance(WayPoint t)
        {
            // Spherical law of cosines: d = acos(sin(lat1).sin(lat2)+cos(lat1).cos(lat2).cos(long2?long1)).R

            double lat1 = toRadians(this.Latitude);
            double lat2 = toRadians(t.Latitude);
            double lon1 = toRadians(this.Longitude);
            double lon2 = toRadians(t.Longitude);

            double d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Cos(lon2 - lon1)) * WayPoint.radius;
            return d;
        }

        public double distHaversine(WayPoint t)
        {
            double lat1 = toRadians(this.Latitude);
            double lat2 = toRadians(t.Latitude);
            double dLat = lat2 - lat1;
            double dLon = toRadians(t.Longitude - this.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = WayPoint.radius * c;
            return d;
        }

        public double bearing(WayPoint t)
        {
            double lat1 = toRadians(this.Latitude);
            double lat2 = toRadians(t.Latitude);
            double dLon = toRadians(t.Longitude - this.Longitude);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x);
            double n = toDegrees(brng);

            return n;
        }

        public double loxo_distance(WayPoint t)
        {
            double lat1 = toRadians(this.Latitude);
            double lat2 = toRadians(t.Latitude);
            double dLat = lat2 - lat1;
            double dLon = toRadians(t.Longitude - this.Longitude);

            double dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            double q;
            if (dPhi == 0)
                q = Math.Cos(lat1);
            else
                q = dLat / dPhi;

            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (Math.Abs(dLon) > Math.PI)
                if (dLon > 0)
                    dLon = -(2 * Math.PI - dLon);
                else
                    dLon = (2 * Math.PI + dLon);
            double d = Math.Sqrt(dLat * dLat + q * q * dLon * dLon) * WayPoint.radius;
            return d;
        }

        public double loxo_bearing(WayPoint t)
        {
            double lat1 = toRadians(this.Latitude);
            double lat2 = toRadians(t.Latitude);
            double dLon = toRadians(t.Longitude - this.Longitude);

            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (Math.Abs(dLon) > Math.PI)
                if (dLon > 0)
                    dLon = -(2 * Math.PI - dLon);
                else
                    dLon = (2 * Math.PI + dLon);

            double dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));

            double brng = toDegrees(Math.Atan2(dLon, dPhi));
            return brng;
        }
         */
    }
}
