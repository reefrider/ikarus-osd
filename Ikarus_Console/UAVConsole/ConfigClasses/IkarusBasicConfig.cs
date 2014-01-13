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
using UAVConsole.ConfigClasses;
using System.Reflection;

namespace UAVConsole.ConfigClasses
{
    public class IkarusBasicConfig : GenericConfigClass
    {
//        const int ClassSize = 88;
        // OSD
        public byte videoPAL;
        public byte offsetX;
        public byte offsetY;

        // Baterias
        public byte cellsBatt1;
        public float sensorV1_offset;
        public float sensorV1_gain;
        public byte cellsBatt2;
        public float sensorV2_offset;
        public float sensorV2_gain;
        public float total_mAh;
        public float sensorI_offset;
        public float sensorI_gain;
        

        // Alarms
        public float cellAlarm;
        public float distanceAlarm;
        public float altitudeAlarm;
        public float lowSpeedAlarm;

        // GPS
        public float HomeLon;
        public float HomeLat;
        public float HomeAltitude;

        // otros
        public float WptRange;
        public byte DefaultHUD;
        public byte TelemetryMode;
        public byte ControlProportional;
        public byte AbsoluteAltitude;
        public byte BaudRate;
        public byte MetricsImperial;			// Vamos a usar el sistema metrico o imperial
        public sbyte TimeZone;
        public byte CamSel;
        public byte modelo_ruta;
        public byte inicio_telemetry;
        public float rssi_min;
        public float rssi_max;

        public byte Modo_PPM;
        public byte PPM_Channel;
        public byte NumCanales_PPM;
	    public byte Modo_Failsafe;
        public byte Retraso_Failsafe;
        public byte LeftBand;

        public string NombrePiloto;

        public IkarusBasicConfig()
        {
            NombrePiloto = "";
            size_bytes();
        }

        public override void LoadDefaults()
        {
            this.videoPAL = 0;
            this.offsetX = (byte)44;
            this.offsetY = (byte)21;

            this.cellsBatt1 = (byte)3;
            this.cellsBatt2 = (byte)3;
            this.total_mAh = 2200.0f;

            this.cellAlarm = 3.2f;
            this.distanceAlarm = 600.0f;
            this.altitudeAlarm = 400.0f;
            this.lowSpeedAlarm = 40.0f;

            this.HomeLon = -6.0f;
            this.HomeLat = 37.5f;
            this.HomeAltitude = 0.0f;

            this.WptRange = 50.0f;
            this.DefaultHUD = (byte)0;
            this.TelemetryMode = (byte)1;
       
            this.ControlProportional = (byte)1;
            this.AbsoluteAltitude = (byte)0;
            this.BaudRate = 2; 
            this.TimeZone = 2;
            this.MetricsImperial = 0;
            this.CamSel = (byte)0;
            this.modelo_ruta = (byte)0;
            this.inicio_telemetry = (byte)20;
            this.sensorI_offset = 1.5f;
            this.sensorI_gain = 19.0f;
            this.rssi_min = 0.0f;
            this.rssi_max = 3.2f;
            this.Modo_PPM = (byte)1;
            this.PPM_Channel = (byte)5;
            this.NumCanales_PPM = 0;
            this.Modo_Failsafe = 0;
            this.LeftBand = 0;
            this.NombrePiloto = "Ikarus OSD";
        }
    }
}
