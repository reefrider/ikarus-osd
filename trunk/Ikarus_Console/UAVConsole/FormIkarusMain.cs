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
using System.Diagnostics;
using System.Threading;
using UAVConsole.GoogleMaps;
using System.Drawing.Imaging;
using System.IO;
using UAVConsole.ConfigClasses;
using UAVConsole.Modem;
using System.IO.Ports;
using System.Media;
using UAVConsole.USBXpress;
using System.Globalization;

namespace UAVConsole
{
    public partial class FormIkarusMain : Form
    {
        Singleton me = Singleton.GetInstance();

        bool keypressed=false;

        private Capture capture = null;
        private Filters filters;

        public JoystickThread jthread;

        AntTracker antTracker;
        AntTrackerDatosAntena datosAntena;
        
        SenderUDP sender;
        MyWebServer KmlWebServer;
        
        public WayPoint homeWpt, planeWpt, targetWpt;

        ModemAbstract modem;

        int last_wptid;
        
        private float[,] debug_values;

        bool starting_video = false;
        bool saving_video = false;
        bool stoping_video = false;

        bool saving_picture = false;

        AviWriter avi_writer;

        long parse_last_tick = 0;
        long video_last_tick = 0;


        public void TeamRefresh(WayPoint wpt)
        {
            try
            {
                mapControl1.team_pos[wpt.name] = wpt;
                mapControl1.Invalidate();
            }
            catch (Exception) { }
        }

        public FormIkarusMain()
        {

            InitializeComponent();


            if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                instrumento_Altimeter1.Calibration = me.HomeAlt;
            else
                instrumento_Altimeter1.Calibration = me.HomeAlt * 3.28f;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        
            debug_values = new float[5, 5]{{me.uplink_pid_ele_P, me.uplink_pid_ele_I, me.uplink_pid_ele_D, me.uplink_pid_ele_IL, me.uplink_pid_ele_DL},
                {me.uplink_pid_ail_P , me.uplink_pid_ail_I, me.uplink_pid_ail_D, me.uplink_pid_ail_IL, me.uplink_pid_ail_DL},
                {me.uplink_pid_tail_P, me.uplink_pid_tail_I, me.uplink_pid_tail_D, me.uplink_pid_tail_IL, me.uplink_pid_tail_DL},
                {me.uplink_pid_thr_P, me.uplink_pid_thr_I, me.uplink_pid_thr_D, me.uplink_pid_thr_IL, me.uplink_pid_thr_DL },
                {me.uplink_IR_offX, me.uplink_IR_offY, me.uplink_IR_gain, me.uplink_rumbo_ail, me.uplink_altura_ele}};

            comboBox3.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
           
            comboBox1.SelectedIndex = 0;
            this.planeWpt = new WayPoint("Avion", me.HomeLon, me.HomeLat, global::UAVConsole.Properties.Resources.plane3);
            this.planeWpt.icon.MakeTransparent(Color.White);
            this.planeWpt.heading = 0;
            this.targetWpt = new WayPoint("Avion", me.HomeLon, me.HomeLat);
            this.homeWpt = new WayPoint("Home", me.HomeLon, me.HomeLat);

            mapControl1.plane = this.planeWpt;
            mapControl1.target = this.targetWpt;
            mapControl1.ruta = me.Ruta;

            medidorBaterias1.num_cells = me.cells1;
            medidorBaterias2.num_cells = me.cells2;


            try
            {
                filters = new Filters();
            }
            catch (Exception)
            {
            }

            if (filters != null && filters.VideoInputDevices.Count > 0)
            {
                foreach (Filter f in filters.VideoInputDevices)
                {
                    if (f.Name.Equals(me.videoCaptureStr))
                    {
                        capture = new Capture(f, null, false);
                        break;
                    }
                }

                if (capture != null)
                {
                    foreach (Source vs in capture.VideoSources)
                    {
                        if (vs.ToString().Contains("omposi")) // Video Composite
                        {
                            capture.VideoSource = vs;
                            break;
                        }
                    }
                   
                    if (me.videosystem == Singleton.VideoSystem.PAL)
                    {
                        capture.dxUtils.VideoStandard = DShowNET.AnalogVideoStandard.PAL_B;
                        try
                        {
                            capture.FrameRate = 25; 
                            capture.FrameSize = new Size(720, 576);
                            capture.PreviewFrameSize = new Size(720, 576);

                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        capture.dxUtils.VideoStandard = DShowNET.AnalogVideoStandard.NTSC_M;
                        try
                        {
                            capture.FrameRate = 30;
                            capture.FrameSize = new Size(720, 480);
                        }
                        catch (Exception) { }
                    }
                    capture.AllowSampleGrabber = true;
                    capture.PreviewWindow = panel1;
                    capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
                    capture.GrapImg();
                    //capture.ShowPropertyPage(1, this);
                }
            }

            if (me.enableAntTrack)
            {
                antTracker = new AntTracker();
                medidorBaterias3.Enabled = true;
                medidorBaterias3.AutoCalculate = false;
                label6.Enabled = true;
                knob_anttracker.Visible = true;
                knob_anttracker.Manual = false;
                medidorRSSI.Height = 97;
            }
            else
            {
                knob_anttracker.Visible = false;
                medidorRSSI.Height = 137;
            }

            if (me.telemetria == Singleton.Telemetria.Video)
            {
                modem = new ModemVideo();
            }
            else if (me.telemetria == Singleton.Telemetria.XBEE)
                modem = new ModemXbee(me.commPort, me.commBps);
            else if (me.telemetria == Singleton.Telemetria.AntTracker)
                modem = new ModemAntTracker(antTracker);

            if (me.telemetria != Singleton.Telemetria.None)
                modem.listeners += RefreshInstruments;

            if (me.moduloTX == Singleton.ModuloControl.Uplink)
            {
                if (jthread == null)
                    jthread = new JoystickThread(this);

                decimal altura = (decimal)(me.HomeAlt + 100);

                if (altura < numericUpDown1.Minimum)
                    altura = numericUpDown1.Minimum;
                else if (altura > numericUpDown1.Maximum)
                    altura = numericUpDown1.Maximum;

                numericUpDown1.Value = altura;

                label7.Enabled = true;
                medidorBaterias4.Enabled = true;
                medidorBaterias4.volts_max = me.uplinkVmax;
                medidorBaterias4.volts_min = me.uplinkVmin;
            }
            else
            {
                splitContainer1.SplitterDistance+=panel4.Height;
                panel5.Location = panel4.Location;
                panel6.Location = new Point(panel6.Location.X, panel5.Location.Y);
                panel4.Visible = false;
                
            }

            if (me.enableUDPinout)
            {
                if (this.sender == null)
                    this.sender = new SenderUDP(me.portUDPinout);
                this.sender.listeners += TeamRefresh;
            }
            else
            {
                comboBoxTeam.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                //panel8.Width -= 56;
            }

            if (me.enableWebServer)
            {
                this.KmlWebServer = new MyWebServer(me.portWebServer);
            }

            timer1.Enabled = true;
        }

        ~FormIkarusMain()
        {
            modem = null;
            if (capture != null)
            {
                capture.Stop();
                capture.Dispose();
            }
        }

        bool semaforo = false;
        public void RefreshInstruments()
        {
            if (modem != null)
            {
                me.planeState = modem.getTelemetry();
                if (me.planeState != null)
                {
                    medidorBaterias1.volts = me.planeState.v1;
                    medidorBaterias2.volts = me.planeState.v2;

                    if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                        instrumento_Altimeter1.Altitude = me.planeState.Alt;
                    else
                        instrumento_Altimeter1.Altitude = me.planeState.Alt*3.28f;
                    
                    instrumento_DirectionalGyro1.Value = me.planeState.Rumbo;

                    if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                        instrumento_VerticalSpeed1.Value=me.planeState.vertSpeed*100;
                    else
                        instrumento_VerticalSpeed1.Value = me.planeState.vertSpeed*328.0f;
                    
                    
                    instrumento_HorizonteArtificial.pitch = me.planeState.pitch;
                    instrumento_HorizonteArtificial.roll = me.planeState.roll;

                    if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Imperial)
                        dG808_AirSpeed1.Value = me.planeState.Knots;
                    else
                        dG808_AirSpeed1.Value = me.planeState.Knots * 1.852f;
     
                    medidorRSSI.valor = me.planeState.RSSI / 100.0f;
                    medidorRSSI.Invalidate();

                    // cambiar por planeWpt
                    mapControl1.plane.heading = me.planeState.Rumbo;
                    mapControl1.plane.Latitude = me.planeState.Lat;
                    mapControl1.plane.Longitude = me.planeState.Lon;
                    mapControl1.plane.Altitude = me.planeState.Alt;


                    if (checkBox2.Checked)
                    {
                        //if (DateTime.Now.Millisecond < 500)
                        if (DateTime.Now.Second %2  == 0)
                        
                        {
                            if (semaforo)
                            {
                                mapControl1.AddHistory();
                                semaforo = false;
                            }

                        }
                        else
                        {
                            if (!semaforo)
                            {
                                //mapControl1.AddHistory();
                                semaforo = true;
                            }

                        }
                    }
                    else
                        mapControl1.ClearHistory();
                   
                    me.HomeAlt = instrumento_Altimeter1.Calibration;
                    homeWpt.Altitude = me.HomeAlt;

                    // cambiar por targetWpt
                    mapControl1.target.Latitude = me.planeState.homeLat;
                    mapControl1.target.Longitude = me.planeState.homeLon;

                    if (antTracker != null)
                    {
                        antTracker.Send(me.planeState, homeWpt);
                        datosAntena = antTracker.datosAntena;
                        if (datosAntena != null)
                        {
                            medidorBaterias3.volts = datosAntena.v_bateria;
                            medidorBaterias3.valor = datosAntena.v_baterial_porcentaje / 100.0f;
                            medidorBaterias3.Invalidate();
                        }
                    }

                    if (checkBoxHomeRX.Checked == false)
                    {
                        if (datosAntena != null && datosAntena.tieneGPS!=0)
                        {
                            if (datosAntena.tienePOS != 0)
                            {
                                homeWpt.Latitude = datosAntena.lat;
                                homeWpt.Longitude = datosAntena.lon;
                            }
                        }
                        else
                        {
                            homeWpt.Latitude = me.HomeLat;
                            homeWpt.Longitude = me.HomeLon;
                        }
                    }
                    else if (me.planeState.WptIndex == -1)
                    {
                        homeWpt.Latitude = me.planeState.homeLat;
                        homeWpt.Longitude = me.planeState.homeLon;
                    }

                    if (me.planeState.WptIndex != -3)
                    {
                        mapControl1.home.Latitude = homeWpt.Latitude;
                        mapControl1.home.Longitude = homeWpt.Longitude;
                        //mapControl1.home = new WayPoint("CASA", homeWpt.Longitude, homeWpt.Latitude);
                        mapControl1.rumboHold = float.NaN;

                        try
                        {
                            if (me.planeState.WptIndex >= 0 && me.planeState.WptIndex < 32 &&
                                (me.planeState.WptIndex != last_wptid || last_wptid != numericUpDown2.Value))
                            {
                                last_wptid = (byte)me.planeState.WptIndex;
                                numericUpDown2.Tag = last_wptid;
                                numericUpDown2.Value = last_wptid;
                            }
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        //mapControl1.home = null;
                        mapControl1.rumboHold = (float)me.planeState.homeLon;
                    }

                    mapControl1.Invalidate();

                    if (me.enableUDPinout)
                        SenderUDP.SendInfo(me.NombrePiloto, me.portUDPinout);

                    led1.valid = me.planeState.lastrx;
                    led1.Invalidate();
                }
            }

            if (jthread != null)
            {
                try
                {
                    UpdateButtonCamera();
                    UpdateButtonAutopilot();
                    UpdateButtonHUD();
                    UpdateButtonRuta();

                    medidorBaterias4.volts = me.uplink_voltage;
                }
                catch (Exception) { }
            }
        }
        void UpdateButtonCamera()
        {
            buttonCAM.Text = jthread.modem_cam ? "CAM 2" : "CAM 1";
        }

        void UpdateButtonAutopilot()
        {
            switch (jthread.modem_autopilot)
            {
                case WflyUplink.Autopilot.Autopilot:
                    buttonAUTO.Text = "AUTO";
                    break;
                case WflyUplink.Autopilot.Copilot:
                    buttonAUTO.Text = "COPLT";
                    break;
                case WflyUplink.Autopilot.Manual:
                    buttonAUTO.Text = "MANUAL";
                    break;
            }
        }
        void UpdateButtonHUD()
        {
            switch (jthread.modem_hud)
            {
                case 0:
                default:
                    buttonHUD.Text = "CLEAR";
                    break;
                case 1:
                    buttonHUD.Text = "HUD 1";
                    break;
                case 2:
                    buttonHUD.Text = "HUD 2";
                    break;
                case 3:
                    buttonHUD.Text = "HUD 3";
                    break;
            }
        }
        void UpdateButtonRuta()
        {
            switch (jthread.modem_ruta)
            {
                case WflyUplink.Ruta.Casa:
                    buttonRUTA.Text = "CASA";
                    break;

                case WflyUplink.Ruta.Hold:
                    buttonRUTA.Text = "HOLD";
                    break;

                case WflyUplink.Ruta.Ruta:
                    buttonRUTA.Text = "RUTA";
                    break;

                case WflyUplink.Ruta.Modem:
                    buttonRUTA.Text = "UPLINK";
                    break;
            }
        }

        void CheckAlarms()
        {
            if(me.AlarmAltitude_enabled && me.planeState.Alt<me.AlarmAltitude)
            {
            }
        }

        void UpdateTeams()
        {
            try
            {
                foreach (String user in mapControl1.team_pos.Keys)
                {
                    if (!comboBoxTeam.Items.Contains(user))
                    {
                        comboBoxTeam.Items.Add(user);
                    }
                }
                if (mapControl1.team_pos.Keys.Count > 0 && comboBoxTeam.SelectedIndex < 0)
                    comboBoxTeam.SelectedIndex = 0;
            }
            catch (Exception) { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button7.Text == "FIXED")
            {
                if (comboBoxTeam.SelectedItem != null)
                {
                    string nombre = comboBoxTeam.SelectedItem.ToString();
                    if (jthread != null && nombre != null)
                    {
                        WayPoint wpt;
                        if (mapControl1.team_pos.TryGetValue(nombre, out wpt))
                        {
                            jthread.modem_lon = (float)wpt.Longitude;
                            jthread.modem_lat = (float)wpt.Latitude;
                            jthread.modem_alt = (float)wpt.Altitude;
                        }
                        else
                        {
                            button7.Text = "Fail";
                        }
                    }
                }
                else
                {
                    button7.Text = "Track";
                }
            }

            if (button2.Text == "STOP")
            {
                mapControl1.SetCenter(mapControl1.plane);
                mapControl1.Invalidate();
            }
            else if (button3.Text == "STOP")
            {
                int texel = 256;
                GeoPos geo = new GeoPos(mapControl1.plane,mapControl1.GetZoom());
                double x = Math.Abs(geo.getdX(mapControl1.home))* texel;
                double y = Math.Abs(geo.getdY(mapControl1.home)) * texel;
                int zoom;

                // zona de seguridad
                x = x * 1.2f;
                y = y * 1.2f;

                while ((x > mapControl1.Width || y > mapControl1.Height) && (geo.getZoom() > 0))
                {
                    geo.setZoom(geo.getZoom() - 1);
                    x = Math.Abs(geo.getdX(mapControl1.home)) * texel;
                    y = Math.Abs(geo.getdY(mapControl1.home)) * texel;
                }

                while ((x < mapControl1.Width / 2) && (y < mapControl1.Height / 2) && (geo.getZoom() < 17))
                {
                    zoom = geo.getZoom();
                    geo.setZoom(geo.getZoom() + 1);
                    x = Math.Abs(geo.getdX(mapControl1.home)) * texel;
                    y = Math.Abs(geo.getdY(mapControl1.home)) * texel;
                }

                WayPoint medio=new WayPoint("",(mapControl1.plane.Longitude+mapControl1.home.Longitude)/2,
                    (mapControl1.plane.Latitude+mapControl1.home.Latitude)/2);
                mapControl1.SetCenter(medio);
                mapControl1.SetZoom(geo.getZoom());
                mapControl1.Invalidate();
            }/* */

            RefreshInstruments();
            UpdateTeams();
            
            if (antTracker != null)
            {
                antTracker.Send(knob_anttracker.Manual, knob_anttracker.Valor, 10);
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            int i = cbo.SelectedIndex;
            switch (i)
            {
                case 0: mapControl1.SetMode(Modes.SAT);
                    break;
                case 1: mapControl1.SetMode(Modes.MAP);
                    break;
                case 2: mapControl1.SetMode(Modes.MAP_SAT);
                    break;
                case 3: mapControl1.SetMode(Modes.TOPO);
                    break;
                default:
                    break;
            }
        }

        private void FormIkarusMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (modem != null)
                modem.dispose();

            if (capture != null)
            {
                capture.Stop();
                capture.PreviewWindow.Dispose();
                capture.PreviewWindow = null;
                capture.Dispose();
                capture.DisposeSampleGrabber();
            }

            if (this.sender != null)
                this.sender.Stop();

            if (this.KmlWebServer != null)
                this.KmlWebServer.Stop();

            if (jthread != null)
                jthread.Close();

            if (antTracker != null)
                antTracker.Close();

            if (mapControl1 != null)
                mapControl1.Destroy();

            if (avi_writer != null)
                avi_writer.avi_close();

        }

        private void splitContainer2_Panel1_Resize(object sender, EventArgs e)
        {
            splitContainer2.SplitterDistance = splitContainer2.Size.Height * 4 / 3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mapControl1.SetCenter(mapControl1.home);
            mapControl1.Refresh();
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            if (button2.Text == "STOP")
            {
                button2.Text = "PLANE";
            }
            else if (e.Button == MouseButtons.Left)
            {
                mapControl1.SetCenter(mapControl1.plane);
                mapControl1.Invalidate();
            }
            else
            {
                button2.Text = "STOP";
                button3.Text = "TRACK";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text != "TRACK")
                button3.Text = "TRACK";
            else
            {
                button2.Text = "PLANE";
                button3.Text = "STOP";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text != "Normal")
            {
                button4.Text = "Normal";
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                button4.Text = "FullScr";
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
            }
        }

        private void buttonRUTA_MouseDown(object sender, MouseEventArgs e)
        {
            if (jthread != null)
            {
                if (e.Button == MouseButtons.Left)
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.GoSWnext);
                else
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.GoSWprev);
                UpdateButtonRuta();
            }
        }
        private void buttonAUTO_MouseDown(object sender, MouseEventArgs e)
        {
            if (jthread != null)
            {
                if (e.Button == MouseButtons.Left)
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.AutoPilotSWnext);
                else
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.AutoPilotSWprev);
                
                UpdateButtonAutopilot();
            }
        }

        private void buttonHUD_MouseDown(object sender, MouseEventArgs e)
        {
            if (jthread != null)
            {
                if (e.Button == MouseButtons.Left)
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.HudSWnext);
                else
                    jthread.ProcesarEventoJoy(Singleton.EventosJoy.HudSWprev);
                UpdateButtonHUD();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int altitud = (int)numericUpDown1.Value;
            if (jthread != null)
                jthread.modem_alt = altitud;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            mapControl1.listeners += home_soft;
        }

        void home_soft(WayPoint waypoint, MouseButtons btn)
        {
            if (btn == MouseButtons.Left)
            {
                if (jthread != null)
                {
                    jthread.modem_lon = (float)waypoint.Longitude;
                    jthread.modem_lat = (float)waypoint.Latitude;
                }

                mapControl1.soft_wpt = waypoint;
                mapControl1.listeners -= home_soft;
                mapControl1.Invalidate();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0)
            {
                float f=0.0f;
                if (float.TryParse(textBox1.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                {
                    int b;
                    debug_values[comboBox3.SelectedIndex, comboBox2.SelectedIndex] = f;
                    b = (comboBox3.SelectedIndex) * 8; //<<3
                    b |= comboBox2.SelectedIndex;
                    if (jthread != null)
                    {
                        jthread.SetDebug((byte)b, f);
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
             if (jthread != null)
            {
                jthread.ProcesarEventoJoy(Singleton.EventosJoy.CamSW);
                UpdateButtonCamera();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            jthread.SetMasks(checkBox1.Checked); 
        }

        private void debug_combo_chg(object sender, EventArgs e)
        {
            int idx = comboBox2.SelectedIndex;
            if (comboBox3.SelectedIndex > 3)
            {
                this.comboBox2.Items.Clear();
                this.comboBox2.Items.AddRange(new object[] {"IR Pitch", "IR Roll", "IR Gain", "Rumbo Ail", "Altura Ele"});
                this.comboBox2.SelectedIndex = idx;
            }
            else if (comboBox3.SelectedIndex >= 0)
            {
                this.comboBox2.Items.Clear();
                this.comboBox2.Items.AddRange(new object[] {"P gain", "I gain", "D gain", "I limit", "Drive Lim" });
                this.comboBox2.SelectedIndex = idx;
            }
        }

        private void debug_combo_chg2(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0)
            {
                textBox1.Text = debug_values[comboBox3.SelectedIndex, comboBox2.SelectedIndex].ToString();
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (button7.Text != "Track")
                button7.Text = "Track";
            else
                button7.Text = "FIXED";
        }


        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Tag==null ||numericUpDown2.Value != (int)numericUpDown2.Tag)
                jthread.wpt_id = (byte)numericUpDown2.Value;
        }

        int pantalla_modo;
        private void FormIkarusMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                float ratio =(float)this.Width / this.Height;

                if (pantalla_modo == 0 && ratio < 1.6f)
                {
                    pantalla_modo = 2;
                }
                else if (pantalla_modo < 3)
                {
                    pantalla_modo++;
                }
                else
                    pantalla_modo = 0;

                switch (pantalla_modo)
                {
                    case 0: splitContainer1.Panel2Collapsed = false;
                            splitContainer2.Panel2Collapsed = false;
                            splitContainer2.Panel1Collapsed = false;
                            break;
                    case 1: splitContainer1.Panel2Collapsed = true;
                            splitContainer2.Panel2Collapsed = false;
                            splitContainer2.Panel1Collapsed = false;
                            break;
                    case 2: splitContainer1.Panel2Collapsed = true;
                            splitContainer2.Panel2Collapsed = true;
                            splitContainer2.Panel1Collapsed = false;
                            break;
                    case 3: splitContainer1.Panel2Collapsed = true;
                            splitContainer2.Panel2Collapsed = false;
                            splitContainer2.Panel1Collapsed = true;
                            break;

                }
            }
        }

        private void FormIkarusMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keypressed)
            {
                if (jthread != null)
                {
                    for (int i = 0; i < me.asignaciones.Length; i++)
                    {
                        if (me.asignaciones[i] == (int)e.KeyData)
                        {
                            jthread.ProcesarEventoJoy((Singleton.EventosJoy)(i + 1));
                            UpdateButtonHUD();
                            UpdateButtonCamera();
                            UpdateButtonAutopilot();
                            UpdateButtonRuta();
                            keypressed = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                }
                else
                {
                    if (me.asignaciones[(int)Singleton.EventosJoy.Picture-1] == (int)e.KeyData)
                        this.Capturar_Foto();
                }
            }
        }

        private void FormIkarusMain_KeyUp(object sender, KeyEventArgs e)
        {
            keypressed = false;
            e.SuppressKeyPress = true;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            if (comboBoxTeam.SelectedIndex >= 0)
            {
                string nombre = comboBoxTeam.SelectedItem.ToString();

                try
                {
                    WayPoint wpt;
                    if (mapControl1.team_pos.TryGetValue(nombre, out wpt))
                    {
                        mapControl1.SetCenter(wpt);
                        mapControl1.Invalidate();
                    }
                }
                catch (Exception) { }
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            jthread.SendCmdCenterIR();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            jthread.SendCmdGainIR();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            this.Capturar_Foto();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            jthread.SendCmdSetHome();

        }

        private void button14_Click(object sender, EventArgs e)
        {
            jthread.SendCmdSaveFlash();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (saving_video)
            {
                stoping_video = true;
                button15.Image = UAVConsole.Properties.Resources.video;

            }
            else
            {
                starting_video = true;
                button15.Image = UAVConsole.Properties.Resources.video_red;
            }
        }

        public void Capturar_Foto()
        {
            saving_picture = true;
        }

        private void CaptureDone(System.Drawing.Bitmap e)
        {
            const float parse_fps = 10.0f;
            float video_fps = me.fpsVideo;
            long max_size = me.trocearTamMB*1048576;
           
            
            if (saving_picture)
            {
                saving_picture = false;

                string path = me.PicturePath;
                if (path != null && path != "")
                {
                    DateTime dt = DateTime.Now;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string filename = path + "\\PIC_"
                        + dt.Year.ToString("00") + dt.Month.ToString("00") + dt.Day.ToString("00")
                        + "-" + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00") + ".jpg";

                    try
                    {
                        e.Save(filename, ImageFormat.Jpeg);
                    }
                    catch (Exception) { }
                }

            }

            if (starting_video)
            {
                starting_video = false;
                
                string path = me.VideosPath;
                if (path != null && path != "")
                {
                    DateTime dt = DateTime.Now;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string filename = path + "\\VID_"
                        + dt.Year.ToString("00") + dt.Month.ToString("00") + dt.Day.ToString("00")
                        + "-" + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00")+".avi";

                    avi_writer = new AviWriter();
                    avi_writer.avi_start(filename);

                    saving_video = true;
                    button15.Image = UAVConsole.Properties.Resources.video_red;
                }

            }
            else if (stoping_video)
            {
                saving_video = false;
                stoping_video = false;
                avi_writer.avi_close();
                avi_writer = null;
                
            }
            else if (saving_video && DateTime.Now.Ticks > video_last_tick)
            {
                video_last_tick = DateTime.Now.Ticks + (long)(TimeSpan.TicksPerSecond / video_fps);
                if (me.trocearVideo && max_size > 0 && avi_writer.current_lenght > max_size)
                {
                    avi_writer.avi_close();
                    avi_writer = new AviWriter();
                    DateTime dt = DateTime.Now;

                    string filename = me.VideosPath + "\\VID_"
                       + dt.Year.ToString("00") + dt.Month.ToString("00") + dt.Day.ToString("00")
                       + "-" + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00") + ".avi";
                    avi_writer.avi_start(filename);
                }
                    avi_writer.SaveFrame(e, me.calidadVideo);
              
            }

            if (modem is ModemVideo && DateTime.Now.Ticks > parse_last_tick)
            {
                parse_last_tick = DateTime.Now.Ticks + (long)(TimeSpan.TicksPerSecond / parse_fps);
                ((ModemVideo)modem).SetImage(e);
            }

            capture.GrapImg2();
        }

       
    }
}