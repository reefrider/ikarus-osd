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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UAVConsole.GoogleMaps;
using System.Drawing;
using UAVConsole.Modem;

namespace UAVConsole
{
    public class SenderUDP
    {
        public delegate void TeamUpdate(WayPoint wpt);
        public TeamUpdate listeners;
        private bool running;

        Thread myThread;
        Singleton me = Singleton.GetInstance();
        int port;


        public SenderUDP(int port)     // SERVER!!! esta aqui solo mientras codificación
        {

            this.port = port;
            this.running = true;
            myThread = new Thread(new ParameterizedThreadStart(tarea));
            myThread.IsBackground = true;
            myThread.Start();
        }

        public void Stop()
        {
            this.running = false;
            //myThread.Abort();
        }

        public void tarea(Object obj)
        {            
            Dictionary<string, WayPoint> team = (Dictionary<string, WayPoint>)obj;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.EnableBroadcast = true;
            sock.ReceiveTimeout = 100;  // 100ms 

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, this.port);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;
            Console.WriteLine("Ready to receive...");
            byte[] data = new byte[1024];
            while (running)
            {
                int recv;
                try
                {
                    recv = sock.ReceiveFrom(data, ref ep);
                }
                catch (Exception) { recv = 0; }
                if (recv != 0)     // paquete vacio
                {
                    string stringData = Encoding.ASCII.GetString(data, 0, recv);
                    string[] values = stringData.Split(new char[1] { ';' });
                    if (values[0] == "IKARUS")
                    {
                        try
                        {
                            WayPoint wpt = new WayPoint();
                            if (values[1].CompareTo(me.NombrePiloto) != 0)
                            {
                                wpt.name = values[1];
                                wpt.Latitude = double.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
                                wpt.Longitude = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                                wpt.Altitude = float.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture);
                                wpt.heading = float.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture);
                                if (me.planeState != null && wpt.Altitude < me.planeState.Alt)
                                    wpt.icon = UAVConsole.Properties.Resources.plane1;
                                else
                                    wpt.icon = UAVConsole.Properties.Resources.plane2;

                                wpt.icon.MakeTransparent(Color.White);
                                if (listeners != null)
                                    listeners.Invoke(wpt);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }
            sock.Close();
        }

        public static void SendInfo(string name, int port)
        {
            PlaneState plane = Singleton.GetInstance().planeState;
            try
            {
                if (plane != null)
                {
                    char chr = ';';
                    string str = "IKARUS";
                    str += chr + name;
                    str += chr + plane.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.Alt.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.Rumbo.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.pitch.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.roll.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    str += chr + plane.Knots.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    // str += chr + plane.vertSpeed.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    // str += chr + plane.homeLon.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    // str += chr + plane.homeLat.ToString(System.Globalization.CultureInfo.InvariantCulture);  

                    SendUDPbroadcast(port, str);
                }
            }
            catch (Exception) { }
        }

        public static void SendUDPbroadcast(int port, string mensaje)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);
            byte[] data = Encoding.ASCII.GetBytes(mensaje);
            sock.EnableBroadcast = true;
            sock.SendTo(data, iep);
            sock.Close();
            
        }

        public static void SendUDPmessage2(IPAddress ip, int port, string mensaje)
        {
            UdpClient sock = new UdpClient();
            IPEndPoint iep = new IPEndPoint(ip, port);
            byte[] data = Encoding.ASCII.GetBytes(mensaje);
            sock.Send(data, data.Length, iep);
            
            sock.Close();
        }

        public static void SendUDPmessage(IPAddress ip, int port, string mensaje)
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endpoint = new IPEndPoint(ip, port);

            byte []data = Encoding.ASCII.GetBytes(mensaje);
            server.SendTo(data, endpoint);
            server.Close();
            //server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }

        public static void ServerUDP2(int puerto)
        {
            UdpClient sock = new UdpClient(puerto);
            Console.WriteLine("Ready to receive...");
            //sock.EnableBroadcast = true;      //??

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = sock.Receive(ref iep);
            string stringData = Encoding.ASCII.GetString(data, 0, data.Length);
            sock.Close();
        }
        public static void ServerUDP(int puerto)
        {
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, puerto);
            
            newsock.EnableBroadcast = true;

            newsock.Bind(ipep);
            Console.WriteLine("Waiting for a client...");

            EndPoint Remote = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

            byte[] data = new byte[50];
            int recv = newsock.ReceiveFrom(data, ref Remote);
            newsock.Close();
      
        }
    }
}
