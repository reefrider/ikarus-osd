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
    public partial class Instrumento_VerticalSpeed : UserControl
    {
       
        Bitmap vsi = global::UAVConsole.Properties.Resources.vsi1;
        Bitmap misc2 = global::UAVConsole.Properties.Resources.misc2;
        Bitmap needle;

        int[] values = { -2000, -1500, -1000, -500, 0, 500, 1000, 1500, 2000 };
        float[] angles = { -173.5f, -131.5f, -81.5f, -35.3f, 0.0f, 35.7f, 81.5f, 131.0f, 172.9f };

        float _valor;

        public Instrumento_VerticalSpeed()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
            vsi.SetResolution(96, 96);
            misc2.SetResolution(96, 96);

            needle = misc2.Clone(new RectangleF(0, (1 - 0.4375f) * misc2.Height, 120, 24), misc2.PixelFormat);
        }

        public float Value
        {
            get
            {
                return _valor;
            }
            set
            {
                if (value > 2000)
                    _valor = 2000;
                else if (value < -2000)
                    _valor = -2000;
                else
                    _valor = value;
                Invalidate();
            }
        }

        private float getAngle(int angle)
        {
            double salida;
            int i;

            for (i = 0; i < values.Length - 1 && values[i] < angle; i++) ;
            double angle2 = angles[i]; // toAngle(centerx,centery,vx[i],vy[i]);
            
            if (i == 0)
            {
                salida = angles[i];
            }
            else
            {
                double angle1 = angles[i - 1];
                double alfa;
                alfa = angles[i] - angles[i - 1];
                if (alfa < 0)
                    alfa += 360.0;

                alfa = alfa / (values[i] - values[i - 1]);
                salida = angles[i - 1] + alfa * (angle - values[i - 1]);

            }
            return (float)(salida);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Bitmap bmp = new Bitmap(vsi.Width, vsi.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackColor);

            g.DrawImage(vsi, 0, 0);

            g.TranslateTransform(vsi.Width/2, vsi.Height/2);

            g.RotateTransform(getAngle((int)_valor));
            g.DrawImage(needle, -needle.Width / 2 - 28, -needle.Height / 2);

            g.Dispose();
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        //    base.OnPaintBackground(e);
        }
    }
}
