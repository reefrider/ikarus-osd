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
using System.IO.Ports;
using UAVConsole.GoogleMaps;
using UAVConsole.USBXpress;
using System.Windows.Forms;
using UAVConsole.Modem;
using UAVConsole.ConfigClasses;

namespace UAVConsole
{
    class AntTracker
    {
        Singleton singleton = Singleton.GetInstance();
        AntenaTracker antenaTracker;
        AntTrackerDatosAvion datosAvion;
        AntTrackerDebug debug;

        public AntTrackerDatosAntena datosAntena;

        PlaneState planeState;
        WayPoint home;

        byte []packet;

        bool planeStateUpdated;
        bool debugUpdated;

        System.Threading.Timer timer;

        bool terminate;
       
        //FiltroMediana filtroLat = new FiltroMediana(10);
        //FiltroMediana filtroLon = new FiltroMediana(10);
        //FiltroMediana filtroAlt = new FiltroMediana(10);


        public AntTracker()
        {
       
            planeStateUpdated = false;
            terminate = false;
            antenaTracker = new AntenaTracker();
            datosAvion = new AntTrackerDatosAvion();
            datosAvion.LoadDefaults();

            debug = new AntTrackerDebug();
            debug.LoadDefaults();

            if (antenaTracker.IsOpen())
            {
                timer = new System.Threading.Timer(TimerTask, this, 1000, 1000 /5);
            }
            else if (singleton.Idioma == 0)
            {
                MessageBox.Show("No se puede abrir dispositivo AntTracker");
            }
            else
            {
                MessageBox.Show("Cannot open AntTracker device");
            }
        }

        public void Close()
        {
            terminate = true;
        }
        WayPoint lastWpt;
        int check_counter;
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


            if (lastWpt == null)
                lastWpt = new WayPoint("", ps.Lon, ps.Lat);

            WayPoint here = new WayPoint("", ps.Lon, ps.Lat);

            if ((check_counter < 20) && (lastWpt.getDistance(here) > 1))
            {
                check_counter++;
                return false;
            }
            else
            {
                check_counter = 0;
                lastWpt = here;
            }

            return true;
        }

        public void Send(PlaneState planeState, WayPoint home)
        {
            if (planeState != null)
            {
                this.planeState = planeState;
                this.home = home;
                this.planeStateUpdated = true;
            }

        }

        public void Send(bool manual, float pan, float tilt)
        {
            if (manual)
            {
                debug.grados_pan = pan;
                debug.grados_tilt = tilt;
                debug.EnableDebug = (byte)0x0A;
            }
            else
            {
                debug.EnableDebug = 0;
            }
            debugUpdated = true;
        }

        public void Send(bool manual, short pan, short tilt)
        {
            if (manual)
            {
                debug.pan = pan;
                debug.tilt = tilt;
                debug.EnableDebug = (byte)0x05;
            }
            else
            {
                debug.EnableDebug = 0;
            }
            debugUpdated = true;
        }

        void tarea(AntTracker obj)
        {
            if (antenaTracker.IsOpen())
            {
                if (singleton.telemetria == Singleton.Telemetria.AntTracker)
                {
                    obj.packet = antenaTracker.ReadModem();
                }
                else if (obj.planeStateUpdated == true && CheckPlaneState(obj.planeState))
                {
                    datosAvion.lon = obj.planeState.Lon;
                    datosAvion.lat = obj.planeState.Lat;
                    datosAvion.alt = (short)obj.planeState.Alt;

                    datosAvion.home_lon = (float)obj.home.Longitude;
                    datosAvion.home_lat = (float)obj.home.Latitude;
                    datosAvion.home_alt = (short)obj.home.Altitude;
                    if (antenaTracker.WriteDatosAvion(datosAvion) != USBXpress.USBXpress.ReturnCodes.SI_SUCCESS)
                    {
                        antenaTracker.Close();
                    }
                    else
                        datosAntena = antenaTracker.ReadDatosAntena();
                   
                }

                if (debugUpdated)
                {
                    antenaTracker.WriteDebugInfo(debug);
                    debugUpdated = false;
                }
            }
            else
            {
                antenaTracker = new AntenaTracker();
            }

        }

        int flag = 0;
        static void TimerTask(Object obj)
        {
            AntTracker ant = (AntTracker)obj;
            if (ant.flag == 0)
            {
                ant.flag = 1;
                ant.tarea(ant);
                if (ant.terminate)
                {
                    ant.timer.Dispose();
                    ant.antenaTracker.Close();
                }
                ant.flag = 0;
            }
        }

        public byte[] ReadModem()
        {
            return packet;
        }

    }
}
