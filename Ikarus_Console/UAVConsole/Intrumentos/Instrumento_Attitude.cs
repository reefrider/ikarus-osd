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
    public partial class Instrumento_Attitude : UserControl
    {
        Bitmap background = global::UAVConsole.Properties.Resources.attitude1;
        Bitmap gyro_back = global::UAVConsole.Properties.Resources.attitude5;
        Bitmap gyro_color = global::UAVConsole.Properties.Resources.attitude2;
        Bitmap pitch_color = global::UAVConsole.Properties.Resources.attitude3;
        Bitmap extra = global::UAVConsole.Properties.Resources.madr_misc1;

        float _pitch, _roll;

        public Instrumento_Attitude()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
            background.SetResolution(96, 96);
            gyro_back.SetResolution(96, 96);
            gyro_color.SetResolution(96, 96);
            pitch_color.SetResolution(96, 96);
            extra.SetResolution(96, 96);
        }

        public float pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                _pitch = value;
                Invalidate();
            }
        }

        public float roll
        {
            get
            {
                return _roll;
            }
            set
            {
                _roll = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
        //    base.OnPaint(e);
            Bitmap bmp = new Bitmap(background.Width, background.Height);
            Graphics g = Graphics.FromImage(bmp);
            float roll=_roll;
            float pitch = _pitch;

            while (roll < 0)
                roll += 360.0f;
            while (roll > 360)
                roll -= 360.0f;
            /*
            while (pitch < -180.0f)
                pitch += 360.0f;
            while (pitch > 180.0f)
                pitch -= 360.0f;
            */

            if (pitch > 45.0f)
                pitch = 45.0f;
            else if (pitch < -45.0f)
                pitch = -45.0f;

            float oy = 45 *(float)Math.Sin(pitch * Math.PI / 180.0f);

            g.Clear(BackColor);

            System.Drawing.Drawing2D.GraphicsState saved=g.Save();

            g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
            g.RotateTransform(-roll);

            g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
            g.DrawImage(gyro_color, 0, 0);
            
            g.DrawImage(pitch_color, 0, oy);

            g.DrawImage(gyro_back, 0, 0);

            g.Restore(saved);
            g.DrawImage(extra, 64, 112);
            g.DrawImage(background, 0, 0);
            g.Dispose();
            //e.Graphics.DrawImageUnscaled(bmp, 0, 0);
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

            
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        
        }
    }
}
