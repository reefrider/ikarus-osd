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
    public partial class FormCalibrarAmperimetro : Form
    {
        Singleton me = Singleton.GetInstance();

        public FormCalibrarAmperimetro()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();
            textBoxMotorV2.Text = "" + 0.0f;
            textBoxVideoV2.Text = "" + 0.0f;
        }

        float GetValue(FlightPlanUSB.ADC_VALUES canal)
        {
            float valor;
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                valor = ikarus.ReadADCs(canal);
                ikarus.Close();
            }
            else
            {
                if (me.Idioma == 0)
                    MessageBox.Show("No esta conectado!");
                else
                    MessageBox.Show("Not connected!");
                valor = 0.0f;
            }
            return valor;
        }

        public void TextSpanish()
        {
           
            this.Text = "Calibrar Amperimetro y Voltimetros";
            button4.Text = "Salvar";
            button5.Text = "Salvar";
            button8.Text = "Salvar";
            button2.Text = "Leer";
            button3.Text = "Leer";
            button6.Text = "Leer";
            button7.Text = "Leer";
            button9.Text = "Leer";
            button10.Text = "Leer";
        }

        public void TextEnglish()
        {
           
            this.Text = "Calibrate Amperimeter and Voltimeters";
            button4.Text = "Save";
            button5.Text = "Save";
            button8.Text = "Save";
            button2.Text = "Set";
            button3.Text = "Set";
            button6.Text = "Set";
            button7.Text = "Set";
            button9.Text = "Set";
            button10.Text = "Set";
        }

        private void numericUpDownVideoV1_ValueChanged(object sender, EventArgs e)
        {
            textBoxVideoV1.Text = "";
        }

        private void numericUpDownVideoV2_ValueChanged(object sender, EventArgs e)
        {
            textBoxVideoV2.Text = "";
        }

        private void numericUpDownMotorV1_ValueChanged(object sender, EventArgs e)
        {
            textBoxMotorV1.Text = "";
        }

        private void numericUpDownMotorV2_ValueChanged(object sender, EventArgs e)
        {
            textBoxMotorV2.Text = "";
        }

        private void numericUpDownMotorI1_ValueChanged(object sender, EventArgs e)
        {
            textBoxMotorI1.Text = "";
        }

        private void numericUpDownMotorI2_ValueChanged(object sender, EventArgs e)
        {
            textBoxMotorI2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxVideoV1.Text = GetValue(FlightPlanUSB.ADC_VALUES.V2).ToString();
        }

       
        private void button3_Click(object sender, EventArgs e)
        {
            textBoxVideoV2.Text = GetValue(FlightPlanUSB.ADC_VALUES.V2).ToString();

        }
        private void button10_Click(object sender, EventArgs e)
        {
            textBoxMotorV1.Text = GetValue(FlightPlanUSB.ADC_VALUES.V1).ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBoxMotorV2.Text = GetValue(FlightPlanUSB.ADC_VALUES.V1).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBoxMotorI1.Text = GetValue(FlightPlanUSB.ADC_VALUES.I).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBoxMotorI2.Text = GetValue(FlightPlanUSB.ADC_VALUES.I).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                float v1, v2;
                if (float.TryParse(textBoxVideoV1.Text, out v1) && float.TryParse(textBoxVideoV2.Text, out v2))
                {
                    float gain = (float)(numericUpDownVideoV1.Value - numericUpDownVideoV2.Value) / (v1 - v2);
                    float offset = (float)numericUpDownVideoV1.Value - gain * float.Parse(textBoxVideoV1.Text);
                    IkarusBasicConfig cfg = ikarus.ReadConfig();
                    cfg.sensorV2_gain = gain;
                    cfg.sensorV2_offset = -offset;
                    ikarus.WriteConfig(cfg);
                    if (me.Idioma == 0)
                    {
                        MessageBox.Show("Valores guardados correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Values saved sucesfully");
                    }
                }
                else if (me.Idioma == 0)
                {
                    MessageBox.Show("Error en los valores capturados");
                }
                else
                {
                    MessageBox.Show("Error in captured values");
                }
                ikarus.Close();
            }
            else if (me.Idioma == 0)
            {
                MessageBox.Show("Error abriendo disp USB");
            }
            else
            {
                MessageBox.Show("Error opening USB device!");
            }
       }

        private void button8_Click(object sender, EventArgs e)
        {
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                float v1, v2;
                if (float.TryParse(textBoxMotorV1.Text, out v1) && float.TryParse(textBoxMotorV2.Text, out v2))
                {
                    float gain = (float)(numericUpDownMotorV1.Value - numericUpDownMotorV2.Value) / (v1 - v2);
                    float offset = (float)numericUpDownMotorV1.Value - gain * float.Parse(textBoxMotorV1.Text);
                    IkarusBasicConfig cfg = ikarus.ReadConfig();
                    cfg.sensorV1_gain = gain;
                    cfg.sensorV1_offset = -offset;
                    ikarus.WriteConfig(cfg);
                    if (me.Idioma == 0)
                    {
                        MessageBox.Show("Valores guardados correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Values saved sucesfully");
                    }
                }
                else if (me.Idioma == 0)
                {
                    MessageBox.Show("Error en los valores capturados");
                }
                else
                {
                    MessageBox.Show("Error in captured values");
                }
                ikarus.Close();
            }
            else if (me.Idioma == 0)
            {
                MessageBox.Show("Error abriendo disp USB");
            }
            else
            {
                MessageBox.Show("Error opening USB device!");
            }
            ikarus.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                float v1, v2;
                if (float.TryParse(textBoxMotorI1.Text, out v1) && float.TryParse(textBoxMotorI2.Text, out v2))
                {
                    float gain = (float)(numericUpDownMotorI1.Value - numericUpDownMotorI2.Value) / (v1 - v2);
                    //float offset = -(float)numericUpDownMotorI1.Value - gain * float.Parse(textBoxMotorI1.Text);
                    float offset = float.Parse(textBoxMotorI1.Text) - (float)numericUpDownMotorI1.Value/ gain;
                    IkarusBasicConfig cfg = ikarus.ReadConfig();
                    cfg.sensorI_gain = gain;
                    cfg.sensorI_offset = offset;
                    ikarus.WriteConfig(cfg);
                    if (me.Idioma == 0)
                    {
                        MessageBox.Show("Valores guardados correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Values saved sucesfully");
                    }
                }
                else if (me.Idioma == 0)
                {
                    MessageBox.Show("Error en los valores capturados");
                }
                else
                {
                    MessageBox.Show("Error in captured values");
                }
                ikarus.Close();
            }
            else if (me.Idioma == 0)
            {
                MessageBox.Show("Error abriendo disp USB");
            }
            else
            {
                MessageBox.Show("Error opening USB device!");
            }
            ikarus.Close();
        }

        private void FormCalibrarAmperimetro_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
