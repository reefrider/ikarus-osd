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

namespace UAVConsole.GoogleMaps
{
    public interface IMapBackground
    {
        void Destroy();
        void SetCenter(WayPoint wpt);
        void SetMode(Modes modo);
        void SetZoom(int zoom);
        int GetZoom();
        double GetLongitude(double dx);
        double GetLatitude(double dy);
        double getdX(WayPoint wpt);
        double getdY(WayPoint wpt);
        void setX(double x);
        void setY(double y);
        double getX();
        double getY();
        void OnPaint(Graphics grp, int Width, int Height);
    }
}
