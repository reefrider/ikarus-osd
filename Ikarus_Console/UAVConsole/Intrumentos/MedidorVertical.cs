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
    public partial class MedidorVertical : UserControl
    {
        public enum Orientacion
        {
            Horizontal, 
            Vertical
        }

        bool _striped = false;
        protected float _valor =1.0f;
        Orientacion _orientacion = Orientacion.Vertical;

        public float valor
        {
            get
            {
                return _valor;
            }
            set
            {
                if (value > 1.0f)
                    _valor = 1.0f;
                else if (value < 0.0f)
                    _valor = 0.0f;
                else
                    _valor = value;
            }
        }

        public MedidorVertical()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            int h = this.Height-5;
            int w = this.Width-5;
            int tw = h * 3 / 4;

            int ntx = w / (tw + 2);

            int lx = (int)(w * _valor + 1);
            LinearGradientBrush grad;
            Font f = new System.Drawing.Font("sans-serif", 3 * h / 4);

            if (_orientacion == Orientacion.Horizontal)
            {
                grad = new LinearGradientBrush(new Rectangle(1, 1, w, h), Color.Red, Color.Green, LinearGradientMode.Horizontal);
                if (_striped)
                {
                    for (int i = 1; i < lx; i += tw + 2)
                        g.FillRectangle(grad, i, 1, Math.Min(tw, lx - i), h);
                }
                else
                    g.FillRectangle(grad, 1, 1, w * _valor-1, h-1);
        
            }
            else
            {
                grad = new LinearGradientBrush(new Rectangle(1, 1, w, h), Color.Green, Color.Red, LinearGradientMode.Vertical);
                if (_striped)
                {
                    for (int i = 1; i < lx; i += tw + 2)
                        g.FillRectangle(grad, i, 1, Math.Min(tw, lx - i), h);
                }
                else
                    g.FillRectangle(grad, 1, 1+(1-_valor)*h, w-1, h * _valor-1);
        
            }
            
            
            }

    }
}
