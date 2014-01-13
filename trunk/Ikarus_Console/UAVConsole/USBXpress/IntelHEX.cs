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
using System.IO.Ports;
using System.Threading;

namespace UAVConsole.USBXpress
{
    class IntelHEX
    {
        byte[] rom;
        int maxaddr;

        public enum Fase { IDLE, DELETE, PROGRAM, END };
        public delegate void Listener(Fase fase, float completed);


        public byte this[int index]
        {
            get { return rom[index]; }
            set { rom[index] = value; }
        }

        public int lenght
        {
            get { return maxaddr; }
        }

        public static IntelHEX FromBinHex(string FileName)
        {
            IntelHEX hex = new IntelHEX();
            string line;
            int i = 0;

            if (FileName.Length == 0)
                return null;
            StreamReader sreader = new StreamReader(FileName);

            while (!sreader.EndOfStream)
            {
                line = sreader.ReadLine();
                hex.rom[i] = tohex(line);
                i++;
                hex.maxaddr = i;
            }
            return hex;
        }

        public static IntelHEX FromBinFw(Stream stream)
        {
            IntelHEX hex = new IntelHEX();
            int dato;
            int i = 0;
            
            dato = stream.ReadByte();
            while (dato >= 0)
            {
                hex.rom[i] = (byte)dato;
                i++;
                hex.maxaddr = i;
                dato = stream.ReadByte();
            }
            return hex;

        }
        public static IntelHEX FromBinFw(string FileName)
        {

            if (FileName.Length == 0)
                return null;

            FileStream stream = File.OpenRead(FileName);
            return FromBinFw(stream);
        }

        public IntelHEX()
        {
            rom = new byte[65536];
            maxaddr = 0;
            
        }

        public IntelHEX(string FileName):this()
        {
            Load(FileName);
        }

        public void SendUART(SerialPort sp)
        {
            SendUART(sp, false, null);
        }
        public void SendUART(SerialPort sp, bool bidir, Listener listener)
        {
            int pages = maxaddr / 512;
            byte crc = 0;
            //UInt32 lfsr = 0x11223344;

            if (maxaddr % 512 == 0)
            {
                sp.Open();
                sp.Write("$");
                sp.Write("A");
                sp.Write("T");
                sp.Write("C");

                if (listener != null) listener.Invoke(Fase.DELETE, 0.0f);

                //enviamos el número de pages
                sp.Write(new byte[] { (byte)pages }, 0, 1);
                Thread.Sleep(3000);         // 125pages*20ms flash pages = 2.5s

                if (listener != null) listener.Invoke(Fase.PROGRAM, 0.0f);

                for (int i = 0; i < pages; i++)
                {
                    byte[] buff = new byte[512];
                    for (int j = 0; j < 512; j++)
                    {
                        byte b = rom[512 * i + j];
                        CRC.CCITT8(b, ref crc);
                        buff[j] = b;
                    }
                    sp.Write(buff, 0, 512);
                    if (listener != null) listener.Invoke(Fase.PROGRAM, (i+1.0f)/pages);

                }
                sp.Write(new byte[] { crc }, 0, 1);
                sp.Close();
                if (listener != null) listener.Invoke(Fase.END, 1.0f);

            }

        }

        public void Load(string FileName)
        {
            string line;

            if (FileName.Length == 0)
                return;
            StreamReader sreader = new StreamReader(FileName);

            line=sreader.ReadLine();
            while (!line.Contains(":00000001FF"))
            {

                if (line.StartsWith(":"))
                {
                    byte[] rafa = tohexarray(line.Substring(1));
                    byte len = rafa[0];
                    int addr = rafa[1] * 256 + rafa[2];
                    byte type = rafa[3];
                    //byte checksum = rafa[rafa.Length - 1];
                    int ndatos = len - 5;
                    byte suma = 0;
                    for (int i = 0; i < rafa.Length; i++)
                        suma += rafa[i];
                    if (suma == 0 && type == 0)
                    {
                        for (int i = 0; i < ndatos; i++)
                        {
                            rom[addr + i] = rafa[i + 4];
                            if (addr + i > maxaddr)
                                maxaddr = addr + i;
                        }
                    }
                    else
                        throw new Exception("BAD CRC or BAD TYPE in HEX file");
                }
                else
                    throw new Exception("No : at begin of line");
                line = sreader.ReadLine();
            }
            sreader.Close();
        }
        static byte []tohexarray(string str)
        {
            
            byte []buff=new byte[str.Length/2];
            for(int i=0,j=0;i<str.Length;i+=2,j++)
            {
                buff[j] = tohex(str.Substring(i, 2));
            }
            return buff;
        }

        static byte tohex(string str)
        {
            int i,j;
            const string digits="0123456789ABCDEF";
            byte dato=0;
            for (i = 0; i < 2; i++)
            {
                j = digits.IndexOf(str.ToUpper()[i]);
                if (j >= 0)
                    dato = (byte)(16 * dato + j);
            }
            return dato;
        }

    }
}
