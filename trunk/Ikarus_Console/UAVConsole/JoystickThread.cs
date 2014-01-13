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
using System.Threading;
using UAVConsole.ConfigClasses;
using UAVConsole.GoogleMaps;
using UAVConsole.USBXpress;
using System.Windows.Forms;

namespace UAVConsole
{
    public class JoystickThread 
    {
        Singleton singleton = Singleton.GetInstance();
        FormIkarusMain formIkarusMain;

        EmisoraUSB wfly;
        //public Mezclas tipo_mezcla;
        System.Threading.Timer timer;

        float pan, tilt, flaps;
        WflyUplink.Autopilot _modem_autopilot;
        WflyUplink.Ruta _modem_ruta;

        bool _modem_cam;
        int _modem_hud = 1;
        float _modem_lon, _modem_lat, _modem_alt;
        byte _wpt_id;

        byte[] _mask;
        bool changed_switches, changed_lonlat, changed_alt, changed_mask, changed_wptid, trig_cmdCenterIR, trig_cmdGainIR, trig_cmdSetHome, trig_cmdSaveFlash;

        int old_buttons;

        bool terminate;

        float TrimAil, TrimEle, TrimTail;

        bool configurado = false;
        bool joy_enabled = false;


        public byte wpt_id
        {
            get { return _wpt_id; }
            set { _wpt_id = value; changed_wptid = true; }
        }

        public WflyUplink.Ruta modem_ruta
        {
            get { return _modem_ruta; }
            set { _modem_ruta = value; changed_switches = true; }
        }

        public WflyUplink.Autopilot modem_autopilot
        {
            get { return _modem_autopilot; }
            set { _modem_autopilot = value; changed_switches = true; }
        }
        
        public bool modem_cam
        {
            get { return _modem_cam; }
            set { _modem_cam = value; changed_switches = true; }
        }
        public int modem_hud
        {
            get { return _modem_hud; }
            set { _modem_hud = value; changed_switches = true; }
        }

        public float modem_alt
        {
            get { return _modem_alt; }
            set { _modem_alt = value; changed_alt = true; }
        }

        public float modem_lon
        {
            get { return _modem_lon; }
            set { _modem_lon = value; changed_lonlat = true; }
        }

        public float modem_lat
        {
            get { return _modem_lat; }
            set { _modem_lat = value; changed_lonlat = true; }
        }

        public byte[] mask
        {
            get { return _mask; }
            set { _mask = value; changed_mask = true; }
        }
             
        /// variables para depuracion pids
        bool changed_debugvar;
        byte debugvar_id;
        float debugvar_value;
        public void SetDebug(byte id, float v)
        {
            debugvar_id = id;
            debugvar_value = v;
            changed_debugvar = true;
        }
        /// fin

        public void SendCmdCenterIR()
        {
            trig_cmdCenterIR = true;
        }

        public void SendCmdGainIR()
        {
            trig_cmdGainIR = true;
        }

        public void SendCmdSetHome()
        {
            trig_cmdSetHome = true;
        }

        public void SendCmdSaveFlash()
        {
            trig_cmdSaveFlash = true;
        }

        public JoystickThread(FormIkarusMain main)
        {
            InitEmisoraUSB();

            formIkarusMain = main;

            TrimAil = 0;
            TrimEle = 0;
            TrimTail = 0;

            
            if (wfly.IsOpen())
            {
                terminate = false;
                timer = new System.Threading.Timer(TimerTask, this, 1000, 1000 / 20);
                pan = 0;
                tilt = 0;
                flaps = 0;
            }
            else if (singleton.Idioma == 0)
            {
                MessageBox.Show("No se puede abrir dispositivo Uplink");
            }
            else
            {
                MessageBox.Show("Cannot open Uplink device");
            }
        }
        int flag = 0;
        static void TimerTask(Object obj)
        {
            JoystickThread joy = (JoystickThread)obj;
            if (joy.wfly.IsOpen())
            {

                if (joy.flag == 0)
                {
                    joy.flag = 1;
                    if (!joy.configurado)
                        joy.ConfigurarCanales(joy.wfly);

                    joy.singleton.uplink_voltage = joy.wfly.ReadBatteryLevel();
                    joy.tareaJOY();
                    if (joy.terminate)
                    {
                        joy.timer.Dispose();
                        joy.Stop();
                        joy.wfly.Close();
                    }
                }

                joy.flag = 0;

            }
            else
            {
                joy.configurado = false;
                joy.wfly = new EmisoraUSB();
            }

        }

        public void Close()
        {
            terminate = true;
        }

        void ConfigurarCanales(EmisoraUSB wfly)
        {
            if (!configurado)
            {
                byte[] mask = new byte[11];
                for (int i = 0; i < mask.Length; i++)
                    mask[i] = 0;

                if (singleton.servo_ch[(int)Singleton.Canales.CTRL] < mask.Length)
                {
                    mask[singleton.servo_ch[(int)Singleton.Canales.CTRL]] = 2;

                    int[] slot0 = WflyUplink.SetSwitches(WflyUplink.Ruta.Casa, WflyUplink.Autopilot.Manual, _modem_hud, _modem_cam);
                    int[] slot1 = WflyUplink.SetAltura((int)_modem_alt);
                    int[] slot2 = WflyUplink.SetLon(_modem_lon);
                    int[] slot3 = WflyUplink.SetLat(modem_lat);
                    int[] slot4 = new int[0]; // WflyUplink.SetDebugVar(0xff, 0.0f);

                    int[] seq = new int[] { 0, 1, 0, 2, 0, 3, 0, 4 };

                    wfly.SetSlot(0, slot0);
                    wfly.SetSlot(1, slot1);
                    wfly.SetSlot(2, slot2);
                    wfly.SetSlot(3, slot3);
                    wfly.SetSlot(4, slot4);

                    wfly.SetSequence((int)Singleton.Canales.CTRL, 1, seq);
                }

                this.SetMasks(joy_enabled);
                configurado = true;
            }
        }

        void InitEmisoraUSB()
        {
            /*
            if (singleton.planeState != null)
            {
                _modem_lon = singleton.planeState.homeLon;
                _modem_lat = singleton.planeState.homeLat;
            }
            else
            {
                _modem_lon = -6.0f;
                _modem_lat = 37.5f;
            }
            */

            _modem_lon = singleton.HomeLon;
            _modem_lat = singleton.HomeLat;
            _modem_alt = singleton.HomeAlt+100;

            wfly = new EmisoraUSB();
            if (wfly.IsOpen())
            {
                //fijamos que canal se modifica
                ConfigurarCanales(wfly);      
                
                wfly.SetLowBattery(singleton.uplinkValarm);
                wfly.SaveToFlash();
             }
        }

        public void SetMasks(bool enableJoy)
        {
            this.joy_enabled = enableJoy;
                
            if (enableJoy)
            {
                byte[] mask = new byte[11];
                for (int i = 0; i < mask.Length; i++)
                    mask[i] = 0;

                if (singleton.servo_ch[(int)Singleton.Canales.CTRL] < mask.Length)
                    mask[singleton.servo_ch[(int)Singleton.Canales.CTRL]] = 2;

                if (singleton.enable_axis)
                {
                    if (singleton.servo_ch[(int)Singleton.Canales.AIL] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.AIL]] = 1;
                    if (singleton.servo_ch[(int)Singleton.Canales.ELE] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.ELE]] = 1;
                    if (singleton.servo_ch[(int)Singleton.Canales.THR] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.THR]] = 1;
                    if (singleton.servo_ch[(int)Singleton.Canales.TAIL] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.TAIL]] = 1;

                    if (singleton.servo_ch[(int)Singleton.Canales.FLAPS] < mask.Length && (singleton.tipo_mezcla == Singleton.Mezclas.Flaperon || singleton.tipo_mezcla == Singleton.Mezclas.Flaperon_Vtail))
                        mask[singleton.servo_ch[(int)Singleton.Canales.FLAPS]] = 1;

                }
                if (singleton.enable_headtrack)
                {
                    if (singleton.enable_pan && singleton.servo_ch[(int)Singleton.Canales.PAN] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.PAN]] = 3;
                    if (singleton.enable_tilt && singleton.servo_ch[(int)Singleton.Canales.TILT] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.TILT]] = 4;
                }
                else
                {
                    if (singleton.enable_pan && singleton.servo_ch[(int)Singleton.Canales.PAN] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.PAN]] = 1;
                    if (singleton.enable_tilt && singleton.servo_ch[(int)Singleton.Canales.TILT] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.TILT]] = 1;
                }
                this.mask = mask;
            }
            else
            {
                byte[] mask = new byte[11];
                for (int i = 0; i < mask.Length; i++)
                    mask[i] = 0;

                if (singleton.servo_ch[(int)Singleton.Canales.CTRL] < mask.Length)
                    mask[singleton.servo_ch[(int)Singleton.Canales.CTRL]] = 2;
                
                if (singleton.enable_headtrack)
                {
                    if (singleton.enable_pan && singleton.servo_ch[(int)Singleton.Canales.PAN] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.PAN]] = 3;
                    if (singleton.enable_tilt && singleton.servo_ch[(int)Singleton.Canales.TILT] < mask.Length)
                        mask[singleton.servo_ch[(int)Singleton.Canales.TILT]] = 4;
                }
                this.mask = mask;
            }
        }

        public void Stop()
        {
            byte[] mask = new byte[11];
            for (int i = 0; i < mask.Length; i++)
                mask[i] = 0;

            /*
            if (!singleton.useEmisora)
            {
                mask[singleton.servo_ch[(int)Singleton.Canales.CTRL]] = 1;
                wfly.SetServo(singleton.servo_ch[(int)Singleton.Canales.CTRL], 2000);
            }
            */

            wfly.SetMask(mask);

           
        }

        float AdjustaValor(float valor, float centro)
        {
            if (valor > 0)
                return (1 + centro) * valor - centro;
            else
                return (1 - centro) * valor - centro;
            //return valor - centro;
        }

        void tareaJOY()
        {
            if (changed_mask)
            {
                changed_mask = false;
                wfly.SetMask(_mask);
            }
            Joystick_WMM.JOYINFOEX joyinfo = Joystick_WMM.getJoyPosEx((uint)0);

            // Analiza si se ha pulsado algun boton nuevo
            int bmask = 1;
            for (int i = 0; i < 16; i++)
            {
                if ((joyinfo.dwButtons & bmask) == 0)
                {
                    old_buttons &= ~bmask;
                }
                else if ((old_buttons & bmask) == 0)
                {
                    old_buttons |= bmask;
                    Singleton.EventosJoy evento = (Singleton.EventosJoy)((int)singleton.joy_buttons[i]);
                    ProcesarEventoJoy(evento);
                }
                bmask <<= 1;
            }

            // Analiza el modem uplink
            if (changed_switches)
            {
                changed_switches = false;
                SetSwitches(modem_ruta, modem_autopilot, (byte)modem_hud, modem_cam);
            }
            if (changed_alt)
            {
                changed_alt = false;
                SetAltitude((int)modem_alt);
            }
            if (changed_lonlat)
            {
                changed_lonlat = false;
                SetLonLat(modem_lon, modem_lat);
            }

            // Comandos varios 
            if (changed_debugvar)
            {
                changed_debugvar = false;
                SetDebugVar(debugvar_id, debugvar_value);
            }
            else if (changed_wptid)
            {
                changed_wptid = false;
                SetWptID(_wpt_id);
            }
            else if (trig_cmdCenterIR)
            {
                trig_cmdCenterIR = false;
                SetCenterIR();
            }
            else if (trig_cmdGainIR)
            {
                trig_cmdGainIR = false;
                SetGainIR();
            }
            else if (trig_cmdSetHome)
            {
                trig_cmdSetHome = false;
                SetHome();
            }
            else if (trig_cmdSaveFlash)
            {
                trig_cmdSaveFlash = false;
                SaveFlash();
            }

            
            // Analiza movimiento PAN & TILT
            int v = ((short)joyinfo.dwPOV);
            if (v >= 0)
            {
                float alfa = (float)(v / 100 * Math.PI / 180.0);
                float mi_pan = pan + (float)(Math.Sin(alfa) * singleton.pantilt_speed / 10.0); // dt=10hz
                if (mi_pan > 1)
                    pan = 1;
                else if (mi_pan < -1)
                    pan = -1;
                else
                    pan = mi_pan;
                float mi_tilt = tilt + (float)(Math.Cos(alfa) * singleton.pantilt_speed / 10.0);
                if (mi_tilt > 1)
                    tilt = 1;
                else if (mi_tilt < -1)
                    tilt = -1;
                else
                    tilt = mi_tilt;
            }
           
            // Analiza Ejes del joystick
            float _pitch = AdjustaValor(2 * (joyinfo.dwYpos / 65535.0f) - 1, TrimEle);
            float _roll = AdjustaValor(2 * (joyinfo.dwXpos / 65535.0f) - 1, TrimAil);
            float _yaw = AdjustaValor(2 * (joyinfo.dwRpos / 65535.0f) - 1, TrimTail);
            float _thr = 1 - 2 * (joyinfo.dwZpos / 65535.0f);
            SetControles(_roll, _pitch, _thr, _yaw, pan, tilt, flaps);
            //wfly.SetServos(pan, _pitch, _thr, tilt);

 //           System.Console.WriteLine("PITCH " + _pitch + " ROLL " + _roll + " THR " + _thr + " TAIL " + _yaw);
           // System.Console.WriteLine("Auto " + modem_autopilot + " HUD " + modem_hud +
           //     " RUTA " + modem_rutacasa + " CAM " + modem_cam + " PAN " + pan + " TILT " + tilt);
            
        }
        
        void SetSwitches(WflyUplink.Ruta ruta, WflyUplink.Autopilot autopilot, byte hud, bool cam)
        {
            int[] slot0 = WflyUplink.SetSwitches(ruta, autopilot, hud,cam);
            if (wfly.IsOpen())
                wfly.SetSlot(0, slot0);
        }

        void SetAltitude(int altitud)
        {
            int[] slot1 = WflyUplink.SetAltura(altitud);
            if (wfly.IsOpen())
                wfly.SetSlot(1, slot1);
        }

        void SetLonLat(float Longitude, float Latitude)
        {
            int[] slot2 = WflyUplink.SetLon(Longitude);
            int[] slot3 = WflyUplink.SetLat(Latitude);
            if (wfly.IsOpen())
            {
                wfly.SetSlot(2, slot2);
                wfly.SetSlot(3, slot3);
            }
        }

        void SetDebugVar(byte id, float valor)
        {

            int[] slot4 = WflyUplink.SetDebugVar(id, valor);
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
            
        }

        void SetWptID(byte id)
        {
            int[] slot4 = WflyUplink.SetWptID(id);
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
         
        }

        void SetCenterIR()
        {
            int[] slot4 = WflyUplink.CmdSetCenterIR();
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
        }

        void SetGainIR()
        {
            int[] slot4 = WflyUplink.CmdSetGainIR();
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
        }

        void SetHome()
        {
            int[] slot4 = WflyUplink.CmdSetHome();
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
        }

        void SaveFlash()
        {
            int[] slot4 = WflyUplink.CmdSaveFlash();
            if (wfly.IsOpen())
                wfly.SetSlot(4, slot4);
        }

        void SetControles(float aileron, float elevator, float thrust, float tail, float pan, float tilt, float flaps)
        {
            Singleton.Mezclas tipo_mezcla = singleton.tipo_mezcla;
            if (tipo_mezcla == Singleton.Mezclas.Elevon)
            {
                float mix1, mix2;
                if (singleton.rev_mix)
                {
                    mix1 = (aileron + elevator) / 2.0f;
                    mix2 = (aileron - elevator) / 2.0f;
                }
                else
                {

                    mix1 = (aileron - elevator) / 2.0f;
                    mix2 = (aileron + elevator) / 2.0f;

                }
                wfly.SetServos(mix1, mix2, thrust, tail, pan, tilt, flaps);
            }
            else if (tipo_mezcla == Singleton.Mezclas.V_Tail|| tipo_mezcla == Singleton.Mezclas.Flaperon_Vtail)
            {
                float mix1, mix2;
                if (singleton.rev_mix)
                {
                    mix1 = (elevator + tail) / 2.0f;
                    mix2 = (elevator - tail) / 2.0f;
                }
                else
                {
                    mix1 = (elevator - tail) / 2.0f;
                    mix2 = (elevator + tail) / 2.0f;
                }

                if (tipo_mezcla == Singleton.Mezclas.Flaperon_Vtail)
                {
                    float _flap = singleton.rev_flap ? -flaps : flaps;
                    float mix3 = (aileron + _flap);// / 2.0f;
                    float mix4 = (aileron - _flap);// / 2.0f;
                    wfly.SetServos(aileron, mix3, thrust, mix2, pan, tilt, mix4);
                }
                else
                    wfly.SetServos(aileron, mix1, thrust, mix2, pan, tilt, flaps);
            }
            else	// normal
            {
                if (tipo_mezcla == Singleton.Mezclas.Flaperon)
                {
                    float _flap = singleton.rev_flap ? -flaps : flaps;
                    float mix1 = (aileron + _flap);// / 2.0f;
                    float mix2 = (aileron - _flap);// / 2.0f;
                    wfly.SetServos(mix1, elevator, thrust, tail, pan, tilt, mix2);
                }
                else
                    wfly.SetServos(aileron, elevator, thrust, tail, pan, tilt, flaps);
            }
        }

        public void ProcesarEventoJoy(Singleton.EventosJoy evento)
        {
            Joystick_WMM.JOYINFOEX joyinfo;

            switch (evento)
            {
                case Singleton.EventosJoy.None: //
                    break;
                case Singleton.EventosJoy.AutoPltON: //
                    modem_autopilot = WflyUplink.Autopilot.Autopilot;
                    break;
                case Singleton.EventosJoy.AutoPilotOFF: //
                    modem_autopilot = WflyUplink.Autopilot.Manual;
                    break;
                case Singleton.EventosJoy.AutoPltCo:
                    modem_autopilot = WflyUplink.Autopilot.Copilot;
                    break;
                case Singleton.EventosJoy.AutoPilotSWnext: //
                    modem_autopilot = (WflyUplink.Autopilot)(((int)modem_autopilot + 1) % 3);
                    break;
                case Singleton.EventosJoy.AutoPilotSWprev:
                    if ((int)modem_autopilot == 0)
                        modem_autopilot = (WflyUplink.Autopilot) 2;
                    else
                        modem_autopilot = (WflyUplink.Autopilot)((int)modem_autopilot - 1);
                    break;
                case Singleton.EventosJoy.CamA: //
                    modem_cam = false;
                    break;
                case Singleton.EventosJoy.CamB: //
                    modem_cam = true;
                    break;
                case Singleton.EventosJoy.CamSW: //
                    modem_cam = !modem_cam;
                    break;
                case Singleton.EventosJoy.HudCL: //
                    modem_hud = 0;
                    break;
                case Singleton.EventosJoy.Hud1: //
                    modem_hud = 1;
                    break;
                case Singleton.EventosJoy.Hud2: //
                    modem_hud = 2;
                    break;
                case Singleton.EventosJoy.Hud3: //
                    modem_hud = 3;
                    break;
                case Singleton.EventosJoy.HudSWnext: //
                    modem_hud = (modem_hud + 1) % 4;
                    break;
                case Singleton.EventosJoy.HudSWprev:
                    if (modem_hud == 0)
                        modem_hud = 3;
                    else
                        modem_hud = modem_hud - 1;
                    break;
                case Singleton.EventosJoy.GoHome: //
                    modem_ruta = WflyUplink.Ruta.Casa;
                    break;
                case Singleton.EventosJoy.GoWptRuta: //
                    modem_ruta = WflyUplink.Ruta.Ruta;
                    break;
                case Singleton.EventosJoy.GoUplink: //
                    modem_ruta = WflyUplink.Ruta.Modem;
                    break;
                case Singleton.EventosJoy.GoHold:
                    modem_ruta = WflyUplink.Ruta.Hold;
                    break;
                case Singleton.EventosJoy.GoSWnext: //
                    modem_ruta = (WflyUplink.Ruta)(((int)modem_ruta + 1) % 4);
                    break;
                case Singleton.EventosJoy.GoSWprev: //
                    if ((int)modem_ruta == 0)
                        modem_ruta = (WflyUplink.Ruta)3;
                    else
                        modem_ruta = (WflyUplink.Ruta)((int)modem_ruta - 1);
                    break;

                case Singleton.EventosJoy.PanTiltCenter:
                    pan = 0;
                    tilt = 0;
                    break;

                case Singleton.EventosJoy.FlapsOff:
                    flaps = 0;
                    break;
                
                case Singleton.EventosJoy.FlapsFull:
                    flaps = singleton.full_flap;
                    break;

                case Singleton.EventosJoy.FlapsInc:
                    flaps += singleton.step_flap * singleton.full_flap;
                    if (flaps > singleton.full_flap)
                        flaps = singleton.full_flap;
                    break;

                case Singleton.EventosJoy.FlapsDec:
                    flaps -= singleton.step_flap * singleton.full_flap;
                    if (flaps < 0)
                        flaps = 0;
                    break;

                case Singleton.EventosJoy.TrimAil:
                    joyinfo = Joystick_WMM.getJoyPosEx((uint)0);
                    TrimAil = -AdjustaValor((2 * (joyinfo.dwXpos / 65535.0f) - 1), TrimAil);
                    break;
                
                case Singleton.EventosJoy.TrimEle:
                    joyinfo = Joystick_WMM.getJoyPosEx((uint)0);
                    TrimEle = -AdjustaValor((2 * (joyinfo.dwYpos / 65535.0f) - 1), TrimEle);
                    break;
                
                case Singleton.EventosJoy.TrimTail:
                    joyinfo = Joystick_WMM.getJoyPosEx((uint)0);
                    TrimTail = -AdjustaValor((2 * (joyinfo.dwRpos / 65535.0f) - 1), TrimTail);
                    break;
               
                case Singleton.EventosJoy.TrimAll:
                    joyinfo = Joystick_WMM.getJoyPosEx((uint)0);
                    TrimEle = -AdjustaValor((2 * (joyinfo.dwYpos / 65535.0f) - 1), TrimEle);
                    TrimAil = -AdjustaValor((2 * (joyinfo.dwXpos / 65535.0f) - 1), TrimAil);
                    TrimTail = -AdjustaValor((2 * (joyinfo.dwRpos / 65535.0f) - 1), TrimTail);
                    break;
                case Singleton.EventosJoy.Picture:
                    formIkarusMain.Capturar_Foto();
                    break;

                default:
                    break;
            }
        }
    }
}
