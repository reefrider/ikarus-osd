using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.ConfigClasses
{
    class AntTrackerDatosAntena : GenericConfigClass
    {
        public byte tieneGPS;
        public byte tienePOS;
        public float lon;
        public float lat;
        public Int16 alt;

        public float v_bateria;
        public float v_baterial_porcentaje;
        public byte v_bateria_alarm;

        public AntTrackerDatosAntena()
        {
            //LoadDefaults();
        }

        public override void LoadDefaults()
        {
            tieneGPS = 0;
            tienePOS = 0;
            lon = -6.0f;
            lat = 37.5f;
            alt = 100;

            v_bateria = 0.0f;
            v_baterial_porcentaje = 0.0f;
            v_bateria_alarm = 0;
        }
    }
}
