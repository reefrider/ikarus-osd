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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UAVConsole.Intrumentos
{
    public partial class Instrumento_AirSpeed : UserControl
    {
        Bitmap background = global::UAVConsole.Properties.Resources.velocidad;
        Bitmap needle = global::UAVConsole.Properties.Resources.asi_needle;

        int[] values = { 0, 20, 40, 60, 80, 100 };
        int[] vx = { 80, 124, 114, 65, 30, 49 };
        int[] vy = { 32, 61, 112, 123, 87, 42 };
        float[] angles;

        float centerx = 79, centery = 79;

        float _valor;
        
        public Instrumento_AirSpeed()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
            background.SetResolution(96.0f, 96.0f);
            needle.MakeTransparent();
            needle.RotateFlip(RotateFlipType.Rotate270FlipNone);
            angles = toAngle((int)centerx, (int)centery, vx, vy);
        }


        public float Value
        {
            get
            {
                return _valor;
            }
            set
            {
                if (value > 100)
                    _valor = 100;
                else if (value < 0)
                    _valor = 0;
                else
                    _valor = value;
                Invalidate();
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Bitmap bmp = new Bitmap(background.Width, background.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackColor);

            g.DrawImage(background, 0, 0);

            g.TranslateTransform(centerx, centery);

            g.RotateTransform(getAngle((int)_valor)-90);
            g.DrawImage(needle, (int)-21.75f, (int)-4.5f);

            g.Dispose();
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected static float toAngle(int x0, int y0, int x1, int y1)
        {
            double angle = Math.Atan2(x1 - x0, y0 - y1) * 180.0 / Math.PI;
            while (angle < 0)
                angle += 360.0;
            return (float)angle;
        }

        protected static float[] toAngle(int x0, int y0, int[] x1, int[] y1)
        {
            float[] salida = new float[x1.Length];
            for (int i = 0; i < x1.Length; i++)
                salida[i] = toAngle(x0, y0, x1[i], y1[i]);
            return salida;
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
    }
}

