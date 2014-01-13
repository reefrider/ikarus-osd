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
    public partial class MedidorBaterias : UserControl
    {
        const float cell_min_v = 3.2f;
        const float cell_max_v = 4.2f;

        private int _num_cells;
        private float _volts;
        private bool _striped = false;

        private float _volts_min;
        private float _volts_max;

        private bool _autoCalculate;
        private float _value;

        public float volts_min
        {
            get
            {
                return _volts_min;
            }
            set
            {
                _volts_min = value;
                _num_cells = 0;
                Invalidate();
            }
        }

        public float volts_max
        {
            get
            {
                return _volts_max;
            }
            set
            {
                _volts_max = value;
                _num_cells = 0;
                Invalidate();
            }
        }

        public bool strip
        {
            get
            {
                return _striped;
            }
            set
            {
                _striped = value;
                Invalidate();
            }
        }

        public int num_cells
        {
            get
            {
                return _num_cells;
            }
            set
            {
                _num_cells = value;
                if (_num_cells > 0)
                {
                    _volts_min = cell_min_v * _num_cells;
                    _volts_max = cell_max_v * _num_cells;
                }
                Invalidate();
            }
        }

        public float volts
        {
            get
            {
                return _volts;
            }
            set
            {
                _volts = value;
                Invalidate();
            }
        }

        public bool AutoCalculate
        {
            get
            {
                return _autoCalculate;
            }
            set
            {
                _autoCalculate = value;
                Invalidate();
            }
        }

        public float valor
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public MedidorBaterias()
        {
            InitializeComponent();
            _autoCalculate = true;
            this.DoubleBuffered = true;
        }

        Color toGrayScale(Color originalColor)
        {
            //create the grayscale version of the pixel
            int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
                + (originalColor.B * .11));

            //create the color object
            Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);
            return newColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color colorIni = Color.Red;
            Color colorFin = Color.Green;
            
            float v;

            /*
            if (!this.Enabled) // || v < 1.0f)
            {
                colorIni = Color.DarkGray;
                colorFin = Color.LightGray;
                //colorIni = toGrayScale(colorIni);
                //colorFin = toGrayScale(colorFin);
            }
            */
            if (this.Enabled)
            {
                if (_autoCalculate)
                    _value = (_volts - _volts_min) / (_volts_max - _volts_min);

                v = _value;

            }
            else
            {
                colorIni = Color.DarkGray;
                colorFin = Color.LightGray;
            
                v = 1.0f;
            }

            
            int h = this.Height - 6;
            int w = this.Width - 2;
            int tw = h * 3 / 4;

            int ntx = w / (tw + 2);

            int lx = (int)(w * v + 1);
            LinearGradientBrush grad = new LinearGradientBrush(new Rectangle(1, 1, w, h), colorIni, colorFin, LinearGradientMode.Horizontal);
            Font f = new System.Drawing.Font("sans-serif", 3*h / 4);


            if (_striped)
            {
                for (int i = 1; i < lx; i += tw + 2)
                    g.FillRectangle(grad, i, 1, Math.Min(tw, lx - i ), h);
            }
            else
                g.FillRectangle(grad, 1, 1, lx - 1, h);

            if (this.Enabled)
            {
                string s = _volts.ToString("#0.0v");

                g.DrawString(s, f, new SolidBrush(this.ForeColor),
                    (w - g.MeasureString(s, f).Width) / 2, (h - f.GetHeight()) / 2);
            }

        }
    }
}
