using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.ConfigClasses
{
    class AntTrackerDatosAvion : GenericConfigClass
    {
        public float lon;
        public float lat;
        public short alt;

        public float home_lon;
        public float home_lat;
        public short home_alt;

        public AntTrackerDatosAvion()
        {
           // LoadDefaults();
        }

        public override void LoadDefaults()
        {
            lon = -6.0f;
            lat = 37.5f;
            alt = 100;

            home_lon = -6.0f;
            home_lat = 37.5f;
            home_alt = 100;
        }
    }

}
