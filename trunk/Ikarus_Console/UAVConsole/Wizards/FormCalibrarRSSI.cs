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
    public partial class FormCalibrarRSSI : Form
    {
        Singleton me = Singleton.GetInstance();

        public FormCalibrarRSSI()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();
        }

        float GetValue()
        {
            float valor;
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                valor = ikarus.ReadADCs(FlightPlanUSB.ADC_VALUES.RSSI);
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

        private void buttonSetOffset_Click(object sender, EventArgs e)
        {
            numericUpDownRSSImax.Value = (decimal)GetValue();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDownRSSImin.Value = (decimal)GetValue();
        }

        private void buttonFIN_Click(object sender, EventArgs e)
        {
            FlightPlanUSB ikarus = new FlightPlanUSB();
            if (ikarus.IsOpen())
            {
                IkarusBasicConfig cfg = ikarus.ReadConfig();
                cfg.rssi_min = (float)numericUpDownRSSImin.Value;
                cfg.rssi_max = (float)numericUpDownRSSImax.Value;
                ikarus.WriteConfig(cfg);

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
            this.Close();
        }

        void TextSpanish()
        {
            label4.Text = "Emisora Encendida";
            label5.Text = "Valor MAX";
            label1.Text = "Emisora Apagada";
            label6.Text = "Valor MIN";
            buttonFIN.Text = "FIN";
            buttonSetOffset.Text = "Fijar";
            button1.Text = "Fijar";
            this.Text = "Calibrar RSSI";
        }

        void TextEnglish()
        {
            label4.Text = "Transmitter ON";
            label5.Text = "Max. Value";
            label1.Text = "Transmitter OFF";
            label6.Text = "Min. Value";
            buttonFIN.Text = "END";
            buttonSetOffset.Text = "Set";
            button1.Text = "Set";
            this.Text = "Calibrate RSSI";
        }
    }
}
