/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of FPVOSD.
 *
 *  FPVOSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FPVOSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with FPVOSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace UAVConsole.USBXpress
{
    class USBXpress
    {
        public const int SI_MAX_DEVICE_STRLEN = 256;
        public const int SI_MAX_READ_SIZE = 4096 * 16;
        public const int SI_MAX_WRITE_SIZE = 4096;

        public enum ReturnCodes : int
        {
            SI_SUCCESS = 0x00,
            SI_INVALID_HANDLE = 0x01,
            SI_READ_ERROR = 0x02,
            SI_RX_QUEUE_NOT_READY = 0x03,
            SI_WRITE_ERROR = 0x04,
            SI_RESET_ERROR = 0x05,
            SI_INVALID_PARAMETER = 0x06,
            SI_INVALID_REQUEST_LENGTH = 0x07,
            SI_DEVICE_IO_FAILED = 0x08,
            SI_INVALID_BAUDRATE = 0x09,
            SI_FUNCTION_NOT_SUPPORTED = 0x0a,
            SI_GLOBAL_DATA_ERROR = 0x0b,
            SI_SYSTEM_ERROR_CODE = 0x0c,
            SI_READ_TIMED_OUT = 0x0d,
            SI_WRITE_TIMED_OUT = 0x0e,
            SI_IO_PENDING = 0x0f,
            SI_DEVICE_NOT_FOUND = 0xFF
        };

        public enum ProductString : int
        {
            SI_RETURN_SERIAL_NUMBER = 0x00,
            SI_RETURN_DESCRIPTION,
            SI_RETURN_LINK_NAME,
            SI_RETURN_VID,
            SI_RETURN_PID
        };

        public enum RXQueueStatus : uint  //UInt32
        {
            SI_RX_NO_OVERRUN = 0x00,
            SI_RX_EMPTY = 0x00,
            SI_RX_OVERRUN,
            SI_RX_READY
        };

        //public enum Handle : int { };
        enum TypeOS { None, x86, i64 };
        static TypeOS tipo = TypeOS.None;

        public USBXpress()
        {
            int numDevices=0;
            if (tipo != TypeOS.i64)
            {
                try
                {
                    USBXpress32.SI_GetNumDevices(ref numDevices);
                    tipo = TypeOS.x86;
                }
                catch (Exception)
                {
                    try
                    {
                        USBXpress64.SI_GetNumDevices(ref numDevices);
                        tipo = TypeOS.i64;
                    }
                    catch (Exception)
                    {
                        tipo = TypeOS.None;
                        MessageBox.Show("Error opening DLL API for USB. Check Instalation.");
                    }
                }
            }
            else
            {

                try
                {
                    USBXpress64.SI_GetNumDevices(ref numDevices);
                    tipo = TypeOS.i64;
                }
                catch (Exception)
                {
                    try
                    {
                        USBXpress32.SI_GetNumDevices(ref numDevices);
                        tipo = TypeOS.x86;
                    }
                    catch (Exception)
                    {
                        tipo = TypeOS.None;
                        MessageBox.Show("Error opening DLL API for USB. Check Instalation.");
                    }
                }
            }
        }
        public USBXpress.ReturnCodes SI_GetNumDevices(ref int lpwdNumDevices)
        {
            if (tipo == TypeOS.i64)
                return USBXpress64.SI_GetNumDevices(ref lpwdNumDevices);
            else
                return USBXpress32.SI_GetNumDevices(ref lpwdNumDevices);
        }

        public USBXpress.ReturnCodes SI_GetProductString(int dwDeviceNum, [MarshalAs(UnmanagedType.LPArray)] byte[] lpvDeviceString, USBXpress.ProductString dwFlags)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_GetProductString(dwDeviceNum, lpvDeviceString, dwFlags);
            else
                return USBXpress32.SI_GetProductString(dwDeviceNum, lpvDeviceString, dwFlags);
        }

        public USBXpress.ReturnCodes SI_Open(UInt32 dwDevice, ref UInt32 cyHandle)
        {
            lock (Singleton.GetInstance())
            {
                if (tipo == TypeOS.i64)
                    return USBXpress64.SI_Open(dwDevice, ref cyHandle);
                else
                    return USBXpress32.SI_Open(dwDevice, ref cyHandle);
            }
        }

        public USBXpress.ReturnCodes SI_Close(UInt32 cyHandle)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_Close(cyHandle);
            else
                return USBXpress32.SI_Close(cyHandle);
        }

        public USBXpress.ReturnCodes SI_Read(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToRead, ref int lpdwBytesReturned, int lpOverlapped)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_Read(cyHandle, lpBuffer, dwBytesToRead, ref lpdwBytesReturned, lpOverlapped);
            else
                return USBXpress32.SI_Read(cyHandle, lpBuffer, dwBytesToRead, ref lpdwBytesReturned, lpOverlapped);
        }

        public USBXpress.ReturnCodes SI_Write(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToWrite, ref int lpdwBytesWritten, int lpOverlapped)
        {
            if (tipo == TypeOS.i64)
                return USBXpress64.SI_Write(cyHandle, lpBuffer, dwBytesToWrite, ref lpdwBytesWritten, lpOverlapped);
            else
                return USBXpress32.SI_Write(cyHandle, lpBuffer, dwBytesToWrite, ref lpdwBytesWritten, lpOverlapped);
        }
        
        
        
        public USBXpress.ReturnCodes SI_SetTimeouts(int dwReadTimeout, int dwWriteTimeout)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_SetTimeouts(dwReadTimeout, dwWriteTimeout);
            else
                return USBXpress32.SI_SetTimeouts(dwReadTimeout, dwWriteTimeout);
        }

        public USBXpress.ReturnCodes SI_GetTimeouts(ref int lpdwReadTimeout, ref int lpdwWriteTimeout)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_GetTimeouts(ref lpdwReadTimeout, ref lpdwWriteTimeout);
            else
                return USBXpress32.SI_GetTimeouts(ref lpdwReadTimeout, ref lpdwWriteTimeout);
        }

        public USBXpress.ReturnCodes SI_CheckRXQueue(UInt32 cyHandle, ref UInt32 lpdwNumBytesInQueue, ref USBXpress.RXQueueStatus lpdwQueueStatus)
        {
            if (tipo == TypeOS.i64) 
                return USBXpress64.SI_CheckRXQueue(cyHandle, ref lpdwNumBytesInQueue, ref lpdwQueueStatus);
            else
                return USBXpress32.SI_CheckRXQueue(cyHandle, ref lpdwNumBytesInQueue, ref lpdwQueueStatus);
        }


        

        /*
        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_GetNumDevices(ref int lpwdNumDevices);
        
        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_GetProductString(int dwDeviceNum, [MarshalAs(UnmanagedType.LPArray)] byte[] lpvDeviceString, USBXpress.ProductString dwFlags);

        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_Open(UInt32 dwDevice, ref UInt32 cyHandle);

        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_Close(UInt32 cyHandle);

        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_Read(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToRead, ref int lpdwBytesReturned, int lpOverlapped);

        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_Write(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToWrite, ref int lpdwBytesWritten, int lpOverlapped);
        
        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_SetTimeouts(int dwReadTimeout, int dwWriteTimeout);
        
        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_GetTimeouts(ref int lpdwReadTimeout, ref int lpdwWriteTimeout);

        [DllImport("SiUSBXp.dll")]
        public static extern USBXpress.ReturnCodes SI_CheckRXQueue(UInt32 cyHandle, ref UInt32 lpdwNumBytesInQueue, ref USBXpress.RXQueueStatus lpdwQueueStatus);
        */

        public static byte[] tobytearray(int[] entrada)
        {
            byte[] tmp = new byte[entrada.Length * 2];
            for (int i = 0; i < entrada.Length; i++)
            {
                tmp[2 * i] = (byte)(entrada[i] & 0xff);
                tmp[2 * i + 1] = (byte)(entrada[i] >> 8);
            }
            return tmp;
        }

        public static void toarray(byte[] b, ref int despla, float valor)
        {
            unsafe
            {
                byte* p = (byte*)&valor;
                for (int i = 0; i < 4; i++)
                    b[despla + i] = p[3 - i];
                despla += 4;
            }
        }

        public static void toarray(byte[] b, ref int despla, Int16 valor)
        {
            unsafe
            {
                byte* p = (byte*)&valor;
                for (int i = 0; i < 2; i++)
                    b[despla + i] = p[1 - i];
                despla += 2;
            }
        }

        public static void toarray(byte[] b, ref int despla, Int32 valor)
        {
            unsafe
            {
                byte* p = (byte*)&valor;
                for (int i = 0; i < 4; i++)
                    b[despla + i] = p[3 - i];
                despla += 4;
            }
        }
        public static void toarray_le(byte[] b, ref int despla, float valor)
        {
            unsafe
            {
                byte* p = (byte*)&valor;
                for (int i = 0; i < 4; i++)
                    b[despla + i] = p[i];
                despla += 4;
            }
        }

        public static void toarray_le(byte[] b, ref int despla, int valor)
        {
            unsafe
            {
                byte* p = (byte*)&valor;
                for (int i = 0; i < 4; i++)
                    b[despla + i] = p[i];
                despla += 4;
            }
        }

        public static void toarray(byte[] b, ref int despla, byte valor)
        {
            b[despla] = valor;
            despla += 1;
        }


        public static byte tobyte(byte[] b, ref int despla)
        {
            byte result;
            result = b[despla];
            despla++;
            return result;
        }

        public static int tochar(byte[] b, ref int despla)
        {
            int result;
            result = (char)b[despla];
            if (result >= 128)
                result = result - 256;
            despla++;
            return result;
        }
        public static string tostring(byte[] b, ref int despla, int longitud)
        {
            string cad = "";
            for (int i = 0; i < longitud; i++)
                cad += (char)b[i + despla];
            despla += longitud;
            return cad;
        }

        public static Int16 toint16(byte[] b, ref int despla)
        {   // Bitconverter
            // El Keil usa big endian para enteros??
            int result;
            result = (int)b[despla + 1];
            result += 256 * (int)b[despla];
            despla += 2;
            if (result > 32767)
                result = result - 65536;
            return (Int16)result;
        }

        public static int toint32(byte[] b, ref int despla)
        {
            int result;
            
            unsafe
            {
                byte* p = (byte*)&result;
                for (int i = 0; i < 4; i++)
                    p[3 - i] = b[despla + i];
            }
            despla += 4;
            return result;
        }

        public static int toint32_le(byte[] b, ref int despla)
        {
            int result = BitConverter.ToInt32(b, despla);
            /*
            unsafe
            {
                byte* p = (byte*)&result;
                for (int i = 0; i < 4; i++)
                    p[i] = b[despla + i];
            }
            */
            despla += 4;
            return result;
        }

        public static float tofloat(byte[] b, ref int despla)
        {
            byte[] buff = new byte[4];
            Array.Copy(b, despla, buff, 0, 4);
            Array.Reverse(buff);
            float result = BitConverter.ToSingle(buff, 0);
            /*
            float result;
            unsafe
            {
                byte* p = (byte*)&result;
                for (int i = 0; i < 4; i++)
                    p[3 - i] = b[despla + i];
            }*/
            despla += 4;
            return result;
        }

        public static float tofloat_le(byte[] b, ref int despla)
        {
            float result = BitConverter.ToSingle(b, despla);
            /*
            float result;
            unsafe
            {
                byte* p = (byte*)&result;
                for (int i = 0; i < 4; i++)
                    p[i] = b[despla + i];
            }
            */
            despla += 4;
            return result;
        }

        public static string tobin(byte b)
        {
            string s = "";
            int i;
            for (i = 0; i < 8; i++)
            {
                s = s + ((b > 127) ? '1' : '0');
                b = (byte)(b * 2);
            }
            return s;
        }
    }
}
