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
using UAVConsole.GoogleMaps;
using System.Threading;
using UAVConsole.IkarusScreens;
using System.IO.Ports;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;
using UAVConsole.ConfigClasses;

namespace UAVConsole.USBXpress
{
 
    class FlightPlanUSB : UsbLayerATC
    {

        public enum Comandos{IkarusConfig, AutoPilotConfig, Ruta, Screen, gpsinfo, Ikarusinfo, MAX7456, ADC_VALUES, SERVOS, SERVOS_RAW};
        public enum ADC_VALUES { V2, I, V1, RSSI, temp, CO_X, CO_Y, CO_Z };

        public FlightPlanUSB()
            : base("9000")
        {

        }

        override public void Flush()
        {
            byte[] buffer = new byte[64];

            int nbytes = 0;

            ClearInputBuffer(handle);
            buffer[0] = (byte)'A';
            buffer[1] = (byte)'T';
            buffer[2] = (byte)'C';
            buffer[3] = (byte)0xff;
            for (int i = 4; i < 64; i++)
                buffer[i] = 0;

            usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
            WaitACK_USB();
        }

        public void RemoteIkarus(byte cmd, int id)
        {
            RemoteIkarus(new byte[2] { cmd, (byte)id });
        }

        public void FirmwareUpdate()
        {
            RemoteIkarus(new byte[1] { 6 });
        }

        public void ClearScrIkarus()
        {
            RemoteIkarus(new byte[1] { 2 });
        }

        public void PrintAtIkarus(int fila, int col, string text)
        {
            byte[] buff = new byte[text.Length + 3];
            buff[0] = 3;
            buff[1] = (byte)fila;
            buff[2] = (byte)col;
            for (int i = 0; i < text.Length; i++)
                buff[i + 3] = (byte)text[i];
            RemoteIkarus(buff);
        }

        public void RemoteIkarus(byte[] buff)
        {
            byte[] buffer = new byte[64];
            int i;
            int nbytes = 0;

            ClearInputBuffer(handle);
            buffer[0] = (byte)'A';
            buffer[1] = (byte)'T';
            buffer[2] = (byte)'C';
            buffer[3] = (byte)0xfe;
            
            for ( i = 4; i < buffer.Length && i<buff.Length+4; i++)
                buffer[i] = buff[i-4];
            for(;i<buffer.Length;i++)
                buffer[i]=0;

            usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
            WaitACK_USB();
        }

        public float[] ReadADCs()
        {
            byte[] buffer = Read(Comandos.ADC_VALUES, 0, 0, 8 * 4);
            float []adcs=new float[buffer.Length/4];
            int i=0;
            for (int j = 0; j < adcs.Length; j++)
            {
                adcs[j] = USBXpress.tofloat(buffer, ref i);
            }
            return adcs;
        }

        public float ReadADCs(ADC_VALUES id)
        {
            float[] values = ReadADCs();
            return values[(int)id];
        }

        public int[] ReadServos()
        {
            byte[] buffer = Read(Comandos.SERVOS, 0, 0, 7 * 2);
            int[] servos = new int[buffer.Length / 2];
            int i = 0;
            for (int j = 0; j < servos.Length; j++)
            {
                servos[j] = USBXpress.toint16(buffer, ref i);
            }
            return servos;
        }

        public int[] ReadServosRAW()
        {
            byte[] buffer = Read(Comandos.SERVOS_RAW, 0, 0, 12 * 2);
            int[] servos = new int[buffer.Length / 2];
            int i = 0;
            for (int j = 0; j < servos.Length; j++)
            {
                servos[j] = USBXpress.toint16(buffer, ref i);
            }
            return servos;
        }

        public IkarusBasicConfig ReadConfig()
        {
            IkarusBasicConfig cfg = new IkarusBasicConfig();
            byte[] buffer= Read(Comandos.IkarusConfig, 0, 0, cfg.size_bytes());
            cfg.FromByteArray(buffer);
            return cfg;
        }

        public void Write(Comandos cmd, int id, int offset, byte[] buff)
        {
            Write((int)cmd, id, offset, buff);
        }
        public byte[] Read(Comandos cmd, byte id, int offset, int len)
        {
            return Read((int)cmd, id, offset, len);
        }

        public void WriteConfig(IkarusBasicConfig cfg)
        {
            byte[] buffer = cfg.ToByteArray();
            
            Write(Comandos.IkarusConfig, 0, 0, buffer);
            Flush();
            RemoteIkarus(1, 0);
         }
        
        public int ReadMaxWpts()
        {
            byte[] buffer = Read(Comandos.Ruta, 0xff, 0, 2);
            int i = 0;
            int numwpts = USBXpress.toint16(buffer, ref i);
            return numwpts;
        }

        public WayPoint ReadWpt(int id)
        {
            WayPoint wpt = null;
            wpt = new WayPoint();
            int i = 0;

            byte[] buffer = Read(Comandos.Ruta, (byte)id, 0, 32);
            wpt.name = USBXpress.tostring(buffer, ref i, 20);
            wpt.Longitude = USBXpress.tofloat(buffer, ref i);
            wpt.Latitude = USBXpress.tofloat(buffer, ref i);
            wpt.Altitude = USBXpress.tofloat(buffer, ref i);

            return wpt;
        }

        public void WriteMaxWpt(int num)
        {
            byte[] buffer = new byte[2];
            int i=0;
            USBXpress.toarray(buffer, ref i, (Int16) num);
            Write(Comandos.Ruta, 0xff, 0, buffer);
            ClearScrIkarus();
            PrintAtIkarus(3, 3, string.Format("Escritos {0} Wpts",num));
     
        }

        public void WriteWpt(int id, string Name, float lon, float lat, float alt)
        {
            byte[] buffer = new byte[32];
            int i;
            
            for(i=0;i<20&&i<Name.Length;i++)
                buffer[i] = (byte)Name[i];
            for (; i < 20; i++)
                buffer[i] = 0;

            USBXpress.toarray(buffer, ref i, lon);
            USBXpress.toarray(buffer, ref i, lat);
            USBXpress.toarray(buffer, ref i, alt);
            Write(Comandos.Ruta, (byte)id, 0, buffer);
        }

        public WayPoint ReadGPS()
        {
            IkarusGPSInfo gps = new IkarusGPSInfo();
            byte[] tmp = Read(Comandos.gpsinfo, 0, 0, gps.size_bytes());
            gps.FromByteArray(tmp);

            WayPoint wpt=new WayPoint("GPS");

            wpt.Altitude = gps.alt;
            wpt.Longitude = gps.lon;
            wpt.Latitude = gps.lat;

            
            if (gps.fix != 0) //gpsinfo.valid
                return wpt;
            else
                return null;
        }

        public byte [] ReadCharSet(int id)
        {
            byte[] buffer = Read(Comandos.MAX7456,(byte) id, 0, 64);
            return buffer;
        }

        public void WriteCharSet(int id, byte []character)
        {
            byte[] buffer = new byte[54];
            int i;

            for (i=0; i < buffer.Length; i++)
                buffer[i] = character[i];
            Write(Comandos.MAX7456, id, 0, buffer);        
        }

        public IkarusAutopilotConfig ReadConfigAutopilot()
        {
            IkarusAutopilotConfig cfg = new IkarusAutopilotConfig();
          
            byte[] buffer = Read(Comandos.AutoPilotConfig, 0, 0, cfg.size_bytes());
            cfg.FromByteArray(buffer);

            return cfg;
        }

        public void WriteConfigAutopilot(IkarusAutopilotConfig cfg)
        {
            byte[] buffer = cfg.ToByteArray();
            Write(Comandos.AutoPilotConfig, 0, 0, buffer);
            Flush();
        }

        public IkarusScreenConfig ReadScreen(int id)
        {
            IkarusScreenConfig data = new IkarusScreenConfig();
            byte[] buffer = Read(Comandos.Screen,(byte)id,0, data.size_bytes());

            for (int i = 0; i < buffer.Length; i++)
                data.setByte(i, buffer[i]);
            return data;
        }

        public void WriteScreen(int id, IkarusScreenConfig data)
        {
            byte[] buffer = new byte[data.size_bytes()];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = data.getByte(i);

            Write(Comandos.Screen, id, 0, buffer);
            Flush();
            RemoteIkarus(0,id);
        }


        public IkarusInfo ReadIkarusInfo()
        {
            IkarusInfo info = new IkarusInfo();

            byte[] buffer = Read(Comandos.Ikarusinfo, 0, 0, info.size_bytes());
            info.FromByteArray(buffer);

            return info;
        }

    }
}
