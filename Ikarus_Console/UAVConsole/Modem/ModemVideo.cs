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
using System.Drawing.Imaging;
using System.Windows.Forms;
using DirectX.Capture;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace UAVConsole.Modem
{

    class ModemVideo : ModemAbstract
    {
        int COL = 2;
        int FILA = 0;
        int THR = 0;
        
        Bitmap bmp;
        Thread thread;

        int contador_badrx = 0;
        enum Estado{INIT, DECODING, AUTOAJUSTE};
        Estado miestado = Estado.INIT;

       
        public ModemVideo()
        {
        
        }

        override public void dispose()
        {
            base.dispose();
        }

        ~ModemVideo()  // destructor
        {
            dispose();
        }

        void Rafa()
        {
            miestado = Estado.AUTOAJUSTE;
            if (AutoAjustarModem(bmp))
                miestado = Estado.DECODING;
            else
                miestado = Estado.INIT;
        }


        public void SetImage(System.Drawing.Bitmap e)
        {
            long tics = DateTime.Now.Ticks;

            if (miestado == Estado.INIT)
            {
                bmp = e;
                contador_badrx = 0;
                thread = new Thread(new ThreadStart(Rafa));
                thread.IsBackground = true;
                thread.Start();
            }
            else if (miestado == Estado.DECODING)
            {
                bmp = e;
                byte[] buffer = ProcesarImagen(bmp, COL);
                // Llama al modem propiamente dicho
                if (buffer != null)
                {
                    //THR = CalcularThreshold(buffer, FILA);
                    byte[] packet = ProcesarPacket(buffer, FILA, THR);

                    if (ParsePacket(packet))
                    {
                        listeners.Invoke();
                        contador_badrx = 0;
                    }
                    else if (contador_badrx == 10)
                    {
                        miestado = Estado.INIT;
                    }
                    else
                        contador_badrx++;
                }

            }
            long elapsed = DateTime.Now.Ticks - tics;
            float ms = ((float)elapsed) / TimeSpan.TicksPerMillisecond;
            Console.WriteLine("DECODE MODEM: elapsed (ms) " + ms);
           
        }

        public void SetImage2(System.Drawing.Bitmap e)
        {
            long tics = DateTime.Now.Ticks;

            bmp = e;
            if (contador_badrx > 10)
            {
                if (AutoAjustarModem(bmp))
                    contador_badrx = 0;
            }
            else
            {
                byte[] buffer = ProcesarImagen(bmp, COL);
                // Llama al modem propiamente dicho
                if (buffer != null)
                {
                    //THR = CalcularThreshold(buffer, FILA);
                    byte[] packet = ProcesarPacket(buffer, FILA, THR);

                    if (ParsePacket(packet))
                    {
                        listeners.Invoke();
                        contador_badrx = 0;
                    }
                    else
                        contador_badrx++;
                }
            }

            long elapsed = DateTime.Now.Ticks - tics;
            float ms = ((float)elapsed) / TimeSpan.TicksPerMillisecond;
            Console.WriteLine("DECODE MODEM: elapsed (ms) " + ms);

        }
      
        bool AutoAjustarModem(Bitmap bmp)
        {
            Bitmap mibmp;
            int i,_col;
            
            if (bmp != null)
            {
                mibmp = (Bitmap)bmp.Clone();

                for (_col = 0; _col <100 ; _col++)    //20 //del 2 al mibmp.Width/2
                {
                    byte[] buffer = ProcesarImagen(mibmp, _col);
                    if (buffer != null)
                    {
                        for (i = 0; i < buffer.Length - DATALEN * 8; i++)
                        {

                            THR = CalcularThreshold(buffer, i);
                            // Llama al modem propiamente dicho
                            if (THR < 0)
                                break;

                            byte[] packet = ProcesarPacket(buffer, i, THR);

                            if (packet[0] == DATALEN)
                            {
                                if (ParsePacket(packet))
                                {
                                    FILA = i;
                                    COL = _col;
                                    //Debug.WriteLine("FILA=" + FILA + " COL=" + COL + " THR=" + THRESHOLD);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        byte []ProcesarImagen(Bitmap e, int col)
        {
            // Obtiene la columna
            try
            {
                BitmapData bmpd = e.LockBits(new Rectangle(0, 0, 1, e.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                byte[] buffer = new byte[bmpd.Height / 2];
                bmpd.PixelFormat = PixelFormat.Format32bppRgb;
                unsafe
                {
                    for (int i = 0; i < bmpd.Height / 2; i++)
                    {
                        byte* row = (byte*)bmpd.Scan0 + (2 * i * bmpd.Stride);
                        buffer[i] = (byte)row[4 * col];
                    }
                }
                e.UnlockBits(bmpd);
                return buffer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        int CalcularThreshold(byte[] buffer, int fila)
        {
            int min, max, media;
            media = 0;
            max = buffer[fila];
            min = buffer[fila];
            for (int x = 0; x < DATALEN * 8; x++)
            {
                media += buffer[fila + x];
                if (buffer[fila + x] > max)
                    max = buffer[fila + x];
                if (buffer[fila + x] < min)
                    min = buffer[fila + x];
            }

            if (max - min < 20)
                return -1;
            else
            {
                int threshold = (max + min) / 2;
                return threshold;
            }
        }

        byte[] ProcesarPacket(byte[] buffer, int fila, int threshold)
        {
            int j = 0, k = 0;
            byte tmp = 0;
        
            byte[] salida = new byte[buffer.Length / 8];

            for (int i = fila; i < buffer.Length; i++)  // aqui ponia fila+1, pero creo q era un error
            {
                tmp >>= 1;
                tmp |= (buffer[i] > threshold) ? (byte)128 : (byte)0;       // LSB first
                
                if (k >= 7)
                {
                    salida[j] = tmp;
                    tmp = 0;
                    k = 0;
                    j++;
                }
                else
                    k++;

            }
            return salida;
        }
    }
}
