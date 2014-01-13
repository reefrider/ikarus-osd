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
//using System.Reflection;
using System.IO;
using UAVConsole.ConfigClasses;
using UAVConsole.USBXpress;
//using System.Runtime.InteropServices;

namespace UAVConsole
{
    public partial class FormJoystick : Form
    {
        Singleton me = Singleton.GetInstance();

        public FormJoystick()
        {
            InitializeComponent();
            
            comboBoxCtrlCH.SelectedIndex = 5;
            comboAilCH.SelectedIndex = 1;
            comboEleCH.SelectedIndex = 2;
            comboThrCH.SelectedIndex = 3;
            comboTailCH.SelectedIndex = 4;
            comboPanCH.SelectedIndex = 7;
            comboTiltCH.SelectedIndex = 8;
            comboAil2CH.SelectedIndex = 6;

            comboBoxHeadTrackPanCH.SelectedIndex = 0;
            comboBoxHeadTrackTiltCH.SelectedIndex = 0;

            comboBoxTipoMezcla.SelectedIndex = 0;
           
            comboBoxJoy1.Items.AddRange(nombre_eventos());
            comboBoxJoy2.Items.AddRange(nombre_eventos());
            comboBoxJoy3.Items.AddRange(nombre_eventos());
            comboBoxJoy4.Items.AddRange(nombre_eventos());
            comboBoxJoy5.Items.AddRange(nombre_eventos());
            comboBoxJoy6.Items.AddRange(nombre_eventos());
            comboBoxJoy7.Items.AddRange(nombre_eventos());
            comboBoxJoy8.Items.AddRange(nombre_eventos());
            comboBoxJoy9.Items.AddRange(nombre_eventos());
            comboBoxJoy10.Items.AddRange(nombre_eventos());
            comboBoxJoy11.Items.AddRange(nombre_eventos());
            comboBoxJoy12.Items.AddRange(nombre_eventos());
            comboBoxJoy13.Items.AddRange(nombre_eventos());
            comboBoxJoy14.Items.AddRange(nombre_eventos());
            comboBoxJoy15.Items.AddRange(nombre_eventos());
            comboBoxJoy16.Items.AddRange(nombre_eventos());

            checkBoxUseTX.Checked = true;

            FromSingleton();

            timer1.Enabled = true;
        }

        String [] nombre_eventos()
        {

            string[] eventos = new string[]
                {"",
                "AutoPilot ON",
                "AutoPilot Copilot",
                "AutoPilot OFF",
                "AutoPilot Next",
                "AutoPilot Prev",
                "Cam - 1",
                "Cam - 2",
                "Cam - ALT",
                "HUD - Clear",
                "HUD - 1",
                "HUD - 2",
                "HUD - 3",
                "HUD - Next",
                "HUD - Prev",
                "Ir a - Ruta",
                "Ir a - Casa",
                "Ir a - Hold",
                "Ir a - Uplink",
                "Ir a - Next",
                "Ir a - Prev",
                "Pan&Tilt Center",
                "Flaps OFF",
                "Flaps FULL",
                "Flaps Inc",
                "Flaps Dec",
                "Trim Ail",
                "Trim Ele",
                "Trim Tail",
                "Trim All",
                "Picture"};
            return eventos;
        }
        void FromSingleton()
        {
            // Canales
            try
            {
                comboBoxCtrlCH.SelectedIndex = (byte)(me.servo_ch[0] +1);
                comboAilCH.SelectedIndex = (byte)(me.servo_ch[1]+1);
                comboEleCH.SelectedIndex = (byte)(me.servo_ch[2]+1);
                comboThrCH.SelectedIndex = (byte)(me.servo_ch[3]+1);
                comboTailCH.SelectedIndex = (byte)(me.servo_ch[4]+1);
                comboPanCH.SelectedIndex = (byte)(me.servo_ch[5] +1);
                comboTiltCH.SelectedIndex = (byte)(me.servo_ch[6] +1);
                comboAil2CH.SelectedIndex = (byte)(me.servo_ch[7] + 1);


                comboBoxTipoMezcla.SelectedIndex = (int)me.tipo_mezcla;
//                comboBoxTipoControl.SelectedIndex = (int)me.tipo_control;

                // Botones joystick
                comboBoxJoy1.SelectedIndex = me.joy_buttons[0];
                comboBoxJoy2.SelectedIndex = me.joy_buttons[1];
                comboBoxJoy3.SelectedIndex = me.joy_buttons[2];
                comboBoxJoy4.SelectedIndex = me.joy_buttons[3];
                comboBoxJoy5.SelectedIndex = me.joy_buttons[4];
                comboBoxJoy6.SelectedIndex = me.joy_buttons[5];
                comboBoxJoy7.SelectedIndex = me.joy_buttons[6];
                comboBoxJoy8.SelectedIndex = me.joy_buttons[7];
                comboBoxJoy9.SelectedIndex = me.joy_buttons[8];
                comboBoxJoy10.SelectedIndex = me.joy_buttons[9];
                comboBoxJoy11.SelectedIndex = me.joy_buttons[10];
                comboBoxJoy12.SelectedIndex = me.joy_buttons[11];
                comboBoxJoy13.SelectedIndex = me.joy_buttons[12];
                comboBoxJoy14.SelectedIndex = me.joy_buttons[13];
                comboBoxJoy15.SelectedIndex = me.joy_buttons[14];
                comboBoxJoy16.SelectedIndex = me.joy_buttons[15];

                // Numeric updown
                numericUpDownServoCTRLmin.Value = me.servo_min[0];
                numericUpDownServoCTRLcenter.Value = me.servo_center[0];
                numericUpDownServoCTRLmax.Value = me.servo_max[0];
                checkBoxServoCTRLrev.Checked = me.servo_rev[0];

                numericUpDownServoAILmin.Value = me.servo_min[1];
                numericUpDownServoAILcenter.Value = me.servo_center[1];
                numericUpDownServoAILmax.Value = me.servo_max[1];
                checkBoxServoAILrev.Checked = me.servo_rev[1];

                numericUpDownServoELEmin.Value = me.servo_min[2];
                numericUpDownServoELEcenter.Value = me.servo_center[2];
                numericUpDownServoELEmax.Value = me.servo_max[2];
                checkBoxServoELErev.Checked = me.servo_rev[2];

                numericUpDownServoTHRmin.Value = me.servo_min[3];
                numericUpDownServoTHRcenter.Value = me.servo_center[3];
                numericUpDownServoTHRmax.Value = me.servo_max[3];
                checkBoxServoTHRrev.Checked = me.servo_rev[3];

                numericUpDownServoTAILmin.Value = me.servo_min[4];
                numericUpDownServoTAILcenter.Value = me.servo_center[4];
                numericUpDownServoTAILmax.Value = me.servo_max[4];
                checkBoxServoTAILrev.Checked = me.servo_rev[4];

                numericUpDownServoPANmin.Value = me.servo_min[5];
                numericUpDownServoPANcenter.Value = me.servo_center[5];
                numericUpDownServoPANmax.Value = me.servo_max[5];
                checkBoxServoPANrev.Checked = me.servo_rev[5];

                numericUpDownServoTILTmin.Value = me.servo_min[6];
                numericUpDownServoTILTcenter.Value = me.servo_center[6];
                numericUpDownServoTILTmax.Value = me.servo_max[6];
                checkBoxServoTILTrev.Checked = me.servo_rev[6];

                numericUpDownServoAIL2min.Value = me.servo_min[7];
                numericUpDownServoAIL2center.Value = me.servo_center[7];
                numericUpDownServoAIL2max.Value = me.servo_max[7];
                checkBoxServoAIL2rev.Checked = me.servo_rev[7];

                checkBoxMix1.Checked = me.rev_mix;
                checkBoxRevFlaps.Checked = me.rev_flap;
                numericUpDownStepFlap.Value = (decimal)me.step_flap;
                numericUpDownFullFlap.Value = (decimal)me.full_flap;
                
                checkBoxEnableAxis.Checked = me.enable_axis;

                checkBoxUseTX.Checked = me.useEmisora;
                numericUpDownNumCanales.Value = me.txNumCanales;
                numericUpDownPeriodo.Value = me.txPeriodo;
                numericUpDownSeparador.Value = me.txSeparador;
                checkBoxPolarity.Checked = me.txPolarity;

                checkBoxEnablePAN.Checked = me.enable_pan;
                checkBoxEnableTILT.Checked = me.enable_tilt;
                numericUpDownPanTiltSpeed.Value = (decimal)me.pantilt_speed;
                checkBoxEnableHeadTracker.Checked = me.enable_headtrack;
                comboBoxHeadTrackPanCH.SelectedIndex = (byte)(me.headtrack_panCh+1);
                comboBoxHeadTrackTiltCH.SelectedIndex = (byte)(me.headtrack_tiltCh+1);

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message + " - " + e.Source+ " - "+e.ToString());
            }
        }

        void ToSingleton()
        {
            // Canales
            me.servo_ch[0] = (byte)(comboBoxCtrlCH.SelectedIndex-1);
            me.servo_ch[1] = (byte)(comboAilCH.SelectedIndex-1);
            me.servo_ch[2] = (byte)(comboEleCH.SelectedIndex-1);
            me.servo_ch[3] = (byte)(comboThrCH.SelectedIndex-1);
            me.servo_ch[4] = (byte)(comboTailCH.SelectedIndex-1);
            me.servo_ch[5] = (byte)(comboPanCH.SelectedIndex -1);
            me.servo_ch[6] = (byte)(comboTiltCH.SelectedIndex -1);
            me.servo_ch[7] = (byte)(comboAil2CH.SelectedIndex - 1);

            me.tipo_mezcla = (Singleton.Mezclas)comboBoxTipoMezcla.SelectedIndex;
        //    me.tipo_control = (Singleton.ModoControl)comboBoxTipoControl.SelectedIndex;

            // Botones joystick
            me.joy_buttons[0] = (byte)(comboBoxJoy1.SelectedIndex);
            me.joy_buttons[1] = (byte)(comboBoxJoy2.SelectedIndex);
            me.joy_buttons[2] = (byte)(comboBoxJoy3.SelectedIndex);
            me.joy_buttons[3] = (byte)(comboBoxJoy4.SelectedIndex);
            me.joy_buttons[4] = (byte)(comboBoxJoy5.SelectedIndex);
            me.joy_buttons[5] = (byte)(comboBoxJoy6.SelectedIndex);
            me.joy_buttons[6] = (byte)(comboBoxJoy7.SelectedIndex);
            me.joy_buttons[7] = (byte)(comboBoxJoy8.SelectedIndex);
            me.joy_buttons[8] = (byte)(comboBoxJoy9.SelectedIndex);
            me.joy_buttons[9] = (byte)(comboBoxJoy10.SelectedIndex);
            me.joy_buttons[10] = (byte)(comboBoxJoy11.SelectedIndex);
            me.joy_buttons[11] = (byte)(comboBoxJoy12.SelectedIndex);
            me.joy_buttons[12] = (byte)(comboBoxJoy13.SelectedIndex);
            me.joy_buttons[13] = (byte)(comboBoxJoy14.SelectedIndex);
            me.joy_buttons[14] = (byte)(comboBoxJoy15.SelectedIndex);
            me.joy_buttons[15] = (byte)(comboBoxJoy16.SelectedIndex);

            for (int i = 0; i < 16; i++)
                if (me.joy_buttons[i] == 0xff)
                    me.joy_buttons[i] = 0;
            // Numeric updown
            me.servo_min[0] = (int)numericUpDownServoCTRLmin.Value;
            me.servo_center[0] = (int)numericUpDownServoCTRLcenter.Value;
            me.servo_max[0] = (int)numericUpDownServoCTRLmax.Value;
            me.servo_rev[0] = checkBoxServoCTRLrev.Checked;

            me.servo_min[1] = (int)numericUpDownServoAILmin.Value;
            me.servo_center[1] = (int)numericUpDownServoAILcenter.Value;
            me.servo_max[1] = (int)numericUpDownServoAILmax.Value;
            me.servo_rev[1] = checkBoxServoAILrev.Checked;

            me.servo_min[2] = (int)numericUpDownServoELEmin.Value;
            me.servo_center[2] = (int)numericUpDownServoELEcenter.Value;
            me.servo_max[2] = (int)numericUpDownServoELEmax.Value;
            me.servo_rev[2] = checkBoxServoELErev.Checked;

            me.servo_min[3] = (int)numericUpDownServoTHRmin.Value;
            me.servo_center[3] = (int)numericUpDownServoTHRcenter.Value;
            me.servo_max[3] = (int)numericUpDownServoTHRmax.Value;
            me.servo_rev[3] = checkBoxServoTHRrev.Checked;

            me.servo_min[4] = (int)numericUpDownServoTAILmin.Value;
            me.servo_center[4] = (int)numericUpDownServoTAILcenter.Value;
            me.servo_max[4] = (int)numericUpDownServoTAILmax.Value;
            me.servo_rev[4] = checkBoxServoTAILrev.Checked;

            me.servo_min[5] = (int)numericUpDownServoPANmin.Value;
            me.servo_center[5] = (int)numericUpDownServoPANcenter.Value;
            me.servo_max[5] = (int)numericUpDownServoPANmax.Value;
            me.servo_rev[5] = checkBoxServoPANrev.Checked;

            me.servo_min[6] = (int)numericUpDownServoTILTmin.Value;
            me.servo_center[6] = (int)numericUpDownServoTILTcenter.Value;
            me.servo_max[6] = (int)numericUpDownServoTILTmax.Value;
            me.servo_rev[6] = checkBoxServoTILTrev.Checked;

            me.servo_min[7] = (int)numericUpDownServoAIL2min.Value;
            me.servo_center[7] = (int)numericUpDownServoAIL2center.Value;
            me.servo_max[7] = (int)numericUpDownServoAIL2max.Value;
            me.servo_rev[7] = checkBoxServoAIL2rev.Checked;

            me.rev_mix = checkBoxMix1.Checked;
            me.rev_flap = checkBoxRevFlaps.Checked;
            me.full_flap = (float)numericUpDownFullFlap.Value;
            me.step_flap = (float)numericUpDownStepFlap.Value;

            me.pantilt_speed = (float)numericUpDownPanTiltSpeed.Value;
            me.enable_axis = checkBoxEnableAxis.Checked;
            me.enable_pan = checkBoxEnablePAN.Checked;
            me.enable_tilt = checkBoxEnableTILT.Checked;

            me.enable_headtrack = checkBoxEnableHeadTracker.Checked;
            me.headtrack_panCh = (byte)(comboBoxHeadTrackPanCH.SelectedIndex-1);
            me.headtrack_tiltCh = (byte)(comboBoxHeadTrackTiltCH.SelectedIndex-1);

            me.useEmisora = checkBoxUseTX.Checked;
            me.txNumCanales = (byte)numericUpDownNumCanales.Value;
            me.txPeriodo = (int)numericUpDownPeriodo.Value;
            me.txSeparador = (int)numericUpDownSeparador.Value;
            me.txPolarity = checkBoxPolarity.Checked;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Label[] labels ={labelBtn1, labelBtn2, labelBtn3, labelBtn4, labelBtn5,labelBtn6, labelBtn7,
                                labelBtn8,labelBtn9, labelBtn10, labelBtn11, labelBtn12, labelBtn13,labelBtn14,
                                labelBtn15,labelBtn16};

            Joystick_WMM.JOYINFOEX info = Joystick_WMM.getJoyPosEx((uint)0);

            uint mask = 1;
            for (int i = 0; i < 16; i++)
            {
                if ((info.dwButtons & mask) != 0)
                    labels[i].ForeColor = Color.Red;
                else
                    labels[i].ForeColor = DefaultForeColor;

                mask <<= 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToSingleton();
            me.ToRegistry();
            
            EmisoraUSB wfly = new EmisoraUSB();
            if (wfly.IsOpen())
            {
                wfly.SaveToFlash();
                wfly.Close();
            }

            if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                MessageBox.Show("Configuración Actualizada");
            else
                MessageBox.Show("Config Updated");
            //this.Close();
        }

        private void comboBoxTipoMezcla_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTipoMezcla.SelectedIndex == 0)    // Normal
            {
                checkBoxMix1.Enabled = false;
                checkBoxRevFlaps.Enabled = false;
            }
            else if (comboBoxTipoMezcla.SelectedIndex == 1)    // Elevon
            {
                checkBoxMix1.Text = "Elevon Rev";
                checkBoxMix1.Enabled = true;
                checkBoxRevFlaps.Enabled = false;
            }
            else if (comboBoxTipoMezcla.SelectedIndex == 2)    // V-Tail
            {
                checkBoxMix1.Text = "V-Tail Rev";
                checkBoxMix1.Enabled = true;
                checkBoxRevFlaps.Enabled = false;
            }
            else if (comboBoxTipoMezcla.SelectedIndex == 3)    // Flaperons
            {
                checkBoxMix1.Enabled = false;
                checkBoxRevFlaps.Enabled = true;
            }
            else if (comboBoxTipoMezcla.SelectedIndex == 4)    //Flaperons & V-Tail
            {
                checkBoxMix1.Text = "V-Tail Rev";
                checkBoxMix1.Enabled = true;
                checkBoxRevFlaps.Enabled = true;
            }


        }

        private void comboBoxTipoControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (comboBoxTipoControl.SelectedIndex == 5)
            {
                numericUpDownServoCTRLcenter.Enabled = false;
                numericUpDownServoCTRLmin.Enabled = false;
                numericUpDownServoCTRLmax.Enabled = false;
                checkBoxServoCTRLrev.Enabled = false;
            }
            else
            {

                numericUpDownServoCTRLcenter.Enabled = true;
                numericUpDownServoCTRLmin.Enabled = true;
                numericUpDownServoCTRLmax.Enabled = true;
                checkBoxServoCTRLrev.Enabled = true;
            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".ikarus_joy";
            dlg.AddExtension = true;
            dlg.Filter = "Conf. Joystick (*.ikarus_joy)|*.ikarus_joy";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                
                    StreamReader fin = File.OpenText(dlg.FileName);
                    try
                    {
                        comboBoxCtrlCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboAilCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboEleCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboThrCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboTailCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboPanCH.SelectedIndex = int.Parse(fin.ReadLine());
                        comboTiltCH.SelectedIndex = int.Parse(fin.ReadLine());

                        comboBoxTipoMezcla.SelectedIndex = int.Parse(fin.ReadLine());
                        //     comboBoxTipoControl.SelectedIndex = int.Parse(fin.ReadLine());

                        // Botones joystick
                        comboBoxJoy1.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy2.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy3.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy4.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy5.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy6.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy7.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy8.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy9.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy10.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy11.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy12.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy13.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy14.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy15.SelectedIndex = int.Parse(fin.ReadLine());
                        comboBoxJoy16.SelectedIndex = int.Parse(fin.ReadLine());

                        // Numeric updown
                        numericUpDownServoCTRLmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoCTRLcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoCTRLmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoCTRLrev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoAILmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoAILcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoAILmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoAILrev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoELEmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoELEcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoELEmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoELErev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoTHRmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTHRcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTHRmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoTHRrev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoTAILmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTAILcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTAILmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoTAILrev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoPANmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoPANcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoPANmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoPANrev.Checked = bool.Parse(fin.ReadLine());

                        numericUpDownServoTILTmin.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTILTcenter.Value = int.Parse(fin.ReadLine());
                        numericUpDownServoTILTmax.Value = int.Parse(fin.ReadLine());
                        checkBoxServoTILTrev.Checked = bool.Parse(fin.ReadLine());

                        checkBoxMix1.Checked = bool.Parse(fin.ReadLine());
                        checkBoxRevFlaps.Checked = bool.Parse(fin.ReadLine());
                        numericUpDownFullFlap.Value = (decimal)float.Parse(fin.ReadLine());
                        numericUpDownStepFlap.Value = (decimal)float.Parse(fin.ReadLine());

                        numericUpDownPanTiltSpeed.Value = (decimal)float.Parse(fin.ReadLine());
                        checkBoxEnableAxis.Checked = bool.Parse(fin.ReadLine());
                        checkBoxEnablePAN.Checked = bool.Parse(fin.ReadLine());
                        checkBoxEnableTILT.Checked = bool.Parse(fin.ReadLine());
                        checkBoxEnableHeadTracker.Checked = bool.Parse(fin.ReadLine());
                    }
                    catch (Exception) { }
                
                fin.Close();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".ikarus_joy";
            dlg.AddExtension = true;
            dlg.Filter = "Conf. Joystick (*.ikarus_joy)|*.ikarus_joy";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                StreamWriter fout = File.CreateText(dlg.FileName);
                // Canales
                fout.WriteLine(comboBoxCtrlCH.SelectedIndex);
                fout.WriteLine(comboAilCH.SelectedIndex);
                fout.WriteLine(comboEleCH.SelectedIndex);
                fout.WriteLine(comboThrCH.SelectedIndex);
                fout.WriteLine(comboTailCH.SelectedIndex);
                fout.WriteLine(comboPanCH.SelectedIndex);
                fout.WriteLine(comboTiltCH.SelectedIndex);

                fout.WriteLine(comboBoxTipoMezcla.SelectedIndex);
       //         fout.WriteLine(comboBoxTipoControl.SelectedIndex);

                // Botones joystick
                fout.WriteLine(comboBoxJoy1.SelectedIndex);
                fout.WriteLine(comboBoxJoy2.SelectedIndex);
                fout.WriteLine(comboBoxJoy3.SelectedIndex);
                fout.WriteLine(comboBoxJoy4.SelectedIndex);
                fout.WriteLine(comboBoxJoy5.SelectedIndex);
                fout.WriteLine(comboBoxJoy6.SelectedIndex);
                fout.WriteLine(comboBoxJoy7.SelectedIndex);
                fout.WriteLine(comboBoxJoy8.SelectedIndex);
                fout.WriteLine(comboBoxJoy9.SelectedIndex);
                fout.WriteLine(comboBoxJoy10.SelectedIndex);
                fout.WriteLine(comboBoxJoy11.SelectedIndex);
                fout.WriteLine(comboBoxJoy12.SelectedIndex);
                fout.WriteLine(comboBoxJoy13.SelectedIndex);
                fout.WriteLine(comboBoxJoy14.SelectedIndex);
                fout.WriteLine(comboBoxJoy15.SelectedIndex);
                fout.WriteLine(comboBoxJoy16.SelectedIndex);
                
                // Numeric updown
                fout.WriteLine(numericUpDownServoCTRLmin.Value);
                fout.WriteLine(numericUpDownServoCTRLcenter.Value);
                fout.WriteLine(numericUpDownServoCTRLmax.Value);
                fout.WriteLine(checkBoxServoCTRLrev.Checked);

                fout.WriteLine(numericUpDownServoAILmin.Value);
                fout.WriteLine(numericUpDownServoAILcenter.Value);
                fout.WriteLine(numericUpDownServoAILmax.Value);
                fout.WriteLine(checkBoxServoAILrev.Checked);

                fout.WriteLine(numericUpDownServoELEmin.Value);
                fout.WriteLine(numericUpDownServoELEcenter.Value);
                fout.WriteLine(numericUpDownServoELEmax.Value);
                fout.WriteLine(checkBoxServoELErev.Checked);

                fout.WriteLine(numericUpDownServoTHRmin.Value);
                fout.WriteLine(numericUpDownServoTHRcenter.Value);
                fout.WriteLine(numericUpDownServoTHRmax.Value);
                fout.WriteLine(checkBoxServoTHRrev.Checked);

                fout.WriteLine(numericUpDownServoTAILmin.Value);
                fout.WriteLine(numericUpDownServoTAILcenter.Value);
                fout.WriteLine(numericUpDownServoTAILmax.Value);
                fout.WriteLine(checkBoxServoTAILrev.Checked);

                fout.WriteLine(numericUpDownServoPANmin.Value);
                fout.WriteLine(numericUpDownServoPANcenter.Value);
                fout.WriteLine(numericUpDownServoPANmax.Value);
                fout.WriteLine(checkBoxServoPANrev.Checked);

                fout.WriteLine(numericUpDownServoTILTmin.Value);
                fout.WriteLine(numericUpDownServoTILTcenter.Value);
                fout.WriteLine(numericUpDownServoTILTmax.Value);
                fout.WriteLine(checkBoxServoTILTrev.Checked);

                fout.WriteLine(checkBoxMix1.Checked);
                fout.WriteLine(checkBoxRevFlaps.Checked);
                fout.WriteLine(numericUpDownFullFlap.Value);
                fout.WriteLine(numericUpDownStepFlap.Value);

                fout.WriteLine((float)numericUpDownPanTiltSpeed.Value);
                fout.WriteLine(checkBoxEnableAxis.Checked);
                fout.WriteLine(checkBoxEnablePAN.Checked);
                fout.WriteLine(checkBoxEnableTILT.Checked);
                
                fout.Close();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            FlightPlanUSB planUSB = new FlightPlanUSB();
            IkarusAutopilotConfig config_autopilot = planUSB.ReadConfigAutopilot();
            IkarusBasicConfig config = planUSB.ReadConfig();
            planUSB.Close();
            try
            {

                comboBoxCtrlCH.SelectedIndex = (byte)(config.PPM_Channel +1);

                comboAilCH.SelectedIndex = (byte)(config_autopilot.ail_ch+1);
                comboEleCH.SelectedIndex = (byte)(config_autopilot.ele_ch+1);
                comboThrCH.SelectedIndex = (byte)(config_autopilot.thr_ch+1);
                comboTailCH.SelectedIndex = (byte)(config_autopilot.tail_ch+1);
                comboPanCH.SelectedIndex = (byte)(config_autopilot.pan_ch +1);
          
                // SERVO CTRL
                numericUpDownServoCTRLmin.Value = (decimal)config_autopilot.servo_ctrl.min;
                numericUpDownServoCTRLcenter.Value = (decimal)config_autopilot.servo_ctrl.center;
                numericUpDownServoCTRLmax.Value = (decimal)config_autopilot.servo_ctrl.max;
                checkBoxServoCTRLrev.Checked = config_autopilot.servo_ctrl.reverse != 0;

                // SERVO AIL
                numericUpDownServoAILmin.Value = (decimal)config_autopilot.servo_ail.min;
                numericUpDownServoAILcenter.Value = (decimal)config_autopilot.servo_ail.center;
                numericUpDownServoAILmax.Value = (decimal)config_autopilot.servo_ail.max;
                checkBoxServoAILrev.Checked = config_autopilot.servo_ail.reverse != 0;

                // SERVO ELE
                numericUpDownServoELEmin.Value = (decimal)config_autopilot.servo_ele.min;
                numericUpDownServoELEcenter.Value = (decimal)config_autopilot.servo_ele.center;
                numericUpDownServoELEmax.Value = (decimal)config_autopilot.servo_ele.max;
                checkBoxServoELErev.Checked = config_autopilot.servo_ele.reverse != 0;

                // SERVO THR
                numericUpDownServoTHRmin.Value = (decimal)config_autopilot.servo_thr.min;
                numericUpDownServoTHRcenter.Value = (decimal)config_autopilot.servo_thr.center;
                numericUpDownServoTHRmax.Value = (decimal)config_autopilot.servo_thr.max;
                checkBoxServoTHRrev.Checked = config_autopilot.servo_thr.reverse != 0;

                // SERVO TAIL
                numericUpDownServoTAILmin.Value = (decimal)config_autopilot.servo_tail.min;
                numericUpDownServoTAILcenter.Value = (decimal)config_autopilot.servo_tail.center;
                numericUpDownServoTAILmax.Value = (decimal)config_autopilot.servo_tail.max;
                checkBoxServoTAILrev.Checked = config_autopilot.servo_tail.reverse != 0;

                // SERVO PAN
                numericUpDownServoPANmin.Value = (decimal)config_autopilot.servo_pan.min;
                numericUpDownServoPANcenter.Value = (decimal)config_autopilot.servo_pan.center;
                numericUpDownServoPANmax.Value = (decimal)config_autopilot.servo_pan.max;
                checkBoxServoPANrev.Checked = config_autopilot.servo_pan.reverse != 0;


                // Mezclas...

                checkBoxMix1.Checked = (config_autopilot.rev_mezcla != 0);
                        
                
                 
                switch (config_autopilot.tipo_mezcla)
                {
                    case (byte) Singleton.Mezclas.Normal:
                        if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.AIL2)
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Flaperon;
                            
                            comboAil2CH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                            numericUpDownServoAIL2min.Value = (decimal)config_autopilot.servo_aux.min;
                            numericUpDownServoAIL2center.Value = (decimal)config_autopilot.servo_aux.center;
                            numericUpDownServoAIL2max.Value = (decimal)config_autopilot.servo_aux.max;
                            checkBoxServoAIL2rev.Checked = config_autopilot.servo_aux.reverse != 0;
                        }
                        else if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.TILT)
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Normal;
                            
                            comboTiltCH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                            numericUpDownServoTILTmin.Value = (decimal)config_autopilot.servo_aux.min;
                            numericUpDownServoTILTcenter.Value = (decimal)config_autopilot.servo_aux.center;
                            numericUpDownServoTILTmax.Value = (decimal)config_autopilot.servo_aux.max;
                            checkBoxServoTILTrev.Checked = config_autopilot.servo_aux.reverse != 0;
                         }
                        else 
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Normal;
                        }
                        break;
                    
                    case (byte)Singleton.Mezclas.Elevon:

                        if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.AIL2)
                        {
                            MessageBox.Show("Elevon & Flaperon???");
                            //comboAil2CH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                        }
                        else if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.TILT)
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Elevon;
                            
                            comboTiltCH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                            numericUpDownServoTILTmin.Value = (decimal)config_autopilot.servo_aux.min;
                            numericUpDownServoTILTcenter.Value = (decimal)config_autopilot.servo_aux.center;
                            numericUpDownServoTILTmax.Value = (decimal)config_autopilot.servo_aux.max;
                            checkBoxServoTILTrev.Checked = config_autopilot.servo_aux.reverse != 0;
                        
                        }
                        else 
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Elevon;
                        }
                        break;
                    
                    case (byte)Singleton.Mezclas.V_Tail:

                        if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.AIL2)
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.Flaperon_Vtail;
                        
                            comboAil2CH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                            numericUpDownServoAIL2min.Value = (decimal)config_autopilot.servo_aux.min;
                            numericUpDownServoAIL2center.Value = (decimal)config_autopilot.servo_aux.center;
                            numericUpDownServoAIL2max.Value = (decimal)config_autopilot.servo_aux.max;
                            checkBoxServoAIL2rev.Checked = config_autopilot.servo_aux.reverse != 0;

                        }
                        else if (config_autopilot.CanalAuxMode == (int)Singleton.ModoCanalAux.TILT)
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.V_Tail;
                            
                            comboTiltCH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
                            numericUpDownServoTILTmin.Value = (decimal)config_autopilot.servo_aux.min;
                            numericUpDownServoTILTcenter.Value = (decimal)config_autopilot.servo_aux.center;
                            numericUpDownServoTILTmax.Value = (decimal)config_autopilot.servo_aux.max;
                            checkBoxServoTILTrev.Checked = config_autopilot.servo_aux.reverse != 0;
                        }
                        else
                        {
                            comboBoxTipoMezcla.SelectedIndex = (int)Singleton.Mezclas.V_Tail;
                        }
                       
                        break;
                }
            }
            catch (Exception) { }

            // Valores de los PID a memorizar....
            try
            {
                me.uplink_pid_ail_P = config_autopilot.pidRoll.P;
                me.uplink_pid_ail_I = config_autopilot.pidRoll.I;
                me.uplink_pid_ail_D = config_autopilot.pidRoll.D;
                me.uplink_pid_ail_IL = config_autopilot.pidRoll.ILim;
                me.uplink_pid_ail_DL = config_autopilot.pidRoll.DriveLim;

                me.uplink_pid_ele_P = config_autopilot.pidPitch.P;
                me.uplink_pid_ele_I = config_autopilot.pidPitch.I;
                me.uplink_pid_ele_D = config_autopilot.pidPitch.D;
                me.uplink_pid_ele_IL = config_autopilot.pidPitch.ILim;
                me.uplink_pid_ele_DL = config_autopilot.pidPitch.DriveLim;

                me.uplink_pid_thr_P = config_autopilot.pidMotor.P;
                me.uplink_pid_thr_I = config_autopilot.pidMotor.I;
                me.uplink_pid_thr_D = config_autopilot.pidMotor.D;
                me.uplink_pid_thr_IL = config_autopilot.pidMotor.ILim;
                me.uplink_pid_thr_DL = config_autopilot.pidMotor.DriveLim;

                me.uplink_pid_tail_P = config_autopilot.pidTail.P;
                me.uplink_pid_tail_I = config_autopilot.pidTail.I;
                me.uplink_pid_tail_D = config_autopilot.pidTail.D;
                me.uplink_pid_tail_IL = config_autopilot.pidTail.ILim;
                me.uplink_pid_tail_DL = config_autopilot.pidTail.DriveLim;

                me.uplink_IR_offX = config_autopilot.x_off;
                me.uplink_IR_offY = config_autopilot.y_off;
                me.uplink_IR_gain = config_autopilot.IR_max;
                me.uplink_rumbo_ail = config_autopilot.Rumbo_Ail;
                me.uplink_altura_ele = config_autopilot.Altitud_Ele;

                me.uplink_IR_rev_P = config_autopilot.IR_pitch_rev != 0;
                me.uplink_IR_rev_R = config_autopilot.IR_roll_rev != 0;
                me.uplink_IR_cross = config_autopilot.IR_cross_sensor != 0;
                me.uplink_IR_rev_cross = config_autopilot.IR_cross_rev != 0;
            }
            catch (Exception) { }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            FlightPlanUSB planUSB = new FlightPlanUSB();
            IkarusAutopilotConfig config_autopilot = planUSB.ReadConfigAutopilot();


            //config.PPM_Channel = comboBoxCtrlCH.SelectedIndex + 4;

            config_autopilot.ail_ch = (byte)(comboAilCH.SelectedIndex - 1);
            config_autopilot.ele_ch = (byte)(comboEleCH.SelectedIndex- 1);
            config_autopilot.thr_ch = (byte)(comboThrCH.SelectedIndex - 1);
            config_autopilot.tail_ch = (byte)(comboTailCH.SelectedIndex - 1);
            config_autopilot.pan_ch = (byte)(comboPanCH.SelectedIndex - 1);

            // SERVO CTRL
            //config_autopilot.servo_ctrl.min = (Int16)numericUpDownServoCTRLmin.Value;
            //config_autopilot.servo_ctrl.center = (Int16)numericUpDownServoCTRLcenter.Value;
            //config_autopilot.servo_ctrl.max = (Int16)numericUpDownServoCTRLmax.Value;
            //config_autopilot.servo_ctrl.reverse = checkBoxServoCTRLrev.Checked ? (byte)1 : (byte)0;

            // SERVO AIL
            config_autopilot.servo_ail.min = (Int16)numericUpDownServoAILmin.Value;
            config_autopilot.servo_ail.center = (Int16)numericUpDownServoAILcenter.Value;
            config_autopilot.servo_ail.max = (Int16)numericUpDownServoAILmax.Value;
            config_autopilot.servo_ail.reverse = checkBoxServoAILrev.Checked ? (byte)1 : (byte)0;

            // SERVO ELE
            config_autopilot.servo_ele.min = (Int16)numericUpDownServoELEmin.Value;
            config_autopilot.servo_ele.center = (Int16)numericUpDownServoELEcenter.Value;
            config_autopilot.servo_ele.max = (Int16)numericUpDownServoELEmax.Value;
            config_autopilot.servo_ele.reverse = checkBoxServoELErev.Checked ? (byte)1 : (byte)0;

            // SERVO THR
            config_autopilot.servo_thr.min = (Int16)numericUpDownServoTHRmin.Value;
            config_autopilot.servo_thr.center = (Int16)numericUpDownServoTHRcenter.Value;
            config_autopilot.servo_thr.max = (Int16)numericUpDownServoTHRmax.Value;
            config_autopilot.servo_thr.reverse = checkBoxServoTHRrev.Checked ? (byte)1 : (byte)0;

            // SERVO TAIL
            config_autopilot.servo_tail.min = (Int16)numericUpDownServoTAILmin.Value;
            config_autopilot.servo_tail.center = (Int16)numericUpDownServoTAILcenter.Value;
            config_autopilot.servo_tail.max = (Int16)numericUpDownServoTAILmax.Value;
            config_autopilot.servo_tail.reverse = checkBoxServoTAILrev.Checked ? (byte)1 : (byte)0;

            // SERVO PAN
            config_autopilot.servo_pan.min = (Int16)numericUpDownServoPANmin.Value;
            config_autopilot.servo_pan.center = (Int16)numericUpDownServoPANcenter.Value;
            config_autopilot.servo_pan.max = (Int16)numericUpDownServoPANmax.Value;
            config_autopilot.servo_pan.reverse = checkBoxServoPANrev.Checked ? (byte)1 : (byte)0;

            config_autopilot.rev_mezcla = checkBoxMix1.Checked ? (byte)1 : (byte)0;

            switch (comboBoxTipoMezcla.SelectedIndex)
            {
                case (int)Singleton.Mezclas.Normal:
                    if (comboTiltCH.SelectedIndex > 0)
                    {
                        config_autopilot.tipo_mezcla = (byte)Singleton.Mezclas.Normal;
                        config_autopilot.aux_ch = (byte)(comboTiltCH.SelectedIndex - 1);
                        config_autopilot.servo_aux.min = (Int16)numericUpDownServoTILTmin.Value;
                        config_autopilot.servo_aux.center = (Int16)numericUpDownServoTILTcenter.Value;
                        config_autopilot.servo_aux.max = (Int16)numericUpDownServoTILTmax.Value;
                        config_autopilot.servo_aux.reverse = checkBoxServoTILTrev.Checked ? (byte)1 : (byte)0;
                    }
                    else
                    {
                    }
                    break;

                case (int)Singleton.Mezclas.Elevon:
                    if (comboTiltCH.SelectedIndex > 0)
                    {
                        config_autopilot.tipo_mezcla = (byte)Singleton.Mezclas.Elevon;
                        config_autopilot.aux_ch = (byte)(comboTiltCH.SelectedIndex - 1);
                        config_autopilot.servo_aux.min = (Int16)numericUpDownServoTILTmin.Value;
                        config_autopilot.servo_aux.center = (Int16)numericUpDownServoTILTcenter.Value;
                        config_autopilot.servo_aux.max = (Int16)numericUpDownServoTILTmax.Value;
                        config_autopilot.servo_aux.reverse = checkBoxServoTILTrev.Checked ? (byte)1 : (byte)0;
                    }
                    else
                    {
                    }
                    break;

                case (int)Singleton.Mezclas.V_Tail:
                    if (comboTiltCH.SelectedIndex > 0)
                    {
                        config_autopilot.tipo_mezcla = (byte)Singleton.Mezclas.V_Tail;
                    
                        config_autopilot.aux_ch = (byte)(comboTiltCH.SelectedIndex - 1);
                        config_autopilot.servo_aux.min = (Int16)numericUpDownServoTILTmin.Value;
                        config_autopilot.servo_aux.center = (Int16)numericUpDownServoTILTcenter.Value;
                        config_autopilot.servo_aux.max = (Int16)numericUpDownServoTILTmax.Value;
                        config_autopilot.servo_aux.reverse = checkBoxServoTILTrev.Checked ? (byte)1 : (byte)0;
                    }
                    else
                    {
                    }
                    break;

                case (int)Singleton.Mezclas.Flaperon:
                    config_autopilot.tipo_mezcla = (byte)Singleton.Mezclas.Normal;
                    config_autopilot.CanalAuxMode = (byte)Singleton.ModoCanalAux.AIL2;
                    
                    config_autopilot.aux_ch = (byte)(comboAil2CH.SelectedIndex - 1);
                    config_autopilot.servo_aux.min = (Int16)numericUpDownServoAIL2min.Value;
                    config_autopilot.servo_aux.center = (Int16)numericUpDownServoAIL2center.Value;
                    config_autopilot.servo_aux.max = (Int16)numericUpDownServoAIL2max.Value;
                    config_autopilot.servo_aux.reverse = checkBoxServoAIL2rev.Checked ? (byte)1 : (byte)0;
                    
                    
                    break;

                case (int)Singleton.Mezclas.Flaperon_Vtail:
                    config_autopilot.tipo_mezcla = (byte)Singleton.Mezclas.V_Tail;
                    config_autopilot.CanalAuxMode = (byte)Singleton.ModoCanalAux.AIL2;
                    
                    config_autopilot.aux_ch = (byte)(comboAil2CH.SelectedIndex - 1);
                    config_autopilot.servo_aux.min = (Int16)numericUpDownServoAIL2min.Value;
                    config_autopilot.servo_aux.center = (Int16)numericUpDownServoAIL2center.Value;
                    config_autopilot.servo_aux.max = (Int16)numericUpDownServoAIL2max.Value;
                    config_autopilot.servo_aux.reverse = checkBoxServoAIL2rev.Checked ? (byte)1 : (byte)0;
                    
                    break;

            }
 /*
            switch (config_autopilot.tipo_mezcla)
            {
                case (byte)Singleton.Mezclas.Elevon:
                    checkBoxMix1.Checked = config_autopilot.pidRoll.rev != 0;
                    checkBoxMix2.Checked = config_autopilot.pidPitch.rev != 0;
                    break;
                case (byte)Singleton.Mezclas.V_Tail:
                    checkBoxMix1.Checked = config_autopilot.pidPitch.rev != 0;
                    checkBoxMix2.Checked = config_autopilot.pidTail.rev != 0;
                    break;
            }
            * */
            planUSB.WriteConfigAutopilot(config_autopilot);
            planUSB.Close();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormActualizarFirmware form = new FormActualizarFirmware(FormActualizarFirmware.Devices.Uplink);
            form.Show(this);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownPeriodo.Minimum = 2000 * numericUpDownNumCanales.Value + 4000;
        }

        private void checkBoxUseTX_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseTX.Checked)
            {
                label15.Enabled = false;
                label17.Enabled = false;
                label19.Enabled = false;
                numericUpDownNumCanales.Enabled = false;
                numericUpDownPeriodo.Enabled = false;
                numericUpDownSeparador.Enabled = false;
                checkBoxPolarity.Enabled = false;
            }
            else
            {
                label15.Enabled = true;
                label17.Enabled = true;
                label19.Enabled = true;
                numericUpDownNumCanales.Enabled = true;
                numericUpDownPeriodo.Enabled = true;
                numericUpDownSeparador.Enabled = true;
                checkBoxPolarity.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormConfigurarTeclas().Show(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FlightPlanUSB planUSB = new FlightPlanUSB();
            if (planUSB.IsOpen())
            {
                int[] servos = planUSB.ReadServosRAW();

                if (comboAilCH.SelectedIndex > 0)
                {
                    numericUpDownServoAILcenter.Value =servos[comboAilCH.SelectedIndex - 1];
                }
                if (comboEleCH.SelectedIndex > 0)
                {
                    numericUpDownServoELEcenter.Value = servos[comboEleCH.SelectedIndex - 1];
                }
                /*
                if (comboThrCH.SelectedIndex > 0)
                {
                    numericUpDownServoTHRcenter.Value = servos[comboThrCH.SelectedIndex - 1];
                }
                */
                if (comboTailCH.SelectedIndex > 0)
                {
                    numericUpDownServoTAILcenter.Value = servos[comboTailCH.SelectedIndex - 1];
                }

                if (comboPanCH.SelectedIndex > 0)
                {
                    numericUpDownServoPANcenter.Value = servos[comboPanCH.SelectedIndex - 1];
                }
                if (comboTiltCH.SelectedIndex > 0)
                {
                    numericUpDownServoTILTcenter.Value = servos[comboTiltCH.SelectedIndex - 1];
                }

                if (comboAil2CH.SelectedIndex > 0)
                {
                    numericUpDownServoAIL2center.Value = servos[comboAil2CH.SelectedIndex - 1];
                }
                planUSB.Close();
            }
            else if (me.Idioma == 0)
            {
                MessageBox.Show("No se puede abrir dispositivo Ikarus OSD");
            }
            else
            {
                MessageBox.Show("Cannot open Ikarus OSD device");
            }

        }
    }
}
