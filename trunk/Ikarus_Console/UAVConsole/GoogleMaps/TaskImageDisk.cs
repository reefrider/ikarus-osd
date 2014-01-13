using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Drawing;

namespace UAVConsole.GoogleMaps
{
    public class TaskImageDisk
    {
        ImageProxy imageProxy;
        Thread thread;
        Queue<ImageQuery> queue;
        TaskImageInet inet;

        public TaskImageDisk(ImageProxy imageProxy)
        {
            this.imageProxy = imageProxy;
            queue = new Queue<ImageQuery>();
            inet = new TaskImageInet(imageProxy);
        }

        public void Destroy()
        {
            if (thread != null)
                thread.Abort();
            if (inet != null)
                inet.Destroy();
        }

        public void AddElement(ImageQuery elem)
        {
            if(!queue.Contains(elem))
            {
                queue.Enqueue(elem);
            }

            if (thread == null)
            {
                thread = new Thread(new ParameterizedThreadStart(tarea));
                thread.IsBackground = true;
                thread.Start(this);                
            }

        }

        void tarea(object obj)
        {
            TaskImageDisk t = (TaskImageDisk)obj;
            while (true)
            {
                if (t.queue.Count > 0)
                {
                    ImageQuery query = t.queue.Dequeue();
                    Bitmap bmp = t.imageProxy.getDiskCachedImage(query.x, query.y, query.zoom, query.mode);
                    if (bmp != null)
                    {
                        t.imageProxy.imageCache.putImage(query.x, query.y, query.zoom, query.mode, bmp);
                        t.imageProxy.listeners.Invoke();
                    }
                    else
                    {
                        bmp = t.imageProxy.getImageInterpolated(query.x, query.y, query.zoom, query.mode, Origen.DISK);
                        t.imageProxy.imageCache.putImage(query.x, query.y, query.zoom, query.mode, bmp);
                   
                        inet.AddElement(new ImageQuery(query.x, query.y, query.zoom, query.mode));
                    }
                }
                else
                    Thread.Sleep(100);
            }

        }
    }
}
