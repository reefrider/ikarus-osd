using System;
using System.Collections.Generic;
using System.Text;

namespace UAVConsole.GoogleMaps
{
    public class ImageQuery
    {
        public int x;
        public int y;
        public int zoom;
        public Modes mode;

        public ImageQuery()
            : this(-1, -1, -1, Modes.NULL)
        {
        }
        public ImageQuery(int x, int y, int zoom, Modes mode)
        {
            this.x = x;
            this.y = y;
            this.zoom = zoom;
            this.mode = mode;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                ImageQuery t = (ImageQuery)obj;
                if (t.x == this.x && t.y == this.y && t.zoom == this.zoom && t.mode.Equals(this.mode))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
