using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.ConfigClasses
{
    public class GradosServos : GenericConfigClass
    {
        public float grados;
        public short servo;
       
        public override void LoadDefaults()
        {
            grados = 0.0f;
            servo = 1500;
        }
    }
}
