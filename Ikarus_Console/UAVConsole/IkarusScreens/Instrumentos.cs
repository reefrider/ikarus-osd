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
using System.Drawing;
using UAVConsole.ConfigClasses;

namespace UAVConsole.IkarusScreens
{
    public class Instrumentos
    {
        public Graphics g=null;
        FileCharset fcs;
        int NumFilas = 16;
        bool transparente = true;

        public Instrumentos()
        {
            fcs = new FileCharset("Ikarus.mcm");
        }

        public Instrumentos(Graphics g):this()
        {
            this.g = g;
        }

        public Instrumentos(Graphics g, FileCharset fc)
        {
            this.g = g;
            this.fcs = fc;
        }

        public void SetPAL()
        {
            NumFilas = 16;
        }

        public void SetNTSC()
        {
            NumFilas = 13;
        }

        public void CharAttrBackGr()
        {
            transparente = false;
        }

        public void CharAttrNoBackGr()
        {
            transparente = true;
        }

        public void Bar(int fila, int col, int width, int v)
        {
            int i, j;
            int steps = (width - 2) * 4;
            if (v < 0)
                v = 0;
            else if (v > 255)
                v = 255;

            j = steps * v / 255;
            if (width == 0)
                return;

            writeAtChr(fila, col, 0xF7);

            for (i = 1; i < width - 1; i++)
            {
                int c;
                if (j > 3)
                    c = 0xF8;
                else if (j > 2)
                    c = 0xF9;
                else if (j > 1)
                    c = 0xF0;
                else if (j > 0)
                    c = 0xF1;
                else
                    c = 0xF2;
                if (j > 0)
                    j -= 4;

                writeAtChr(fila, col + i, c);
            }
            writeAtChr(fila, col + i, 0xF3);
        }

        public void HorizonteArtificial(int fila, int col, int width, float pitch, float roll, int modo)
        {
            // 12 es el ancho del caracter, 18 el alto.
            int maxx = width;
            int maxy = 12 * maxx + 12 / 2;
            //int wide = maxx * 2 + 1;
            //int[] oldy = new int[wide];

            for (int i = -maxx; i <= maxx; i++)
            {
                float y = (float)(12 * i * Math.Tan(roll * Math.PI / 180));
                y -= (float)(Math.Sin(pitch * Math.PI / 180) * maxy);

                if (y > maxy)
                    y = maxy;
                else if (y < -maxy)
                    y = -maxy;

                float aux = y + 18 / 2;
                int pos = (int)Math.Floor(aux / 18);        // No vale con hacer casting. Floor da la vida
                int c = (int)((aux - (18 * pos)) / 3);
                /*
                if (oldy[maxx + i] != pos)
                {
                    ins.writeAtChr(fila - oldy[maxx + i], col + i, 0);
                    oldy[maxx + i] = pos;
                }
                */
                if(modo==0||((modo==1)&&(i==maxx||i==-maxx)))
                    writeAtChr(fila - pos, col + i, 0xFA + c);
                
            }
            writeAtChr(fila, col + maxx + 1, 0xFA + 2);
            writeAtChr(fila, col - maxx - 1, 0xFA + 2);
        }
        public void COMPAS_grp(int fila, int col, float valor)
        {
            int v;
            byte ch;

            valor = valor + 90.0f - 180.0f / 16.0f;
            while (valor < 0)
                valor += 360.0f;
            while (valor >= 360.0f)
                valor -= 360.0f;

            valor = valor * 16.0f / 360.0f;
            v = (int)valor;

            writeAtChr(fila, col, 0x90);
            writeAtChr(fila, col + 3, 0x91);
            writeAtChr(fila + 1, col, 0x92);
            writeAtChr(fila + 1, col + 3, 0x93);

            if (v < 8)
                ch = (byte)(0x50 + 0x0E - 2 * v);
            else
                ch = (byte)(0x70 + 0x0E - 2 * (v - 8));
            writeAtChr(fila, col + 1, ch);
            writeAtChr(fila, col + 2, ch + 1);
            writeAtChr(fila + 1, col + 1, ch + 0x10);
            writeAtChr(fila + 1, col + 2, ch + 0x11);
        }

        public void COMPAS_chr(int fila, int col, float valor)
        {
            valor = valor + 45.0f - 180.0f / 8.0f;
            while (valor < 0)
                valor += 360.0f;
            while (valor >= 360.0f)
                valor -= 360.0f;

            valor = valor * 8.0f / 360.0f;

            writeAtChr(fila, col, 0xB0 + 2 * ((int)valor));
            writeAtChr(fila, col + 1, 0xB1 + 2 * ((int)valor));

        }

        public void Compas(int fila, int col, int width, float heading, float bearing)
        {
            int i;
            int center;
            int steps = 4;
            int entero, resto, entero2;
            float t;

            t = bearing - heading;
            while (t > 180.0)
                t -= 360.0f;
            while (t < -180.0)
                t += 360.0f;

            if (t > 0)
            {
                writeAtChr(fila, col + width - 1, 0x96);
                writeAtChr(fila, col, 0x00);

            }
            else
            {
                writeAtChr(fila, col, 0x97);
                writeAtChr(fila, col + width - 1, 0x00);
            }
            width -= 2;
            col += 1;

            center = width / 2;
            heading = -heading +center *4* 360.0f / 192.0f;// -33.75f;

            bearing = -bearing;

            while (heading < 0)
                heading += 360.0f;
            while (heading >= 360.0f)
                heading -= 360.0f;
            heading = heading * 192.0f / 360.0f;

         
            if (bearing < 0)
                bearing += 360.0f;
            else if (bearing >= 360.0f)
                bearing -= 360.0f;
            bearing = bearing * 192.0f / 360.0f;
            entero2 = (int)bearing;
            entero2 = entero2 / steps;

            entero = (int)(heading / steps);
            resto = (int)heading % steps;

            for (i = 0; i < width; i++)
            {
                int ie = i - entero;
                int ie2 = i - entero + entero2;
                if (ie < 0)
                    ie += 48;
                if (ie2 < 0)
                    ie2 += 48;
                else if (ie2 >= 48)
                    ie2 -= 48;


                printAtStr(fila + 1, col + i, " ");

                if (ie2 % 48 == 0)
                    writeAtChr(fila + 1, col + i, 0x95);
                else if ((ie) % 12 == 0)
                {
                    int te = ie / 12;
                    te = te % 4;
                    if (te == 0)
                        printAtStr(fila + 1, col + i, "N");
                    else if (te == 1)
                        printAtStr(fila + 1, col + i, "E");
                    else if (te == 2)
                        printAtStr(fila + 1, col + i, "S");
                    else if (te == 3)
                        printAtStr(fila + 1, col + i, "W");
                }

                if ((ie) % 3 == 0)
                {
                    switch (resto)
                    {
                        case 0:
                            writeAtChr(fila, col + i, 0xD2);
                            break;
                        case 1:
                            writeAtChr(fila, col + i, 0xD4);
                            break;
                        case 2:
                            writeAtChr(fila, col + i, 0xD3);
                            break;
                        case 3:
                            writeAtChr(fila, col + i, 0xD5);
                            break;
                    }
                }
                else
                {
                    if (resto == 0 || resto == 2)
                        writeAtChr(fila, col + i, 0xD0);
                    else
                        writeAtChr(fila, col + i, 0xD1);

                }
            }
        }

        public void Velocimetro(int fila, int col, int height, float valor)
        {
            int i;
            int steps = 6;
            int entero, resto;


            valor = (valor + 1) * 12.0f / 5;
            entero = (int)(valor / steps);
            resto = (int)valor % steps;

            for (i = 0; i < height; i++)
            {
                int ie = i - entero;

                if ((ie) % 2 == 0)
                {
                    switch (resto)
                    {
                        case 0:
                            writeAtChr(fila + i, col, 0xC2);
                            break;
                        case 1:
                            writeAtChr(fila + i, col, 0xC5);
                            break;
                        case 2:
                            writeAtChr(fila + i, col, 0xC3);
                            break;
                        case 3:
                            writeAtChr(fila + i, col, 0xC6);
                            break;
                        case 4:
                            writeAtChr(fila + i, col, 0xC4);
                            break;
                        case 5:
                            writeAtChr(fila + i, col, 0xC7);
                            break;

                    }
                }
                else
                {
                    if (resto == 0 || resto == 2 || resto == 4)
                        writeAtChr(fila + i, col, 0xC0);
                    else
                        writeAtChr(fila + i, col, 0xC1);

                }
            }
        }

        public void Altimetro(int fila, int col, int height, float valor)
        {
            int i;
            int steps = 6;
            int entero, resto;

            valor = (valor + 1) * 12.0f / 5;
            entero = (int)(valor / steps);
            resto = (int)valor % steps;
            if (resto < 0)
                resto = 6 + resto;
            for (i = 0; i < height; i++)
            {
                int ie = i - entero;

                if ((ie) % 2 == 0)
                {
                    switch (resto)
                    {
                        case 0:
                            writeAtChr(fila + i, col, 0xCA);
                            break;
                        case 1:
                            writeAtChr(fila + i, col, 0xCD);
                            break;
                        case 2:
                            writeAtChr(fila + i, col, 0xCB);
                            break;
                        case 3:
                            writeAtChr(fila + i, col, 0xCE);
                            break;
                        case 4:
                            writeAtChr(fila + i, col, 0xCC);
                            break;
                        case 5:
                            writeAtChr(fila + i, col, 0xCF);
                            break;

                    }
                }
                else
                {
                    if (resto == 0 || resto == 2 || resto == 4)
                        writeAtChr(fila + i, col, 0xC8);
                    else
                        writeAtChr(fila + i, col, 0xC9);

                }
            }
        }


        public void Variometro1(int fila, int col, int valor)
        {
            int valores = 7;
            int rango = 256 / valores;
            int result;
            byte[] var1 =new byte[]{ 0x9E, 0x9D, 0x9C, 0x98, 0x99, 0x9A, 0x9B };

            if (valor < -3)
                valor = -3;
            else if (valor > 3)
                valor = 3;

            result = valor;
            writeAtChr(fila, col, var1[result + 3]);
        }

        public void Variometro2(int fila1, int fila2, int col, int valor)
        {
            int valores = 16;
            int rango = 256 / valores;
            int result;

            if (valor < -7)
                valor = -7;
            else if (valor > 7)
                valor = 7;

            result = valor;///rango;
            if (result < 0)
            {
                writeAtChr(fila1, col, 0xA0);
                writeAtChr(fila2, col, (byte)(0xAF + result));
            }
            else
            {
                writeAtChr(fila1, col, (byte)(0xA0 + result));
                writeAtChr(fila2, col, 0xAF);
            }

        }

        public void printAtChr(int fila, int col, char c)
        {
            byte t;
            if (c == '0')
                t = 0x0a;
            else if (c >= '1' && c <= '9')
                t = (byte)(c - '1' + 1);
            else if (c >= 'A' && c <= 'Z')
                t = (byte)(c - 'A' + 0x0B);
            else if (c >= 'a' && c <= 'z')
                t = (byte)(c - 'a' + 0x25);
            else if (c == '(')
                t = 0x3f;
            else if (c == ')')
                t = 0x40;
            else if (c == '.')
                t = 0x41;
            else if (c == '?')
                t = 0x42;
            else if (c == ';')
                t = 0x43;
            else if (c == ':')
                t = 0x44;
            else if (c == ',')
                t = 0x45;
            else if (c == '\'')
                t = 0x46;
            else if (c == '/')
                t = 0x47;
            else if (c == '\"')
                t = 0x48;
            else if (c == '-')
                t = 0x49;
            else if (c == '<')
                t = 0x4A;
            else if (c == '>')
                t = 0x4B;
            else if (c == '@')
                t = 0x4C;
            else if (c == '=')
                t = 0x4D;
            else if (c == 'Ñ')
                t = 0x4E;
            else if (c == 'ñ')
                t = 0x4F;
           
            else
                t = 0;
            writeAtChr(fila, col, t);
        }

        public void printAtStr(int fila, int col, string cad)
        {
            for (int i = 0; i < cad.Length; i++)
                printAtChr(fila, col + i, cad[i]);
        }

        public void printAtStr2(int fila, int col, string cad, int len)
        {
            for (int i = 0; i < cad.Length && i < len; i++)
                printAtChr(fila, col + i, cad[i]);
        }

        public void writeAtChr(int fila, int cols, int c)
        {
            Bitmap bmp;
            int f;
            if (fila < 0)
                f = NumFilas + fila;
            else
                f = fila;
            bmp = fcs.getCharBitmap((byte)c);
            if (!transparente)
            {
                g.FillRectangle(Brushes.Gray, cols * bmp.Width, f * bmp.Height, bmp.Width, bmp.Height);
            }
            g.DrawImage(bmp, cols * bmp.Width, f * bmp.Height);
        }

        public void writeAtStr(int fila, int cols, byte[] cad)
        {
            for (int i = 0; i < cad.Length; i++)
                writeAtChr(fila, cols + i, cad[i]);
        }

    }
}
