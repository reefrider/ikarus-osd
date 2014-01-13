using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    class Hilo
    {
        public int id;
        public TaskImageInet inet;

        public Hilo(int id, TaskImageInet inet)
        {
            this.id = id;
            this.inet = inet;
        }
    }

    public class TaskImageInet
    {
        ImageProxy imageProxy;
        Thread []thread;
        Queue<ImageQuery> queue;


        public TaskImageInet(ImageProxy imageProxy)
        {
            this.imageProxy = imageProxy;
            queue = new Queue<ImageQuery>();
        }

        public void Destroy()
        {
            if (thread != null)
            {
                for (int i = 0; i < thread.Length; i++)
                {
                    if (thread[i] != null)
                        thread[i].Abort();
                }
            }
        }

        public void AddElement(ImageQuery elem)
        {
            try
            {
                if (!queue.Contains(elem))
                {
                    queue.Enqueue(elem);
                }
            }
            catch (Exception) { } // Mirar por que lanza una excepcion a veces


            if (thread == null)
            {
                thread = new Thread[4];
                for (int i = 0; i <thread.Length; i++)
                {
                    thread[i] = new Thread(new ParameterizedThreadStart(tarea));
                    thread[i].IsBackground = true;
                    thread[i].Start(new Hilo(i,this));
                }
            }

        }

        void tarea(object obj)
        {
            Hilo h = (Hilo)obj;
            while (true)
            {
                if (h.inet.queue.Count > 0)
                {
                    ImageQuery query = h.inet.queue.Dequeue();
                    try
                    {
                        h.inet.imageProxy.internetDiskCachedImage(query.x, query.y, query.zoom, query.mode, h.id);
                        Bitmap bmp = h.inet.imageProxy.getDiskCachedImage(query.x, query.y, query.zoom, query.mode);
                        if (bmp != null)
                        {
                            h.inet.imageProxy.imageCache.putImage(query.x, query.y, query.zoom, query.mode, bmp);
                            h.inet.imageProxy.listeners.Invoke();
                        }
                    }
                    catch (Exception) { }
                }
                else
                    Thread.Sleep(100);
            }

        }
    }
}
