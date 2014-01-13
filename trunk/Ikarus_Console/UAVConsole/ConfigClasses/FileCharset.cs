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
using System.Drawing;

namespace UAVConsole.ConfigClasses
{
    public class FileCharset
    {
        byte[] buff = new byte[16384];
        public const int CharWidth = 12;
        public const int CharHeight = 18;

        public FileCharset()
        {
            clear();
        }

        public FileCharset(string FileName)
        {
            StreamReader sr = File.OpenText(FileName);
            load(sr);
            sr.Close();
        }

        public FileCharset(StreamReader sr)
        {
            load(sr);
        }

        public void clear()
        {
            for (int i = 0; i < buff.Length; i++)
                buff[i] = 85;
        }

        public void load(string FileName)
        {
            StreamReader sr = File.OpenText(FileName);
            load(sr);
            sr.Close();
        }

        public void load(StreamReader sr)
        {
            string linea;
            int i = 0;
            
            linea=sr.ReadLine();
            if (linea.Equals("MAX7456"))
            {
                do
                {
                    linea = sr.ReadLine();
                    buff[i] = bin2byte(linea);
                    i++;
                } while (!sr.EndOfStream);
            }
        }

        public void save(string FileName)
        {
            StreamWriter sw = File.CreateText(FileName);
            sw.WriteLine("MAX7456");
            for (int i = 0; i < buff.Length; i++)
            {
                sw.WriteLine(byte2bin(buff[i]));
            }
            sw.Close();
        }
        public byte[] getChar(byte id)
        {
            byte[] b = new byte[64];
            for (int i = 0; i < 64; i++)
                b[i] = this.buff[64 * id + i];
            return b;
        }

        public void setChar(byte id, byte[] b)
        {
            for (int i = 0; i < 64; i++)
                this.buff[64 * id + i] = b[i];
        }

        public Bitmap getCharBitmap(byte id)
        {
            Bitmap bmp = new Bitmap(CharWidth, CharHeight);
            Graphics g = Graphics.FromImage(bmp);
            byte[] buffer = getChar(id);

            for (int filas = 0; filas < bmp.Height; filas++)
                for (int cols = 0; cols < bmp.Width; cols++)
                {
                    int i = filas * 3 + cols / 4;
                    byte b = buffer[i];
                    b = (byte)((b >> (6 - 2 * (cols % 4))) & 3);
                    if (b == 0)
                        bmp.SetPixel(cols, filas, Color.Black);
                    else if (b == 2)
                        bmp.SetPixel(cols, filas, Color.White);
                    else
                        bmp.SetPixel(cols, filas, Color.Transparent);
                }

            g.Dispose();
            return bmp;
        }

        byte bin2byte(string bin)
        {
            byte res = 0;
            if (bin.Length == 8)
                for (int i = 0; i < 8; i++)
                    if (bin[i] == '0')
                        res = (byte)(res * 2);
                    else
                        res = (byte)(res * 2 + 1);
            return res;
        }
        string byte2bin(byte b)
        {
            string s= "";
            for (int i = 0; i < 8; i++)
            {
                if (b>=128)
                    s += "1";
                else
                    s += "0";
                b = (byte)(b * 2);
            }
            return s;
        }

        public override string ToString()
        {
            const string hexchars = "0123456789ABCDEF";
            string tmp = "";
            for (int i = 0; i < buff.Length; i++)
            {
                tmp += hexchars[(buff[i] >> 4) & 0xf];
                tmp += hexchars[buff[i] & 0xf];
            }
            return tmp;
        }

        public void FromString(string str)
        {
            const string hexchars = "0123456789ABCDEF";
            for (int i = 0; i < buff.Length; i++)
            {
                byte tmp;
                tmp = (byte)(hexchars.IndexOf(Char.ToUpper(str[2 * i])) * 16);
                tmp += (byte)(hexchars.IndexOf(Char.ToUpper(str[2 * i + 1])));
                buff[i] = tmp;
            }
        }
    }
}
