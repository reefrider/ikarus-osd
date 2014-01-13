/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
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

using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.Modem
{
    public class PlaneState
    {
        public float Lon;
        public float Lat;
        public float Alt;
        public float Rumbo;
        public float Knots;
        public float vertSpeed;

  //      public float RPM;
        public float WptIndex;
        public float homeLon;
        public float homeLat;
        public float v1;
        public float v2;
        public float pitch;
        public float roll;
        public float RSSI;

        // modem status
        public bool lastrx;
        
        


        public PlaneState()
        {
            this.Lon = -6.0f;
            this.Lat = 37.5f;
            this.homeLon = -6.0f;
            this.homeLat = 37.5f;
            this.Alt = 0;
            this.Knots = 0;
            this.Rumbo = 0;
        }
    }
}
