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
    public class PidInfo : GenericConfigClass
    {
        public float P;
        public float I;
        public float D;
        public float ILim;
        public float DriveLim;
    //    public byte rev;

        public override void LoadDefaults()
        {
            this.P = 0.02f;
            this.I = 0.0f;
            this.D = 0.0f;
            this.ILim = 0.0f;
            this.DriveLim = 1.0f;
      //      this.rev = 0;
        }
    }
}
