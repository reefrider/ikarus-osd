/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of FPVOSD.
 *
 *  FPVOSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FPVOSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with FPVOSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.GoogleMaps
{
    public class MicrosoftProxy : ImageProxy
    {
        public override Modes[] getModes()
        {
            return new Modes[] { Modes.MAP, Modes.SAT, Modes.MAP_SAT };
        }

        protected override string getDiskFilePath(int x, int y, int zoom, Modes mode)
        {
            String filePath = rootPath + "\\Microsoft\\";
            switch (mode)
            {
                case Modes.MAP:
                    filePath += "Map";
                    break;
                case Modes.SAT:
                    filePath += "Sat";
                    break;
                case Modes.MAP_SAT:
                    filePath += "MapSat";
                    break;
            }
            filePath += "\\" + zoom + "\\";
            return filePath;
        }

        protected override string getInternetURL(int x, int y, int zoom, Modes mode, int server_balance)
        {
            string url;
            int max = 1 << zoom;

            switch (mode)
            {
                case Modes.MAP: // Microsoft MAP http://r2.ortho.tiles.virtualearth.net/tiles/r033110?g=81&shading=hill
                    url = "http://r"+server_balance+".ortho.tiles.virtualearth.net/tiles/r";
                    url += Pos2StrMicrosoft(x, y, zoom) + ".png?g=81&shading=hill";
                    break;
                case Modes.SAT: // Microsoft SAT http://a2.ortho.tiles.virtualearth.net/tiles/a0.jpeg?g=81
                    url = "http://a" + server_balance + ".ortho.tiles.virtualearth.net/tiles/a";
                    url += Pos2StrMicrosoft(x, y, zoom) + ".jpeg?g=81";
                    break;
                case Modes.MAP_SAT: // Microsoft MAP+OVL! http://h2.ortho.tiles.virtualearth.net/tiles/h0.jpeg?g=81
                    url = "http://h" + server_balance + ".ortho.tiles.virtualearth.net/tiles/h";
                    url += Pos2StrMicrosoft(x, y, zoom) + ".jpeg?g=81";
                    break;
                default:
                    url = "";
                    break;
            }


            return url;

        }


        public static String Pos2StrMicrosoft(int x, int y, int zoom)
        {
            String tmp = "";
            int i, peso = (int)Math.Pow(2.0, zoom - 1);

            for (i = 0; i < zoom; i++)
            {
                if ((x & peso) != 0)
                    if ((y & peso) != 0)
                        tmp = tmp + '3';
                    else
                        tmp = tmp + '1';
                else 
                    if ((y & peso) != 0)
                    tmp = tmp + '2';
                else
                    tmp = tmp + '0';
                peso = peso / 2;
            }
            return tmp;
        }
    }
}
