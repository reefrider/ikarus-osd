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
using System.Drawing;
using System.Net;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace UAVConsole.GoogleMaps
{
    public enum Modes {MAP,SAT, MAP_OVERLAY, MAP_SAT, TOPO, NULL};
    public enum Origen { MEM, DISK };
    

    public abstract class ImageProxy
    {
        protected static string rootPath = @"D:\Ikarus UAV project\GoogleMaps\ImagesMaps";
        
        
        private Modes displaymode = Modes.SAT;
        protected int server_balance = 0;

        public delegate void Listener();
        public Listener listeners;



        TaskImageDisk taskDisk;
        public ImageCache imageCache;
        

        public ImageProxy():this(150)
        {
        }

        public ImageProxy(int max)
        {
            imageCache = new ImageCache(max);
            taskDisk = new TaskImageDisk(this);

            rootPath = Singleton.GetInstance().CacheMapsPath;         
        }

        public void Destroy()
        {
            taskDisk.Destroy();
        }

        public void setMode(Modes m)
        {
            displaymode = m;
            //imageCache.Clean();
        }

        public Modes getMode()
        {
            return displaymode;
        }
        public bool hasMode(Modes m)
        {
            Modes[] modos = getModes();
            for (int i = 0; i < modos.Length; i++)
                if (modos[i] == m)
                    return true;
            return false;
        }

        public abstract Modes[] getModes();
        protected abstract String getDiskFilePath(int x, int y, int zoom, Modes mode);
        protected abstract String getInternetURL(int x, int y, int zoom, Modes mode, int server_balance);

        protected virtual HttpWebResponse getHttpWebResponse(string url)
        {
            WebRequest request = WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
        /*
        protected virtual Bitmap getInternetImage(int x, int y, int zoom, Modes mode)
        {
            string url = getInternetURL(x, y, zoom, mode);
            HttpWebResponse response = getHttpWebResponse(url);

            Stream stream = response.GetResponseStream();
            Bitmap bmp2 = new Bitmap(stream);
            return bmp2;

        }
        */
        /*
        public void internetDiskCachedImage(int x, int y, int zoom, Modes mode)
        {
            server_balance = (server_balance + 1) % 4;
            internetDiskCachedImage(x, y, zoom, mode, server_balance);

        }
        */
        public virtual void internetDiskCachedImage(int x, int y, int zoom, Modes mode, int server_balance)
        {
            string url = getInternetURL(x, y, zoom, mode, server_balance);
            HttpWebResponse response = getHttpWebResponse(url);
            
            Stream sin = response.GetResponseStream();
            int encontrado = response.ContentType.IndexOf("image");
            if (encontrado >= 0)
            {
                byte[] b = new byte[response.ContentLength];
                int res = 0;
                while ((res += sin.Read(b, res, b.Length - res)) < response.ContentLength) ;

                string path = getDiskFilePath(x, y, zoom, mode);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);   

                string filepath = getDiskFileName(x, y, zoom, mode);
                if(!File.Exists(filepath))
                {
                    FileStream sout = new FileStream(filepath, FileMode.OpenOrCreate);
                    sout.Write(b, 0, res);
                    sout.Close();
                    sin.Close();
                }
            }
        }


        public Bitmap getDiskCachedImage(int x, int y, int zoom, Modes mode)
        {
            string path = getDiskFileName(x, y, zoom, mode);
            if (!File.Exists(path))
                return null;
            else
            {
                Bitmap bmp = new Bitmap(path);
                return bmp;
            }
        }

        

        protected string getDiskFileName(int x, int y, int zoom, Modes mode)
        {
            string path = getDiskFilePath(x, y, zoom, mode);
            return path +  "X" + x + "Y" + y + ".JPG";
        }
        
        protected Bitmap getImage(int x, int y, int zoom, Modes mode)
        {
            Bitmap srcImage;
            
            srcImage = null;
            srcImage = imageCache.getImage(x, y, zoom, mode);
            if (srcImage == null)
            {
                srcImage = getImageInterpolated(x, y, zoom, mode, Origen.MEM);
                imageCache.putImage(x, y, zoom, mode, srcImage);
                this.taskDisk.AddElement(new ImageQuery(x, y, zoom, mode));
            }
            if (srcImage != null)
                return srcImage;
            else if (mode == Modes.MAP_OVERLAY)
                return new Bitmap(256, 256);
            else
                return global::UAVConsole.Properties.Resources.default_texture;
        }

        public Bitmap getImageInterpolated(int x, int y, int zoom, Modes mode, Origen orig)
        {
            Bitmap srcImage = getImageFromLessZoom(x, y, zoom, mode, orig, 4);

            if (srcImage == null)
            {
                if (mode == Modes.MAP_OVERLAY)
                    srcImage = new Bitmap(256, 256);
                else
                    srcImage = global::UAVConsole.Properties.Resources.default_texture;
            }
            Bitmap tmp = getImageFromMoreZoom(x, y, zoom, mode, orig, 2);
            if (tmp != null)
            {
                Graphics g = Graphics.FromImage(srcImage);
                g.DrawImage(tmp, 0, 0);
                g.Dispose();
            }

            return srcImage;
        }

        Bitmap getImageFromMoreZoom(int x, int y, int zoom, Modes mode, Origen orig, int depth)
        {
            Bitmap srcImage = null;
            Bitmap tmp = null;
            int i, j;


            for (i = 0; i < 2; i++)
                for (j = 0; j < 2; j++)
                {
                    if (orig == Origen.MEM)
                        tmp = imageCache.getImage(x * 2 + i, y * 2 + j, zoom + 1, mode);
                    else
                        tmp = getDiskCachedImage(x * 2 + i, y * 2 + j, zoom + 1, mode);

                    if (tmp == null && depth > 0)
                    {
                        tmp = getImageFromMoreZoom(x * 2 + i, y * 2 + j, zoom + 1, mode, orig, depth - 1);
                    }
                    if (tmp != null)
                    {

                        if (srcImage == null)
                            srcImage = new Bitmap(256, 256);

                        Graphics g = Graphics.FromImage(srcImage);

                        g.DrawImage(tmp, new Rectangle(i * 128, j * 128, 128, 128));
                        g.Dispose();
                    }
                }

            return srcImage;
        }

        Bitmap getImageFromLessZoom(int x, int y, int zoom, Modes mode, Origen orig, int depth)
        {
            // Interpolar desde menos zoom
            Bitmap srcImage=null;
            Bitmap tmp;
            if(orig == Origen.MEM)
                tmp = imageCache.getImage(x / 2, y / 2, zoom - 1, mode);
            else
                tmp = getDiskCachedImage(x / 2, y / 2, zoom - 1, mode);
            
            if (tmp == null && depth > 0)
            {
                tmp = getImageFromLessZoom(x / 2, y / 2, zoom - 1, mode, orig, depth - 1);
            }
            
            if (tmp != null)
            {
                srcImage = new Bitmap(tmp.Width, tmp.Height);
                Graphics g = Graphics.FromImage(srcImage);
                                
                g.DrawImage(tmp, 
                    new Rectangle(0, 0, tmp.Width, tmp.Height), 
                    new Rectangle(x % 2 * (srcImage.Width / 2-1), y % 2 * (srcImage.Height / 2-1), srcImage.Width / 2, srcImage.Height / 2), GraphicsUnit.Pixel);
               
                g.Dispose();
                //tmp.Dispose();
            }
           
            return srcImage;
        }

       

        public Image getTexel(int x, int y, int zoom) {
            Image img;
            int max = 1 << zoom;
            if (x >= max) {
                x = x % max;
            } else if (x < 0) {
                x += max;
            }

            if (y >= max) {
                y = y % max;
            } else if (y < 0) {
                y += max;
            }
            if (hasMode(displaymode))
                img = getImage(x, y, zoom, displaymode);
            else if (displaymode == Modes.MAP_SAT && hasMode(Modes.SAT) && hasMode(Modes.MAP_OVERLAY))
            {
                img = new Bitmap(256, 256);
                Graphics g = Graphics.FromImage(img);
                  
                Image img1=getImage(x, y, zoom, Modes.SAT);
                if (img1 != null)
                    g.DrawImage(img1, 0, 0);
                Image img2 = getImage(x, y, zoom, Modes.MAP_OVERLAY);
                if (img2 != null)
                    g.DrawImage(img2, 0, 0);
                g.Dispose();
                
            }
            else
                img = null;
            return img;
        }

    }
}

