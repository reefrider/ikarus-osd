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

namespace UAVConsole.USBXpress
{
    class USBXpress32
    {
        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_GetNumDevices(ref int lpwdNumDevices);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_GetProductString(int dwDeviceNum, [MarshalAs(UnmanagedType.LPArray)] byte[] lpvDeviceString, USBXpress.ProductString dwFlags);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_Open(UInt32 dwDevice, ref UInt32 cyHandle);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_Close(UInt32 cyHandle);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_Read(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToRead, ref int lpdwBytesReturned, int lpOverlapped);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_Write(UInt32 cyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            int dwBytesToWrite, ref int lpdwBytesWritten, int lpOverlapped);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_SetTimeouts(int dwReadTimeout, int dwWriteTimeout);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_GetTimeouts(ref int lpdwReadTimeout, ref int lpdwWriteTimeout);

        [DllImport("SiUSBXp32.dll")]
        public static extern USBXpress.ReturnCodes SI_CheckRXQueue(UInt32 cyHandle, ref UInt32 lpdwNumBytesInQueue, ref USBXpress.RXQueueStatus lpdwQueueStatus);
    }
}
