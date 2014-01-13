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
using System.Drawing.Drawing2D;

namespace UAVConsole.Intrumentos
{
    public partial class Instrumento_DirectionalGyro : UserControl
    {
        Bitmap background = global::UAVConsole.Properties.Resources.hdg2;
        Bitmap gyrocard = global::UAVConsole.Properties.Resources.hdg1;
        float heading = 0.0f;
        
        public Instrumento_DirectionalGyro()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
            background.SetResolution(96, 96);
            gyrocard.SetResolution(96, 96);


        }

        public float Value
        {
            get
            {
                return heading;
            }
            set
            {
                heading = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(background.Width, background.Height,background.PixelFormat);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(BackColor);

            while (heading > 360.0)
                heading -= 360.0f;
            while (heading < 0)
                heading += 360.0f;

            GraphicsState saved=g.Save();
            g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);

            g.RotateTransform(-heading);
            g.DrawImage(gyrocard, -bmp.Width / 2, -bmp.Height / 2);
            g.Restore(saved);

            g.DrawImage(background, 0, 0);
         
            g.Dispose();
            //e.Graphics.DrawImageUnscaled(bmp, 0, 0);
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

        }

        protected override void OnResize(EventArgs e)
        {
//            base.OnResize(e);
          //  this.Width = background.Width;
          //  this.Height = background.Height;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
