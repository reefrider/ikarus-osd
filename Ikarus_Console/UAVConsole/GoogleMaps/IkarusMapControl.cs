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

namespace UAVConsole.GoogleMaps
{
    public partial class IkarusMapControl : UserControl
    {
        Singleton singleton = Singleton.GetInstance();

        public WayPoint plane;
        public WayPoint target;
        public WayPoint home;
        public WayPoint soft_wpt;
        public string mensaje;

        public float rumboHold = float.NaN;

        public Dictionary<string, WayPoint> team_pos = new Dictionary<string,WayPoint>();

        IMapBackground map;
        
        WebMapBackgrnd web_bg;//= new MicrosoftMapBackgrnd();
        //OziexplorerMapBackgrnd ozi_bg; //= new OziexplorerMapBackgrnd();
        
        protected Boolean isdown = false;
        protected int x_down, y_down, x_curr, y_curr;
        protected Boolean esta_dentro = false;
    
        public List<WayPoint> ruta = new List<WayPoint>();

        public delegate void Listener(WayPoint wpt, MouseButtons btn);
        public Listener listeners;

        float rutaKM = 0.0f;

        List<WayPoint> historia = new List<WayPoint>();
               
        public IkarusMapControl()
        {
            home = new WayPoint("HOME", singleton.HomeLon, singleton.HomeLat);
            web_bg = new WebMapBackgrnd(this);
            //ozi_bg = new OziexplorerMapBackgrnd();
            map = web_bg;
            mensaje = null;
            SetCenter(home);
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public void Destroy()
        {
            if (map != null)
                map.Destroy();
        }

        public void AddHistory()
        {
            historia.Add(new WayPoint(plane));
        }

        public void ClearHistory()
        {
            historia.Clear();
        }

        public void SetCenter(WayPoint wpt)
        {
            map.SetCenter(wpt);
        }
        public void SetMode(Modes modo)
        {
           /* if (modo == Modes.TOPO)
                map = ozi_bg;
            else */
                map = web_bg;
            map.SetMode(modo);
            this.Invalidate();

        }

        public int GetZoom()
        {
            return map.GetZoom();
        }

        public void SetZoom(int level)
        {
            map.SetZoom(level);
        }

        void PintarOtherPlanes(Graphics g)
        {
            int x1, y1;
            Font mifont = new Font(Font.SystemFontName, 12.0f);
            GraphicsState restaurar = g.Save();
            try
            {
                // Pintamos a los demas aviones
                foreach (KeyValuePair<string, WayPoint> keypair in team_pos)
                {
                    WayPoint _plane = keypair.Value;
                    GraphicsState st = g.Save();
                    try
                    {
                        // Pintamos el avion
                        x1 = (int)map.getdX(_plane) + Width / 2;
                        y1 = (int)map.getdY(_plane) + Height / 2;

                        g.TranslateTransform(x1, y1);
                        g.RotateTransform(_plane.heading);
                        g.DrawImage(_plane.icon, -_plane.icon.Width / 2, -_plane.icon.Height / 2);

                        g.TranslateTransform(mifont.Height / 2, _plane.icon.Height / 2);
                        g.RotateTransform(90);
                        String etiqueta = keypair.Key;
                        if (singleton.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                            etiqueta += "(" + (int)(_plane.Altitude - singleton.HomeAlt) + "m)";
                        else
                            etiqueta += "(" + (int)((_plane.Altitude - singleton.HomeAlt) * 3.28f) + "ft)";
                        SizeF tam = g.MeasureString(etiqueta, mifont);
                        g.FillRectangle(Brushes.Yellow, 0, 0, tam.Width, tam.Height);
                        if (_plane.heading < 0 || _plane.heading > 180)
                            g.DrawString(etiqueta, mifont, Brushes.Red, 0, 0);
                        else
                        {
                            g.RotateTransform(180.0f);
                            g.DrawString(etiqueta, mifont, Brushes.Red, -tam.Width, -tam.Height);
                        }

                    }
                    catch (Exception) { }
                    g.Restore(st);
                }
            }
            catch (Exception) { }
            g.Restore(restaurar);
        }

        void PintarRuta(Graphics g)
        {
            int x1, y1, x2, y2;
       
            rutaKM = 0.0f;
                
            // Pintamos la ruta
            if (ruta.Count > 0)
            {
                rutaKM = 0.0f;
                
                x1 = (int)map.getdX(ruta[0]) + Width / 2;
                y1 = (int)map.getdY(ruta[0]) + Height / 2; ;

                for (int i = 0; i < ruta.Count; i++)
                {
                    if (i != 0)
                    {
                        x2 = (int)map.getdX(ruta[i]) + Width / 2;
                        y2 = (int)map.getdY(ruta[i]) + Height / 2;
                        rutaKM = rutaKM + ruta[i - 1].getDistance(ruta[i]);
                        if (x1 != x2 || y1 != y2)
                            g.DrawLine(Pens.Red, x1, y1, x2, y2);
                        x1 = x2;
                        y1 = y2;
                    }
                    g.FillEllipse(Brushes.Red, x1 - 4, y1 - 4, 8, 8);
                    Font mifont = new Font(Font.SystemFontName, 12.0f);
                    string nombre = ruta[i].name;
                    SizeF tam = g.MeasureString(nombre, mifont);
                    g.DrawString(nombre, mifont, Brushes.Red, x1-tam.Width / 2, y1-6-tam.Height);
                }
            }
        }

        void PintarSoftWpt(Graphics g)
        {
            if (soft_wpt != null)
            {
                try
                {
                    Bitmap modem_icon = global::UAVConsole.Properties.Resources.sight_icon_2;

                    int sx = (int)map.getdX(soft_wpt) + Width / 2;
                    int sy = (int)map.getdY(soft_wpt) + Height / 2;
                    if (modem_icon == null)
                        g.FillEllipse(Brushes.Coral, sx - 4, sy - 4, 8, 8);
                    else
                        g.DrawImage(modem_icon, sx - 16 / 2, sy - 16 / 2, 16, 16);
                }
                catch (Exception) { }
            }
        }

        void DrawTarget(Graphics g, Point avion, Point destino)
        {
            // Pinta linea plane-target

            try
            {
                if (!avion.IsEmpty&&!destino.IsEmpty && avion != destino)
                {
                    g.DrawLine(Pens.Yellow, avion, destino);
                }

                g.FillEllipse(Brushes.Yellow, destino.X - 4, destino.Y - 4, 8, 8);
            }
            catch (Exception) { }
        }

        void DrawHome(Graphics g, Point avion, Point casa)
        {
            GraphicsState st = g.Save();
            try
            {
                if (!casa.IsEmpty && avion != casa)
                    g.DrawLine(Pens.Red, avion, casa); // Pinta linea plane-casa

            }
            catch (Exception)
            {
            }

            // despues el avion
            g.TranslateTransform(avion.X, avion.Y);
            g.RotateTransform(plane.heading);
            try
            {
                g.DrawImage(plane.icon, -plane.icon.Width / 2, -plane.icon.Height / 2);
                if (singleton.enableUDPinout||true)
                {
                    Font mifont2 = new Font(Font.SystemFontName, 12.0f);
                    string etiqueta;

                    if (singleton.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                        etiqueta = "(" + (int)(plane.Altitude-singleton.HomeAlt) + "m)";
                    else
                        etiqueta = "(" + (int)((plane.Altitude-singleton.HomeAlt) * 3.28f) + "ft)";
                             
                    g.TranslateTransform(mifont2.Height / 2, plane.icon.Height / 2);
                    g.RotateTransform(90);
                    SizeF tam = g.MeasureString(etiqueta, mifont2);
                    g.FillRectangle(Brushes.Yellow, 0, 0, tam.Width, tam.Height);
                    if (plane.heading < 0)
                        g.DrawString(etiqueta, mifont2, Brushes.Red, 0, 0);
                    else
                    {
                        g.RotateTransform(180.0f);
                        g.DrawString(etiqueta, mifont2, Brushes.Red, -tam.Width, -tam.Height);
                    }
                }
            }
            catch (Exception) { }
            g.Restore(st);

        }

        void DrawRumboHold(Graphics g, Point plane, float rumbo)
        {
            int max = Math.Max(this.Width, this.Height);
            GraphicsState save = g.Save();
            g.TranslateTransform(plane.X, plane.Y);
            g.RotateTransform(rumbo);
            g.DrawLine(Pens.Yellow, 0, -max, 0, max);
            g.Restore(save);

        }

        void PintarHistorico(Graphics g)
        {
            int x1, y1, x2, y2;
            float a1, a2;
            WayPoint w1, w2;

            if (historia.Count > 0)
            {
                rutaKM = 0.0f;

                w1 = historia[0];
                x1 = (int)map.getdX(historia[0]) + Width / 2;
                y1 = (int)map.getdY(historia[0]) + Height / 2; ;
                a1 = historia[0].Altitude;

                Color c = Color.FromArgb(128, 128, 0);
                for (int i = 0; i < historia.Count; i++)
                {
                    if (i != 0)
                    {
                        x2 = (int)map.getdX(historia[i]) + Width / 2;
                        y2 = (int)map.getdY(historia[i]) + Height / 2;
                        a2 = historia[i].Altitude;
                        w2 = historia[i];

                        float dist = w1.getDistance(w2);
                        int diff = (int)((a2 - a1)/dist*2 + 128);
                        if (diff > 255)
                            diff = 255;
                        else if (diff < 0)
                            diff = 0;

                        c = Color.FromArgb(255- diff, diff, 0);
                        
                        if (x1 != x2 || y1 != y2)
                            g.DrawLine(new Pen(c), x1, y1, x2, y2);
                        x1 = x2;
                        y1 = y2;
                        a1 = a2;
                        w1 = w2;
                    }

                    
                   // g.FillEllipse(new SolidBrush(c), x1 - 2, y1 - 2, 4, 4);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Bitmap bmp = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bmp);
                Bitmap home_icon = global::UAVConsole.Properties.Resources.ICO_dossier_home_ico_64x64;
                Point casa, avion, destino;

                
                home_icon.MakeTransparent();

                map.OnPaint(g, Width, Height);

                PintarHistorico(g);


                if (!double.IsNaN(home.Latitude) && !double.IsNaN(home.Longitude))
                {
                    casa = new Point();
                    casa.X = (int)map.getdX(home) + Width / 2;
                    casa.Y = (int)map.getdY(home) + Height / 2;
                }
                else
                    casa = Point.Empty;

                if (plane != null && !double.IsNaN(plane.Latitude) && !double.IsNaN(plane.Longitude) &&
                    (plane.Latitude != 0 || plane.Longitude != 0))
                {
                    avion = new Point();
                    avion.X = (int)map.getdX(plane) + Width / 2;
                    avion.Y = (int)map.getdY(plane) + Height / 2;
                }
                else
                    avion = Point.Empty;

                if (target != null && !double.IsNaN(target.Latitude) && !double.IsNaN(target.Longitude) &&
                    (target.Latitude != 0 || target.Longitude != 0))
                {
                    destino = new Point();
                    destino.X = (int)map.getdX(target) + Width / 2;
                    destino.Y = (int)map.getdY(target) + Height / 2;
                }
                else
                    destino = Point.Empty;

                PintarOtherPlanes(g);
                PintarRuta(g);
                PintarSoftWpt(g);

                try
                {
                    if (!casa.IsEmpty)
                        g.DrawImage(home_icon, casa.X - 32 / 2, casa.Y - 32 / 2, 32, 32);
                    
                }
                catch (Exception) { }

                if (!avion.IsEmpty)
                {
                    DrawHome(g, avion, casa);
                    if (!float.IsNaN(rumboHold))
                        DrawRumboHold(g, avion, rumboHold);
                    else
                        DrawTarget(g, avion,destino);
                }
                else if (!destino.IsEmpty)
                {
                    DrawTarget(g, Point.Empty, destino);
                }

                if (!avion.IsEmpty)
                {
                    Font mifont = new Font(Font.SystemFontName, 12.0f);

                    String cad1;
                    if(esta_dentro)
                        cad1= "Cursor Lon: " + map.GetLongitude(x_curr - Width / 2).ToString("0.000000") + " Lat: " + map.GetLatitude(y_curr - Height / 2).ToString("0.000000")+" ";
                    else
                        cad1 = "Plane Lon: " + plane.Longitude.ToString("0.000000") + " Lat: " + plane.Latitude.ToString("0.000000") + " ";
                    
                    String cad2 = "Dist: " + home.getDistance(plane).ToString("0.000") + "Km.";

                    SizeF size1 = g.MeasureString(cad1, mifont);
                    SizeF size2 = g.MeasureString(cad2, mifont);

                    if (size1.Width + size2.Width > this.Width)
                    {

                        g.FillRectangle(Brushes.Yellow, 0, bmp.Height - 2*mifont.Height, Math.Max(size1.Width,size2.Width),2* mifont.Height);
                        g.DrawString(cad1, mifont, Brushes.Red, 0.0f, bmp.Height - mifont.Height);
                        g.DrawString(cad2, mifont, Brushes.Red, 0.0f, bmp.Height - 2*mifont.Height);
                    
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Yellow, 0, bmp.Height - mifont.Height, size1.Width, mifont.Height);
                        g.DrawString(cad1, mifont, Brushes.Red, 0.0f, bmp.Height - mifont.Height);
                        g.FillRectangle(Brushes.Yellow, this.Width - size2.Width, bmp.Height - mifont.Height, size2.Width, mifont.Height);
                        g.DrawString(cad2, mifont, Brushes.Red, this.Width-size2.Width, bmp.Height - mifont.Height);
                    }
                }
                else if (esta_dentro)
                {
                    Font mifont = new Font(Font.SystemFontName, 12.0f);
                    String cad1 = "Cursor Lon: " + map.GetLongitude(x_curr - Width / 2).ToString("0.000000") + " Lat: " + map.GetLatitude(y_curr - Height / 2).ToString("0.000000") + " ";
                    SizeF size1 = g.MeasureString(cad1, mifont);
                    g.FillRectangle(Brushes.Yellow, 0, bmp.Height - mifont.Height, size1.Width, mifont.Height);
                    g.DrawString(cad1, mifont, Brushes.Red, 0.0f, bmp.Height - mifont.Height);
                        
                }

                if (ruta.Count > 0 || mensaje !=null)
                {
                    Font mifont = new Font(Font.SystemFontName, 12.0f);
                    String cad3 = "";
                    if (mensaje != null)
                        cad3 += mensaje;
                    if (ruta.Count > 0)
                    {
                        if (mensaje != null)
                            cad3 += " ";
                        cad3 += "Ruta: " + rutaKM.ToString("0.000") + "Km.";
                    }
                    SizeF size3 = g.MeasureString(cad3, mifont);
                    g.FillRectangle(Brushes.Yellow, this.Width - size3.Width, this.Height - size3.Height, size3.Width, size3.Height);
                    g.DrawString(cad3, mifont, Brushes.Red, this.Width - size3.Width, this.Height - size3.Height);
      
                }
                
                g.Dispose();
                e.Graphics.DrawImage(bmp, 0, 0);
            }
            catch (Exception) { }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Don't allow the background to paint
        }
       
        protected override void OnMouseClick(MouseEventArgs e)
        {
            double lon = map.GetLongitude(e.X - this.Width / 2);
            double lat = map.GetLatitude(e.Y - this.Height / 2);
            if(listeners!=null)
                listeners.Invoke(new WayPoint("", lon, lat), e.Button);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right||true)//_setRuta==false)
            {
                isdown = true;
                x_down = e.X;
                y_down = e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || true)//_setRuta == false)
                isdown = false;
        }

    
    
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isdown && Math.Abs(x_down - e.X) + Math.Abs(y_down - e.Y) > 10)
            {
                int x = e.X;
                int y = e.Y;
                map.setX(map.getX() + x_down - x);
                map.setY(map.getY() + y_down - y);
                x_down = x;
                y_down = y;
            }
            else
            {
                double lon = map.GetLongitude(e.X - this.Width / 2);
                double lat = map.GetLatitude(e.Y - this.Height / 2);
                if (listeners != null)
                    listeners.Invoke(new WayPoint("", lon, lat), MouseButtons.None);
            }
            x_curr = e.X;
            y_curr = e.Y;
            
            this.Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left))
                map.SetZoom(map.GetZoom() + 1);
            else if (e.Button.Equals(MouseButtons.Right))
                map.SetZoom(map.GetZoom() - 1);
            this.Invalidate();
        }

        private void IkarusMapControl_MouseLeave(object sender, EventArgs e)
        {
            esta_dentro = false;
            Invalidate();
        }

        private void IkarusMapControl_MouseEnter(object sender, EventArgs e)
        {
            esta_dentro = true;
        }    
    }
}
