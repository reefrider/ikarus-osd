using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.ConfigClasses
{
    class AntTrackerDebug : GenericConfigClass
    {
        public byte EnableDebug;
        public short pan;
        public short tilt;
        public float grados_pan;
        public float grados_tilt;

        public AntTrackerDebug()
        {
            LoadDefaults();
        }

        public override void LoadDefaults()
        {
            EnableDebug = 0;
            pan = 1500;
            tilt = 1500;
            grados_pan = 0.0f;
            grados_tilt = 0.0f;

        }
    }
}
