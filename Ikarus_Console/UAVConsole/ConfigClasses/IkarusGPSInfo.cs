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

namespace UAVConsole.ConfigClasses
{
    public class IkarusGPSInfo : GenericConfigClass
    {
        public float lon;
        public float lat;
        public float alt;
        public float knots;
        public float rumbo;
        //public float mag_desv;
        public byte fix;
        public byte act;
        public float hdop;
        public Int16 numsats;
        public float hora;
        public Int32 fecha;

        // estado
        public byte conected;
        public byte nmea_ok;
        public byte pos_valid;

        public byte RMC_received;
        public byte GGA_received;
        public byte bad_crc;
        public byte uart_timeout;
        public byte nmea_timeout;

        // filtradas
        public float alt_filter;

        // calculadas
        public float kmph;
        public float verticalSpeed;
        public float kmphMAX;
        public float altitudeMAX;
        public byte en_movimiento;
    }
}
