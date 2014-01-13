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
using System.IO;

namespace UAVConsole.GoogleMaps
{
    class Ruta
    {
        const float WptRange = 1.5f;
        private List<WayPoint> ruta;
        private int currWptID = 0;
        private float lastdistance = float.PositiveInfinity;

        public Ruta()
        {
            ruta = new List<WayPoint>();
        }

        public Ruta(List<WayPoint> lista)
        {
            ruta = lista;
        }

        public Ruta(string filename)
        {
            ruta = new List<WayPoint>();
            LoadFromFile(filename);
        }
        public int getWptID()
        {
            return currWptID;
        }

        public WayPoint getNextWpt(WayPoint pos)
        {
            float dist=pos.getDistance(ruta[currWptID]);
            if (dist <= lastdistance)
                lastdistance = dist;
            else if (dist > lastdistance && lastdistance < WptRange)
            {
                lastdistance = float.PositiveInfinity;
                if (currWptID < ruta.Count - 1)
                    currWptID++;
                else
                    currWptID = 0;
            }

            return ruta[currWptID];
        }

        public void LoadFromList(List<WayPoint> lista_wpt)
        {
            ruta = lista_wpt;
        }

        public void LoadFromFile(string filename)
        {
            StreamReader fin = File.OpenText(filename);
            int numwpts = 0;
            if (int.TryParse(fin.ReadLine(), out numwpts))
            {
                ruta.Clear();
                for (int i = 0; i < numwpts; i++)
                {
                    WayPoint wpt = new WayPoint();
                    wpt.name = fin.ReadLine();
                    wpt.Longitude = float.Parse(fin.ReadLine());
                    wpt.Latitude = float.Parse(fin.ReadLine());
                    wpt.Altitude = float.Parse(fin.ReadLine());
                    ruta.Add(wpt);
                }
            }
        }

        public void SaveToFile(string filename)
        {
            StreamWriter fout = File.CreateText(filename);
            fout.WriteLine(ruta.Count);
            for (int i = 0; i < ruta.Count; i++)
            {
                fout.WriteLine(ruta[i].name);
                fout.WriteLine(ruta[i].Longitude);
                fout.WriteLine(ruta[i].Latitude);
                fout.WriteLine(ruta[i].Altitude);
            }
            fout.Close();
        }
    }

}
