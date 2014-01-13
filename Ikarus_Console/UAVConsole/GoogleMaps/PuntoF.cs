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

namespace UAVConsole.GoogleMaps
{
    public class PuntoF
    {
        public float x, y;

        public PuntoF()
        {
        }

        public PuntoF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        /*
        public static bool operator ==(PuntoF a, PuntoF b)
        {
            if ((a.x == b.x) && (a.y == b.y))
                return true;
            else
                return false;
        }

        public static bool operator !=(PuntoF a, PuntoF b)
        {
            if ((a.x != b.x) || (a.y != b.y))
                return true;
            else
                return false;
        }
        */
        public static PuntoF operator+(PuntoF a, PuntoF b)
        {
            PuntoF p = new PuntoF();
            p.x = b.x + a.x;
            p.y = b.y + a.y;
            return p;
        }

        public static PuntoF operator -(PuntoF a)
        {
            PuntoF p = new PuntoF();
            p.x = - a.x;
            p.y = - a.y;
            return p;
        }

        public static PuntoF operator -(PuntoF a, PuntoF b)
        {
            PuntoF p = new PuntoF();
            p.x = a.x - b.x;
            p.y = a.y - b.y;
            return p;
        }

        public static PuntoF operator *(PuntoF a, float f)
        {
            PuntoF p = new PuntoF();
            p.x = f * a.x;
            p.y = f * a.y;
            return p;
        }

        public static PuntoF operator *(float f, PuntoF a)
        {
            PuntoF p = new PuntoF();
            p.x = f * a.x;
            p.y = f * a.y;
            return p;
        }

        public float mod()
        {
            PuntoF a = this;
            return (float)Math.Sqrt(a.x * a.x + a.y * a.y);
        }

        public static float operator *(PuntoF a, PuntoF b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static PuntoF operator /(PuntoF a, float f)
        {
            PuntoF p = new PuntoF();
            p.x = a.x/f;
            p.y = a.y/f;
            return p;
        }

    }
}
