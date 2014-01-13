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
using System.Threading;

namespace UAVConsole.USBXpress
{
    class EmisoraUSB : UsbLayerATC
    {
        public enum Comandos { PC, IN, SEQ, SLOT };
        Singleton singleton = Singleton.GetInstance();

        public EmisoraUSB()
            : base("9100")
        {
            
        }
        public void SetAllByPass()
        {
            SetMask(new byte[] { });
        }

        public void SetMask(byte []mask)
        {
            WriteRAW(0xff, mask);
        }

        public void SetPanTiltPort(byte pan, byte tilt)     // obsoleto
        {
            WriteRAW(0xfd, new byte[] { pan, tilt });
        }

        public void SetLowBattery(float v)                  // obsoleto
        {
            byte[] buff = new byte[4];
            int i = 0;
            USBXpress.toarray(buff, ref i, v);
            WriteRAW(0xfa, buff);
        
        }

        public void SaveToFlash()
        {
            byte[] buff = new byte[17];
            int i = 0;
            USBXpress.toarray(buff, ref i, (byte)singleton.txNumCanales);
            USBXpress.toarray(buff, ref i, (Int16)singleton.txPeriodo);
            USBXpress.toarray(buff, ref i, (Int16)singleton.txSeparador);
            USBXpress.toarray(buff, ref i, (byte)(singleton.txPolarity?1:0));

            USBXpress.toarray(buff, ref i, (byte)singleton.headtrack_panCh);
            USBXpress.toarray(buff, ref i, (byte)singleton.servo_ch[(int)Singleton.Canales.PAN]);

            USBXpress.toarray(buff, ref i, (byte)singleton.headtrack_tiltCh);
            USBXpress.toarray(buff, ref i, (byte)singleton.servo_ch[(int)Singleton.Canales.TILT]);

            USBXpress.toarray(buff, ref i, (float)singleton.uplinkValarm);
            USBXpress.toarray(buff, ref i, (byte)(singleton.useEmisora?1:0));
            if (singleton.enable_headtrack)
            {
                USBXpress.toarray(buff, ref i, (byte)(singleton.enable_pan ? 1 : 0));
                USBXpress.toarray(buff, ref i, (byte)(singleton.enable_tilt ? 1 : 0));
            }
            WriteRAW(0xfc, buff);
        }

        public float ReadBatteryLevel()
        {
            byte[] buff = ReadRAW(0xfb, sizeof(float));
            int i=0;
            float v = USBXpress.tofloat(buff, ref i);
            return v;
        }

        public void UpdateFirmware()
        {
            WriteRAW(0xf0, new byte[] { });
        }

        void SetServof(byte ch, float v, byte []buff, ref int i)
        {
            int valor;
            int max, min, center;

            if (ch < singleton.servo_ch.Length && singleton.servo_ch[ch]<255)
            {
                max = singleton.servo_max[ch];
                min = singleton.servo_min[ch];
                center = singleton.servo_center[ch];

                if (v > 1.0f)
                    v = 1.0f;
                else if (v < -1.0f)
                    v = -1.0f;

                if (singleton.servo_rev[ch])
                    v = -v;

                if (v >= 0)
                    valor = (int)((max - center) * v + center);
                else
                    valor = (int)((center - min) * v + center);

                buff[i] = singleton.servo_ch[ch];
                buff[i + 1] = (byte)((valor >> 8) & 0xff);
                buff[i + 2] = (byte)(valor & 0xff);
                i += 3;
            }
        }

        public void SetServos(float ail, float ele, float thr, float tail, float pan, float tilt, float flaps)
        {
            byte[] buffer = new byte[64];
            int i;
            int nbytes = 0;

            ClearInputBuffer(handle);
            buffer[0] = (byte)'A';
            buffer[1] = (byte)'T';
            buffer[2] = (byte)'C';
            buffer[3] = (byte)0xfe;
            
            i = 5;
            SetServof((byte)Singleton.Canales.AIL, ail, buffer, ref i);
            SetServof((byte)Singleton.Canales.ELE, ele, buffer, ref i);
            SetServof((byte)Singleton.Canales.THR, thr, buffer, ref i);
            SetServof((byte)Singleton.Canales.TAIL, tail, buffer, ref i);
            SetServof((byte)Singleton.Canales.PAN, pan, buffer, ref i);
            SetServof((byte)Singleton.Canales.TILT, tilt, buffer, ref i);
            SetServof((byte)Singleton.Canales.FLAPS, flaps, buffer, ref i);

            buffer[4] = (byte)((i - 5) / 3);

            usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
            //WaitACK_USB();
        }

        public void Write(Comandos cmd, int id, int offset, byte[] buff)
        {
            Write((int)cmd, id, offset, buff);
        }
        public byte[] Read(Comandos cmd, byte id, int offset, int len)
        {
            return Read((int)cmd, id, offset, len);
        }

        public void SetSlot(int slot, int[] mensaje)
        {
            byte[] buff = new byte[mensaje.Length * 2 + 1];
            byte []msg=USBXpress.tobytearray(mensaje);
            buff[0] = (byte) mensaje.Length;
            for(int i=0;i<msg.Length;i+=2)
            {
                buff[i+1]=msg[i+1];
                buff[i+2]=msg[i];
            }

            Write(Comandos.SLOT, slot, 0, buff);
        }

        public void SetSequence(int canal, int clk_div, int[] seq)
        {
            byte[] buff = new byte[seq.Length + 3]; ;
            buff[0] = (byte)canal;
            buff[1] = (byte)clk_div;
            buff[2] = (byte)seq.Length;
          
            for (int i = 0; i < seq.Length; i++)
                buff[i+3] = (byte)seq[i];
            Write(Comandos.SEQ, 0, 0, buff);
        }

        public void SetServo(byte id, int value)
        {
            if (value >= 500 && value <= 2500)
            {
                this.Write(Comandos.PC, 0, 6 + 2 * id, new byte[] { (byte)(value / 256), (byte)(value & 0xff) });
            }
        }

        public int GetServo(byte id)
        {
            byte[] buff;
            buff = this.Read(Comandos.IN, 0, 6 + 2 * id, 2);
            return buff[0] * 256 + buff[1];
        }

    }
}
