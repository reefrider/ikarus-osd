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
using UAVConsole.USBXpress;

namespace UAVConsole.Wizards
{
    public partial class FormDefaultConfig : Form
    {
        Singleton me = Singleton.GetInstance();

        public FormDefaultConfig()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            comboBoxCanalPPM.SelectedIndex = 0;
            comboBoxGPSBaudRate.SelectedIndex = 5;
            comboBoxModoPPM.SelectedIndex = 0;
            comboBoxVideoSystem.SelectedIndex = 0;
            comboBoxTipoMezcla.SelectedIndex = 0;
        }

        private void comboBoxModoPPM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxModoPPM.SelectedIndex == 0)
            {
                comboBoxCanalPPM.Enabled = false;
                comboBoxTipoMezcla.Enabled = false;
            }
            else
            {
                comboBoxCanalPPM.Enabled = true;
                comboBoxTipoMezcla.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             FlightPlanUSB fp = new FlightPlanUSB();
             if (fp.IsOpen())
             {
                 IkarusBasicConfig basiccfg = new IkarusBasicConfig();
                 basiccfg.LoadDefaults();

                 basiccfg.LoadFromXmlString(global::UAVConsole.Properties.Resources.osd_config);
                 
                 basiccfg.videoPAL = (byte)(1 - comboBoxVideoSystem.SelectedIndex);
                 basiccfg.BaudRate = (byte)comboBoxGPSBaudRate.SelectedIndex;
                 basiccfg.Modo_PPM = (byte)comboBoxModoPPM.SelectedIndex;
                 basiccfg.PPM_Channel = (byte)(comboBoxCanalPPM.SelectedIndex+4);

                 TimeSpan rafa=TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime());
                 basiccfg.TimeZone = (sbyte)rafa.Hours;
              
                 fp.WriteConfig(basiccfg);

                 IkarusAutopilotConfig autocfg = new IkarusAutopilotConfig();
                 autocfg.LoadDefaults();
                 
                 autocfg.LoadFromXmlString(global::UAVConsole.Properties.Resources.autopilot_config);
                 
                 autocfg.tipo_mezcla = (byte)comboBoxTipoMezcla.SelectedIndex;
                 fp.WriteConfigAutopilot(autocfg);

                 
                 if (checkBoxActualizarHUDs.Checked)
                 {
                     IkarusScreenConfig scr = new IkarusScreenConfig();
                     scr.LoadFromXmlString(global::UAVConsole.Properties.Resources.HUD1);
                     fp.WriteScreen(0, scr);    // HUD 0
                     scr.LoadFromXmlString(global::UAVConsole.Properties.Resources.HUD2);
                     fp.WriteScreen(1, scr);    // HUD 1
                     scr.LoadFromXmlString(global::UAVConsole.Properties.Resources.HUD3);
                     fp.WriteScreen(2, scr);    // HUD 2
                     scr.LoadFromXmlString(global::UAVConsole.Properties.Resources.Failsafe);
                     fp.WriteScreen(3, scr);    // FailSafe
                     scr.LoadFromXmlString(global::UAVConsole.Properties.Resources.Resumen);
                     fp.WriteScreen(4, scr);    // Resumen
                 }

                 if (checkBoxActualizarCharSet.Checked)
                 {
                     MemoryStream stream = new MemoryStream(global::UAVConsole.Properties.Resources.Ikarus);
                     FileCharset fc = new FileCharset(new StreamReader(stream));
                     byte[] buff;

                     for(int i=0;i<256;i++)
                     {
                         buff = fc.getChar((byte)i);
                         fp.WriteCharSet(i, buff);
                     }
                 }
                 
                 fp.Close();

                 if (me.Idioma == 0)
                     MessageBox.Show("Realizado!");
                 else
                     MessageBox.Show("Done!");
                 this.Close();
             }
             else
                 if (me.Idioma == 0)
                     MessageBox.Show("No esta conectado!");
                 else
                     MessageBox.Show("Not connected!");
        }
        void TextSpanish()
        {
            label1.Text = "Configurar Defecto";
            label16.Text = "Sistema Video";
            label18.Text = "Velocidad GPS";
            label26.Text = "Entrada Control";
            label27.Text = "Canal PPM";
            label28.Text = "Mezcla";
            checkBoxActualizarHUDs.Text = "Actualizar Pantallas OSD";
            checkBoxActualizarCharSet.Text = "Actualizar Juego Caracteres";
            this.Text = "Configuración Inicial";
        }

        void TextEnglish()
        {
            label1.Text = "Default Config";
            label16.Text = "Video System";
            label18.Text = "GPS Baud Rate";
            label26.Text = "Control Input";
            label27.Text = "PPM Channel";
            label28.Text = "Mix Mode";
            checkBoxActualizarHUDs.Text = "Update OSD Screens";
            checkBoxActualizarCharSet.Text = "Update Charset";
            this.Text = "Initial Config";
        }
    }
}
