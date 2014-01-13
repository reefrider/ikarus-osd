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
using System.Runtime.InteropServices;

namespace UAVConsole
{
    public class Joystick_WMM
    {
        public enum wCaps
        {
            JOYCAPS_HASZ = 0x0001,
            JOYCAPS_HASR = 0x0002,
            JOYCAPS_HASU = 0x0004,
            JOYCAPS_HASV = 0x0008,
            JOYCAPS_HASPOV = 0x0010,
            JOYCAPS_POV4DIR = 0x0020,
            JOYCAPS_POVCTS = 0x0040
        }

        public enum dwFlags
        {
            JOY_RETURNBUTTONS = 0x80,
            JOY_RETURNCENTERED = 0x400,
            JOY_RETURNPOV = 0x40,
            JOY_RETURNR = 0x08,
            JOY_RETURNU = 0x10,
            JOY_RETURNV = 0x20,
            JOY_RETURNX = 0x01,
            JOY_RETURNY = 0x02,
            JOY_RETURNZ = 0x04,
            JOY_RETURNALL = (JOY_RETURNX | JOY_RETURNY | JOY_RETURNZ | JOY_RETURNR | JOY_RETURNU | JOY_RETURNV | JOY_RETURNPOV | JOY_RETURNBUTTONS)
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct JOYCAPS
        {
            public uint pid;
            public fixed sbyte szPname[32];
            public uint wXmin;
            public uint wXmax;
            public uint wYmin;
            public uint wYmax;
            public uint wZmin;
            public uint wZmax;
            public uint wNumButtons;
            public uint wPeriodMin;
            public uint wPeriodMax;
            public uint wRmin;
            public uint wRmax;
            public uint wUmin;
            public uint wUmax;
            public uint wVmin;
            public uint wVmax;
            public uint wCaps;
            public uint wMaxAxes;
            public uint wNumAxes;
            public uint wMaxButtons;
            public fixed sbyte szRegKey[32];
            public fixed sbyte szOEMVxD[260];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFOEX
        {
            public int dwSize;                            // size of structure
            public int dwFlags;                           // flags to indicate what to return
            public int dwXpos;                            // x position
            public int dwYpos;                            // y position
            public int dwZpos;                            // z position
            public int dwRpos;                            // rudder/4th axis position
            public int dwUpos;                            // 5th axis position
            public int dwVpos;                            // 6th axis position
            public int dwButtons;                         // button states
            public int dwButtonNumber;                    // current button number pressed
            public int dwPOV;                             // point of view state
            public int dwReserved1;                       // reserved for communication between winmm driver
            public int dwReserved2;                       // reserved for future expansion
        }

        [DllImport("WinMM.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern int joyGetDevCapsA(uint uJoyID, ref JOYCAPS caps, uint size);

        [DllImport("WinMM.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern int joyGetNumDevs();

        [DllImport("WinMM.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern int joyGetPosEx(uint uJoyID, ref JOYINFOEX pji);

        [DllImport("WinMM.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern int joyGetThreshold(uint uJoyID, ref uint uThreshold);

        [DllImport("WinMM.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern int joySetThreshold(uint uJoyID, uint uThreshold);

        public static JOYINFOEX getJoyPosEx(uint uJoyID)
        {
            JOYINFOEX joyinfo = new JOYINFOEX();
            joyinfo.dwFlags = 0xff;
            joyinfo.dwSize = 13 * 4;
            joyGetPosEx(uJoyID, ref joyinfo);
            return joyinfo;
        }
    }
}
