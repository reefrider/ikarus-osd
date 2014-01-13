using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    
    public class ImageCache
    {
        private int cacheSize;
        private Bitmap[] cacheImage;
        private int[] cachelastaccess;
        private ImageQuery[] cacheQuery;
        private int cachetimer;
        /*
        public ImageCache()
            : this(40)
        {
        }
        */
        public ImageCache(int max)
        {
            cacheSize = max;
            cacheImage = new Bitmap[max];
            cacheQuery = new ImageQuery[max];
            cachelastaccess = new int[max];
            cachetimer = 0;

            Clean();
        }

        public void Clean()
        {
            for (int i = 0; i < cacheQuery.Length; i++)
            {
                cacheQuery[i] = new ImageQuery();
            }
        }

        public Bitmap getImage(int x, int y, int zoom, Modes mode)
        {
            int i;
            cachetimer++;

            for (i = 0; i < cacheSize; i++)
                if (cacheQuery[i].x == x && cacheQuery[i].y == y && cacheQuery[i].zoom == zoom && cacheQuery[i].mode.Equals(mode))
                {
                    cachelastaccess[i] = cachetimer;
                    return cacheImage[i];
                }
            return null;
        }

        public void putImage(int x, int y, int zoom, Modes mode, Bitmap img)
        {
            int older = 0, i;
            bool encontrado = false;
            try
            {
                if (img != null && img.Height * img.Width > 0)
                {
                    for (i = 0; i < cacheSize && !encontrado; i++)
                    {
                        if (cachelastaccess[i] < cachelastaccess[older])
                            older = i;
                        if (cacheQuery[i].x == x && cacheQuery[i].y == y && cacheQuery[i].zoom == zoom && cacheQuery[i].mode.Equals(mode))
                        {
                            cachelastaccess[i] = cachetimer;
                            cacheImage[i] = img;
                            encontrado = true;
                        }
                    }

                    if (!encontrado)
                    {
                        cacheImage[older] = img;
                        cachelastaccess[older] = cachetimer;

                        cacheQuery[older].x = x;
                        cacheQuery[older].y = y;
                        cacheQuery[older].zoom = zoom;
                        cacheQuery[older].mode = mode;
                    }
                }
            }
            catch (Exception) { Console.WriteLine("Rafa"); }
        }
    }
}
