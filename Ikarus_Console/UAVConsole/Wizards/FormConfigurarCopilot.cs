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
using UAVConsole.USBXpress;

namespace UAVConsole.Wizards
{
    public partial class FormConfigurarCopilot : Form
    {
        float THR=0.15f;

        Bitmap ir_bmp = UAVConsole.Properties.Resources.Copilot;
        float off_p, off_r;
        
        int position;
        int position2;
            
        bool IR_UP, IR_DOWN, IR_LEFT, IR_RIGHT;

        Singleton me = Singleton.GetInstance();

        public FormConfigurarCopilot()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            lblPitch1.Text = "      ";
            lblPitch2.Text = "      ";
            lblRoll1.Text = "      ";
            lblRoll2.Text = "      ";

            off_p = (float)numericUpDown1.Value;
            off_r = (float)numericUpDown2.Value;
            
            timer1.Enabled = true;
        }

        void GetValues(ref float p, ref float r)
        {
            float []valores;
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                valores = ikarus.ReadADCs();
                ikarus.Close();
                p = valores[(int)FlightPlanUSB.ADC_VALUES.CO_X];
                r = valores[(int)FlightPlanUSB.ADC_VALUES.CO_Y];
            }
            else
            {
                p = 1.66f;
                r = 1.66f;
                //MessageBox.Show("No esta conectado!");
            }
         }
      
        private void pictureBox1_Paint_1(object sender, PaintEventArgs e)
        {
            int w=40;
            int h=10;
            Graphics g = e.Graphics;
            //g.DrawImageUnscaled(ir_bmp, 0, 0);

            if (IR_UP)
                g.FillRegion(Brushes.Red, new Region(new Rectangle(pictureBox1.Width / 2 - w / 2, 0, w, h)));
            if(IR_DOWN)
                g.FillRegion(Brushes.Red, new Region(new Rectangle(pictureBox1.Width / 2 - w / 2, pictureBox1.Height-h, w, h)));
            if(IR_LEFT)
                g.FillRegion(Brushes.Red, new Region(new Rectangle(0, pictureBox1.Height/2-w/2, h, w)));
            if(IR_RIGHT)
                g.FillRegion(Brushes.Red, new Region(new Rectangle(pictureBox1.Width-h, pictureBox1.Height / 2 - w / 2, h, w)));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            float p=0, r=0;
            GetValues(ref p, ref r);
            off_p = p;
            off_r = r;
            numericUpDown1.Value = (decimal)p;
            numericUpDown2.Value = (decimal)r;

            lblPitch1.Text = "      ";
            lblPitch2.Text = "      ";
            lblRoll1.Text = "      ";
            lblRoll2.Text = "      ";
            buttonFIN.Enabled = false;
            buttonSetRollSensor.Enabled = false;
            buttonSetPitchSensor.Enabled = true;
        }

        private void buttonSetPitchSensor_Click(object sender, EventArgs e)
        {
            if (IR_UP  && (IR_LEFT || IR_RIGHT))
            {
                lblPitch1.Text = "  UP  ";
                lblPitch2.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                position = (IR_LEFT) ? 7 : 1;
                buttonSetRollSensor.Enabled = true;
            }
            else if (IR_DOWN  && (IR_LEFT || IR_RIGHT))
            {
                lblPitch1.Text = " DOWN ";
                lblPitch2.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                position = (IR_LEFT) ? 5 : 3;
                buttonSetRollSensor.Enabled = true;
            }
            else if (IR_UP || IR_DOWN)
            {
                lblPitch1.Text = (IR_UP) ? "  UP  " : " DOWN ";
                lblPitch2.Text = "      ";

                position = (IR_UP) ? 0 : 4;
            
                buttonSetRollSensor.Enabled = true;
            }
            else if (IR_LEFT || IR_RIGHT)
            {
                lblPitch1.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                lblPitch2.Text = "      ";

                position = (IR_LEFT) ? 6 : 2;
                buttonSetRollSensor.Enabled = true;
            }
            else
            {
                lblPitch1.Text = "      ";
                lblPitch2.Text = "      ";
                buttonSetRollSensor.Enabled = false;
            }
            lblRoll1.Text = "      ";
            lblRoll2.Text = "      ";
            buttonFIN.Enabled = false;

        }

        private void buttonSetRollSensor_Click(object sender, EventArgs e)
        {
            if (IR_UP  && (IR_LEFT || IR_RIGHT))
            {
                lblRoll1.Text = "  UP  ";
                lblRoll2.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                position2 = (IR_LEFT) ? 7 : 1;
            
                //buttonSetRollSensor.Enabled = true;
            }
            else if (IR_DOWN && (IR_LEFT || IR_RIGHT))
            {
                lblRoll1.Text = " DOWN ";
                lblRoll2.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                position2 = (IR_LEFT) ? 5 : 3;

                //buttonSetRollSensor.Enabled = true;
            }
            else if (IR_UP || IR_DOWN)
            {
                lblRoll1.Text = (IR_UP) ? "  UP  " : " DOWN ";
                lblRoll2.Text = "      ";

                position2 = (IR_UP) ? 0 : 4;

                //buttonSetRollSensor.Enabled = true;
            }
            else if (IR_LEFT || IR_RIGHT)
            {
                lblRoll1.Text = (IR_LEFT) ? " LEFT " : " RIGHT ";
                lblRoll2.Text = "      ";
                position2 = (IR_LEFT) ? 6 : 2;
                
               // buttonSetRollSensor.Enabled = true;
            }
            else
            {
                position2 = -1;
                lblRoll1.Text = "      ";
                lblRoll2.Text = "      ";
                //buttonSetRollSensor.Enabled = false;
            }

            if (position2 >= 0)
            {
                if (position == ((position2 + 2) % 8))
                    buttonFIN.Enabled = true;
                else if (position == ((position2 + 6) % 8))
                    buttonFIN.Enabled = true;
                else
                    buttonFIN.Enabled = false;
            }
            else
                buttonFIN.Enabled = false;
            
        }

        private void buttonFIN_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                IkarusAutopilotConfig autocfg = ikarus.ReadConfigAutopilot();
                autocfg.x_off = off_r;
                autocfg.y_off = off_p;

                switch (position)
                {
                    case 0: 
                        autocfg.IR_cross_sensor = 0;
                        autocfg.IR_cross_rev = 0;
                        autocfg.IR_pitch_rev = 1;
                        autocfg.IR_roll_rev = (byte)((position2 == 2) ? 0 : 1);
                        break;

                    case 1:
                        autocfg.IR_cross_sensor = 1;
                        autocfg.IR_cross_rev = 0;
                        autocfg.IR_pitch_rev = 1;
                        autocfg.IR_roll_rev = (byte)((position2 == 3) ? 1 : 0);
                        break;
                    
                    case 2:
                        autocfg.IR_cross_sensor = 0;
                        autocfg.IR_cross_rev = 1;
                        autocfg.IR_pitch_rev = 1;
                        autocfg.IR_roll_rev = (byte)((position2 == 4) ? 1 : 0);
                        break;
                    
                    case 3:
                        autocfg.IR_cross_sensor = 1;
                        autocfg.IR_cross_rev = 1;
                        autocfg.IR_pitch_rev = 0;
                        autocfg.IR_roll_rev = (byte)((position2 == 5) ? 1 : 0);
                        break;
                    
                    case 4:
                        autocfg.IR_cross_sensor = 0;
                        autocfg.IR_cross_rev = 0;
                        autocfg.IR_pitch_rev = 0;
                        autocfg.IR_roll_rev = (byte)((position2 == 6) ? 1 : 0);
                        break;
                    
                    case 5:
                        autocfg.IR_cross_sensor = 1;
                        autocfg.IR_cross_rev = 0;
                        autocfg.IR_pitch_rev = 0;
                        autocfg.IR_roll_rev = (byte)((position2 == 7) ? 0 : 1);
                        break;
                    
                    case 6:
                        autocfg.IR_cross_sensor = 0;
                        autocfg.IR_cross_rev = 1;
                        autocfg.IR_pitch_rev = 0;
                        autocfg.IR_roll_rev = (byte)((position2 == 0) ? 0 : 1);
                        break;
                    
                    case 7:
                        autocfg.IR_cross_sensor = 1;
                        autocfg.IR_cross_rev = 1;
                        autocfg.IR_pitch_rev = 1;
                        autocfg.IR_roll_rev = (byte)((position2 == 1) ? 0 : 1);
                        break;
                }

                ikarus.WriteConfigAutopilot(autocfg);
                ikarus.Close();
            }           
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            buttonSetOffset.Enabled = true;
            
            float ir_p=0, ir_r=0;
            GetValues(ref ir_p, ref ir_r);
            labelP.Text = String.Format("P: {0,0:0.00}", ir_p);
            labelR.Text = String.Format("R: {0,0:0.00}", ir_r);
            
            if (ir_p - off_p > THR)
            {
                IR_UP = true;
                IR_DOWN = false;
            }
            else if (off_p - ir_p > THR)
            {
                IR_UP = false;
                IR_DOWN = true;
            }
            else
            {
                IR_UP = false;
                IR_DOWN = false;
            }

            if (ir_r - off_r > THR)
            {
                IR_RIGHT = true;
                IR_LEFT = false;
            }
            else if (off_r - ir_r > THR)
            {
                IR_RIGHT = false;
                IR_LEFT = true;
            }
            else
            {
                IR_RIGHT = false;
                IR_LEFT = false;
            }
            pictureBox1.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            off_p = (float)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            off_r = (float)numericUpDown2.Value;
        }

        void TextSpanish()
        {
            label4.Text = "Valores centrales";
            label7.Text = "Cabeceo abajo";
            label12.Text = "Alabeo derecha";
            buttonFIN.Text = "FIN";
            buttonSetOffset.Text = "Fijar";
            buttonSetPitchSensor.Text = "Fijar";
            buttonSetRollSensor.Text = "Fijar";
            this.Text = "Configurar sensor IR";
        }

        void TextEnglish()
        {
            label4.Text = "Center Offsets";
            label7.Text = "Pitch Down";
            label12.Text = "Roll Right";
            buttonFIN.Text = "END";
            buttonSetOffset.Text = "Set";
            buttonSetPitchSensor.Text = "Set";
            buttonSetRollSensor.Text = "Set";
            this.Text = "Configure IR Sensor";
        }
    }
}
