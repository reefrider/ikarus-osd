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
using System.Globalization;
using Microsoft.Win32;
using UAVConsole.GoogleMaps;

namespace UAVConsole.Modem
{
    public abstract class ModemAbstract
    {
        protected const int DATALEN = 29;
        protected byte[] packet = new byte[DATALEN];
        protected PlaneState planeState = null;

        public delegate void Listener();
        public Listener listeners;

        public int tramas_rx=0;
        public int tramas_ok=0;

        StreamWriter swlog_kml;
        StreamWriter swlog_txt;

        Singleton singleton = Singleton.GetInstance();

        WayPoint lastWpt;

        public ModemAbstract()
        {
            string path = singleton.FlightLogPath;
            if (path != null)
            {
                DateTime dt = DateTime.Now;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filename = path + "\\Flight Log "
                    + dt.Year.ToString("00") + dt.Month.ToString("00") + dt.Day.ToString("00")
                    + "-" + dt.Hour.ToString("00") + dt.Minute.ToString("00") ;

                swlog_txt = File.CreateText(filename + ".txt");

                swlog_kml = File.CreateText(filename + ".kml");

                swlog_kml.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                swlog_kml.WriteLine("<kml xmlns=\"http://earth.google.com/kml/2.1\"");
                swlog_kml.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                swlog_kml.WriteLine("<Folder>");
                swlog_kml.WriteLine("<name>IKARUS OSD</name>");
                swlog_kml.WriteLine("    <Placemark>");
                swlog_kml.WriteLine("<name>"+filename+"</name>");
                swlog_kml.WriteLine("<visibility>1</visibility>");
                swlog_kml.WriteLine("<open>0</open>");
                swlog_kml.WriteLine("<Style>");
                swlog_kml.WriteLine(" <LineStyle>");
                swlog_kml.WriteLine(" <color>ff00ffff</color>");
                swlog_kml.WriteLine("</LineStyle>");
                swlog_kml.WriteLine("<PolyStyle>");
                swlog_kml.WriteLine(" <color>7f00ff00</color>");
                swlog_kml.WriteLine("</PolyStyle>");
                swlog_kml.WriteLine("</Style>");
                swlog_kml.WriteLine("     <LineString>");
                swlog_kml.WriteLine("<extrude>1</extrude>");
                swlog_kml.WriteLine("<altitudeMode>absolute</altitudeMode>");
                swlog_kml.WriteLine("  <tessellate>1</tessellate>");
                swlog_kml.WriteLine("       <coordinates>");

                RegistryKey key = Registry.CurrentUser.OpenSubKey(Singleton.registryPath, true);
                key.SetValue("LastKML", filename + ".kml");
                key.Close();        
            }
        }

        virtual public PlaneState getTelemetry()
        {
            return planeState;
        }

        bool CheckPlaneState(PlaneState ps)
        {
            if (float.IsNaN(ps.Lat) || float.IsNaN(ps.Lon) || float.IsNaN(ps.homeLat) || float.IsNaN(ps.homeLon))
                return false;

            if (ps.Lat > 90.0f || ps.Lat < -90.0f)
                return false;
            if (ps.Lon > 180.0f || ps.Lon < -180.0f)
                return false;

            if (ps.homeLat > 90.0f || ps.homeLat < -90.0f)
                return false;
            if (ps.homeLon > 180.0f || ps.homeLon < -180.0f)
                return false;

            if (ps.Lat < 1 && ps.Lat > -1 && ps.Lon < 1 && ps.Lon > -1)
                return false;

            return true;
        }

        protected bool ParsePacket(byte[] packet)
        {
            tramas_rx++;
            if (packet != null&&packet[0]==DATALEN)
            {
                byte crc = CRC.CCITT8(packet, packet[0] - 1);
                byte t = packet[packet[0] - 1];

                if (crc == t && crc !=255 && crc !=0)
                {
                    tramas_ok++;
                    int i = 1;
                    if(planeState==null)
                        planeState=new PlaneState();

                    planeState.Lon = USBXpress.USBXpress.tofloat(packet, ref i);
                    planeState.Lat = USBXpress.USBXpress.tofloat(packet, ref i);
                    planeState.Alt = USBXpress.USBXpress.toint16(packet, ref i);
                    planeState.Rumbo = USBXpress.USBXpress.tochar(packet, ref i) * 360.0f / 256.0f;
                    planeState.Knots = USBXpress.USBXpress.tobyte(packet, ref i);
                    planeState.vertSpeed = USBXpress.USBXpress.tochar(packet, ref i);
                    planeState.WptIndex = USBXpress.USBXpress.tochar(packet, ref i);
                    planeState.homeLon = USBXpress.USBXpress.tofloat(packet, ref i);
                    planeState.homeLat = USBXpress.USBXpress.tofloat(packet, ref i);
                    planeState.v1 = USBXpress.USBXpress.tobyte(packet, ref i) / 10.0f;
                    planeState.v2 = USBXpress.USBXpress.tobyte(packet, ref i) / 10.0f;
                    planeState.pitch = USBXpress.USBXpress.tochar(packet, ref i) * 180 / 127.0f;
                    planeState.roll = USBXpress.USBXpress.tochar(packet, ref i) * 180 / 127.0f;
                    planeState.RSSI = USBXpress.USBXpress.tobyte(packet, ref i);

                    if(CheckPlaneState(planeState))
                    {
                        planeState.lastrx = true;
                        Log();
                        return true;
                    }
                    else
                    {
                        planeState.lastrx = false;
                        planeState = null;
                        return false;
                    }
                }
            }
            if (planeState != null)
                planeState.lastrx = false;
            return false;
        }

        void Log()
        {
            if (swlog_kml != null)
            {
                swlog_kml.WriteLine(planeState.Lon.ToString(CultureInfo.InvariantCulture) + "," + 
                    planeState.Lat.ToString(CultureInfo.InvariantCulture) + "," + 
                    planeState.Alt.ToString(CultureInfo.InvariantCulture));
                swlog_kml.Flush();
            }

            if (swlog_txt != null)
            {
                float altura, velocidad;
                DateTime dt = DateTime.Now;
                
                if(singleton.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                {
                    velocidad = planeState.Knots * 1.852f;
                    altura = planeState.Alt;
                }
                else
                {
                    velocidad = planeState.Knots;
                    altura = planeState.Alt*3.280f;
                }

                swlog_txt.WriteLine(dt.TimeOfDay.ToString() + "," +
                    planeState.Lon.ToString(CultureInfo.InvariantCulture) + "," +
                    planeState.Lat.ToString(CultureInfo.InvariantCulture) + "," +
                    altura.ToString(CultureInfo.InvariantCulture) + "," +
                    planeState.Rumbo.ToString(CultureInfo.InvariantCulture) + "," +
                    velocidad.ToString(CultureInfo.InvariantCulture)+","+
                    planeState.RSSI.ToString(CultureInfo.InvariantCulture)+","+
                    planeState.pitch.ToString(CultureInfo.InvariantCulture)+","+
                    planeState.roll.ToString(CultureInfo.InvariantCulture)
                    );
                swlog_txt.Flush();
            }
        }

        virtual public void dispose()
        {
            if (swlog_kml != null)
            {
                swlog_kml.WriteLine("</coordinates>");
                swlog_kml.WriteLine("         </LineString>");
                swlog_kml.WriteLine("       </Placemark>");
                swlog_kml.WriteLine("   </Folder>");
                swlog_kml.WriteLine("</kml>");
                swlog_kml.Close();
                swlog_kml = null;
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Singleton.registryPath, true);
                key.DeleteValue("LastKML");
                key.Close();

            }

            if (swlog_txt != null)
            {
                swlog_txt.Close();
                swlog_txt = null;
            }
        }
        
        /*
        public abstract void setElevator(float elevator);
        */
        

    }
}
