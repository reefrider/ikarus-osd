using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UAVConsole.Intrumentos
{
    public partial class Instrumento_Altimeter : UserControl
    {
        Bitmap altimeter = global::UAVConsole.Properties.Resources.alt2;
        Bitmap misc3 = global::UAVConsole.Properties.Resources.misc3;
        
        Bitmap needle;
        Bitmap needle1k;

        float _altitude;

        Timer mitimer;
        int incremento;
        float _calibration;
        bool parar;

        public Instrumento_Altimeter()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        
            altimeter.SetResolution(96, 96);
            misc3.SetResolution(96, 96);
        
            needle = misc3.Clone(new RectangleF(0.8671875f * misc3.Width, (1 - 0.45703125f) * misc3.Height, 28, 117), misc3.PixelFormat);
            needle1k = misc3.Clone(new RectangleF(0.75f * misc3.Width, (1 - 0.4296875f) * misc3.Height, 30, 110), misc3.PixelFormat);

            mitimer = new Timer();
            mitimer.Interval = 100;
            mitimer.Tick += new EventHandler(mitimer_Tick);

        }

        public float Calibration
        {
            get
            {
                return _calibration;
            }
            set
            {
                _calibration = value;
                Invalidate();
            }
        }
        
        public float Altitude
        {
            get
            {
                return _altitude;
            }

            set
            {
                _altitude = value;
                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Math.Abs(e.X - this.Width / 2) < 10 && Math.Abs(e.Y - this.Height / 2) < 10)
            {
                _calibration = _altitude;
                this.Invalidate();
            }
            //base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            parar = true;
            //base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
           
            if (e.X < this.Width / 2-10)
            {
                incremento =20;
                parar = false;
                mitimer.Start();
            
            }
            else if (e.X > this.Width / 2 + 10)
            {
                incremento = -20;
                parar = false;
                mitimer.Start();
            }
            //base.OnMouseDown(e);
        }

        void mitimer_Tick(object sender, EventArgs e)
        {
            _calibration += incremento;
            if (parar)
                mitimer.Stop();
            
            this.Invalidate();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Bitmap bmp = new Bitmap(altimeter.Width, altimeter.Height);
            Graphics g = Graphics.FromImage(bmp);
            float altitud = _altitude - _calibration;

            g.Clear(BackColor);
            g.DrawImage(altimeter, 0, 0);

            g.TranslateTransform(128, 128);

            System.Drawing.Drawing2D.GraphicsState saved = g.Save();

            g.RotateTransform(altitud * 360 / 10000);
            g.DrawImage(needle1k, -needle1k.Width / 2, -needle1k.Height / 2 - 16);
            
            g.Restore(saved);

            g.RotateTransform(altitud * 360 / 1000);
            g.DrawImage(needle, -needle.Width / 2, -needle.Height / 2 - 26);
            
            g.Dispose();
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
