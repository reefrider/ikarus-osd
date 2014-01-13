using System;
using System.Collections.Generic;
using System.Text;
using UAVConsole.ConfigClasses;
using UAVConsole.GoogleMaps;

namespace UAVConsole.USBXpress
{
    enum Comandos { AntTrackerConfig, WriteAvion, ReadAntena, ReadModem, DebugInfo };
    
    class AntenaTracker : UsbLayerATC
    {
        public AntenaTracker()
            : base("9200")
        {

        }

        public AntTrackerConfig ReadConfig()
        {
            AntTrackerConfig antTrackerCfg = new AntTrackerConfig();
            byte[] buffer = Read(Comandos.AntTrackerConfig, 0, 0, antTrackerCfg.size_bytes());
            antTrackerCfg.FromByteArray(buffer);
            return antTrackerCfg;
        }

        public void WriteConfig(AntTrackerConfig antTrackerCfg)
        {
            byte[] buffer = antTrackerCfg.ToByteArray();

            Write(Comandos.AntTrackerConfig, 0, 0, buffer);
            Flush();
            //RemoteIkarus(1, 0);
        }

        public USBXpress.ReturnCodes Write(Comandos cmd, int id, int offset, byte[] buff)
        {
            return Write((int)cmd, id, offset, buff);
        }
        public byte[] Read(Comandos cmd, byte id, int offset, int len)
        {
            return Read((int)cmd, id, offset, len);
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

        public void UpdateFirmware()
        {
            WriteRAW(0xf0, new byte[] { });
        }

        public byte[] ReadModem()
        {
            if (this.IsOpen())
            {
                byte[] buffer = Read(Comandos.ReadModem, 0, 0, 29);
                return buffer;
            }
            else
                return null;

        }

        public USBXpress.ReturnCodes WriteDatosAvion(AntTrackerDatosAvion datos)
       {
           
           if (this.IsOpen() && datos !=null)
           {
               byte[] buffer = datos.ToByteArray();
               return Write(Comandos.WriteAvion, 0, 0, buffer);
           }

           return USBXpress.ReturnCodes.SI_WRITE_ERROR;
             
       }

        public USBXpress.ReturnCodes WriteDebugInfo(AntTrackerDebug debug)
       {
           if (this.IsOpen() && debug != null)
           {
               byte[] buffer = debug.ToByteArray();
               return Write(Comandos.DebugInfo, 0, 0, buffer);
           } 
           return USBXpress.ReturnCodes.SI_WRITE_ERROR;
       }
       
       public AntTrackerDatosAntena ReadDatosAntena()
       {
            
           if (this.IsOpen())
           {
               AntTrackerDatosAntena datos = new AntTrackerDatosAntena();
               byte[] buff = Read(Comandos.ReadAntena, 0, 0, datos.size_bytes());
               datos.FromByteArray(buff);
               return datos;
           }
           else
               return null;
       }
    }
}
