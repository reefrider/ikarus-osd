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
    public partial class FormCalibrarCopilot : Form
    {
        float x_offset, y_offset, x_max, y_max;

        Singleton me = Singleton.GetInstance();

        public FormCalibrarCopilot()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();
        }


        void GetValues(ref float p, ref float r)
        {
            float[] valores;
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
                if (me.Idioma == 0)
                    MessageBox.Show("No esta conectado!");
                else
                    MessageBox.Show("Not connected!");
            }
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            GetValues(ref x_offset, ref y_offset);
            label1.Enabled = true;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetValues(ref x_max, ref y_max);
            //label2.Enabled = true;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                IkarusAutopilotConfig autocfg = ikarus.ReadConfigAutopilot();
                
                float total;
                float co_x = Math.Abs(x_max - x_offset);
                float co_y = Math.Abs(y_max - y_offset);
                
                autocfg.x_off = x_offset;
                autocfg.y_off = y_offset;

                if (autocfg.IR_cross_sensor == 0)
                {
                    if (autocfg.IR_cross_rev == 0)
                        total = co_x;
                    else
                        total = co_y;
                }
                else
                {
                    total = co_x + co_y;
                }
                autocfg.IR_max = total;
                ikarus.WriteConfigAutopilot(autocfg);
                ikarus.Close();
            }
            else
                MessageBox.Show("No conectado!");
            this.Close();
        }

        public void TextSpanish()
        {
            label4.Text = "Mantenta el avion nivelado";
            label1.Text = "Coloque el avion vertical";
            button3.Text = "FIN";
            this.Text = "Calibrar Sensor IR";
        }

        public void TextEnglish()
        {
            label4.Text = "Keep plane level to ground";
            label1.Text = "Put plane in vertical";
            button3.Text = "END";
            this.Text = "Calibrate IR Sensor";
        }
    }
}
