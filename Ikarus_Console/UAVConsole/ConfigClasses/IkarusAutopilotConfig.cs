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
using UAVConsole.ConfigClasses;
using System.Runtime.InteropServices;

namespace UAVConsole.ConfigClasses
{
    [StructLayout(LayoutKind.Sequential)]
    class IkarusAutopilotConfig : GenericConfigClass
    {
        public ServoInfo servo_ctrl;
        public ServoInfo servo_ail;
        public ServoInfo servo_ele;
        public ServoInfo servo_thr;
        public ServoInfo servo_tail;
        public ServoInfo servo_pan;
        public ServoInfo servo_aux;


        public byte AutopilotMode;
        public float baseCruiseAltitude;
        public float distanceAltitude;

        public byte ail_ch;
        public byte ele_ch;
        public byte thr_ch;
        public byte tail_ch;
        public byte pan_ch;
        public byte aux_ch;

        public float pantilt_gain;
        public byte tipo_mezcla;
        public byte rev_mezcla;

        public byte IR_pitch_rev;
        public byte IR_roll_rev;
        public byte IR_cross_sensor;
        public byte IR_cross_rev;

        public byte IR_Z_enabled;
        public float x_off; 
        public float y_off; 
        public float z_off;

        public float IR_max;

        public PidInfo pidPitch;
        public PidInfo pidRoll;
        public PidInfo pidMotor;
        public PidInfo pidTail;

        public float Rumbo_Ail;
        public float Altitud_Ele;

        public byte CanalAuxMode;
        public Int16 MotorSafeAlt;
       
        // Copilot ganancias
        public byte copilot_use_PID_gains;
        public float copilot_pitch_P;
        public float copilot_roll_P;

        // Autopilot mezclado
        public byte mezclaAutopilot_roll_on;
        public float mezclaAutopilot_roll_value;

        public byte ServoMotorEnableAlt;
        public Int16 ServoMotorSafeAlt;


        public IkarusAutopilotConfig()
        {
            this.servo_ctrl = new ServoInfo();
            this.servo_ail = new ServoInfo();
            this.servo_ele = new ServoInfo();
            this.servo_thr = new ServoInfo();
            this.servo_tail = new ServoInfo();
            this.servo_aux = new ServoInfo();
            this.servo_pan = new ServoInfo();

            this.pidPitch = new PidInfo();
            this.pidRoll = new PidInfo();
            this.pidMotor = new PidInfo();
            this.pidTail = new PidInfo();
            size_bytes();
        }

        public override void LoadDefaults()
        {
            this.servo_ctrl.LoadDefaults();
            this.servo_ail.LoadDefaults();
            this.servo_ele.LoadDefaults();
            this.servo_thr.LoadDefaults();
            this.servo_tail.LoadDefaults();
            this.servo_aux.LoadDefaults();
            this.servo_pan.LoadDefaults();

            this.AutopilotMode = 1;
            this.baseCruiseAltitude = 125.0f;
            this.distanceAltitude = 50.0f;

            this.pidPitch.LoadDefaults();
            this.pidRoll.LoadDefaults();
            this.pidMotor.LoadDefaults();
            this.pidTail.LoadDefaults();

            this.Rumbo_Ail = 20.0f;
            this.Altitud_Ele = 10.0f;

            this.ail_ch = 1;
            this.ele_ch = 2;
            this.thr_ch = 3;
            this.tail_ch = 4;
            this.pan_ch = 6;
            this.aux_ch = 7;

            this.pantilt_gain = 2.0f;
            this.tipo_mezcla = 0;

            this.IR_pitch_rev = 0;
            this.IR_roll_rev = 0;
            this.IR_cross_sensor = 0;
            this.IR_cross_rev = 0;

            this.IR_Z_enabled = 0;
            this.x_off = 1.66f;
            this.y_off = 1.66f;
            this.z_off = 1.66f;

            this.IR_max = 2.0f;

            this.CanalAuxMode = 0;

            this.MotorSafeAlt = 20;

            this.copilot_use_PID_gains = (byte)1;
            this.copilot_pitch_P = 0.02f;
            this.copilot_roll_P = 0.02f;

            // Autopilot mezclado
            this.mezclaAutopilot_roll_on = (byte)1;
            this.mezclaAutopilot_roll_value = 0.9f;

            this.ServoMotorSafeAlt = 1000;
            this.ServoMotorEnableAlt = 0;
        }
    }
}
