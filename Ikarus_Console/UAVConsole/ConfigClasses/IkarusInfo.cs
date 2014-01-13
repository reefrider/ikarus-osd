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
    public class IkarusInfo : GenericConfigClass
    {
        public float v1;
        public float v2;
        public float currI;
        public float consumidos_mAh;
        public float mv_currI;
        public float mv_sensorV1;
        public float mv_sensorV2;

        public byte AutoPilot_Enabled;

        public float navigator_bearing;
        public float navigator_altitude;
        public float navigator_rel_bearing;

        public float AntTracker;
        public float AntTrackerV;

        public float distance_home;
        public float distance_wpt;

        public byte modem_sw;						// datos subidos desde la emisora
        public Int16 modem_alt;
        public float modem_lon;
        public float modem_lat;

        // Obtenido desde los switches de la emisora
        public byte ctrl_hudnum;
        public byte ctrl_autopilot;
        public byte ctrl_doruta;
        public byte ctrl_camsel;

        // Sensor Copilot/IMU
        public float Pitch;
        public float Roll;

        // Salida del piloto automatico
        public float AutoPilot_ail;
        public float AutoPilot_ele;
        public float AutoPilot_thr;
        public float AutoPilot_tail;

        public float AutoPilot_pitch;
        public float AutoPilot_roll;

        // Calculados
        public float distancia_recorrida;
        public float max_distance_home;
        public float hora_inicio;
        public float tasa_planeo;
        public float coste_km_Ah;
        public float RSSI;

        //otros
        public byte mostrarResumen;

        public float tempC;
    }
}
