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
using System.Drawing;
using System.Net;
using System.IO;

namespace UAVConsole.GoogleMaps
{
    public class GoogleProxy : ImageProxy
    {
        String myCookieStr;

        public GoogleProxy(): base()
        {
            myCookieStr = "PREF=ID=e6255332d3c67bd1:TM=1202171733:LM=1202171733:S=v5hI_WYwE8-CQSp4";
        }

        public GoogleProxy(String cookieStr):base()
        {
            myCookieStr = cookieStr;
        }

        public override Modes[] getModes()
        {
            return new Modes[] { Modes.MAP, Modes.SAT, Modes.TOPO, Modes.MAP_OVERLAY };
        }

        protected override HttpWebResponse getHttpWebResponse(string url)
        {
            WebRequest request = WebRequest.Create(url);
            
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)request;
            myHttpWebRequest.Accept = "*/*";
            myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648)";
            //myHttpWebRequest.IfModifiedSince = "Fri, 17 Dec 2004 04:58:08 GMT";
            myHttpWebRequest.KeepAlive = true;
            request.Headers.Add("Accept-Language: es\r\n");
            request.Headers.Add("UA-CPU: x86\r\n");
            request.Headers.Add("Accept-Encoding: gzip, deflate\r\n");
            //request.Headers.Add("Host: khm1.google.com\r\n");
            request.Headers.Add("Cookie: "+myCookieStr+"\r\n");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        protected override string getDiskFilePath(int x, int y, int zoom, Modes mode)
        {
            String filePath = rootPath + "\\GoogleMaps\\";
            switch (mode)
            {
                case Modes.MAP:
                    filePath += "Map";
                    break;
                case Modes.SAT:
                    filePath += "Sat";
                    break;
                case Modes.TOPO:
                    filePath += "Phy";
                    break;
                case Modes.MAP_OVERLAY:
                    filePath += "Ovl";
                    break;
            }
            filePath += "\\" + zoom + "\\";
            return filePath;
        }

        protected override string getInternetURL(int x, int y, int zoom, Modes mode, int server_balance)
        {
            string url;
            int max = 1 << zoom;
            
            switch (mode) {
                case Modes.MAP: // Google MAP http://mt1.google.com/vt/lyrs=m@133&hl=es&x=31&y=24&z=6&s=Galil 
                    url = "http://mt"+server_balance+".google.com/vt";
                    url += "/lyrs=m@152000000&hl=es&x=" + x + "&y=" + y + "&z=" + zoom;
                    break;

                case Modes.SAT: // Google SAT http://khm1.google.es/kh/v=102&x=31&y=23&z=6&s=Gali 
                                            
                    url = "http://khm"+server_balance+".google.com/kh";
                    url += "/v=102&x=" + x + "&y=" + y + "&z=" + zoom;
                    break;

                case Modes.MAP_OVERLAY: // Google MAP OVL http://mt1.google.com/vt/lyrs=h@133&hl=es&x=31&y=24&z=6&s=Galil
                    url = "http://mt"+server_balance+".google.com/vt";
                    url += "/lyrs=h@152000000&hl=es&x=" + x + "&y=" + y + "&z=" + zoom;
                    break;

                case Modes.TOPO: // Google Terrain http://mt1.google.com/vt/lyrs=t@128,r@169000000&hl=es&x=31&y=21&z=6&s=Galileo
                    url = "http://mt"+server_balance+".google.com/vt";
                    url += "/lyrs=t@128,r@169000000&hl=es&x=" + x + "&y=" + y + "&z=" + zoom + "&s=Galileo";
                    break;
                default:
                    url = "";
                    break;
            }

            return url;
           
        }

        private static String Pos2StrGoogle(int x, int y, int zoom)
        {
            String tmp = "t";
            int i, peso = (int)Math.Pow(2.0, zoom - 1);

            for (i = 0; i < zoom; i++)
            {
                if ((x & peso) != 0)
                    if ((y & peso) != 0)
                        tmp = tmp + 's';
                    else
                        tmp = tmp + 'r';
                else if ((y & peso) != 0)
                    tmp = tmp + 't';
                else
                    tmp = tmp + 'q';
                peso = peso / 2;
            }
            return tmp;
        }
    }

}
