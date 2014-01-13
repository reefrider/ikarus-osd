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
using System.IO.Ports;
using System.Globalization;

namespace UAVConsole
{
    public partial class FormConfigConsole : Form
    {

        Singleton me = Singleton.GetInstance();

        public FormConfigConsole()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            buscaCOMs();
            comboBoxModem.SelectedIndex = (int)me.telemetria;
            comboBoxVideoSist.SelectedIndex = (int)me.videosystem;
            comboBoxCommBps.SelectedIndex = comboBoxCommBps.Items.IndexOf(me.commBps.ToString());
            textBoxCacheMapsPath.Text = me.CacheMapsPath;
            textBoxFlightLogPath.Text = me.FlightLogPath;
            textBoxPicturePath.Text = me.PicturePath;
            textBoxVideosPath.Text = me.VideosPath;

            checkBoxWebServer.Checked = me.enableWebServer;
            checkBoxUDP.Checked = me.enableUDPinout;
            textBoxUDPport.Text = me.portUDPinout.ToString();
            textBoxWebServer.Text = me.portWebServer.ToString();
            textBoxNombrePiloto.Text = me.NombrePiloto;
            comboBoxLanguage.SelectedIndex = me.Idioma;
            comboBoxSistemaMetrico.SelectedIndex = me.SistemaMetrico;

            try
            {
                comboBoxModuloTX.SelectedIndex = (int)me.moduloTX;
                comboBoxNumCellsUplink.SelectedIndex = (int)(me.uplinkNumCells - 2);
                checkBoxLipoUplink.Checked = me.uplinkLipo;
                numericUpDownVminUplink.Value = (decimal) me.uplinkVmin;
                numericUpDownVmaxUplink.Value = (decimal) me.uplinkVmax;
                numericUpDownValarmUplink.Value = (decimal) me.uplinkValarm;
                updateUplinkBattery();
            }
            catch (Exception)
            {
            }

           

            try
            {
                textBoxAlarmaAltitud.Text = me.AlarmAltitude.ToString(CultureInfo.InvariantCulture);
                //textBoxAlarmAscenso.Text = me.AlarmAscenso.ToString(CultureInfo.InvariantCulture);
                textBoxAlarmCellVoltage.Text = me.AlarmCellVoltage.ToString(CultureInfo.InvariantCulture);
                textBoxAlarmDistance.Text = me.AlarmDistance.ToString(CultureInfo.InvariantCulture);
                textBoxAlarmFastDescent.Text = me.AlarmFastDescentRate.ToString(CultureInfo.InvariantCulture);

                checkBoxAlarmAltitude.Checked = me.AlarmAltitude_enabled;
                checkBoxAlarmAscenso.Checked = me.AlarmAscenso_enabled;
                checkBoxAlarmCellVoltage.Checked = me.AlarmCellVoltage_enabled;
                checkBoxAlarmDistance.Checked = me.AlarmDistance_enabled;
                checkBoxAlarmFastDescent.Checked = me.AlarmFastDescentRate_enabled;
            }
            catch (Exception)
            {
            }

            try
            {
                checkBoxAntTrack.Checked = me.enableAntTrack;
                UpdateAnttrackControls();
            }
            catch (Exception)
            {
            }

            try
            {
                checkBoxTrocearVideo.Checked = me.trocearVideo;
                textBoxTrocearMB.Text=me.trocearTamMB.ToString();
                trackBarCalidadVideo.Value = me.calidadVideo;
                numericUpDownFPScaptura.Value = me.fpsVideo;
                UpdateVideoTrocear();
            }
            catch (Exception)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            validate();
            this.Close();
        }

        void validate()
        {
            if (comboBoxCOM.SelectedItem != null)
                me.commPort = comboBoxCOM.SelectedItem.ToString();
            if (comboBoxCommBps.SelectedItem != null)
                me.commBps = int.Parse(comboBoxCommBps.SelectedItem.ToString());
            me.videosystem = (Singleton.VideoSystem)comboBoxVideoSist.SelectedIndex;
            me.telemetria = (Singleton.Telemetria)comboBoxModem.SelectedIndex;
            me.CacheMapsPath = textBoxCacheMapsPath.Text;
            me.FlightLogPath = textBoxFlightLogPath.Text;
            me.PicturePath = textBoxPicturePath.Text;
            me.VideosPath = textBoxVideosPath.Text;
            me.NombrePiloto = textBoxNombrePiloto.Text;
            me.enableUDPinout = checkBoxUDP.Checked;
            me.enableWebServer = checkBoxWebServer.Checked;
            int.TryParse(textBoxWebServer.Text, out me.portWebServer);
            int.TryParse(textBoxUDPport.Text, out me.portUDPinout);

            try
            {
                me.moduloTX = (Singleton.ModuloControl)comboBoxModuloTX.SelectedIndex;

                me.uplinkNumCells = comboBoxNumCellsUplink.SelectedIndex + 2;
                me.uplinkLipo = checkBoxLipoUplink.Checked;
                me.uplinkVmin = (float)numericUpDownVminUplink.Value;
                me.uplinkVmax = (float)numericUpDownVmaxUplink.Value;
                me.uplinkValarm = (float)numericUpDownValarmUplink.Value;
            }
            catch (Exception)
            {
            }

            try
            {
                me.enableCasaAntTrack = false;
                me.enableAntTrack = checkBoxAntTrack.Checked;
            }
            catch (Exception)
            {
            }

            
            me.Idioma = comboBoxLanguage.SelectedIndex;
            me.SistemaMetrico = comboBoxSistemaMetrico.SelectedIndex;

            try
            {
                float.TryParse(textBoxAlarmaAltitud.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out me.AlarmAltitude);
                // float.TryParse(textBoxAlarmAscenso.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out me.AlarmAscenso);
                float.TryParse(textBoxAlarmCellVoltage.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out me.AlarmCellVoltage);
                float.TryParse(textBoxAlarmDistance.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out me.AlarmDistance);
                float.TryParse(textBoxAlarmFastDescent.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out me.AlarmFastDescentRate);

                me.AlarmAltitude_enabled = checkBoxAlarmAltitude.Checked;
                me.AlarmAscenso_enabled = checkBoxAlarmAscenso.Checked;
                me.AlarmCellVoltage_enabled = checkBoxAlarmCellVoltage.Checked;
                me.AlarmDistance_enabled = checkBoxAlarmDistance.Checked;
                me.AlarmFastDescentRate_enabled = checkBoxAlarmFastDescent.Checked;
            }
            catch (Exception)
            {
            }

            try
            {
                me.trocearVideo = checkBoxTrocearVideo.Checked;
                int.TryParse(textBoxTrocearMB.Text, out me.trocearTamMB);
                me.calidadVideo = trackBarCalidadVideo.Value;
                me.fpsVideo = (int)numericUpDownFPScaptura.Value;
            }
            catch (Exception) { }

            me.ToRegistry(); // Singleton.GetInstance().ToRegistry();
        }

        void buscaCOMs()
        {
            string[] comms = SerialPort.GetPortNames();
            if (comms.Length > 0)
            {
                int _selected_index = 0;
                comboBoxCOM.Items.Clear();
                for (int i = 0; i < comms.Length; i++)
                {
                    comboBoxCOM.Items.Add(comms[i]);
                    if (comms[i].Equals(me.commPort))
                        _selected_index = i;
                }
                comboBoxCOM.SelectedIndex = _selected_index;
            }
        }


        private void checkBoxUDP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUDP.Checked)
            {
                if (me.portUDPinout < 1024)
                    textBoxUDPport.Text = "9500";
                else
                    textBoxUDPport.Text = me.portUDPinout.ToString();
                
                textBoxUDPport.Enabled = true;
            }
            else
                textBoxUDPport.Enabled = false;
        }

        private void checkBoxWebServer_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWebServer.Checked)
            {
                if (me.portWebServer < 1024)
                    textBoxWebServer.Text = "8080";
                else
                    textBoxWebServer.Text = me.portWebServer.ToString();
                
                textBoxWebServer.Enabled = true;
            }
            else
                textBoxWebServer.Enabled = false;
        }

        private void comboBoxModem_SelectedIndexChanged(object sender, EventArgs e)
        {
            commEnabled();
        }

        private void comboBoxModuloTX_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxModuloTX.SelectedIndex == 0)
            {
                checkBoxLipoUplink.Enabled = false;
                comboBoxNumCellsUplink.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label12.Enabled = false;
                numericUpDownValarmUplink.Enabled = false;
                numericUpDownVmaxUplink.Enabled = false;
                numericUpDownVminUplink.Enabled = false;
            }
            else
            {
                checkBoxLipoUplink.Enabled = true;
                label12.Enabled = true;
                numericUpDownValarmUplink.Enabled = true;
                updateUplinkBattery();
            }
            //commEnabled();
        }

        private void commEnabled()
        {
            if (comboBoxModuloTX.SelectedIndex == 2 || comboBoxModem.SelectedIndex == 2)
            {
                label6.Enabled = true;
                label7.Enabled = true;
                comboBoxCOM.Enabled = true;
                comboBoxCommBps.Enabled = true;
            }
            else
            {
                label6.Enabled = false;
                label7.Enabled = false;
                comboBoxCOM.Enabled = false;
                comboBoxCommBps.Enabled = false;
            }
        }

        private void checkBoxAlarmAltitude_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAlarmAltitude.Checked)
            {
                textBoxAlarmaAltitud.Enabled = true;
            }
            else
            {
                textBoxAlarmaAltitud.Enabled = false;
            }

        }

        private void checkBoxAlarmFastDescent_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAlarmFastDescent.Enabled = checkBoxAlarmFastDescent.Checked;
        }

        private void checkBoxAlarmDistance_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAlarmDistance.Enabled = checkBoxAlarmDistance.Checked;
        }

        private void checkBoxAlarmCellVoltage_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAlarmCellVoltage.Enabled = checkBoxAlarmCellVoltage.Checked;
        }

        private void checkBoxAlarmAscenso_CheckedChanged(object sender, EventArgs e)
        {
            //textBoxAlarmAscenso.Enabled = checkBoxAlarmAscenso.Checked;
        }

        void TextSpanish()
        {
            label25.Text = "Nombre Piloto";
            label4.Text = "Idioma";
            label3.Text = "Sistema Video";
            label16.Text = "Sistema Metrico";
            label2.Text = "Modem Telemetria";
            label6.Text = "Puerto COM";
            label7.Text = "Baudios COM";
            label5.Text = "Modulo TX";
            groupBox5.Text = "Directorios";
            label8.Text = "Ruta mapas";
            label9.Text = "Ruta de logs";
            label1.Text= "Ruta fotos";
            label13.Text = "Ruta videos";
            checkBoxAntTrack.Text = "Posicionador Antena";
            checkBoxUDP.Text = "Servidor UDP (Puerto)";
            checkBoxWebServer.Text = "Servidor Web (Puerto)";
            button2.Text = "Aceptar";
            this.Text = "Configurar Consola";
            buttonConfigAntTracker.Text = "Configurar";
            this.comboBoxModuloTX.Items.Clear();
            this.comboBoxModuloTX.Items.AddRange(new object[] {
            "Ninguno",
            "Modulo Uplink"});
            //"Modem XBEE"});

            groupBox8.Text = "Grabacion de Video";
            checkBoxTrocearVideo.Text = "Trocear Video";
            label14.Text = "Tam (MB)";
            label42.Text = "Calidad";
            label15.Text = "FPS Captura";
            checkBoxLipoUplink.Text = "Bateria LIPO";
        }

        void TextEnglish()
        {
            label25.Text = "Pilot Name";
            label4.Text = "Language";
            label3.Text = "Video System";
            label16.Text = "Units";
            label2.Text = "Telemetry Downlink";
            label6.Text = "Serial COM Port";
            label7.Text = "COM Baud Rate";
            label5.Text = "TX System";
            groupBox5.Text = "Folders";
            label8.Text = "Maps Path";
            label9.Text = "Logs Path";
            label1.Text = "Photo Path";
            label13.Text = "Video Path";
            checkBoxAntTrack.Text = "Antenna Tracker";
            checkBoxUDP.Text = "UDP Server Port";
            checkBoxWebServer.Text = "WEB Server Port";
            button2.Text = "Acept";
            this.Text = "Config Console";
            buttonConfigAntTracker.Text = "Configure";
            this.comboBoxModuloTX.Items.Clear();
            this.comboBoxModuloTX.Items.AddRange(new object[] {
            "None",
            "Uplink Module"});
            //"XBEE Modem"});

            groupBox8.Text = "Video Recording";
            checkBoxTrocearVideo.Text = "Split File";
            label14.Text = "Size (MB)";
            label42.Text = "Quality";
            label15.Text = "FPS Capture";
            checkBoxLipoUplink.Text = "LIPO Battery";
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            updateUplinkBattery();
        }

        private void updateUplinkBattery()
        {
            if (checkBoxLipoUplink.Checked)
            {
                comboBoxNumCellsUplink.Enabled = true;
                numericUpDownVminUplink.Enabled = false;
                numericUpDownVmaxUplink.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
            }
            else
            {
                comboBoxNumCellsUplink.Enabled = false;
                numericUpDownVminUplink.Enabled = true;
                numericUpDownVmaxUplink.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num_cells;
            if (checkBoxLipoUplink.Enabled)
            {
                if (comboBoxNumCellsUplink.SelectedIndex == 0)
                    num_cells = 2;
                else
                    num_cells = 3;

                numericUpDownVminUplink.Value = (decimal)(3.2f * num_cells);
                numericUpDownVmaxUplink.Value = (decimal)(4.2f * num_cells);
                numericUpDownValarmUplink.Value = (decimal)(3.6f * num_cells);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormConfigurarAntracker().Show(this);
        }

        private void checkBoxAntTrack_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnttrackControls();
        }

        private void UpdateAnttrackControls()
        {
            if (checkBoxAntTrack.Checked)
            {
                buttonConfigAntTracker.Enabled = true;
            }
            else
            {
                buttonConfigAntTracker.Enabled = false;
            }
        }

        private void UpdateVideoTrocear()
        {
            if (checkBoxTrocearVideo.Checked)
            {
                label14.Enabled = true;
                textBoxTrocearMB.Enabled = true;
            }
            else
            {
                label14.Enabled = false;
                textBoxTrocearMB.Enabled = false;
            }
        }

        private void checkBoxTrocearVideo_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVideoTrocear();
        }

    }
}
