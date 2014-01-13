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
using System.Text;
using DirectX.Capture;
using Microsoft.Win32;
using UAVConsole.GoogleMaps;
using System.IO;
using UAVConsole.Modem;
using System.Globalization;

namespace UAVConsole
{
    public class Singleton
    {
        static Singleton me;
        private static readonly object singletonLock = new object();

        public const string registryPath = "Software\\Ikarus\\Ikarus OSD";
           
        public enum VideoSystem { PAL, NTSC };
        public enum Telemetria { None, Video, XBEE, AntTracker };
        public enum Mezclas { Normal, Elevon, V_Tail, Flaperon, Flaperon_Vtail };
        public enum ModoControl { MODO_SW2, MODO_SW3_BASIC, MODO_SW3, MODO_RUEDA, MODO_MIX223, MODO_AJUSTE223, MODO_MIX224, MODO_MODEM };
        public enum ModuloControl { Ninguno, Uplink, XBEE };
        public enum Canales { CTRL, AIL, ELE, THR, TAIL, PAN, TILT, FLAPS };
        public enum ModoCanalAux { AutoPilot, WayPoint, TILT, AIL2 };

        public enum EventosJoy {None, AutoPltON, AutoPltCo, AutoPilotOFF, AutoPilotSWnext, AutoPilotSWprev,  CamA, CamB, CamSW,
            HudCL, Hud1, Hud2, Hud3, HudSWnext, HudSWprev, GoWptRuta, GoHome, GoHold, GoUplink, GoSWnext, GoSWprev, PanTiltCenter, 
            FlapsOff, FlapsFull, FlapsInc, FlapsDec, TrimAil, TrimEle, TrimTail, TrimAll, Picture
            };

        public enum Idiomas { Spanish, English };
        public enum SistemasMetricos { Metrico, Imperial };

        // Non persistent members
        public Capture videoCapture;
        public PlaneState planeState;
        public List<WayPoint> Ruta;
        public string default_path = Directory.GetCurrentDirectory();
        //public JoystickThread jthread;

        public float uplink_voltage;
        public float anttracker_voltage;

        // Persistent members
        public string GoogleMapsCookie;
        public string CacheMapsPath = "C:\\ImagesMaps";
        public string FlightLogPath = "C:\\FlightLogs";
        public string PicturePath = "C:\\Fotos";
        public string VideosPath = "C:\\Videos";

        public string videoCaptureStr;
        public Telemetria telemetria;
        public VideoSystem videosystem;
        public string commPort;
        public int commBps;

        public int cells1 = 3;
        public int cells2 = 3;
       
        public float HomeLat;
        public float HomeLon;
        public float HomeAlt;
        public string NombrePiloto;
        public bool enableWebServer;
        public bool enableUDPinout;
        public int portWebServer;
        public int portUDPinout;

        public int Idioma;
        public int SistemaMetrico;

        // Configuracion Joystick
        public int []servo_min, servo_center, servo_max;
        public bool []servo_rev;
        public byte[] servo_ch;
        
        public byte[] joy_buttons;
        public Mezclas tipo_mezcla;
      //  public ModoControl tipo_control;
        public bool rev_mix, rev_flap;
        public float full_flap, step_flap;
        public float pantilt_speed;
        public bool enable_axis;
        public bool enable_pan, enable_tilt;
        
        // Configuracion Uplink
        public ModuloControl moduloTX = ModuloControl.Ninguno;

        public bool enable_headtrack;
        public byte headtrack_panCh;
        public byte headtrack_tiltCh;
       
        public float uplinkVmax;
        public float uplinkVmin;
        public float uplinkValarm;
        public bool uplinkLipo;
        public int uplinkNumCells;

        // Parametros emisora
        public bool useEmisora;
        public byte txNumCanales;
        public int txPeriodo;
        public int txSeparador;
        public bool txPolarity;

        // Configuracion Anttracer
        public bool enableAntTrack;
        public bool enableCasaAntTrack;

        // Alarmas sonoras
        public bool AlarmAltitude_enabled;
        public float AlarmAltitude;

        public bool AlarmFastDescentRate_enabled;
        public float AlarmFastDescentRate;

        public bool AlarmDistance_enabled;
        public float AlarmDistance;

        public bool AlarmCellVoltage_enabled;
        public float AlarmCellVoltage;

        public bool AlarmAscenso_enabled;
        public float AlarmAscenso;

        // shortcut teclado

        public int[] asignaciones;

        // Uplink debug values
        public float uplink_pid_ail_P;
        public float uplink_pid_ail_I;
        public float uplink_pid_ail_D;
        public float uplink_pid_ail_IL;
        public float uplink_pid_ail_DL;

        public float uplink_pid_ele_P;
        public float uplink_pid_ele_I;
        public float uplink_pid_ele_D;
        public float uplink_pid_ele_IL;
        public float uplink_pid_ele_DL;

        public float uplink_pid_thr_P;
        public float uplink_pid_thr_I;
        public float uplink_pid_thr_D;
        public float uplink_pid_thr_IL;
        public float uplink_pid_thr_DL;

        public float uplink_pid_tail_P;
        public float uplink_pid_tail_I;
        public float uplink_pid_tail_D;
        public float uplink_pid_tail_IL;
        public float uplink_pid_tail_DL;

        public float uplink_IR_offX;
        public float uplink_IR_offY;
        public float uplink_IR_gain;
        public float uplink_rumbo_ail;
        public float uplink_altura_ele;

        public bool uplink_IR_rev_P;
        public bool uplink_IR_rev_R;
        public bool uplink_IR_cross;
        public bool uplink_IR_rev_cross;

       
        // Parametros captura de video

        public bool trocearVideo;
        public int trocearTamMB;
        public int calidadVideo;
        public int fpsVideo;

        protected Singleton()
        {
            asignaciones = new int[Enum.GetNames(typeof(EventosJoy)).Length - 1];
            Ruta = new List<WayPoint>();
            
            commPort = "";
            videoCaptureStr = "";
            HomeLon = -6.0345f;
            HomeLat = 37.2342f;
            HomeAlt = 100.0f;
            telemetria = Telemetria.Video;
            videosystem = VideoSystem.PAL;
            
            this.NombrePiloto = "Ikarus";
            this.enableUDPinout = false;
            this.enableWebServer = false;
            this.portWebServer = 8080;
            this.portUDPinout = 9500;

            this.Idioma = 0;    // Español
            this.SistemaMetrico = 0; // Metrico
   
            this.servo_ch = new byte[8];
            this.servo_min = new int[servo_ch.Length];
            this.servo_center = new int[servo_ch.Length];
            this.servo_max = new int[servo_ch.Length];
            this.servo_rev = new bool[servo_ch.Length];
         

            for (int i = 0; i < servo_ch.Length; i++)
            {
                this.servo_min[i] = 1000;
                this.servo_center[i] = 1500;
                this.servo_max[i] = 2000;
                this.servo_rev[i] = false;
                this.servo_ch[i] = (byte)i;
            }

            this.joy_buttons = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                this.joy_buttons[i] = 0;
            }
            this.tipo_mezcla = Mezclas.Normal;
          //  this.tipo_control = ModoControl.MODO_MODEM;
            this.rev_mix = false;
            this.rev_flap = false;
            this.full_flap = 1.0f;
            this.step_flap = 0.1f;

            this.pantilt_speed = 1.0f;

            this.AlarmAltitude_enabled = false;
            this.AlarmAscenso_enabled = false;
            this.AlarmCellVoltage_enabled = false;
            this.AlarmDistance_enabled = false;
            this.AlarmFastDescentRate_enabled = false;

            this.AlarmAltitude = 80.0f;
            this.AlarmAscenso = 5.0f;
            this.AlarmCellVoltage = 3.5f;
            this.AlarmDistance = 1000.0f;
            this.AlarmFastDescentRate = 10.0f;

            this.enable_headtrack = false;
            this.headtrack_panCh = 0;
            this.headtrack_tiltCh = 1;

            this.uplinkVmax = 12.6f;
            this.uplinkVmin = 9.6f;
            this.uplinkValarm = 10.0f;
            this.uplinkLipo = true;
            this.uplinkNumCells = 3;

            this.enableAntTrack = false;
            this.enableCasaAntTrack = false;

            this.useEmisora = true;
            this.txNumCanales = 8;
            this.txPeriodo = 20000;
            this.txSeparador = 400;
            this.txPolarity = false;

            // Uplink debug values
            this.uplink_pid_ail_P=0.02f;
            this.uplink_pid_ail_I=0.0f;
            this.uplink_pid_ail_D=0.0f;
            this.uplink_pid_ail_IL=0.0f;
            this.uplink_pid_ail_DL=1.0f;

            this.uplink_pid_ele_P=0.02f;
            this.uplink_pid_ele_I=0.0f;
            this.uplink_pid_ele_D=0.0f;
            this.uplink_pid_ele_IL=0.0f;
            this.uplink_pid_ele_DL=1.0f;

            this.uplink_pid_thr_P=0.02f;
            this.uplink_pid_thr_I=0.0f;
            this.uplink_pid_thr_D=0.0f;
            this.uplink_pid_thr_IL=0.0f;
            this.uplink_pid_thr_DL=1.0f;

            this.uplink_pid_tail_P=0.02f;
            this.uplink_pid_tail_I=0.0f;
            this.uplink_pid_tail_D=0.0f;
            this.uplink_pid_tail_IL=0.0f;
            this.uplink_pid_tail_DL=1.0f;

            this.uplink_IR_offX=1.66f;
            this.uplink_IR_offY = 1.66f;
            this.uplink_IR_gain = 1.14f;
            this.uplink_rumbo_ail = 20.0f;
            this.uplink_altura_ele = 10.0f;

            this.uplink_IR_rev_P = false;
            this.uplink_IR_rev_R = false;
            this.uplink_IR_cross = false;
            this.uplink_IR_rev_cross = false;

            this.trocearVideo = false;
            this.trocearTamMB = 1024;
            this.calidadVideo = 50;
            this.fpsVideo = 15;


            FromRegistry();
        }

        public static Singleton GetInstance()
        {
            lock (singletonLock)
            {

                if (me == null)
                {
                    me = new Singleton();
                }
                return me;
            }
        }

        public void FromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath);
            if (key != null)
            {
                try
                {
                    this.GoogleMapsCookie = (string)key.GetValue("GoogleMapsCookie");
                    this.HomeLon = float.Parse(key.GetValue("HomeLon").ToString(),CultureInfo.InvariantCulture);
                    this.HomeLat = float.Parse(key.GetValue("HomeLat").ToString(), CultureInfo.InvariantCulture);
                    this.HomeAlt = float.Parse(key.GetValue("HomeAlt").ToString(), CultureInfo.InvariantCulture);

                    this.videoCaptureStr = (string)key.GetValue("VideoCaptureStr");
                    this.telemetria = (Telemetria)key.GetValue("Telemetry");
                    this.videosystem = (VideoSystem)key.GetValue("VideoSystem");
                    this.commPort = (string)key.GetValue("CommPort");
                    this.commBps = (int)key.GetValue("CommBauds");
                    this.CacheMapsPath = (string)key.GetValue("CacheMapsPath");
                    this.FlightLogPath = (string)key.GetValue("FlightLogPath");
                    this.cells1 = (int)key.GetValue("CellCount1");
                    this.cells2 = (int)key.GetValue("CellCount2");


                    this.moduloTX = (Singleton.ModuloControl)(int)key.GetValue("ModuloTX");

                    try
                    {
                        for (int i = 0; i < servo_ch.Length; i++)
                        {
                            this.servo_min[i] = (int)key.GetValue("ServoMin" + i);
                            this.servo_center[i] = (int)key.GetValue("ServoCenter" + i);
                            this.servo_max[i] = (int)key.GetValue("ServoMax" + i);
                            this.servo_rev[i] = (int)key.GetValue("ServoRev" + i) == 1;
                            this.servo_ch[i] = (byte)((int)key.GetValue("ServoCh" + i));
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            this.joy_buttons[i] = (byte)((int)key.GetValue("JoyButtom" + i));
                        }

                        this.tipo_mezcla = (Mezclas)key.GetValue("TipoMezcla");
                        //      this.tipo_control = (ModoControl)key.GetValue("TipoControl");
                        this.rev_mix = (int)key.GetValue("rev_mix") == 1;
                        this.rev_flap = (int)key.GetValue("rev_flap") == 1;
                        this.full_flap = float.Parse(key.GetValue("full_flap").ToString());
                        this.step_flap = float.Parse(key.GetValue("step_flap").ToString());

                        this.pantilt_speed = float.Parse(key.GetValue("pantilt_speed").ToString());
                        this.enable_axis = (int)key.GetValue("AXISenable") == 1;
                        this.enable_pan = (int)key.GetValue("PANenable") == 1;
                        this.enable_tilt = (int)key.GetValue("TILTenable") == 1;

                        this.enable_headtrack = (int)key.GetValue("enable_headtrack") == 1;
                        object pepe = key.GetValue("headtrack_panCh");
                        this.headtrack_panCh = (byte)((int)key.GetValue("headtrack_panCh"));
                        this.headtrack_tiltCh = (byte)((int)key.GetValue("headtrack_tiltCh"));

                        this.uplinkVmax = float.Parse(key.GetValue("uplinkVmax").ToString(), CultureInfo.InvariantCulture);
                        this.uplinkVmin = float.Parse(key.GetValue("uplinkVmin").ToString(), CultureInfo.InvariantCulture);
                        this.uplinkValarm = float.Parse(key.GetValue("uplinkValarm").ToString(), CultureInfo.InvariantCulture);
                        this.uplinkLipo = (int)key.GetValue("uplinkLipo") == 1;
                        this.uplinkNumCells = (int)key.GetValue("uplinkNumCells");

                        this.useEmisora = (int)key.GetValue("useEmisora") == 1;
                        this.txNumCanales = (byte)((int)key.GetValue("txNumCanales")); ;
                        this.txPeriodo = (int)key.GetValue("txPeriodo"); ;
                        this.txSeparador = (int)key.GetValue("txSeparador");
                        this.txPolarity = (int)key.GetValue("txPolarity") == 1;
                    }
                    catch (Exception) { };
                    //Configuracion joystick

                    try
                    {

                        this.Idioma = (int)key.GetValue("Language");
                        this.SistemaMetrico = (int)key.GetValue("SistemaMetrico");
                    }
                    catch (Exception) { }

                    this.NombrePiloto = key.GetValue("NombrePiloto").ToString();
                    
                    this.portUDPinout = (int)key.GetValue("portUDPinout");
                    this.portWebServer = (int)key.GetValue("portWebServer");

                    this.enableWebServer = (int)key.GetValue("enableWebServer") == 1;
                    this.enableUDPinout = (int)key.GetValue("enableUDPinout") == 1;

                    try
                    {
                        this.AlarmAltitude_enabled = (int)key.GetValue("AlarmAltitude_enabled") == 1;
                        this.AlarmAltitude = float.Parse(key.GetValue("AlarmAltitude").ToString(), CultureInfo.InvariantCulture);

                        this.AlarmAscenso_enabled = (int)key.GetValue("AlarmAscenso_enabled") == 1;
                        this.AlarmAscenso = float.Parse(key.GetValue("AlarmAscenso").ToString(), CultureInfo.InvariantCulture);

                        this.AlarmCellVoltage_enabled = (int)key.GetValue("AlarmCellVoltage_enabled") == 1;
                        this.AlarmCellVoltage = float.Parse(key.GetValue("AlarmCellVoltage").ToString(), CultureInfo.InvariantCulture);

                        this.AlarmDistance_enabled = (int)key.GetValue("AlarmDistance_enabled") == 1;
                        this.AlarmDistance = float.Parse(key.GetValue("AlarmDistance").ToString(), CultureInfo.InvariantCulture);

                        this.AlarmFastDescentRate_enabled = (int)key.GetValue("AlarmFastDescentRate_enabled") == 1;
                        this.AlarmFastDescentRate = float.Parse(key.GetValue("AlarmFastDescentRate").ToString(), CultureInfo.InvariantCulture);
                    }
                    catch (Exception) { };

                    try
                    {
                        this.enableAntTrack = (int)key.GetValue("enableAntTrack") == 1;
                        this.enableCasaAntTrack = (int)key.GetValue("enaleCasaAntTrack")==1;
                    
                    } catch (Exception){};

                    try
                    {
                        for (int i = 0; i < asignaciones.Length; i++)
                        {
                            this.asignaciones[i] = (int)key.GetValue("TeclaAsignada" + i);
                        }
                    }
                    catch (Exception) { };

                    try
                    {
                        // Uplink debug values
                        this.uplink_pid_ail_P = float.Parse(key.GetValue("uplink_pid_ail_P").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ail_I = float.Parse(key.GetValue("uplink_pid_ail_I").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ail_D = float.Parse(key.GetValue("uplink_pid_ail_D").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ail_IL = float.Parse(key.GetValue("uplink_pid_ail_IL").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ail_DL = float.Parse(key.GetValue("uplink_pid_ail_DL").ToString(), CultureInfo.InvariantCulture);

                        this.uplink_pid_ele_P = float.Parse(key.GetValue("uplink_pid_ele_P").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ele_I = float.Parse(key.GetValue("uplink_pid_ele_I").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ele_D = float.Parse(key.GetValue("uplink_pid_ele_D").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ele_IL = float.Parse(key.GetValue("uplink_pid_ele_IL").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_ele_DL = float.Parse(key.GetValue("uplink_pid_ele_DL").ToString(), CultureInfo.InvariantCulture);

                        this.uplink_pid_thr_P = float.Parse(key.GetValue("uplink_pid_thr_P").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_thr_I = float.Parse(key.GetValue("uplink_pid_thr_I").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_thr_D = float.Parse(key.GetValue("uplink_pid_thr_D").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_thr_IL = float.Parse(key.GetValue("uplink_pid_thr_IL").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_thr_DL = float.Parse(key.GetValue("uplink_pid_thr_DL").ToString(), CultureInfo.InvariantCulture);

                        this.uplink_pid_tail_P = float.Parse(key.GetValue("uplink_pid_tail_P").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_tail_I = float.Parse(key.GetValue("uplink_pid_tail_I").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_tail_D = float.Parse(key.GetValue("uplink_pid_tail_D").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_tail_IL = float.Parse(key.GetValue("uplink_pid_tail_IL").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_pid_tail_DL = float.Parse(key.GetValue("uplink_pid_tail_DL").ToString(), CultureInfo.InvariantCulture);

                        this.uplink_IR_offX = float.Parse(key.GetValue("uplink_IR_offX").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_IR_offY = float.Parse(key.GetValue("uplink_IR_offY").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_IR_gain = float.Parse(key.GetValue("uplink_IR_gain").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_rumbo_ail = float.Parse(key.GetValue("uplink_rumbo_ail").ToString(), CultureInfo.InvariantCulture);
                        this.uplink_altura_ele = float.Parse(key.GetValue("uplink_altura_ele").ToString(), CultureInfo.InvariantCulture);

                        this.uplink_IR_rev_P = (int)key.GetValue("uplink_IR_rev_P") == 1;
                        this.uplink_IR_rev_R = (int)key.GetValue("uplink_IR_rev_R") == 1;
                        this.uplink_IR_cross = (int)key.GetValue("uplink_IR_cross") == 1;
                        this.uplink_IR_rev_cross = (int)key.GetValue("uplink_IR_rev_cross")==1;
                    }
                    catch (Exception) { };

                    this.PicturePath = (string)key.GetValue("PicturePath", "C:\\Fotos");
                    this.VideosPath = (string)key.GetValue("VideosPath", "C:\\Videos");

                    try
                    {
                        this.trocearVideo = (int)key.GetValue("trocearVideo") == 1;
                        this.trocearTamMB = (int)key.GetValue("trocearTamMB");
                        this.calidadVideo = (int)key.GetValue("calidadVideo");
                        this.fpsVideo = (int)key.GetValue("fpsVideo");
                    }
                    catch (Exception) { };
                }
                catch (Exception)
                {
                }        
            }
            else
                Registry.CurrentUser.CreateSubKey(registryPath);

        }

        public void ToRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, true);

            key.SetValue("VideoCaptureStr", this.videoCaptureStr);
            key.SetValue("Telemetry", (int)this.telemetria);
            key.SetValue("VideoSystem", (int)this.videosystem);
            key.SetValue("CommPort", this.commPort);
            key.SetValue("CommBauds", this.commBps);

            key.SetValue("HomeLon", this.HomeLon.ToString(CultureInfo.InvariantCulture));
            key.SetValue("HomeLat", this.HomeLat.ToString(CultureInfo.InvariantCulture));
            key.SetValue("HomeAlt", this.HomeAlt.ToString(CultureInfo.InvariantCulture));

            key.SetValue("CacheMapsPath", this.CacheMapsPath);
            key.SetValue("FlightLogPath", this.FlightLogPath);
            key.SetValue("PicturePath", this.PicturePath);
            key.SetValue("VideosPath", this.VideosPath);
            key.SetValue("CellCount1", this.cells1);
            key.SetValue("CellCount2", this.cells2);
 
            //Configuracion joystick
            for (int i = 0; i < servo_ch.Length; i++)
            {
                key.SetValue("ServoMin" + i, this.servo_min[i]);
                key.SetValue("ServoCenter" + i, this.servo_center[i]);
                key.SetValue("ServoMax" + i, this.servo_max[i]);
                key.SetValue("ServoRev" + i, this.servo_rev[i] ? 1 : 0);
                key.SetValue("ServoCh" + i, (int)this.servo_ch[i]);
            }

            for (int i = 0; i < 16; i++)
            {
                key.SetValue("JoyButtom" + i, (int)this.joy_buttons[i]);
            }

            key.SetValue("TipoMezcla", (int)this.tipo_mezcla);
        //    key.SetValue("TipoControl", (int)this.tipo_control);
            key.SetValue("rev_mix", this.rev_mix ? 1 : 0);
            key.SetValue("rev_flap", this.rev_flap ? 1 : 0);

            key.SetValue("full_flap", this.full_flap);
            key.SetValue("step_flap", this.step_flap);

            key.SetValue("pantilt_speed", this.pantilt_speed);

            key.SetValue("AXISenable", this.enable_axis ? 1 : 0);
            key.SetValue("PANenable", this.enable_pan ? 1 : 0);
            key.SetValue("TILTenable", this.enable_tilt ? 1 : 0);
            
            // Uplink
            key.SetValue("ModuloTX", (int)this.moduloTX);
            key.SetValue("enable_headtrack", this.enable_headtrack ? 1 : 0);
            key.SetValue("headtrack_panCh", (int)this.headtrack_panCh);
            key.SetValue("headtrack_tiltCh", (int)this.headtrack_tiltCh);

            key.SetValue("useEmisora", this.useEmisora ? 1 : 0);
            key.SetValue("txNumCanales", (int)this.txNumCanales);
            key.SetValue("txPeriodo", this.txPeriodo);
            key.SetValue("txSeparador", this.txSeparador);
            key.SetValue("txPolarity", this.txPolarity ? 1 : 0);
           
            key.SetValue("uplinkVmax", this.uplinkVmax.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplinkVmin", this.uplinkVmin.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplinkValarm", this.uplinkValarm.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplinkLipo", this.uplinkLipo ? 1 : 0);
            key.SetValue("uplinkNumCells", this.uplinkNumCells);
            
            // AntTracker
            key.SetValue("enableAntTrack", this.enableAntTrack ? 1 : 0);
            key.SetValue("enaleCasaAntTrack", this.enableCasaAntTrack ? 1 : 0);

            key.SetValue("NombrePiloto", this.NombrePiloto);
            key.SetValue("enableWebServer", this.enableWebServer ? 1 : 0);
            key.SetValue("enableUDPinout", this.enableUDPinout ? 1 : 0);

            //if (this.enableUDPinout)
            key.SetValue("portUDPinout", this.portUDPinout);
            //if (this.enableWebServer)
            key.SetValue("portWebServer", this.portWebServer);

            
            key.SetValue("Language", this.Idioma);
            key.SetValue("SistemaMetrico", this.SistemaMetrico);


            key.SetValue("AlarmAltitude_enabled", this.AlarmAltitude_enabled ? 1 : 0);
            key.SetValue("AlarmAltitude", this.AlarmAltitude.ToString(CultureInfo.InvariantCulture));

            key.SetValue("AlarmAscenso_enabled", this.AlarmAscenso_enabled ? 1 : 0);
            key.SetValue("AlarmAscenso", this.AlarmAscenso.ToString(CultureInfo.InvariantCulture));

            key.SetValue("AlarmCellVoltage_enabled", this.AlarmCellVoltage_enabled ? 1 : 0);
            key.SetValue("AlarmCellVoltage", this.AlarmCellVoltage.ToString(CultureInfo.InvariantCulture));

            key.SetValue("AlarmDistance_enabled", this.AlarmDistance_enabled ? 1 : 0);
            key.SetValue("AlarmDistance", this.AlarmDistance.ToString(CultureInfo.InvariantCulture));

            key.SetValue("AlarmFastDescentRate_enabled", this.AlarmFastDescentRate_enabled ? 1 : 0);
            key.SetValue("AlarmFastDescentRate", this.AlarmFastDescentRate.ToString(CultureInfo.InvariantCulture));


            for (int i = 0; i < asignaciones.Length; i++)
            {
                key.SetValue("TeclaAsignada" + i, this.asignaciones[i]);
            }


            // Uplink debug values
            key.SetValue("uplink_pid_ail_P", this.uplink_pid_ail_P.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ail_I", this.uplink_pid_ail_I.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ail_D", this.uplink_pid_ail_D.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ail_IL", this.uplink_pid_ail_IL.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ail_DL", this.uplink_pid_ail_DL.ToString(CultureInfo.InvariantCulture));

            key.SetValue("uplink_pid_ele_P", this.uplink_pid_ele_P.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ele_I", this.uplink_pid_ele_I.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ele_D", this.uplink_pid_ele_D.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ele_IL", this.uplink_pid_ele_IL.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_ele_DL", this.uplink_pid_ele_DL.ToString(CultureInfo.InvariantCulture));

            key.SetValue("uplink_pid_thr_P", this.uplink_pid_thr_P.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_thr_I", this.uplink_pid_thr_I.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_thr_D", this.uplink_pid_thr_D.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_thr_IL", this.uplink_pid_thr_IL.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_thr_DL", this.uplink_pid_thr_DL.ToString(CultureInfo.InvariantCulture));

            key.SetValue("uplink_pid_tail_P", this.uplink_pid_tail_P.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_tail_I", this.uplink_pid_tail_I.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_tail_D", this.uplink_pid_tail_D.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_tail_IL", this.uplink_pid_tail_IL.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_pid_tail_DL", this.uplink_pid_tail_DL.ToString(CultureInfo.InvariantCulture));

            key.SetValue("uplink_IR_offX", this.uplink_IR_offX.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_IR_offY", this.uplink_IR_offY.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_IR_gain", this.uplink_IR_gain.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_rumbo_ail", this.uplink_rumbo_ail.ToString(CultureInfo.InvariantCulture));
            key.SetValue("uplink_altura_ele", this.uplink_altura_ele.ToString(CultureInfo.InvariantCulture));

            key.SetValue("uplink_IR_rev_P", this.uplink_IR_rev_P ? 1 : 0);
            key.SetValue("uplink_IR_rev_R", this.uplink_IR_rev_R ? 1 : 0);
            key.SetValue("uplink_IR_cross", this.uplink_IR_cross ? 1 : 0);
            key.SetValue("uplink_IR_rev_cross", this.uplink_IR_rev_cross ? 1 : 0);

            key.SetValue("trocearVideo", this.trocearVideo ? 1 : 0);
            key.SetValue("trocearTamMB", this.trocearTamMB );
            key.SetValue("calidadVideo", this.calidadVideo);
            key.SetValue("fpsVideo", this.fpsVideo);
        }
    }
}
