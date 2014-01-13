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
    public class ListaMapas
    {
        public List<MapaCalibrado> lista;
        

        public ListaMapas()
        {
            lista = new List<MapaCalibrado>();
        }

        public MapaCalibrado GetBest(PuntoF pos)
        {
            MapaCalibrado best = lista[0];
            float dist = (best.CentroLonLat() - pos).mod();
            foreach (MapaCalibrado mapa in lista)
            {
                float f = (mapa.CentroLonLat() - pos).mod();
                if (f < dist)
                {
                    dist = f;
                    best = mapa;
                }
            }
            return best;
        }

        public void BuscarFiles(string path, string ext, bool recursive)
        {
            if (lista == null)
                lista = new List<MapaCalibrado>();
            else if (lista.Count != 0)
                lista.Clear();
            BuscarFiles2(path, ext, recursive);
        }

        void BuscarFiles2(string path, string ext, bool recursive)
        {
            if (!path.EndsWith("\\"))
                path += "\\";
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(ext);
            foreach (FileInfo file in files)
            {
                lista.Add(new MapaCalibrado(path + file));
            }
            if (recursive)
            {
                DirectoryInfo[] subdirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in subdirs)
                {
                    BuscarFiles2(path + subdir, ext, true);
                }
            }
            //
        }
    }
}
