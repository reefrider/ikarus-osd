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
using System.Windows.Forms;

namespace UAVConsole.USBXpress
{
    class UsbLayerATC
    {
        protected UInt32 handle = 0;
        protected const int ILEN = 64;
        protected const int OLEN = 64;
        bool isOpen = false;
        string Serial;
        protected USBXpress usbXpress = new USBXpress();

        public UsbLayerATC(): this(null)
        {
            
        }

        public UsbLayerATC(string serial)
        {
            this.Serial = serial;
            Open();
        }
        
        protected bool Open()
        {
            int numdev = 0;

            USBXpress.ReturnCodes status;
            byte[] buffer = new byte[200];

            status = usbXpress.SI_SetTimeouts(500, 500);
            status = usbXpress.SI_GetNumDevices(ref numdev);

            for (int i = 0; i < numdev; i++)
            {
                status = usbXpress.SI_GetProductString(i, buffer, USBXpress.ProductString.SI_RETURN_SERIAL_NUMBER);
                string sn = System.Text.Encoding.ASCII.GetString(buffer);

                if (this.Serial == null || this.Serial.Length == 0 || sn.StartsWith(this.Serial))
                {
                    status = usbXpress.SI_Open((UInt32)i, ref handle);
                    if (status == USBXpress.ReturnCodes.SI_SUCCESS)
                    {
                        this.isOpen = true;
                        ClearInputBuffer(handle);
                        return true;
                    }
                }
            }
            return false;
        }

        public USBXpress.ReturnCodes WriteRAW(int cmd, byte[] buff)
        {
            if (this.IsOpen())
            {
                byte[] buffer = new byte[64];
                const int max_payload = 64 - 4; // 64 (long paquete) - 4 (long. cabecera)
                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);
                buffer[0] = (byte)'A';
                buffer[1] = (byte)'T';
                buffer[2] = (byte)'C';
                buffer[3] = (byte)cmd;

                for (int i = 0; i < max_payload && i < buff.Length; i++)
                    buffer[4 + i] = buff[i];

                ret = usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
                WaitACK_USB();

                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;
                
                return ret;
            }
            else
                return USBXpress.ReturnCodes.SI_WRITE_ERROR;
        }

        public USBXpress.ReturnCodes Write(int cmd, int id, int offset, byte[] buff)
        {
            if (this.IsOpen())
            {
                byte[] buffer = new byte[64];
                const int max_payload = 64 - 7; // 64 (long paquete) - 7 (long. cabecera)
                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                for (int j = 0; j < buff.Length; j += max_payload)
                {
                    ClearInputBuffer(handle);
                    buffer[0] = (byte)'A';
                    buffer[1] = (byte)'T';
                    buffer[2] = (byte)'C';
                    buffer[3] = (byte)((int)cmd | 0x10);
                    buffer[4] = (byte)id;
                    buffer[5] = (byte)(offset + j);
                    if (buff.Length - j >= max_payload)
                        buffer[6] = (byte)max_payload;
                    else
                        buffer[6] = (byte)(buff.Length - j);
                    for (int i = 0; i < max_payload && i < (buff.Length - j); i++)
                        buffer[7 + i] = buff[i + j];

                    ret = usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
                    WaitACK_USB();
                }
                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return ret;
            }
            else
                return USBXpress.ReturnCodes.SI_WRITE_ERROR;
        }

        public byte[] ReadRAW(int cmd, int len)
        {
            if (this.IsOpen())
            {
                byte[] buff = new byte[len];
                byte[] buffer = new byte[64];
                uint numbytes = 0;
                int i;

                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);
                buffer[0] = (byte)'A';
                buffer[1] = (byte)'T';
                buffer[2] = (byte)'C';
                buffer[3] = (byte)cmd;

                for (i = 4; i < 64; i++)
                    buffer[i] = 0;
                usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);

                USBXpress.RXQueueStatus queueStatus = USBXpress.RXQueueStatus.SI_RX_EMPTY;
                numbytes = 0;

                for (i = 0; i < 50 && numbytes < ILEN; i++)
                {
                    usbXpress.SI_CheckRXQueue(handle, ref numbytes, ref queueStatus);
                    Thread.Sleep(20);
                }
                ret = usbXpress.SI_Read(handle, buffer, 64, ref nbytes, 0);
                for (i = 0; i < 64 && i < len; i++)
                    buff[i] = buffer[i];

                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return buff;
            }
            else
                return null;
        }

        public byte[] Read(int cmd, byte id, int offset, int len)
        {
            if (this.IsOpen())
            {
                byte[] buff = new byte[len];
                byte[] buffer = new byte[64];
                const int max_payload = 64; // 64 (long paquete) 
                uint numbytes = 0;

                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);

                for (int j = 0; j < buff.Length; j += max_payload)
                {
                    buffer[0] = (byte)'A';
                    buffer[1] = (byte)'T';
                    buffer[2] = (byte)'C';
                    buffer[3] = (byte)cmd;
                    buffer[4] = (byte)id;
                    buffer[5] = (byte)j;
                    if (len - j > max_payload)
                        buffer[6] = (byte)max_payload;
                    else
                        buffer[6] = (byte)(len - j);
                    for (int i = 7; i < 64; i++)
                        buffer[i] = 0;
                    usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);

                    USBXpress.RXQueueStatus queueStatus = USBXpress.RXQueueStatus.SI_RX_EMPTY;
             
                    numbytes = 0;
                    for (int i = 0; i < 100 && numbytes < ILEN; i++)
                    {
                        usbXpress.SI_CheckRXQueue(handle, ref numbytes, ref queueStatus);
                        Thread.Sleep(10);
                    }
                    if (numbytes >= ILEN)
                    {
                        ret = usbXpress.SI_Read(handle, buffer, 64, ref nbytes, 0);
                        for (int i = 0; i < 64 && i + j < len; i++)
                            buff[j + i] = buffer[i];
                    }
                }
                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return buff;
            }
            else
                return null;
        }
        /*
        public byte[] Read(int cmd, byte id, int offset, int len)
        {
            if (this.IsOpen())
            {
                byte[] buff = new byte[len];
                byte[] buffer = new byte[64];
                const int max_payload = 64; // 64 (long paquete) 
                uint numbytes = 0;

                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);
                buffer[0] = (byte)'A';
                buffer[1] = (byte)'T';
                buffer[2] = (byte)'C';
                buffer[3] = (byte)cmd;
                buffer[4] = (byte)id;
                buffer[5] = (byte)offset;
                buffer[6] = (byte)len;
                for (int i = 7; i < 64; i++)
                    buffer[i] = 0;
                usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);

                for (int j = 0; j < buff.Length; j += max_payload)
                {
                    USBXpress.RXQueueStatus queueStatus = USBXpress.RXQueueStatus.SI_RX_EMPTY;
                    int i;
                    numbytes = 0;
                    for (i = 0; i < 100 && numbytes < ILEN; i++)
                    {
                        usbXpress.SI_CheckRXQueue(handle, ref numbytes, ref queueStatus);
                        Thread.Sleep(10);
                    }
                    if (numbytes >= ILEN)
                    {
                        ret = usbXpress.SI_Read(handle, buffer, 64, ref nbytes, 0);
                        for (i = 0; i < 64 && i + j < len; i++)
                            buff[j + i] = buffer[i];
                    }
                }
                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return buff;
            }
            else
                return null;
        }
         */
        public bool IsOpen()
        {
            if (this.isOpen)
                return true;
            else
                return Open();
        }

        public bool IsOpen2()
        {
            return this.isOpen;
        }

        protected void WaitACK_USB()
        {
            uint numbytes = 0;
            byte[] buffer = new byte[OLEN];
            int nbytes = 0;

            USBXpress.RXQueueStatus queueStatus = USBXpress.RXQueueStatus.SI_RX_EMPTY;
            int i;

            for (i = 0; i < 20 && numbytes != ILEN; i++)
            {
                usbXpress.SI_CheckRXQueue(handle, ref numbytes, ref queueStatus);
                Thread.Sleep(5);
            }
            if (numbytes == ILEN)
            {
                usbXpress.SI_Read(handle, buffer, 64, ref nbytes, 0);
            }
        }

        virtual public void Flush()
        {

        }

        public void Close()
        {
            this.Flush();
            usbXpress.SI_Close(handle);
            isOpen = false;
        }

        public void ClearInputBuffer(UInt32 handle)
        {
            USBXpress.RXQueueStatus queueStatus = USBXpress.RXQueueStatus.SI_RX_EMPTY;
            uint unumbytes = 0;
            int numbytes;
            do
            {
                usbXpress.SI_CheckRXQueue(handle, ref unumbytes, ref queueStatus);
                numbytes = (int)unumbytes;
                usbXpress.SI_Read(handle, new byte[numbytes], numbytes, ref numbytes, 0);
            } while (queueStatus != USBXpress.RXQueueStatus.SI_RX_EMPTY);
        }
    }
}
