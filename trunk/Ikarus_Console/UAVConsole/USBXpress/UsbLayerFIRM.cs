using System;
using System.Collections.Generic;
using System.Text;

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
    class UsbLayerFIRM
    {
        protected UInt32 handle = 0;
        protected const int ILEN = 64;
        protected const int OLEN = 64;
        bool isOpen = false;
        string Serial;
        protected USBXpress usbXpress = new USBXpress();

        public UsbLayerFIRM()
            : this(null)
        {

        }

        public UsbLayerFIRM(string serial)
        {
            this.Serial = serial;
            Open();
        }

        public bool Open()
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

        public byte[] ClearPage(int addr)
        {
            return WriteRAW(0, addr, null);
        }

        public byte [] WriteData(int addr, byte[] buff)
        {
            return WriteRAW(1, addr, buff);
        }

        public byte[] ReadData(int addr)
        {
            return ReadRAW(2, addr);
        }

        public void FirmwareUpdate(int pages)
        {
            int addr = pages <<8 | 0x55;
            WriteRAW(3, addr, null);
        }

        protected byte[] WriteRAW(int cmd, int addr, byte[] buff)
        {
            if (this.IsOpen())
            {
                byte[] buffer = new byte[64];
                const int max_payload = 32;
                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);
                buffer[0] = (byte)'F';
                buffer[1] = (byte)'R';
                buffer[2] = (byte)'M';
                buffer[3] = (byte)cmd;
                buffer[4] = (byte)(addr & 0xff);
                buffer[5] = (byte)((addr >> 8) & 0xff);

                if (buff != null)
                {
                    for (int i = 0; i < max_payload && i < buff.Length; i++)
                        buffer[6 + i] = buff[i];
                }
                ret = usbXpress.SI_Write(handle, buffer, OLEN, ref nbytes, 0);
                 buffer = WaitACK_USB();

                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return buffer;
            }
            else
                return null;
        }



        protected byte[] ReadRAW(int cmd, int addr)
        {
            if (this.IsOpen())
            {
                byte[] buff = new byte[64];
                byte[] buffer = new byte[64];
                uint numbytes = 0;
                int i;

                int nbytes = 0;
                USBXpress.ReturnCodes ret = USBXpress.ReturnCodes.SI_SUCCESS;

                ClearInputBuffer(handle);
                buffer[0] = (byte)'F';
                buffer[1] = (byte)'R';
                buffer[2] = (byte)'M';
                buffer[3] = (byte)cmd;
                buffer[4] = (byte)(addr & 0xff);
                buffer[5] = (byte)((addr>>8) & 0xff);


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
                for (i = 0; i < 64 ; i++)
                    buff[i] = buffer[i];

                if (ret != USBXpress.ReturnCodes.SI_SUCCESS)
                    isOpen = false;

                return buff;
            }
            else
                return null;
        }

        
       
        public bool IsOpen()
        {
            if (this.isOpen)
                return true;
            else
                return Open();
        }

        protected byte[] WaitACK_USB()
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
            return buffer;
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

