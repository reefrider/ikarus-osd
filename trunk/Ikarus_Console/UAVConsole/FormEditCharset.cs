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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UAVConsole.ConfigClasses;
using System.IO;

namespace UAVConsole
{
    public partial class FormEditCharset : Form
    {
        const int fsize = 9;
        
        FileCharset charsetA = new FileCharset();
        FileCharset charsetB = new FileCharset();
        byte[] character = new FileCharset().getChar(0);

        public FormEditCharset()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            if (dialog.FileName!="")
                textBox1.Text = dialog.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
                //charsetA = new FileCharset(textBox1.Text);
            try
            {
                charsetA.load(textBox1.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error abriendo fichero. Usando buildin");
                MemoryStream stream = new MemoryStream(global::UAVConsole.Properties.Resources.Ikarus);
                charsetA.load(new StreamReader(stream));
            }
            panel1.Invalidate();
            
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            panel1.Width = 16 * (FileCharset.CharWidth + 2)+2;
            panel1.Height = 16 * (FileCharset.CharHeight + 2)+2;
            Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
            Graphics g = Graphics.FromImage(bmp);
            
            paintCharset(g, charsetA);
            e.Graphics.DrawImage(bmp, 0, 0);
            g.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            if (dialog.FileName != "")
                textBox2.Text = dialog.FileName;           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                charsetB.load(textBox2.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error abriendo fichero. Usando buildin");
                MemoryStream stream = new MemoryStream(global::UAVConsole.Properties.Resources.Ikarus);
                charsetB.load(new StreamReader(stream));
            }    
            panel2.Invalidate();
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
             panel2.Width = 16 * (FileCharset.CharWidth + 2)+2;
             panel2.Height = 16 * (FileCharset.CharHeight + 2)+2;
            
            paintCharset(e.Graphics, charsetB);               
        }

        private void paintCharset(Graphics g, FileCharset charset)
        {
            for (int f = 0; f < 16; f++)
                for (int c = 0; c < 16; c++)
                {
                    g.DrawRectangle(Pens.Black, c * (FileCharset.CharWidth + 2), f * (FileCharset.CharHeight + 2),
                        FileCharset.CharWidth + 1, FileCharset.CharHeight + 1);
                    if (charset != null)
                    {
                        Bitmap bmp = charset.getCharBitmap((byte)(f * 16 + c));
                        g.DrawImageUnscaled(bmp, c * (FileCharset.CharWidth + 2) + 1, f * (FileCharset.CharHeight + 2) + 1);
                    }
                }

        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            mouseEvent(panel1, e, charsetA);
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            mouseEvent(panel2, e, charsetB);
        }

        void mouseEvent(Panel panel, MouseEventArgs e, FileCharset charset)
        {
            int c = e.X / (FileCharset.CharWidth + 2);
            int f = e.Y / (FileCharset.CharHeight + 2);
            byte id = (byte)((f * 16) + c);
            if (e.Button == MouseButtons.Left)
            {
                character = charset.getChar(id);
                panel3.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (charset!=null && character != null)
                    charset.setChar(id, character);
                panel.Invalidate();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            charsetA.clear();
            panel1.Invalidate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            charsetB.clear();
            panel2.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                charsetA.save(textBox1.Text);
            }
            else
                MessageBox.Show("Debe indicar un nombre de fichero");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                charsetB.save(textBox2.Text);
            }
            else
                MessageBox.Show("Debe indicar un nombre de fichero");
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

            panel3.Width = FileCharset.CharWidth * fsize + 2;
            panel3.Height = FileCharset.CharHeight * fsize + 2;
            Bitmap bmp = new Bitmap(FileCharset.CharWidth*fsize+2, FileCharset.CharHeight*fsize+2);
            Graphics g = Graphics.FromImage(bmp);
            Color c;
            for (int filas = 0; filas < FileCharset.CharHeight; filas++)
                for (int cols = 0; cols < FileCharset.CharWidth; cols++)
                {
                    int i = filas * 3 + cols / 4;
                    byte b = character[i];
                    b = (byte)((b >> (6 - 2 * (cols % 4))) & 3);
                    if (b == 0)
                        c = Color.Black;
                    else if (b == 2)
                        c = Color.White;
                    else
                        c = Color.Transparent;
                    g.DrawRectangle(Pens.Black, cols * fsize, filas * fsize, fsize, fsize);
                    g.FillRectangle(new SolidBrush(c), cols * fsize+1, filas * fsize+1, fsize-1, fsize-1);
                    //bmp.SetPixel(cols, filas, c);
                }

            e.Graphics.DrawImage(bmp, 0, 0);
            g.Dispose();
         
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            int c = e.X / fsize;
            int f = e.Y / fsize;
  
            int i = f * 3 + c / 4;
            byte b = character[i];
            int valor = (byte)((b >> (6 - 2 * (c % 4))) & 3);
            int mask =(byte)~( 3 << (6 - 2 * (c % 4)));

            if (e.Button == MouseButtons.Left)
            {
                valor = (valor + 1) % 3;   
            }
            else if (e.Button == MouseButtons.Right)
            {
                valor = (valor + 2) % 3;
            }
            int value = valor << (6 - 2 * (c % 4));
            b = (byte)(b & mask | value);
            character[i] = b;
            panel3.Invalidate();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < character.Length; i++)
                character[i] = 85;
            panel3.Invalidate();
        }

        private void button10_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < character.Length; i++)
            {
                int v = character[i];
                if ((v & 0x3) != 0x1)
                    v = v ^ 0x2;
                if ((v & 0xC) != 0x4)
                    v = v ^ 0x8;
                if ((v & 0x30) != 0x10)
                    v = v ^ 0x20;
                if ((v & 0xC0) != 0x40)
                    v = v ^ 0x80;
                character[i] = (byte)v;
            }
            panel3.Invalidate();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] tmp = (byte[])character.Clone();
            for (int i = 0; i < FileCharset.CharHeight; i++)
            {
                character[3 * i] = tmp[3 * (FileCharset.CharHeight - i-1)];
                character[3 * i + 1] = tmp[3 * (FileCharset.CharHeight - i-1) + 1];
                character[3 * i + 2] = tmp[3 * (FileCharset.CharHeight - i-1) + 2];
            }
            panel3.Invalidate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] tmp = (byte[])character.Clone();
            for (int i = 0; i < FileCharset.CharHeight; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int v = tmp[3*i+j];
                    int w = 0;
                    w = (v & 3) << 6;
                    w |= (v & 0xC) << 2;
                    w |= (v & 0x30) >> 2;
                    w |= (v & 0xC0) >> 6;
                    character[3*i+2-j] = (byte)w;
                }
            }
            panel3.Invalidate();
        }
    }
}
