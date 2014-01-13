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
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    class OziexplorerMapBackgrnd:IMapBackground
    {
        ListaMapas lista = null;
        MapaCalibrado mapa;

        PuntoF homeXY = new PuntoF(0, 0);
        float _dx, _dy, _x, _y;
        int _width, _height;

        WayPoint center = null;
        WayPoint tmpCenter = null;

        Singleton me = Singleton.GetInstance();

        public OziexplorerMapBackgrnd()
        {
            lista = new ListaMapas();
            lista.BuscarFiles(me.CacheMapsPath, "*.map", true);
            
            //lista.BuscarFiles(@"D:\Mapas José Antonio", "*.map", true);
            center = new WayPoint("", -6.1939f, 37.2630f);
        
        }

        public void Destroy()
        {
        }
        public void SetCenter(WayPoint wpt)
        {
            this.tmpCenter = wpt;
            mapa = lista.GetBest(new PuntoF((float)tmpCenter.Longitude, (float)tmpCenter.Latitude));
        }

        public void SetMode(Modes modo)
        {
        
        }

        public void SetZoom(int zoom)
        {
        }
        
        public int GetZoom()
        {
            return 0;
        }

        public double GetLongitude(double dx)
        {
            _dx = (float)dx + _width / 2;
            PuntoF wpt = mapa.InterpolaXY(_dx - homeXY.x, _dy - homeXY.y);
            return wpt.x;
        }
       
        public double GetLatitude(double dy)
        {
            _dy = (float)dy + _height / 2;
            PuntoF wpt = mapa.InterpolaXY(_dx - homeXY.x, _dy - homeXY.y);
            return wpt.y;
        }

        public double getdX(WayPoint wpt)
        {
            PuntoF p = mapa.InterpolaLonLat((float)wpt.Longitude, (float)wpt.Latitude)-
                mapa.InterpolaLonLat((float)center.Longitude, (float)center.Latitude);
            return p.x;
        }

        public double getdY(WayPoint wpt)
        {
            PuntoF p = mapa.InterpolaLonLat((float)wpt.Longitude, (float)wpt.Latitude) -
                mapa.InterpolaLonLat((float)center.Longitude, (float)center.Latitude); 
            return p.y;
        }

        
        public void setX(double x)
        {
            _x = (float)x-homeXY.x;
        }

        public void setY(double y)
        {
            _y = (float)y-homeXY.y;
            
            PuntoF p = mapa.InterpolaLonLat((float)center.Longitude, (float)center.Latitude);
            p.x += _x;
            p.y += _y;
            PuntoF tmp = mapa.InterpolaXY(p.x, p.y);
            tmpCenter = new WayPoint("", tmp.x, tmp.y);
        }

        public double getX()
        {
            return homeXY.x;
        }

        public double getY()
        {
            return homeXY.y;
        }

        void UpdateHomeXY(int Width, int Height)
        {
            mapa = lista.GetBest(new PuntoF((float)tmpCenter.Longitude, (float)tmpCenter.Latitude));
        
            if (tmpCenter != null && mapa != null)
            {
                PuntoF centroControl = new PuntoF(Width, Height) / 2;
                PuntoF centroGPS = mapa.InterpolaLonLat((float)tmpCenter.Longitude, (float)tmpCenter.Latitude);
                PuntoF resta = centroControl - centroGPS;
                /* */
                if (resta.x > 0)
                    resta.x = 0;
                if (resta.y > 0)
                    resta.y = 0;
                if (resta.x < Width - mapa.img.Width)
                    resta.x = Width - mapa.img.Width;
                if (resta.y < Height - mapa.img.Height)
                    resta.y = Height - mapa.img.Height;
                /* */
                centroGPS = centroControl - resta;
                PuntoF tmp= mapa.InterpolaXY(centroGPS.x, centroGPS.y);
                center = new WayPoint("", tmp.x, tmp.y);
                
                homeXY = resta;
            }
            else
            {
                homeXY = new PuntoF(0, 0);
            }
        }

        public void OnPaint(Graphics grp, int Width, int Height)
        {
            _width = Width;
            _height = Height;
            //base.OnPaint(e);
            UpdateHomeXY(Width, Height);

            if (mapa != null)
            {
                Graphics g = grp;
                Font f = new Font(FontFamily.GenericSansSerif, 12.0f);
                g.DrawImage(mapa.img, homeXY.x, homeXY.y);
            }
        }
    }
}
