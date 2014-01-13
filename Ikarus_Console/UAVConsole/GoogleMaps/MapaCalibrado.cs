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
using System.IO;

namespace UAVConsole.GoogleMaps
{
    public class MapaCalibrado
    {
        struct Calibracion
        {
            public float lon, lat;
            public float x, y;
        };

        Image _img;
        string img_filename;
        int img_with, img_heigh;
        Calibracion[] puntos;
        PuntoF TopLeft, TopRight, BottonLeft, BottonRight;
        
        public Image img
        {
            get
            {
                if (_img == null)
                {
                    this._img = Image.FromFile(this.img_filename);
                    this.img_with = _img.Width;
                    this.img_heigh = _img.Height;
                }
               return _img;
            }
        }
        
        public MapaCalibrado(string filename)
        {
            bool res = this.LoadOziexplorer(filename);
//            if (!res)
//                this.LoadCompeGPS(filename);
        }

        public PuntoF CentroLonLat()
        {
            return InterpolaXY(img_with / 2, img_heigh / 2);
        }
        
        PuntoF Buscar(bool x, bool y)
        {
            for (int i = 0; i < puntos.Length; i++)
            {
                if (((puntos[i].x != 0) == x) && ((puntos[i].y != 0) == y))
                    return new PuntoF(puntos[i].lon, puntos[i].lat);
            }
            return null;
        }
        
        public bool LoadOziexplorer(string Filename)
        {
            string str;
            StreamReader sr = new StreamReader(Filename);
            str = sr.ReadLine();
            if (str.Contains("OziExplorer"))
            {
                str = sr.ReadLine();
                this.img_filename = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                this.img_filename += str;
                this.img_heigh = 0;
                this.img_with = 0;
             
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    string[] var = str.Split(',');
                    if (var[0] == "MMPNUM")
                    {
                        int i = int.Parse(var[1]);
                        puntos = new Calibracion[4];
                    }
                    else if (var[0] == "MMPXY")
                    {
                        int i = int.Parse(var[1]) - 1;
                        int mi_x = int.Parse(var[2]);
                        int mi_y = int.Parse(var[3]);
                        puntos[i].x = mi_x;
                        puntos[i].y = mi_y;
                        if (puntos[i].x > img_with)
                            img_with = mi_x;
                        if (puntos[i].y > img_heigh)
                            img_heigh = mi_y;
                    }
                    else if (var[0] == "MMPLL")
                    {
                        int i = int.Parse(var[1]) - 1;
                        puntos[i].lon = float.Parse(var[2], System.Globalization.CultureInfo.InvariantCulture);
                        puntos[i].lat = float.Parse(var[3], System.Globalization.CultureInfo.InvariantCulture);
                    }

                }

                TopLeft = Buscar(false, false);
                TopRight = Buscar(true, false);
                BottonLeft = Buscar(false, true);
                BottonRight = Buscar(true, true);
                return true;
            }
            return false;
        }


        public bool LoadCompeGPS(string Filename)
        {
            string section;
            string str;
            puntos = new Calibracion[4];

            StreamReader sr = new StreamReader(Filename);
            str = sr.ReadLine();
            if (str.Contains("CompeGPS"))
            {
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    if (str.StartsWith("<") && str.EndsWith(">"))
                    {
                        int start = str.IndexOf("<") + 1;
                        int end = str.IndexOf(">");
                        section = str.Substring(start, end - start);
                        str = sr.ReadLine();
                        while (!sr.EndOfStream && str != "</" + section + ">")
                        {
                            string var_name;
                            string var_value;

                            var_name = str.Substring(0, str.IndexOf('='));
                            var_value = str.Substring(str.IndexOf('=') + 1);

                            if (section == "Map")
                            {
                                switch (var_name)
                                {
                                    case "Bitmap":
                                        this.img_filename = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                        this.img_filename += var_value;
                                        this._img = Image.FromFile(this.img_filename);
                                        break;
                                    case "BitmapWidth":
                                        this.img_with = int.Parse(var_value);
                                        break;

                                    case "BitmapHeight":
                                        this.img_heigh = int.Parse(var_value);
                                        break;
                                }
                            }
                            else if (section == "Calibration")
                            {
                                if (var_name.StartsWith("P"))
                                {
                                    int i = var_name[1] - '0';
                                    string[] var = var_value.Split(',');
                                    puntos[i].x = float.Parse(var[0], System.Globalization.CultureInfo.InvariantCulture);
                                    puntos[i].y = float.Parse(var[1], System.Globalization.CultureInfo.InvariantCulture);
                                    puntos[i].lon = float.Parse(var[3], System.Globalization.CultureInfo.InvariantCulture);
                                    puntos[i].lat = float.Parse(var[4], System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }

                            str = sr.ReadLine();
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public PuntoF InterpolaXY(float x, float y)
        {
            PuntoF punto1 = TopLeft * (1.0f - x / img_with) + TopRight * (x / img_with);
            PuntoF punto2 = BottonLeft * (1.0f - x / img_with) + BottonRight * (x / img_with);
            PuntoF punto = punto1 * (1.0f - y / img_heigh) + punto2 * (y / img_heigh);
            return punto;
        }

        public PuntoF InterpolaLonLat(float lon, float lat)
        {   /*
            if ((lon >= Math.Max(TopLeft.x, BottonLeft.x)) &&
                (lon <= Math.Min(TopRight.x, BottonRight.x)) &&
                (lat <= Math.Min(TopLeft.y, TopRight.y)) &&
                (lat >= Math.Max(BottonLeft.y, BottonRight.y))||true)
            */
            float y1 = img_heigh - (lat - BottonLeft.y) * img_heigh / (TopLeft.y - BottonLeft.y);
            float y2 = img_heigh - (lat - BottonRight.y) * img_heigh / (TopRight.y - BottonRight.y);

            float lon1 = InterpolaXY(0, y1).x;
            float lon2 = InterpolaXY(img_heigh, y2).x;

            float x = (lon - lon1) * img_with / (lon2 - lon1);
            float y = y1 + (y2 - y1) / img_with * x;

            return new PuntoF(x, y);
        }


    }
}
