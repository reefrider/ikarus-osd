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
using DirectX.Capture;
using System.Threading;
using System.IO.Ports;
using Microsoft.Win32;
using UAVConsole.GoogleMaps;
using UAVConsole.ConfigClasses;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using UAVConsole.Modem;

namespace UAVConsole
{
    public partial class FormArranque : Form
    {
        private FilterCollection videoDevices;
        
        Singleton me = Singleton.GetInstance();
        DateTime buildDate;

        public FormArranque()
        {
            InitializeComponent();
            
           // buildDate = new DateTime(2012, 1, 22);
            Assembly assem = Assembly.GetExecutingAssembly();
            Version vers = assem.GetName().Version;
            buildDate = new DateTime(2000, 1, 1).AddDays(vers.Build).AddSeconds(vers.Revision * 2);
                
            
            // Repara el ultimo KML si es necesario
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Singleton.registryPath, true);
                string lastKML = key.GetValue("LastKML").ToString();
                if (lastKML != null)
                {
                    key.DeleteValue("LastKML");
                    key.Close();
                    StreamWriter swlog_kml = File.AppendText(lastKML);

                    swlog_kml.WriteLine("</coordinates>");
                    swlog_kml.WriteLine("         </LineString>");
                    swlog_kml.WriteLine("       </Placemark>");
                    swlog_kml.WriteLine("   </Folder>");
                    swlog_kml.WriteLine("</kml>");
                    swlog_kml.Close();
                }
            }
            catch (Exception) { }
            
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            buscaVideoDevices(); 
            numericUpDown1.Value = me.cells1;
            numericUpDown2.Value = me.cells2;
             
        }

        void validate()
        {
            if (comboBoxVideoDevice.SelectedItem != null)
                me.videoCaptureStr = comboBoxVideoDevice.SelectedItem.ToString();
            me.cells1 = (int)numericUpDown1.Value;
            me.cells2 = (int)numericUpDown2.Value;   
            me.ToRegistry(); // Singleton.GetInstance().ToRegistry();
        }

        void buscaVideoDevices()
        {
            try
            {
                videoDevices = new Filters().VideoInputDevices;
            }
            catch (Exception)
            {
            }

            if (videoDevices!=null&& videoDevices.Count > 0)
            {
                comboBoxVideoDevice.Items.Clear();
                int _selected_index = 0;
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    comboBoxVideoDevice.Items.Add(videoDevices[i].Name);
                    if (videoDevices[i].Name.Equals(me.videoCaptureStr))
                        _selected_index = i;
                }
                comboBoxVideoDevice.SelectedIndex = _selected_index;
                videoDevices.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            validate();
            new FormIkarusMain().Show(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new FormIkarusConfigOSD().Show(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FormIkarusConfigAutoPilot().Show(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            validate();
            new FormGestionRutas().Show(this); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            validate();
            new FormScreen().Show(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new Wizards.FormWizardsMenu().Show(this);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new FormConfigConsole().Show(this);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            new FormJoystick().Show(this);
        }

        void TextSpanish()
        {
            button1.Text = "Volar!";
            button2.Text = "Gestion Rutas";
            button4.Text = "Gest. Pantallas";
            button5.Text = "Configurar OSD";
            button6.Text = "Config. Joystick";
            button7.Text = "Config. Autopilot";
            button8.Text = "Asistentes";
            button9.Text = "Config. Consola";
            label1.Text = "Dispositivo captura";
            label10.Text = "Nº celdas motor";
            label11.Text = "Nº celdas video";
            this.Text = "Ikarus OSD";
        }

        void TextEnglish()
        {
            button1.Text = "FLY!";
            button2.Text = "Route Manager";
            button4.Text = "Screen Config";
            button5.Text = "OSD Config";
            button6.Text = "Joystick Config";
            button7.Text = "Autopilot Config";
            button8.Text = "Wizards";
            button9.Text = "Console Config";
            label1.Text = "Capture Device";
            label10.Text = "Cells # motor";
            label11.Text = "Cells # video";
            this.Text = "Ikarus OSD";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FormConfigurarTeclas().Show(this);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            if (buildDate != null)
            {
                Graphics g = e.Graphics;
                string text = "Build #" + buildDate.Year.ToString("0000") +"." +buildDate.Month.ToString("00") +"."+ buildDate.Day.ToString("00");

                SizeF size = g.MeasureString(text, DefaultFont);
                g.DrawString(text, DefaultFont, Brushes.White, pictureBox1.Width - size.Width, pictureBox1.Height - size.Height);
            }
            
        }


    }
}