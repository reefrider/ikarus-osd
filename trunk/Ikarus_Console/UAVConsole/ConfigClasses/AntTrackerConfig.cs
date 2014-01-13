using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.ConfigClasses
{
    public class AntTrackerConfig : GenericConfigClass
    {
       
        public byte useInternalGPS;
        public float AltitudSinGPS;

        public byte useInternalCompas;
        public float offsetCompas;
        public byte enableCompasOverride;
        public float speedCompasOverride;

        public byte useLipo;
        public byte numCellsLipo;
        public float Vmin;
        public float Vmax;
        public float Valarm;

        public float offsetPan;

        public byte decodeTelemetry;

        public byte useServo360;

        public GradosServos GradosPANleft2;
        public GradosServos GradosPANleft;
        public GradosServos GradosPANcenter;
        public GradosServos GradosPANright;
        public GradosServos GradosPANright2;

        public GradosServos GradosTILTdown;
        public GradosServos GradosTILTcenter;
        public GradosServos GradosTILTup;

        public override void LoadDefaults()
        {
            useInternalGPS = 0;
            AltitudSinGPS = 100.0f;

            useInternalCompas = 0;
            offsetCompas = 0.0f;
            speedCompasOverride = 20.0f;

            useLipo = 1;
            numCellsLipo = 2;
            Vmin = 6.4f;
            Vmax = 8.4f;
            Valarm = 7.4f;

            offsetPan = 0.0f;
            decodeTelemetry = 0;

            useServo360 = 0;

          
        }
    }
}
