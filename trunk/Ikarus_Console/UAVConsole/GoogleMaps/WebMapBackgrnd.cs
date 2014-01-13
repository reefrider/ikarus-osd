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
using System.Windows.Forms;
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    class WebMapBackgrnd : IMapBackground
    {
        const int texelsize = 256;
        ImageProxy imageProxy;
        GeoPos geo;
        IkarusMapControl mapControl;
        Singleton me = Singleton.GetInstance();

        public WebMapBackgrnd(IkarusMapControl map)
        {
            mapControl = map;
            if (me.GoogleMapsCookie == null)
                imageProxy = new MicrosoftProxy();
            else
                imageProxy = new GoogleProxy(me.GoogleMapsCookie);
            
            imageProxy.listeners += notify;
            
            geo = new GeoPos(new WayPoint("gps", -6.003753, 37.391162), 10);
        }

        public void Destroy()
        {
            imageProxy.Destroy();
        }

        void notify()
        {
            mapControl.Invalidate();
        }

        public void SetCenter(WayPoint wpt)
        {
            geo.setWpt(wpt);
        }

        public void SetMode(Modes modo)
        {
            imageProxy.setMode(modo);
        }

        public void SetZoom(int zoom)
        {
            geo.setZoom(zoom);
        }
        public int GetZoom()
        {
            return geo.getZoom();
        }

        public double GetLongitude(double dx)
        {
            return geo.getLongitude(dx / texelsize);
        }
       
        public double GetLatitude(double dy)
        {
            return geo.getLatitude(dy / texelsize);
        }

        public double getdX(WayPoint wpt)
        {
            return geo.getdX(wpt) * texelsize;
        }

        public double getdY(WayPoint wpt)
        {
            return geo.getdY(wpt) * texelsize;
        }

        
        public void setX(double x)
        {
            geo.setX(x / texelsize);
        }

        public void setY(double y)
        {
            geo.setY(y / texelsize);
        }

        public double getX()
        {
            return geo.getX() * texelsize;
        }

        public double getY()
        {
            return geo.getY() * texelsize;
        }

        public void OnPaint(Graphics grp, int Width, int Height)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bmp);
 
            double x = geo.getX();
            double y = geo.getY();


            x += -Width / 2.0 / texelsize;
            y += -Height / 2.0 / texelsize;

            int ix = (int)x;
            int iy = (int)y;
            int offX = (int)((x - ix) * texelsize);
            int offY = (int)((y - iy) * texelsize);
            int z = geo.getZoom();
            int i, j;

            //Dibuja el fondo
            for (i = 0; i < Width / texelsize + 2; i++)
                for (j = 0; j < Height / texelsize + 2; j++)
                {

                    Image texel = imageProxy.getTexel(ix + i, iy + j, z);
                    if (texel != null)
                        g.DrawImage(texel, i * texelsize - offX, j * texelsize - offY);
                }

            g.Dispose();
            grp.DrawImage(bmp, 0, 0);
        }

    }
}
