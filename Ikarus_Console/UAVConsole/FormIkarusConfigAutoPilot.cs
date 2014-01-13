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
using UAVConsole.USBXpress;

namespace UAVConsole
{
    public partial class FormIkarusConfigAutoPilot : Form
    {
        Singleton me = Singleton.GetInstance();
        IkarusAutopilotConfig config_autopilot = new IkarusAutopilotConfig();
        FlightPlanUSB planUSB = new FlightPlanUSB();
            
        bool firstTime = true;
        
        public FormIkarusConfigAutoPilot()
        {
            InitializeComponent();
            if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                TextSpanish();
            else
                TextEnglish();
            comboBoxAutoPilotMode.SelectedIndex = 0;

            comboBoxSensorIR.SelectedIndex = 0;
            comboBoxTipoMezcla.SelectedIndex = 0;
            comboAilCH.SelectedIndex = 1;
            comboEleCH.SelectedIndex = 2;
            comboThrCH.SelectedIndex = 3;
            comboTailCH.SelectedIndex = 4;
            comboTiltCH.SelectedIndex = 0;
            comboPanCH.SelectedIndex = 0;

            comboBoxSensorIR.SelectedIndex = 0;

            comboBoxCanalAUXmode.SelectedIndex = 0;
            firstTime = true;

            timer1.Enabled = true;
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            UpdateStruct();
            if (!planUSB.IsOpen())
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("No puedo conectar con Ikarus OSD");
                else
                    MessageBox.Show("Cannot connect with Ikarus OSD");
            }
            else
            {
                planUSB.WriteConfigAutopilot(config_autopilot);
                
                UpdateRegistry();

                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("Configuración actualizada correctamente");
                else
                    MessageBox.Show("Config updated successfully");
            }

            timer1.Enabled = true;
        }

        void UpdateRegistry()
        {
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
                me.ToRegistry();
            }
            catch (Exception) { }
        }

        void UpdateStruct()
        {
            // SERVO CTRL
            config_autopilot.servo_ctrl.min = (Int16)numericUpDownServoCTRLmin.Value;
            config_autopilot.servo_ctrl.center = (Int16)numericUpDownServoCTRLcenter.Value;
            config_autopilot.servo_ctrl.max = (Int16)numericUpDownServoCTRLmax.Value;
            config_autopilot.servo_ctrl.reverse = checkBoxServoCTRLrev.Checked ? (byte)1 : (byte)0;
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

            // SERVO AUX
            config_autopilot.servo_aux.min = (Int16)numericUpDownServoAUXmin.Value;
            config_autopilot.servo_aux.center = (Int16)numericUpDownServoAUXcenter.Value;
            config_autopilot.servo_aux.max = (Int16)numericUpDownServoAUXmax.Value;
            config_autopilot.servo_aux.reverse = checkBoxServoAUXrev.Checked ? (byte)1 : (byte)0;

            // Otros
            config_autopilot.AutopilotMode = (byte)comboBoxAutoPilotMode.SelectedIndex;
            if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
            {
                config_autopilot.baseCruiseAltitude = (float)numericUpDownCruiseAltitude.Value;
                config_autopilot.distanceAltitude = (float)numericUpDownDistanceAltitude.Value / 1000.0f;
                config_autopilot.MotorSafeAlt = (Int16)(numericUpDownMotorSafeAlt.Value);
            }
            else
            {
                config_autopilot.baseCruiseAltitude = (float)numericUpDownCruiseAltitude.Value / 3.28f;
                config_autopilot.distanceAltitude = (float)numericUpDownDistanceAltitude.Value * 3.28f / 1852.0f;
                config_autopilot.MotorSafeAlt = (Int16)((float)numericUpDownMotorSafeAlt.Value / 3.28f);   // Altura : Pies -> metros
            }
        
            // Asignacion de canales
            config_autopilot.ail_ch = (byte)(comboAilCH.SelectedIndex-1);
            config_autopilot.ele_ch = (byte)(comboEleCH.SelectedIndex-1);
            config_autopilot.thr_ch = (byte)(comboThrCH.SelectedIndex-1);
            config_autopilot.tail_ch = (byte)(comboTailCH.SelectedIndex-1);

            config_autopilot.pan_ch = (byte)(comboPanCH.SelectedIndex-1 /*+ 4*/);
            config_autopilot.aux_ch = (byte)(comboTiltCH.SelectedIndex-1 /*+ 5*/);

            config_autopilot.pantilt_gain = (float)numericUpDownPanTiltGain.Value;
            config_autopilot.tipo_mezcla = (byte)comboBoxTipoMezcla.SelectedIndex;
            config_autopilot.rev_mezcla = (byte)(checkBoxMixRev.Checked ? 1 : 0);
           
            // IR sensor
            config_autopilot.IR_pitch_rev = (byte)(checkBoxIR_PitchRev.Checked ? 1 : 0);
            config_autopilot.IR_roll_rev = (byte)(checkBoxIR_RollRev.Checked ? 1 : 0);
            config_autopilot.IR_cross_sensor = (byte)(checkBoxIR_Cross.Checked ? 1 : 0);
            config_autopilot.IR_cross_rev = (byte)(checkBoxIR_CrossRev.Checked ? 1 : 0);

            config_autopilot.IR_Z_enabled = (byte)comboBoxSensorIR.SelectedIndex;
            config_autopilot.IR_max = (float)numericUpDownIRGain.Value;
            config_autopilot.x_off = (float)numericUpDownXoffset.Value;
            config_autopilot.y_off = (float)numericUpDownYoffset.Value;
            config_autopilot.z_off = (float)numericUpDownZoffset.Value;

            // PID
            config_autopilot.pidRoll.P = (float)numericUpDownRollP.Value;
            config_autopilot.pidRoll.I = (float)numericUpDownRollI.Value;
            config_autopilot.pidRoll.D = (float)numericUpDownRollD.Value;
            config_autopilot.pidRoll.ILim = (float)numericUpDownRollILim.Value;
            config_autopilot.pidRoll.DriveLim = (float)numericUpDownRollDriveLim.Value;

            config_autopilot.pidPitch.P = (float)numericUpDownPitchP.Value;
            config_autopilot.pidPitch.I = (float)numericUpDownPitchI.Value;
            config_autopilot.pidPitch.D = (float)numericUpDownPitchD.Value;
            config_autopilot.pidPitch.ILim = (float)numericUpDownPitchILim.Value;
            config_autopilot.pidPitch.DriveLim = (float)numericUpDownPitchDriveLim.Value;

            config_autopilot.pidTail.P = (float)numericUpDownTailP.Value;
            config_autopilot.pidTail.I = (float)numericUpDownTailI.Value;
            config_autopilot.pidTail.D = (float)numericUpDownTailD.Value;
            config_autopilot.pidTail.ILim = (float)numericUpDownTailIlim.Value;
            config_autopilot.pidTail.DriveLim = (float)numericUpDownTailDriveLim.Value;

            config_autopilot.pidMotor.P = (float)numericUpDownMotorP.Value;
            config_autopilot.pidMotor.I = (float)numericUpDownMotorI.Value;
            config_autopilot.pidMotor.D = (float)numericUpDownMotorD.Value;
            config_autopilot.pidMotor.ILim = (float)numericUpDownMotorIlim.Value;
            config_autopilot.pidMotor.DriveLim = (float)numericUpDownMotorDriveLim.Value;

            config_autopilot.Rumbo_Ail = (float)numericUpDownRumboAil.Value;
            config_autopilot.Altitud_Ele = (float)numericUpDownAltitudEle.Value;

            config_autopilot.CanalAuxMode = (byte)(comboBoxCanalAUXmode.SelectedIndex);

            // Ganancias especificas para copilot
            config_autopilot.copilot_use_PID_gains = (byte)(checkBoxCopilotHeredar.Checked ? 1 : 0);
            config_autopilot.copilot_pitch_P = (float)numericUpDownCopilotPitch.Value;
            config_autopilot.copilot_roll_P = (float)numericUpDownCopilotRoll.Value;
        
            // Control de giro coordinado
            config_autopilot.mezclaAutopilot_roll_on = (byte)(checkBoxGiroCoordinado.Checked ? 1 : 0);
            config_autopilot.mezclaAutopilot_roll_value = (float)trackBarGiroCoordinado.Value / 100.0f;

            config_autopilot.ServoMotorSafeAlt = (Int16)numericUpDownMotorOff.Value;
            config_autopilot.ServoMotorEnableAlt = (byte)(checkBoxMotorAlt.Checked ? 1 : 0);
        }

        void UpdateControls()
        {
            string error_msg = "";
            int error_count = 0;

            try
            {
                // SERVO CTRL
                numericUpDownServoCTRLmin.Value = (decimal)config_autopilot.servo_ctrl.min;
                numericUpDownServoCTRLcenter.Value = (decimal)config_autopilot.servo_ctrl.center;
                numericUpDownServoCTRLmax.Value = (decimal)config_autopilot.servo_ctrl.max;
                checkBoxServoCTRLrev.Checked = config_autopilot.servo_ctrl.reverse != 0;
            }
            catch(Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo de control\n";
                else
                    error_msg += "- Error loading control servo range\n";
                error_count++;
            }

            try
            {
                // SERVO AIL
                numericUpDownServoAILmin.Value = (decimal)config_autopilot.servo_ail.min;
                numericUpDownServoAILcenter.Value = (decimal)config_autopilot.servo_ail.center;
                numericUpDownServoAILmax.Value = (decimal)config_autopilot.servo_ail.max;
                checkBoxServoAILrev.Checked = config_autopilot.servo_ail.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo alerones\n";
                else
                    error_msg += "- Error loading aileron servo range\n";
                error_count++;
            }

            try
            {
                // SERVO ELE
                numericUpDownServoELEmin.Value = (decimal)config_autopilot.servo_ele.min;
                numericUpDownServoELEcenter.Value = (decimal)config_autopilot.servo_ele.center;
                numericUpDownServoELEmax.Value = (decimal)config_autopilot.servo_ele.max;
                checkBoxServoELErev.Checked = config_autopilot.servo_ele.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo elevador\n";
                else
                    error_msg += "- Error loading elevator servo range\n";
                error_count++;
            }

            try
            {
                // SERVO THR
                numericUpDownServoTHRmin.Value = (decimal)config_autopilot.servo_thr.min;
                numericUpDownServoTHRcenter.Value = (decimal)config_autopilot.servo_thr.center;
                numericUpDownServoTHRmax.Value = (decimal)config_autopilot.servo_thr.max;
                checkBoxServoTHRrev.Checked = config_autopilot.servo_thr.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo de gases\n";
                else
                    error_msg += "- Error loading thrust servo range\n";
                error_count++;
            }

            try
            {
                // SERVO TAIL
                numericUpDownServoTAILmin.Value = (decimal)config_autopilot.servo_tail.min;
                numericUpDownServoTAILcenter.Value = (decimal)config_autopilot.servo_tail.center;
                numericUpDownServoTAILmax.Value = (decimal)config_autopilot.servo_tail.max;
                checkBoxServoTAILrev.Checked = config_autopilot.servo_tail.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo de cola\n";
                else
                    error_msg += "- Error loading tail servo range\n";
                error_count++;
            }

            try
            {
                // SERVO PAN
                numericUpDownServoPANmin.Value = (decimal)config_autopilot.servo_pan.min;
                numericUpDownServoPANcenter.Value = (decimal)config_autopilot.servo_pan.center;
                numericUpDownServoPANmax.Value = (decimal)config_autopilot.servo_pan.max;
                checkBoxServoPANrev.Checked = config_autopilot.servo_pan.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo pan\n";
                else
                    error_msg += "- Error loading pan servo range\n";
                error_count++;
            }

            try
            {
                // SERVO AUX
                numericUpDownServoAUXmin.Value = (decimal)config_autopilot.servo_aux.min;
                numericUpDownServoAUXcenter.Value = (decimal)config_autopilot.servo_aux.center;
                numericUpDownServoAUXmax.Value = (decimal)config_autopilot.servo_aux.max;
                checkBoxServoAUXrev.Checked = config_autopilot.servo_aux.reverse != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando rangos servo aux\n";
                else
                    error_msg += "- Error loading aux servo range\n";
                error_count++;
            }

            try
            {
                // Otros
                comboBoxAutoPilotMode.SelectedIndex = config_autopilot.AutopilotMode;
                if (me.SistemaMetrico == (int)Singleton.SistemasMetricos.Metrico)
                {
                    numericUpDownCruiseAltitude.Value = (decimal)config_autopilot.baseCruiseAltitude;
                    numericUpDownDistanceAltitude.Value = (decimal)(config_autopilot.distanceAltitude * 1000.0f);
                    numericUpDownMotorSafeAlt.Value = (decimal)config_autopilot.MotorSafeAlt;
                }
                else
                {
                    numericUpDownCruiseAltitude.Value = (decimal)(config_autopilot.baseCruiseAltitude * 3.28f);
                    numericUpDownDistanceAltitude.Value = (decimal)(config_autopilot.distanceAltitude * 1852.0f / 3.28f);
                    numericUpDownMotorSafeAlt.Value = (decimal)(config_autopilot.MotorSafeAlt * 3.28f);
                }
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando valores autopilot\n";
                else
                    error_msg += "- Error loading autopilot values\n";
                error_count++;
            }

            try
            {
                // Asignacion de canales
                comboAilCH.SelectedIndex = (byte)(config_autopilot.ail_ch + 1);
                comboEleCH.SelectedIndex = (byte)(config_autopilot.ele_ch + 1);
                comboThrCH.SelectedIndex = (byte)(config_autopilot.thr_ch + 1);
                comboTailCH.SelectedIndex = (byte)(config_autopilot.tail_ch + 1);

                comboPanCH.SelectedIndex = (byte)(config_autopilot.pan_ch + 1);
                comboTiltCH.SelectedIndex = (byte)(config_autopilot.aux_ch + 1);
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando asignación de canales\n";
                else
                    error_msg += "- Error loading channel assignements\n";
                error_count++;
            }

            try
            {
                numericUpDownPanTiltGain.Value = (decimal)config_autopilot.pantilt_gain;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando ganancia pan & tilt\n";
                else
                    error_msg += "- Error loading pan & tilt gain\n";
                error_count++;
            }
            try
            {
                comboBoxCanalAUXmode.SelectedIndex = config_autopilot.CanalAuxMode;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando modo canal aux\n";
                else
                    error_msg += "- Error loading channel aux mode\n";
                error_count++;
            }

            try
            {
                comboBoxTipoMezcla.SelectedIndex = config_autopilot.tipo_mezcla;
                checkBoxMixRev.Checked = config_autopilot.rev_mezcla != 0;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración mezcla\n";
                else
                    error_msg += "- Error loading mix config\n";
                error_count++;
            }

            try
            {
                // PID
                numericUpDownPitchP.Value = (decimal)config_autopilot.pidPitch.P;
                numericUpDownPitchI.Value = (decimal)config_autopilot.pidPitch.I;
                numericUpDownPitchD.Value = (decimal)config_autopilot.pidPitch.D;
                numericUpDownPitchILim.Value = (decimal)config_autopilot.pidPitch.ILim;
                numericUpDownPitchDriveLim.Value = (decimal)config_autopilot.pidPitch.DriveLim;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando PID pitch\n";
                else
                    error_msg += "- Error loading pitch PID\n";
                error_count++;
            }

            try
            {
                numericUpDownRollP.Value = (decimal)config_autopilot.pidRoll.P;
                numericUpDownRollI.Value = (decimal)config_autopilot.pidRoll.I;
                numericUpDownRollD.Value = (decimal)config_autopilot.pidRoll.D;
                numericUpDownRollILim.Value = (decimal)config_autopilot.pidRoll.ILim;
                numericUpDownRollDriveLim.Value = (decimal)config_autopilot.pidRoll.DriveLim;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando PID roll\n";
                else
                    error_msg += "- Error loading roll PID\n";
                error_count++;
            }

            try
            {
                numericUpDownTailP.Value = (decimal)config_autopilot.pidTail.P;
                numericUpDownTailI.Value = (decimal)config_autopilot.pidTail.I;
                numericUpDownTailD.Value = (decimal)config_autopilot.pidTail.D;
                numericUpDownTailIlim.Value = (decimal)config_autopilot.pidTail.ILim;
                numericUpDownTailDriveLim.Value = (decimal)config_autopilot.pidTail.DriveLim;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando PID rumbo\n";
                else
                    error_msg += "- Error loading heading PID\n";
                error_count++;
            }

            try
            {
                numericUpDownMotorP.Value = (decimal)config_autopilot.pidMotor.P;
                numericUpDownMotorI.Value = (decimal)config_autopilot.pidMotor.I;
                numericUpDownMotorD.Value = (decimal)config_autopilot.pidMotor.D;
                numericUpDownMotorIlim.Value = (decimal)config_autopilot.pidMotor.ILim;
                numericUpDownMotorDriveLim.Value = (decimal)config_autopilot.pidMotor.DriveLim;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando PID altura\n";
                else
                    error_msg += "- Error loading altitude PID\n";
                error_count++;
            }
     
            try
            {
                // IR sensor
                checkBoxIR_PitchRev.Checked = config_autopilot.IR_pitch_rev != 0;
                checkBoxIR_RollRev.Checked = config_autopilot.IR_roll_rev != 0;
                checkBoxIR_Cross.Checked = config_autopilot.IR_cross_sensor != 0;
                checkBoxIR_CrossRev.Checked = config_autopilot.IR_cross_rev != 0;

                comboBoxSensorIR.SelectedIndex = config_autopilot.IR_Z_enabled;

                numericUpDownIRGain.Value = (decimal)config_autopilot.IR_max;
                numericUpDownXoffset.Value = (decimal)config_autopilot.x_off;
                numericUpDownYoffset.Value = (decimal)config_autopilot.y_off;
                numericUpDownZoffset.Value = (decimal)config_autopilot.z_off;

                numericUpDownRumboAil.Value = (decimal)config_autopilot.Rumbo_Ail;
                numericUpDownAltitudEle.Value = (decimal)config_autopilot.Altitud_Ele;

        
                // Ganancias especificas para copilot
                checkBoxCopilotHeredar.Checked = config_autopilot.copilot_use_PID_gains != 0;
                numericUpDownCopilotPitch.Value = (decimal)config_autopilot.copilot_pitch_P;
                numericUpDownCopilotRoll.Value = (decimal)config_autopilot.copilot_roll_P;
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración sensor IR\n";
                else
                    error_msg += "- Error loading IR sensor config\n";
                error_count++;
            }

            try
            {

                // Control de giro coordinado
                checkBoxGiroCoordinado.Checked = config_autopilot.mezclaAutopilot_roll_on != 0;
                trackBarGiroCoordinado.Value = (int)(config_autopilot.mezclaAutopilot_roll_value * 100.0f);
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando valores giro coordinado\n";
                else
                    error_msg += "- Error loading coordinated turn values\n";
                error_count++;
            }

            try
            {
                numericUpDownMotorOff.Value = (decimal)config_autopilot.ServoMotorSafeAlt;
                checkBoxMotorAlt.Checked = config_autopilot.ServoMotorEnableAlt != 0;
      
            }
            catch (Exception)
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg += "- Error cargando configuración seguridad motor\n";
                else
                    error_msg += "- Error loading motor security values\n";
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
            else
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    error_msg = "Configuración leida correctamente.";
                else
                    error_msg = "Config load sucesfully.";
                MessageBox.Show(error_msg);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (planUSB.IsOpen())
            {
                //timer1.Enabled = false;
                if (firstTime)
                {
                    firstTime = false;
                    config_autopilot = planUSB.ReadConfigAutopilot();
                    UpdateControls();

                    panel1.Enabled = true;

                    UpdateRegistry();
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
                    labelStatus.Text = "Not Connected!!";
                labelStatus.ForeColor = Color.Red;
            }

        }
       
        private void button1_Click(object sender, EventArgs e)
        {   // Load
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Autopilot (*.xml)|*.xml";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                config_autopilot.LoadFromXml(dlg.FileName);
                UpdateControls();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {   // Save
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Autopilot (*.xml)|*.xml";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                UpdateStruct();
                config_autopilot.SaveToXml(dlg.FileName);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (!planUSB.IsOpen())
            {
                if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                    MessageBox.Show("No puedo conectar con Ikarus OSD");
                else
                    MessageBox.Show("Cannot connect with Ikarus OSD");
            }
            else
            {
                firstTime = true;
                //config_autopilot = planUSB.ReadConfigAutopilot();
                //UpdateControls();
                //UpdateRegistry();
            }

            timer1.Enabled = true;
        }

        void TextSpanish()
        {
            labelStatus.Text = "No Conectado!";
            groupBox4.Text = "Controles PID";
            groupBox3.Text = "Recorrido Servos";
            label1.Text = "Servo Alerones";
            label2.Text = "Servo Elevador";
            label3.Text = "Servo Cola";
            label4.Text = "Servo Motor";
            label28.Text = "Mezcla";
            this.Text = "Configurar Autopilot";
        }

        void TextEnglish()
        {
            labelStatus.Text = "Not Connected!";
            groupBox4.Text = "PID Controls";
            groupBox3.Text = "Servo Ranges";
            label1.Text = "Servo Ailerons";
            label2.Text = "Servo Elevator";
            label3.Text = "Servo Tail";
            label4.Text = "Servo Motor";
            label28.Text = "Mixing";
            this.Text = "Autopilot Config";
        }

        private void comboBoxSensorIR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSensorIR.SelectedIndex == 0)
            {
                label32.Enabled = true;
                numericUpDownIRGain.Enabled = true;
                label33.Enabled = false;
                numericUpDownZoffset.Enabled = false;
            }
            else if (comboBoxSensorIR.SelectedIndex == 1)
            {
                label32.Enabled = false;
                numericUpDownIRGain.Enabled = false;
                label33.Enabled = true;
                numericUpDownZoffset.Enabled = true;
            }
            else
            {
                label32.Enabled = false;
                numericUpDownIRGain.Enabled = false;
                label33.Enabled = false;
                numericUpDownXoffset.Enabled = false;
                label30.Enabled = false;
                numericUpDownYoffset.Enabled = false;
                label31.Enabled = false;
                numericUpDownZoffset.Enabled = false;
            }
        }
        
        // Chorrada para que los rangos sean coherentes....

        private void numericUpDownServoCTRLmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoCTRLcenter.Minimum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoCTRLcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoCTRLmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoCTRLmin.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoCTRLmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoCTRLcenter.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoAILmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAILcenter.Minimum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoAILcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAILmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoAILmin.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoAILmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAILcenter.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoELEmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoELEcenter.Minimum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoELEcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoELEmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoELEmin.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoELEmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoELEcenter.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoTHRmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTHRcenter.Minimum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoTHRcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTHRmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoTHRmin.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoTHRmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTHRcenter.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoTAILmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTAILcenter.Minimum = ((NumericUpDown)sender).Value;
        
        }

        private void numericUpDownServoTAILcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTAILmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoTAILmin.Maximum = ((NumericUpDown)sender).Value;
        
        }

        private void numericUpDownServoTAILmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoTAILcenter.Maximum = ((NumericUpDown)sender).Value;
        
        }

        private void numericUpDownServoAUXmin_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAUXcenter.Minimum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoAUXcenter_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAUXmax.Minimum = ((NumericUpDown)sender).Value;
            numericUpDownServoAUXmin.Maximum = ((NumericUpDown)sender).Value;
        }

        private void numericUpDownServoAUXmax_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownServoAUXcenter.Maximum = ((NumericUpDown)sender).Value;
        }

        private void comboBoxCanalAUXmode_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxCanalAUXmode.SelectedIndex == (int)Singleton.ModoCanalAux.TILT)
            {
                label25.Visible = true;
                comboTiltCH.Visible = true;
                label38.Text = "Servo TILT";
                label25.Text = "TILT CH";
            }
            else if (comboBoxCanalAUXmode.SelectedIndex == (int)Singleton.ModoCanalAux.AIL2)
            {
                label25.Visible = true;
                comboTiltCH.Visible = true;
                label38.Text = "Servo AIL2";
                label25.Text = "AIL2 CH";
            }
            else
            {
                label25.Visible = false;
                comboTiltCH.Visible = false;
                label38.Text = "Servo AUX";
                label25.Text = "AUX CH";
            }
        }

        private void comboBoxTipoMezcla_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTipoMezcla.SelectedIndex < 1)
                checkBoxMixRev.Enabled = false;
            else
                checkBoxMixRev.Enabled = true;
        }

        private void checkBoxCopilotHeredar_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCopilotHeredar.Checked)
            {
                numericUpDownCopilotPitch.Enabled = false;
                numericUpDownCopilotRoll.Enabled = false;
                label29.Enabled = false;
                label41.Enabled = false;
            }
            else
            {
                numericUpDownCopilotPitch.Enabled = true;
                numericUpDownCopilotRoll.Enabled = true;
                label29.Enabled = true;
                label41.Enabled = true;
            }
        }

        private void checkBoxGiroCoordinado_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGiroCoordinado.Checked)
            {
                label42.Enabled = true;
                label43.Enabled = true;
                trackBarGiroCoordinado.Enabled = true;
            }
            else
            {
                label42.Enabled = false;
                label43.Enabled = false;
                trackBarGiroCoordinado.Enabled = false;
            }
        }

        private void labelStatus_DoubleClick(object sender, EventArgs e)
        {
            panel1.Enabled = true;
        }

        private void checkBoxMotorAlt_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMotorAlt.Checked)
            {
                numericUpDownMotorOff.Enabled = true;
                label44.Enabled = true;
            }
            else
            {
                numericUpDownMotorOff.Enabled = false;
                label44.Enabled = false;
            }
        }

        private void FormIkarusConfigAutoPilot_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            planUSB.Close();
        }
    }
}