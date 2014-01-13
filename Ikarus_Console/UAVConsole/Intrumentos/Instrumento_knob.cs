using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UAVConsole.Intrumentos
{
    public partial class Instrumento_knob : UserControl
    {
        Bitmap knob_red = global::UAVConsole.Properties.Resources.Know_rojo;
        Bitmap knob_blue = global::UAVConsole.Properties.Resources.Know_azul;
        Timer mitimer;
        
        int incremento;
        int _valor;
        bool parar;
        bool _manual = false;

        public bool Manual
        {
            get
            {
                return _manual;
            }

            set
            {
                _manual = value;
                Invalidate();
            }
        }

        public int Valor
        {
            get
            {
                return _valor;
            }
            set
            {
                if (value > 180)
                    _valor = value - 360;
                else if (value <= -180)
                    _valor = value + 360;
                else
                    _valor = value;
                Invalidate();
            }
        }
        
        public Instrumento_knob()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        

            mitimer = new Timer();
            mitimer.Interval = 100;
            mitimer.Tick += new EventHandler(mitimer_Tick);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(BackColor);

            if (_manual)
            {
                Pen pen = new Pen(Brushes.White, 2);
                g.DrawImage(knob_blue, 0, 0, bmp.Width, bmp.Height);
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(_valor);
                g.DrawLine(pen, 0, 0, 0, -bmp.Height / 2);
            
            }
            else
            {
                g.DrawImage(knob_red, 0, 0, bmp.Width, bmp.Height);
            }
            g.Dispose();
            e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            int w = this.Width;
            int h = this.Height;
            int half = (w + h) / 2;
            this.Width = half;
            this.Height = half;
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Manual = !Manual;
           //base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            parar = true;
            //base.OnMouseUp(e);
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_manual)
            {
                float x = e.X - this.Width / 2;
                float y = e.Y - this.Height / 2;
                float radius = (float)Math.Sqrt(x * x + y * y);
                float angle = (float)(Math.Atan2(y, x) * 180.0 / Math.PI);



                if (e.Button == MouseButtons.Right)
                {
                    if (x > 0)
                        incremento = 5;
                    else
                        incremento = -5;
                    parar = false;
                    mitimer.Start();
                }
                else if (e.Button == MouseButtons.Left && radius > this.Width / 4 && radius < this.Width / 2)
                {
                    incremento = 0;
                    parar = true;
                    Valor = (int)angle + 90;
                }

            }
            else
                parar = true;
            //base.OnMouseDown(e);
        }

        void mitimer_Tick(object sender, EventArgs e)
        {
            Valor += incremento;
            if (parar)
                mitimer.Stop();
            this.Invalidate();
        }
    }
}
