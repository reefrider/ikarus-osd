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

namespace UAVConsole.GoogleMaps
{
    public class GeoPos
    {
        private int Zoom;
        private double Longitude, Latitude;

        /** Creates a new instance of GeoPosString */
        public GeoPos()
        {
            Longitude = -6.000868;
            Latitude = 37.395591;
            Zoom = 2;
        }

        public GeoPos(GeoPos geo)
        {
            Zoom = geo.Zoom;
            Longitude = geo.Longitude;
            Latitude = geo.Latitude;
        }

        public GeoPos(WayPoint wpt, int zoom)
        {
            this.Zoom = zoom;
            this.Longitude = wpt.Longitude;
            this.Latitude = wpt.Latitude;
        }

        public double getLatitude()
        {
            return Latitude;
        }

        public void setLatitude(double lat)
        {
            if (lat >= -90.0 && lat <= 90.0)
                Latitude = lat;
        }
        public double getLongitude()
        {
            return Longitude;
        }

        public void setLongitude(double lon)
        {
            if (lon >= -180.0 && lon <= 180.0)
                Longitude = lon;
        }

        public WayPoint getWpt()
        {
            WayPoint wpt = new WayPoint();
            wpt.Longitude = this.getLongitude();
            wpt.Latitude = this.getLatitude();
            return wpt;
        }

        public void setWpt(WayPoint wpt)
        {
            setLatitude(wpt.Latitude);
            setLongitude(wpt.Longitude);
        }

        public double getX()
        {
            double tmp = 1 + Longitude / 180.0;
            return tmp * Math.Pow(2.0, Zoom - 1);
        }

        public void setX(double x)
        {
            double tmp = x / Math.Pow(2.0, Zoom - 1) - 1;
            this.setLongitude(tmp * 180);
        }

        public double getdX(WayPoint wpt)
        {
            GeoPos geo = new GeoPos(wpt, this.getZoom());
            return geo.getX() - this.getX();
        }

        public double getdY(WayPoint wpt)
        {
            GeoPos geo = new GeoPos(wpt, this.getZoom());
            return geo.getY() - this.getY();
        }

        public double getLongitude(double dx)
        {
            double tmp = (this.getX() + dx) / Math.Pow(2.0, Zoom - 1) - 1;
            return tmp * 180;
        }

        public double getY()
        {
            double phi = -Latitude * Math.PI / 180.0;
            double a = Math.Log(Math.Tan(phi) + 1 / Math.Cos(phi)) / Math.PI;
            return (1 + a) * Math.Pow(2.0, Zoom - 1);
        }

        public void setY(double y)
        {
            double a = (1 - y / Math.Pow(2.0, Zoom - 1)) * Math.PI;
            this.setLatitude(Math.Atan((Math.Exp(a) - Math.Exp(-a)) / 2.0) * 180 / Math.PI);
        }

        public double getLatitude(double dy)
        {
            double j = (1 - (this.getY() + dy) / Math.Pow(2.0, Zoom - 1)) * Math.PI;
            return Math.Atan((Math.Exp(j) - Math.Exp(-j)) / 2.0) * 180.0 / Math.PI;

        }

        public void ZoomIn()
        {
            if (Zoom <= 19)
                Zoom++;
        }

        public void ZoomOut()
        {
            if (Zoom > 0)
                Zoom--;
        }

        public void setZoom(int level)
        {
            if (level < 0)
                Zoom = 0;
            else if (level > 19)
                Zoom = 19;
            else
                Zoom = level;
        }

        public int getZoom()
        {
            return Zoom;
        }


    }

}
