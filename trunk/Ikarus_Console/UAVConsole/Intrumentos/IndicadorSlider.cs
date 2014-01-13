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
    public partial class IndicadorSlider : UserControl
    {
        private float _inicio=0.0f;
        private float _fin=0.5f;
        private Brush _pincel= Brushes.Aquamarine;
        private string _mensaje = "";

        public float PosInicio
        {
            get
            {
                return _inicio;
            }
            set
            {
                _inicio = value;
                Invalidate();
            }
        }

        public float PosFin
        {
            get
            {
                return _fin;
            }
            set
            {
                _fin = value;
                Invalidate();
            }
        }

        public Brush Pincel
        {
            get
            {
                return _pincel;
            }
            set
            {
                _pincel = value;
                Invalidate();
            }
        }

        public string Texto
        {
            get
            {
                return _mensaje;
            }
            set
            {
                _mensaje = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
             Font f = new System.Drawing.Font("sans-serif", 3*this.Height / 5);
            
            int p_ini = (int)(this.Width * _inicio);
            int p_fin = (int)(this.Width * _fin);
            int tmp;
            if (p_ini > p_fin)
            {
                tmp = p_ini;
                p_ini = p_fin;
                p_fin = tmp;
            }
            g.FillRectangle(_pincel, p_ini, 0, p_fin - p_ini, this.Height);
            if (_mensaje != null)
            {
                SizeF textSize = g.MeasureString(_mensaje, f);
                g.DrawString(_mensaje, f, new SolidBrush(this.ForeColor), (this.Width - textSize.Width) / 2, (this.Height - textSize.Height) / 2);
            }
        }

        public IndicadorSlider()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
        }
    }
}
