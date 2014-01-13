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
using UAVConsole.Intrumentos;
using UAVConsole.USBXpress;

namespace UAVConsole.Wizards
{
    public partial class FormCalibrarServos : Form
    {
        Singleton me = Singleton.GetInstance();
        FlightPlanUSB ikarus = new FlightPlanUSB();
        int[] valores;
        int[] minimos;
        int[] maximos;

        public FormCalibrarServos()
        {
            InitializeComponent();
            InitValores();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            timer1.Enabled = true;
        }

        void InitValores()
        {
            minimos = new int[7];
            maximos = new int[7];
            
            for (int i = 0; i < minimos.Length; i++)
            {
                int valor;
                if (valores != null)
                {
                    valor = valores[i];
                    if (valor < 500 || valor > 2500)
                        valor = 1500;
                }
                else
                    valor = 1500;
                minimos[i] = valor;
                maximos[i] = valor;
            }
            UpdateLabels();
        }

        void UpdateLabels()
        {
            Label[] labelMinimos = { labelCtrlMin, labelAilMin, labelEleMin, labelThrMin, labelTailMin, labelPanMin, labelAuxMin };
            Label[] labelMaximos = { labelCtrlMax, labelAilMax, labelEleMax, labelThrMax, labelTailMax, labelPanMax, labelAuxMax };
            for (int i = 0; i < labelMinimos.Length; i++)
            {
                labelMinimos[i].Text = minimos[i].ToString();
                labelMaximos[i].Text = maximos[i].ToString();
                labelMinimos[i].Invalidate();
                labelMaximos[i].Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ikarus.IsOpen())
            {
                IkarusAutopilotConfig cfg = ikarus.ReadConfigAutopilot();
                ServoInfo[] servos = { cfg.servo_ctrl, cfg.servo_ail, cfg.servo_ele, cfg.servo_thr, cfg.servo_tail, cfg.servo_pan, cfg.servo_aux };
                for (int i = 0; i < servos.Length; i++)
                {
                    servos[i].min = (short)minimos[i];
                    servos[i].max = (short)maximos[i];
                    if (i == 0 || i == 3)// Thr
                        servos[i].center = (short)((servos[i].max + servos[i].min) / 2);
                    else
                        servos[i].center = (short)valores[i];

                }
                ikarus.WriteConfigAutopilot(cfg);
                if (me.Idioma == 0)
                    MessageBox.Show("Calibración realizada!");
                else
                    MessageBox.Show("Calibration done!");
            }
            else
            {
                if (me.Idioma == 0)
                    MessageBox.Show("Error abriendo conexión con Ikarus");
                else
                    MessageBox.Show("Error opening conection with Ikarus");
            }  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!ikarus.IsOpen())
            {
                ikarus = new FlightPlanUSB();
            }
            else
            {
                valores = ikarus.ReadServos();
                UpdateValues();
            }
            
        }

        void UpdateValues()
        {
            IndicadorSlider[] sliders = { indicadorSlider1, indicadorSlider2, indicadorSlider3, indicadorSlider4, indicadorSlider5, indicadorSlider6, indicadorSlider7 };
         
            for (int i = 0; i < valores.Length && i < sliders.Length; i++)
            {
                sliders[i].PosFin = valores[i] / 1000.0f - 1;
                sliders[i].PosInicio = 0.5f;
                sliders[i].Texto = valores[i].ToString();
                sliders[i].Invalidate();
                if (i < maximos.Length)
                {
                    if (valores[i] > maximos[i])
                        maximos[i] = valores[i];
                }
                if (i < minimos.Length)
                {
                    if (valores[i] < minimos[i])
                        minimos[i] = valores[i];
                }
            }

            UpdateLabels();
        }

        void TextSpanish()
        {
            label1.Text = "Control";
            label2.Text = "Alerones";
            label3.Text = "Elevador";
            label4.Text = "Motor";
            label5.Text = "Cola";

            button2.Text = "Reinicia";
            button1.Text = "Actualizar";
            this.Text = "Calibrar Servos";
        }

        void TextEnglish()
        {
            label1.Text = "Control";
            label2.Text = "Ailerons";
            label3.Text = "Elevator";
            label4.Text = "Thrust";
            label5.Text = "Tail";

            button2.Text = "Reset";
            button1.Text = "Update";
            this.Text = "Calibrar Servos";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InitValores();
        }

        private void FormConfigurarServos_FormClosing(object sender, FormClosingEventArgs e)
        {
            ikarus.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ikarus.IsOpen())
            {
                IkarusAutopilotConfig cfg = ikarus.ReadConfigAutopilot();
                ServoInfo[] servos = { cfg.servo_ctrl, cfg.servo_ail, cfg.servo_ele, cfg.servo_thr, cfg.servo_tail, cfg.servo_pan, cfg.servo_aux };
                valores = ikarus.ReadServos();
                for (int i = 0; i < servos.Length; i++)
                {
                    if (i != 0 && i != 3)
                        servos[i].center = (short)valores[i];
                }
                ikarus.WriteConfigAutopilot(cfg);
            }
            else
            {
                if (me.Idioma == 0)
                    MessageBox.Show("Error abriendo conexión con Ikarus");
                else
                    MessageBox.Show("Error opening conection with Ikarus");
            }  
        }
    }
}
