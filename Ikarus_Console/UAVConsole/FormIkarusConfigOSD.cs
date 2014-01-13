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
using System.Threading;
using UAVConsole.GoogleMaps;
using UAVConsole.USBXpress;

namespace UAVConsole
{
    public partial class FormIkarusConfigOSD : Form
    {
        IkarusBasicConfig sconfig = new IkarusBasicConfig();
        FlightPlanUSB planUSB = new FlightPlanUSB();

        Singleton me = Singleton.GetInstance();
        bool firstTime = true;
        bool updateFirmware = false;


        public FormIkarusConfigOSD()
        {
            InitializeComponent();
            if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                TextSpanish();
            else
                TextEnglish();

            comboBoxTelemetry.SelectedIndex = 0;
            comboBoxOSDScreen.SelectedIndex = 0;
            comboBoxVideoSystem.SelectedIndex = 0;
            comboBoxCanalControl.SelectedIndex = 0;
            comboBoxGPSBaudRate.SelectedIndex = 5;
            comboBoxModeloRuta.SelectedIndex = 1;
            comboBoxCanalPPM.SelectedIndex = 0;
            comboBoxModoPPM.SelectedIndex = 0;
            comboBoxCamSel.SelectedIndex = 0;
            comboBoxSistemaMetrico.SelectedIndex = 0;
            comboBoxModoFailsafe.SelectedIndex = 0;
            comboBoxNumCanalesPPM.SelectedIndex = 0;
            firstTime = true;
            updateFirmware = false;
            timer1.Enabled = true;
        }

        private void FormIkarusConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            Singleton me = Singleton.GetInstance();
            timer1.Enabled = false;
            planUSB.Close();
            
            if (me != null&&sconfig!=null&&!updateFirmware && (sconfig.HomeLon!=0||sconfig.HomeLat!=0))
            {
                DialogResult res;
                if (me.Idioma == 0)
                {
                    res = MessageBox.Show(this,
                        "A continuación se va a actualizar la casa del programa con las nuevas coordenadas. Pulse 'Cancelar' si no desea hacer esto.",
                        "Actualizar Casa", MessageBoxButtons.OKCancel);
                }
                else
                {
                    res = MessageBox.Show(this,
                        "Next, program home will be updated with the new coordinates. Press 'Cancel' if you don't want to do this.",
                        "Update Home", MessageBoxButtons.OKCancel);
                }

                if (res == DialogResult.OK)
                {
                    me.HomeLon = sconfig.HomeLon;
                    me.HomeLat = sconfig.HomeLat;
                    me.HomeAlt = sconfig.HomeAltitude;

                    if (me.planeState != null)
                    {
                        me.planeState.homeLon = sconfig.HomeLon;
                        me.planeState.homeLat = sconfig.HomeLat;
                    }
             
                }
                if (me.planeState != null)
                {
                    me.planeState.Lat = sconfig.HomeLat;
                    me.planeState.Lon = sconfig.HomeLon;
                    me.planeState.Alt = sconfig.HomeAltitude;
                }
                me.ToRegistry();
            }
        
        }

        void UpdateControles()
        {
             string error_msg = "";
            int error_count = 0;

            // BATTERIES
            try
            {
                numericUpDownCellsBatt1.Value = sconfig.cellsBatt1;
                numericUpDownGainV1.Value = (decimal)sconfig.sensorV1_gain;
                numericUpDownOffsetV1.Value = (decimal)sconfig.sensorV1_offset;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de sensor V1\n";
                else
                    error_msg += "- Error loading V1 sensor config\n";
                error_count++;
            }

            try
            {
                numericUpDownCellsBatt2.Value = sconfig.cellsBatt2;
                numericUpDownGainV2.Value = (decimal)sconfig.sensorV2_gain;
                numericUpDownOffsetV2.Value = (decimal)sconfig.sensorV2_offset;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de sensor V2\n";
                else
                    error_msg += "- Error loading V2 sensor config\n";
                error_count++;
            }


            try
            {
                numericUpDownBattCapacity.Value = (decimal)sconfig.total_mAh;
                numericUpDownSensorIgain.Value = (decimal)sconfig.sensorI_gain;
                numericUpDownSensorIoffset.Value = (decimal)sconfig.sensorI_offset;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de sensor I\n";
                else
                    error_msg += "- Error loading I sensor config\n";
                error_count++;
            }

            try
            {
                numericUpDownRSSImax.Value = (decimal)sconfig.rssi_max;
                numericUpDownRSSImin.Value = (decimal)sconfig.rssi_min;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de sensor RSSI\n";
                else
                    error_msg += "- Error loading RSSI sensor config\n";
                error_count++;
            }
             
            
            // ALARMS
            try
            {
                if (sconfig.cellAlarm < 2.5f)
                    sconfig.cellAlarm = 3.0f;
                numericUpDownCellAlarm.Value = (decimal)sconfig.cellAlarm;

                if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                {
                    numericUpDownAlarmLowSpeed.Value = (decimal)(sconfig.lowSpeedAlarm * 1.852f);
                    numericUpDownAlarmAltitude.Value = (decimal)sconfig.altitudeAlarm;
                    numericUpDownAlarmDistance.Value = (decimal)sconfig.distanceAlarm;
                    numericUpDownWptRange.Value = (decimal)sconfig.WptRange;
                }
                else
                {
                    numericUpDownAlarmLowSpeed.Value = (decimal)sconfig.lowSpeedAlarm;
                    numericUpDownAlarmAltitude.Value = (decimal)(sconfig.altitudeAlarm * 3.28f);
                    numericUpDownAlarmDistance.Value = (decimal)(sconfig.distanceAlarm * 3.28f); //* 1852.0f);
                    numericUpDownWptRange.Value = (decimal)(sconfig.WptRange * 3.28f);
                }

            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de alarmas\n";
                else
                    error_msg += "- Error loading alarm config\n";
                error_count++;
            }

            try
            {

                // OSD
                comboBoxVideoSystem.SelectedIndex = 1 - sconfig.videoPAL;
                numericUpDownOffsetX.Value = sconfig.offsetX;
                numericUpDownOffsetY.Value = sconfig.offsetY;
                comboBoxTelemetry.SelectedIndex = sconfig.TelemetryMode;
                comboBoxOSDScreen.SelectedIndex = sconfig.DefaultHUD;
                comboBoxCamSel.SelectedIndex = sconfig.CamSel;
                numericUpDownInicioTelemetry.Value = (decimal)sconfig.inicio_telemetry;
                checkBoxLeftBand.Checked = (sconfig.LeftBand == 1);
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de video\n";
                else
                    error_msg += "- Error loading video config\n";
                error_count++;
            }

            try
            {   
                // CONTROL
                comboBoxCanalControl.SelectedIndex = sconfig.ControlProportional;
                comboBoxModoPPM.SelectedIndex = sconfig.Modo_PPM;
                comboBoxCanalPPM.SelectedIndex = sconfig.PPM_Channel;// -4;

                if (sconfig.NumCanales_PPM >= 4 && sconfig.NumCanales_PPM <= 12)
                    comboBoxNumCanalesPPM.SelectedIndex = sconfig.NumCanales_PPM - 3;
                else
                    comboBoxNumCanalesPPM.SelectedIndex = 0;

            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de control\n";
                else
                    error_msg += "- Error loading control config\n";
                error_count++;
            }

            try
            {
                // CASA
                textBoxHomeLat.Text = sconfig.HomeLat.ToString();
                textBoxHomeLon.Text = sconfig.HomeLon.ToString();
                checkBoxRelativeAltitude.Checked = (sconfig.AbsoluteAltitude == 1);
                if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                {
                    numericUpDownHomeAltitude.Value = (decimal)sconfig.HomeAltitude;
                }
                else
                {
                    numericUpDownHomeAltitude.Value = (decimal)(sconfig.HomeAltitude * 3.28f);
                }
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración de casa\n";
                else
                    error_msg += "- Error loading home config\n";
                error_count++;
            }

            try
            {

                // NAVEGADOR
                comboBoxGPSBaudRate.SelectedIndex = sconfig.BaudRate;
                comboBoxModeloRuta.SelectedIndex = sconfig.modelo_ruta;
                comboBoxModoFailsafe.SelectedIndex = sconfig.Modo_Failsafe;
                numericUpDownFailsafeDelay.Value = (decimal)sconfig.Retraso_Failsafe;
                if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                {
                    numericUpDownWptRange.Value = (decimal)sconfig.WptRange;
                }
                else
                {
                    numericUpDownWptRange.Value = (decimal)(sconfig.WptRange * 3.28f);
                }
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración del navegador\n";
                else
                    error_msg += "- Error loading navigator config\n";
                error_count++;
            }
            
            try
            {
                // CONFIG REGIONAL
                comboBoxSistemaMetrico.SelectedIndex = sconfig.MetricsImperial;
                numericUpDownTimeZone.Value = sconfig.TimeZone;
                textBoxNombrePiloto.Text = sconfig.NombrePiloto;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración regional\n";
                else
                    error_msg += "- Error loading culture config\n";
                error_count++;
            }

            if (error_count > 10)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("Error cargando multiples valores");
                else
                    MessageBox.Show("Error loading multiple values");
            }
            else if (error_count > 0)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg = "Errores detectados:\n\n" + error_msg;
                else
                    error_msg = "Error detected\n\n" + error_msg;
                MessageBox.Show(error_msg);
            }
            
        }

        void UpdateStruct()
        {
            sconfig.videoPAL = (byte)(1 - comboBoxVideoSystem.SelectedIndex);
            sconfig.offsetX = (byte)numericUpDownOffsetX.Value;
            sconfig.offsetY = (byte)numericUpDownOffsetY.Value;

            sconfig.cellsBatt1 = (byte)numericUpDownCellsBatt1.Value;
            sconfig.sensorV1_gain = (float)numericUpDownGainV1.Value;
            sconfig.sensorV1_offset = (float)numericUpDownOffsetV1.Value;

            sconfig.cellsBatt2 = (byte)numericUpDownCellsBatt2.Value;
            sconfig.sensorV2_gain = (float)numericUpDownGainV2.Value;
            sconfig.sensorV2_offset = (float)numericUpDownOffsetV2.Value;

            sconfig.total_mAh = (float)numericUpDownBattCapacity.Value;
            sconfig.sensorI_gain = (float)numericUpDownSensorIgain.Value;
            sconfig.sensorI_offset = (float)numericUpDownSensorIoffset.Value;

            sconfig.cellAlarm = (float)numericUpDownCellAlarm.Value;
            if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
            {
                sconfig.altitudeAlarm = (float)numericUpDownAlarmAltitude.Value;
                sconfig.distanceAlarm = (float)numericUpDownAlarmDistance.Value;
                sconfig.lowSpeedAlarm = (float)numericUpDownAlarmLowSpeed.Value / 1.852f;
                sconfig.WptRange = (float)numericUpDownWptRange.Value;
            
            }
            else
            {
                sconfig.altitudeAlarm = (float)numericUpDownAlarmAltitude.Value / 3.28f;
                sconfig.distanceAlarm = (float)numericUpDownAlarmDistance.Value / 3.28f; // / 1852.0f;
                sconfig.lowSpeedAlarm = (float)numericUpDownAlarmLowSpeed.Value;
                sconfig.WptRange = (float)numericUpDownWptRange.Value / 3.28f;
            
            }
            
            
            try
            {
                sconfig.HomeLon = float.Parse(textBoxHomeLon.Text);
                sconfig.HomeLat = float.Parse(textBoxHomeLat.Text);
                if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                    sconfig.HomeAltitude = (float)numericUpDownHomeAltitude.Value;
                else
                    sconfig.HomeAltitude = (float)numericUpDownHomeAltitude.Value / 3.28f;
                
            }
            catch (Exception) { }
            
            sconfig.DefaultHUD = (byte)comboBoxOSDScreen.SelectedIndex;
            sconfig.TelemetryMode = (byte)comboBoxTelemetry.SelectedIndex;
            sconfig.ControlProportional = (byte)comboBoxCanalControl.SelectedIndex;
            sconfig.AbsoluteAltitude = (byte)(checkBoxRelativeAltitude.Checked ? 1 : 0);
            sconfig.BaudRate = (byte)comboBoxGPSBaudRate.SelectedIndex;

            sconfig.MetricsImperial = (byte)comboBoxSistemaMetrico.SelectedIndex;
            sconfig.TimeZone = (sbyte)numericUpDownTimeZone.Value;
            sconfig.CamSel = (byte)comboBoxCamSel.SelectedIndex;
      
            sconfig.modelo_ruta = (byte)comboBoxModeloRuta.SelectedIndex;
            sconfig.inicio_telemetry = (byte)numericUpDownInicioTelemetry.Value;
            sconfig.rssi_max = (float)numericUpDownRSSImax.Value;
            sconfig.rssi_min = (float)numericUpDownRSSImin.Value;
            
            sconfig.Modo_PPM = (byte)comboBoxModoPPM.SelectedIndex;
            sconfig.PPM_Channel = (byte)(comboBoxCanalPPM.SelectedIndex); // + 4);

            if (comboBoxNumCanalesPPM.SelectedIndex > 0)
                sconfig.NumCanales_PPM = (byte)(comboBoxNumCanalesPPM.SelectedIndex + 3);
            else
                sconfig.NumCanales_PPM = 255;

            sconfig.Modo_Failsafe = (byte)comboBoxModoFailsafe.SelectedIndex;
            sconfig.LeftBand = (byte)(checkBoxLeftBand.Checked ? 1 : 0);
            sconfig.Retraso_Failsafe = (byte)numericUpDownFailsafeDelay.Value;

            sconfig.NombrePiloto = textBoxNombrePiloto.Text;   
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                if (planUSB.IsOpen())
                {
                    //timer1.Enabled = false;
                    if (firstTime)
                    {
                        firstTime = false;
                        sconfig = planUSB.ReadConfig();
                        planUSB.Close();


                        panel1.Enabled = true;

                        UpdateControles();
                    }
                    else
                    {
                        planUSB.Close();
                    }
                    if (me.Idioma == 0)
                        labelStatus.Text = "Conectado!";
                    else
                        labelStatus.Text = "Connected!";
                    labelStatus.ForeColor = Color.Green;

                }
                else
                {
                    if (me.Idioma == 0)
                        labelStatus.Text = "No Conectado!";
                    else
                        labelStatus.Text = "Not Connected!";
                    labelStatus.ForeColor = Color.Red;

                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false; 
            UpdateStruct();
            if (planUSB.IsOpen())
            {
                planUSB.WriteConfig(sconfig);
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("Configuración actualizada correctamente");
                else
                    MessageBox.Show("Config updated successfully");
            }
            else
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("No puedo conectar con Ikarus OSD");
                else
                    MessageBox.Show("Cannot connect with Ikarus OSD");
            }
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            timer1.Enabled = false;
            if (planUSB.IsOpen())
            {
                WayPoint wpt = planUSB.ReadGPS();
                if (wpt != null)
                {
                    textBoxHomeLat.Text = wpt.Latitude.ToString();
                    textBoxHomeLon.Text = wpt.Longitude.ToString();
                    numericUpDownHomeAltitude.Value = (decimal)wpt.Altitude;
                }
            }
            else
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("No puedo conectar con Ikarus OSD");
                else
                    MessageBox.Show("Cannot connect with Ikarus OSD");
            }

            timer1.Enabled = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //labelStatus.Text = "Closed!";
            //labelStatus.ForeColor = Color.DarkRed;
            updateFirmware = true;
            this.Close();
            FormActualizarFirmware form=new FormActualizarFirmware(FormActualizarFirmware.Devices.OSD);
            form.Show();
            
        }

        private void comboBoxModoPPM_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxModoPPM.SelectedIndex == 0)
            {
                comboBoxNumCanalesPPM.Enabled = false;
                comboBoxCanalPPM.Enabled = false;
                label27.Enabled = false;
                label32.Enabled = false;
            }
            else
            {
                comboBoxNumCanalesPPM.Enabled = true;
                comboBoxCanalPPM.Enabled = true;
                label27.Enabled = true;
                label32.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Config (*.xml)|*.xml";
            dlg.ShowDialog(this);

            if (!dlg.FileName.Equals(""))
            {
                sconfig.LoadFromXml(dlg.FileName);
                UpdateControles();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Config (*.xml)|*.xml";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                UpdateStruct();
                sconfig.SaveToXml(dlg.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            
            if (planUSB.IsOpen())
            {
                sconfig = planUSB.ReadConfig();
                UpdateControles();
            }
            else
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("No puedo conectar con Ikarus OSD");
                else
                    MessageBox.Show("Cannot connect with Ikarus OSD");
            }

            timer1.Enabled = true;
        }

        void TextSpanish()
        {
            label16.Text = "Sistema Video";
            label5.Text = "Posición Horizontal";
            label6.Text = "Posición Vertical";
            label23.Text = "Pos. Vertical Telemetria";
            label1.Text = "Nº Celdas Motor";
            label33.Text="Ganancia V1";
            label35.Text = "Ganancia V2";
            label2.Text = "Nº Celdas Video";
            label3.Text = "Capacidad Bateria Motor";
            label19.Text = "Offset Sensor I (V)";
            label20.Text = "Ganancia Sensor I (A/V)";
            label22.Text = "RSSI Min (mV)";
            label21.Text = "RSSI Max (mV)";

            label25.Text = "Nombre Piloto";
            label28.Text = "Zona Horaria GMT";
            label18.Text = "Velocidad GPS";
            label15.Text = "Pantalla OSD";
            label14.Text = "Telemetria";
            label26.Text = "Entrada Control";
            label27.Text = "Canal PPM";
            label17.Text = "Modo Control";
            label29.Text = "Camera Sel";
            label30.Text = "Sistema Medida";

            label4.Text = "Alarma Volt. Bateria (celda)";
            label10.Text = "Alarma de distancia";
            label11.Text = "Alarma de altitud";
            label12.Text = "Alarma de velocidad";
            label13.Text = "Precision de Waypoint";
            label24.Text = "Fin de Ruta";
            checkBoxRelativeAltitude.Text = "Altitud Relativa";
            label7.Text = "Latitud Casa";
            label8.Text = "Longitud Casa";
            label9.Text = "Altitud Casa";


            label37.Text = "Failsafe delay (sec)";
            label37.Text = "Failsafe retraso (seg)";

            button3.Text = "Actualiza Firm.";
            button5.Text = "Guarda fich";
            button4.Text = "Leer fich.";
            button6.Text = "Releer";
            button2.Text = "Leer GPS";
            button1.Text = "Actualizar";

            comboBoxCanalControl.Items.Clear();
            comboBoxCanalControl.Items.AddRange(new object[] {
            "Interruptor 2",
            "Int. 3 Basico",
            "Interruptor 3",
            "Proporcional",
            "Mezcla (223)",
            "Ajuste (223)",
            "Mezcla (224)",
            "TX Uplink"});

            comboBoxSistemaMetrico.Items.Clear();
            comboBoxSistemaMetrico.Items.AddRange(new object[] {
            "Metrico",
            "Imperial"});

            comboBoxModeloRuta.Items.Clear();
            comboBoxModeloRuta.Items.AddRange(new object[] {
            "Ir Casa",
            "Repetir Ruta",
            "Invertir Ruta",
            "Invertir+Repetir"});
            
            checkBoxLeftBand.Text = "Banda Izquierda";
            this.Text = "Configuración Ikarus OSD";
        }

        void TextEnglish()
        {
            label16.Text = "Video System";
            label5.Text = "Horizontal Offset";
            label6.Text = "Vertical Offset";
            label23.Text = "Vert. Offset Telemetry";
            label1.Text = "Motor # Cells";
            label2.Text = "Video # Cells";
            label3.Text = "Motor Battery Cap.(mAh)";
            label33.Text = "Gain V1";
            label35.Text = "Gain V2";
            label19.Text = "I Sensor Offset (V)";
            label20.Text = "I Sensor Gain (A/V)";
            label22.Text = "RSSI Min (mV)";
            label21.Text = "RSSI Max (mV)";

            label25.Text = "Pilot Name";
            label28.Text = "GMT TimeZone";
            label18.Text = "GPS Baud Rate";
            label15.Text = "OSD Screen";
            label14.Text = "Telemetry";
            label26.Text = "Input Mode";
            label27.Text = "PPM Channel #";
            label17.Text = "Control Mode";
            label29.Text = "Camera Selector";
            label30.Text = "System Units";

            label4.Text = "Low Battery Alarm (cell)";
            label10.Text = "Distance Alarm";
            label11.Text = "Low Altitude Alarm";
            label12.Text = "Los Speed Alarm";
            label13.Text = "Waypoint Range";
            label24.Text = "After route ends";
            checkBoxRelativeAltitude.Text = "Relative Altitude";
            label7.Text = "Home Latitude";
            label8.Text = "Home Longitude";
            label9.Text = "Home Altitude";
            label37.Text = "Failsafe delay (sec)";

            button3.Text = "Update Firmware";
            button5.Text = "Save File";
            button4.Text = "Load File";
            button6.Text = "OSD Reload";
            button2.Text = "Read GPS";
            button1.Text = "Update";


            comboBoxCanalControl.Items.Clear();
            comboBoxCanalControl.Items.AddRange(new object[] {
            "Switch 2",
            "Sw. 3 basic",
            "Switch 3",
            "Slider",
            "Mixing (223)",
            "Ajust (223)",
            "Mixing (224)",
            "TX Uplink"});

            comboBoxSistemaMetrico.Items.Clear();
            comboBoxSistemaMetrico.Items.AddRange(new object[] {
            "Metric",
            "Imperial"});

            comboBoxModeloRuta.Items.Clear();
            comboBoxModeloRuta.Items.AddRange(new object[] {
            "Go Home",
            "Repeat Route",
            "Reverse Route",
            "Reverse+Repeat"});
            checkBoxLeftBand.Text = "Left Band";
            
            this.Text = "Ikarus OSD Config";
        }

        private void labelStatus_DoubleClick(object sender, EventArgs e)
        {
            panel1.Enabled = true;
        }
    }
}