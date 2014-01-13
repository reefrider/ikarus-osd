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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UAVConsole.Intrumentos
{
    public partial class Led : UserControl
    {
        public bool valid = false;

        Bitmap led_red = global::UAVConsole.Properties.Resources.Sphere_Red;
        Bitmap led_green = global::UAVConsole.Properties.Resources.Sphere_Green;
       
        public Led()
        {
            led_red.SetResolution(96, 96);
            led_green.SetResolution(96, 96);

            InitializeComponent();
            this.DoubleBuffered = true;

        
        }
     
        protected override void OnResize(EventArgs e)
        {
            this.Width = led_red.Width;
            this.Height = led_red.Height;
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap me;

            if(valid)
                me = led_green;
            else
                me = led_red;

            g.DrawImageUnscaled(me, 0, 0);
        }
    }
}
