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

namespace UAVConsole.USBXpress
{
    public class WflyUplink
    {
        const int min = 1000;
        const int max = 2000;
        const int STA = 900;
        const byte CLK = 0x10;
        const int bits = 5;

        const int range = max - min;
        const int valores = 1 << bits;
        const float step = range / (valores - 1.0f);
        
        enum REGS {Switches, Altura, Lon, Lat, Wpt_ID, CMD}; // Cada registro tiene tamaño arbitrario. + versatil
        public enum Autopilot { Manual, Copilot, Autopilot };
        public enum Ruta { Casa, Ruta, Hold, Modem };

        static byte seqid = 0;

        public static int[] SetSwitches(Ruta ruta, Autopilot autopilot,int screen, bool cam)
        {
            byte sw = (byte)(screen & 0x3);
            sw |= (byte)(((int)autopilot) << 2);
            sw |= (byte)(((int)ruta) << 4);

            if (!cam)
                sw |= 0x40;

            return build_packet((byte)REGS.Switches, new byte[] { sw });
        }

        public static int[] SetWptID(int wpt_id)
        {
            byte dato = (byte)(wpt_id & 0x1f);
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            return build_packet((byte)REGS.Wpt_ID, new byte[] { dato });
        }

        public static int[] CmdSetCenterIR()
        {
            byte dato = (byte)1;
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            return build_packet((byte)REGS.CMD, new byte[] { dato });
        }

        public static int[] CmdSetGainIR()
        {
            byte dato = (byte)0;
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            return build_packet((byte)REGS.CMD, new byte[] { dato });
        }

        public static int[] CmdSetHome()
        {
            byte dato = (byte)2;
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            return build_packet((byte)REGS.CMD, new byte[] { dato });
     
        }

        public static int[] CmdSaveFlash()
        {
            byte dato = (byte)3;
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            return build_packet((byte)REGS.CMD, new byte[] { dato });
        }

        public static int[] SetDebugVar(byte var, float value)
        {
            byte dato = (byte)(var & 0x3f);
            int i = 1;
            byte[] buff = new byte[5];
            seqid = (byte)((seqid + 1) & 0x03);
            dato |= (byte)(seqid << 6);
            buff[0] = dato;
            USBXpress.toarray(buff, ref i, value);
            return build_packet((byte)0xF, buff);
        }
        public static int[] SetAltura(int altitude)
        {
            int i = 0;
            byte [] buff= new byte[2];
            USBXpress.toarray(buff,ref i,(Int16)altitude);
            return build_packet((byte)REGS.Altura, buff);
        }

        public static int[] SetLon(float lon)
        {
            int i = 0;
            byte[] buff = new byte[4];
            USBXpress.toarray(buff, ref i, lon);
            return build_packet((byte)REGS.Lon, buff);
        }

        public static int[] SetLat(float lat)
        {
            int i = 0;
            byte[] buff = new byte[4];
            USBXpress.toarray(buff, ref i, lat);
            return build_packet((byte)REGS.Lat, buff);
        }

       

        public static int[] build_packet(byte id, float v)
        {
            int i = 0;
            byte[] buff = new byte[4];
            USBXpress.toarray(buff, ref i, v);
            return build_packet(id, buff);
        
        }

        public static int[] build_packet(byte id, byte []dato)
        {
            byte crc = 0;
            int[] buff = new int[dato.Length * 2 + 3];

            buff[0] = STA;
            buff[1] = (int)(min + step * CRC.gray2bin(id));
            CRC4_nt(id, ref crc);

            for (int i = 0; i < dato.Length; i++)
            {
                byte hi = (byte)(dato[i] >> 4);
                byte lo = (byte)(dato[i] & 0xf);

                CRC4_nt(hi, ref crc);
                CRC4_nt(lo, ref crc);

                buff[2 * i + 2] = (int)(min + step * CRC.gray2bin(hi | CLK));
                buff[2 * i + 3] = (int)(min + step * CRC.gray2bin(lo));
            }
            buff[buff.Length - 1] = (int)(min + step * CRC.gray2bin(crc | CLK));
            return buff;
        }

        public static void CRC4_nt(byte v, ref byte crc)
        {
            const byte POLY = 0x3;// 0x8c;
            byte j, i;

            j = (byte)(crc ^ v);
            for (i = 4; i > 0; i--)
            {
                j = (byte)(((j & 0x8) != 0) ? (j << 1) ^ POLY : j << 1);
                j = (byte)(j & 0x0F);
            }
            crc = j;
        }
    }
}
