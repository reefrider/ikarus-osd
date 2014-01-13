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
using System.IO;

namespace UAVConsole.GoogleMaps
{
    public partial class MapaInterpolado : UserControl
    {
        public float lon, lat;

        PuntoF homeXY = new PuntoF(0, 0);
        WayPoint center = null;
        MapaCalibrado mapa;
        ListaMapas lista = null;

        public MapaInterpolado()
        {
            lista = new ListaMapas();
            //lista.BuscarFiles(@"D:\Mapas José Antonio", "*.map", true);
            center = new WayPoint("", -6.126663f, 37.35648f);
            InitializeComponent();
        }
        public void LoadOziexplorer(string Filename)
        {
            mapa = new MapaCalibrado(Filename);
        }

        public void SetMapaCalibrado(MapaCalibrado map)
        {
            mapa = map;
        }

        public void SetCenter(WayPoint wpt)
        {
            this.center = wpt;
            this.Invalidate();
        }

        void UpdateHomeXY()
        {
            if (mapa == null)
            {
                mapa = lista.GetBest(new PuntoF((float)center.Longitude, (float)center.Latitude));
            }
            if (center != null && mapa != null)
            {
                PuntoF centroControl = new PuntoF(this.Width, this.Height) / 2;
                PuntoF centroGPS = mapa.InterpolaLonLat((float)center.Longitude, (float)center.Latitude);
                PuntoF resta = centroControl - centroGPS;
                if (resta.x > 0)
                    resta.x = 0;
                if (resta.y > 0)
                    resta.y = 0;
                if (resta.x < this.Width - mapa.img.Width)
                    resta.x = this.Width - mapa.img.Width;
                if (resta.y < this.Height - mapa.img.Height)
                    resta.y = this.Height - mapa.img.Height;

                homeXY = resta;
            }
            else
            {
                homeXY = new PuntoF(0, 0);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            UpdateHomeXY();
              
            if (mapa != null)
            {
                Graphics g = e.Graphics;
                Font f = new Font(FontFamily.GenericSansSerif, 12.0f);
                string str = "Lon " + lon + " Lat " + lat;
                SizeF size = g.MeasureString(str, f);
                g.DrawImage(mapa.img,homeXY.x,homeXY.y);
                
                g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                g.DrawString(str, f, Brushes.Black, new PointF(0,0));
                
                PuntoF p = mapa.InterpolaLonLat(lon, lat) + homeXY;
                e.Graphics.FillRectangle(Brushes.Red, p.x - 5, p.y - 5, 10, 10);

                PuntoF q = mapa.InterpolaLonLat((float)center.Longitude, (float)center.Latitude) + homeXY;
                e.Graphics.FillRectangle(Brushes.Yellow, q.x - 5, q.y - 5, 10, 10);

            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mapa != null)
            {
                PuntoF pos = new PuntoF(e.X, e.Y);
                pos = pos - homeXY;
                PuntoF p = mapa.InterpolaXY(pos.x, pos.y);
                lon = p.x;
                lat = p.y;
                this.Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
