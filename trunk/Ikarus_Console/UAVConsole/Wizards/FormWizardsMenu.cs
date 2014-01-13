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
    public partial class FormWizardsMenu : Form
    {
        Singleton me = Singleton.GetInstance();
        public FormWizardsMenu()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormConfigurarCopilot().Show(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new FormCalibrarRSSI().Show(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FormCalibrarAmperimetro().Show(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new FormDefaultConfig().Show(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new FormCalibrarCopilot().Show(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new FormCalibrarServos().Show(this);
         }

        private void button7_Click_1(object sender, EventArgs e)
        {
            new FormConfigurarServos().Show(this);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {
                IkarusCompleteConfig full = new IkarusCompleteConfig();

                full.IkarusBasicConfig = fp.ReadConfig();
                full.IkarusAutopilotConfig = fp.ReadConfigAutopilot();
                full.IkarusScreenConfig1 = fp.ReadScreen(0);
                full.IkarusScreenConfig2 = fp.ReadScreen(1);
                full.IkarusScreenConfig3 = fp.ReadScreen(2);
                full.IkarusScreenConfigFailSafe = fp.ReadScreen(3);
                full.IkarusScreenConfigResumen = fp.ReadScreen(4);
                fp.Close();

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.ShowDialog();
                if (dlg.FileName != "")
                {
                    full.SaveToXml(dlg.FileName);
                    if (me.Idioma == 0)
                        MessageBox.Show("Guardado con exito!");
                    else
                        MessageBox.Show("Saved Succesful!");

                }
                else
                    if (me.Idioma == 0)
                        MessageBox.Show("No se ha salvado!");
                    else
                        MessageBox.Show("Not saved!");
            }
            else
                if (me.Idioma == 0)
                    MessageBox.Show("No esta conectado!");
                else
                    MessageBox.Show("Not connected!");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.ShowDialog();
                if (dlg.FileName != "")
                {
                    IkarusCompleteConfig full = new IkarusCompleteConfig();

                    full.LoadFromXml(dlg.FileName);

                    fp.WriteConfig(full.IkarusBasicConfig);
                    fp.WriteConfigAutopilot(full.IkarusAutopilotConfig);
                    fp.WriteScreen(0, full.IkarusScreenConfig1);
                    fp.WriteScreen(1, full.IkarusScreenConfig2);
                    fp.WriteScreen(2, full.IkarusScreenConfig3);
                    fp.WriteScreen(3, full.IkarusScreenConfigFailSafe);
                    fp.WriteScreen(4, full.IkarusScreenConfigResumen);

                    fp.Close();
                    if (me.Idioma == 0)
                        MessageBox.Show("Restaurado con exito!");
                    else
                        MessageBox.Show("Succesfully restored!");

                }
                else
                    if (me.Idioma == 0)
                        MessageBox.Show("No se ha especificado nombre de fichero");
                    else
                        MessageBox.Show("File Name not specified!");
                    
            }
            else
                if (me.Idioma == 0)
                    MessageBox.Show("No esta conectado!");
                else
                    MessageBox.Show("Not connected!");
        }

        void TextSpanish()
        {
            button4.Text = "Calibrar Sensor IR";
            button2.Text = "Calibrar RSSI";
            button3.Text = "Calibrar Sensor I && V";
            button1.Text = "Configurar Sensor IR";
            button5.Text = "Calibrar Servos";
            button6.Text = "Configurar Defecto";
            button8.Text = "Leer Config. Completa";
            button9.Text = "Salvar Config. Completa";
            button7.Text = "Configurar Servos";
            this.Text = "Asistentes";
        }
        void TextEnglish()
        {
            button4.Text = "Calibrate IR Sensor";
            button2.Text = "Calibrate RSSI";
            button3.Text = "Calibrate I && V Sensor";
            button1.Text = "Configure IR Sensor";
            button5.Text = "Calibrate Servos";
            button6.Text = "Default Config";
            button8.Text = "Load Full Config";
            button9.Text = "Save Full Config";
            button7.Text = "Configure Servos";
            this.Text = "Wizards";
        }

    }
}
