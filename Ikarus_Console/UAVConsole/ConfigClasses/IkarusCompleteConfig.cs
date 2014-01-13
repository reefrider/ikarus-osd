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
    class IkarusCompleteConfig: GenericConfigClass
    {
        public IkarusBasicConfig IkarusBasicConfig;
        public IkarusAutopilotConfig IkarusAutopilotConfig;
        public IkarusScreenConfig IkarusScreenConfig1;
        public IkarusScreenConfig IkarusScreenConfig2;
        public IkarusScreenConfig IkarusScreenConfig3;
        public IkarusScreenConfig IkarusScreenConfigFailSafe;
        public IkarusScreenConfig IkarusScreenConfigResumen;

        public IkarusCompleteConfig()
        {
            this.IkarusAutopilotConfig = new IkarusAutopilotConfig();
            this.IkarusBasicConfig = new IkarusBasicConfig();
            this.IkarusScreenConfig1 = new IkarusScreenConfig();
            this.IkarusScreenConfig2 = new IkarusScreenConfig();
            this.IkarusScreenConfig3 = new IkarusScreenConfig();
            this.IkarusScreenConfigFailSafe = new IkarusScreenConfig();
            this.IkarusScreenConfigResumen = new IkarusScreenConfig();
            size_bytes();
        }

        public override void LoadDefaults()
        {
            this.IkarusAutopilotConfig.LoadDefaults();
            this.IkarusBasicConfig.LoadDefaults();
            this.IkarusScreenConfig1.LoadDefaults();
            this.IkarusScreenConfig2.LoadDefaults();
            this.IkarusScreenConfig3.LoadDefaults();
            this.IkarusScreenConfigFailSafe.LoadDefaults();
            this.IkarusScreenConfigResumen.LoadDefaults();

        }

    }
}
