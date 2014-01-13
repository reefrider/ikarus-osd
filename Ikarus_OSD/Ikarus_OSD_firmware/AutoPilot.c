/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
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

#include <math.h>
#include "ParserNMEA.h"
#include "ikarus.h"
#include "PID.h"
#include "Servos.h"

extern xdata struct GPSInfo gpsinfo; 
extern code struct AutoPilotConfig autopilotCfg;
extern code struct StoredConfig storedConfig;
extern xdata struct IkarusInfo ikarusInfo;
extern xdata float adc_values[];
extern xdata unsigned int imu_ppm[];

xdata struct PID motorPID;
xdata struct PID tailPID;
xdata struct PID aileronsPID;
xdata struct PID elevatorPID;

xdata float modem_irxoff, modem_iryoff, modem_irmax, modem_ail, modem_ele;

void InitAutoPilot() large
{
	// Inicializamos PID motor
	InitPID(&motorPID, autopilotCfg.pid_motor.P, autopilotCfg.pid_motor.I, autopilotCfg.pid_motor.D);
	motorPID.DriveLimit = autopilotCfg.pid_motor.DL;
	motorPID.ILimit = autopilotCfg.pid_motor.IL;

	// Inicializamos PID cola
	InitPID(&tailPID, autopilotCfg.pid_tail.P, autopilotCfg.pid_tail.I,  autopilotCfg.pid_tail.D);
	tailPID.DriveLimit = autopilotCfg.pid_tail.DL;
	tailPID.ILimit = autopilotCfg.pid_tail.IL;

	// Inicializamos PID pitch
	InitPID(&elevatorPID,  autopilotCfg.pid_pitch.P, autopilotCfg.pid_pitch.I, autopilotCfg.pid_pitch.D);
	elevatorPID.DriveLimit = autopilotCfg.pid_pitch.DL;
	elevatorPID.ILimit = autopilotCfg.pid_pitch.IL;

	// Inicializamos PID roll
	InitPID(&aileronsPID,  autopilotCfg.pid_roll.P, autopilotCfg.pid_roll.I, autopilotCfg.pid_roll.D);
	aileronsPID.DriveLimit = autopilotCfg.pid_roll.DL;
	aileronsPID.ILimit = autopilotCfg.pid_roll.IL;


//	modem_irxoff=autopilotCfg.x_off;
//	modem_iryoff=autopilotCfg.y_off;

//	modem_irmax=autopilotCfg.IR_max;

//	modem_ail=autopilotCfg.rumbo_ail;
//	modem_ele=autopilotCfg.altura_ele;

}
#ifndef IMU_UM6
void Sensor_DUMMY() large
{
	ikarusInfo.Pitch=0.0f;		// -> grados
	ikarusInfo.Roll=0.0f;
}

void Sensor_IR() large
{
	float co_x, co_y, co_z;
	float co_pitch, co_roll;

	float total_ir;
	
	co_x=adc_values[ADC_IR_X]-modem_irxoff; //autopilotCfg.x_off;
	co_y=adc_values[ADC_IR_Y]-modem_iryoff; //autopilotCfg.y_off;

	if(autopilotCfg.IR_crossed==0)
	{
		if(autopilotCfg.IR_reverse_cross==0)
		{
			co_pitch=co_x;
			co_roll=co_y;
		}
		else
		{
			co_pitch=co_y;
			co_roll=co_x;
		}
	}
	else
	{
		if(autopilotCfg.IR_reverse_cross==0)
		{
			co_pitch=(co_x+co_y)/2;
			co_roll=(co_x-co_y)/2;
		}
		else
		{
			co_pitch=(co_x-co_y)/2;
			co_roll=(co_x+co_y)/2;
		}
	}

	if(autopilotCfg.IR_Z_enabled==IR_XYZ)
	{
		co_z=adc_values[ADC_IR_Z]-autopilotCfg.z_off;
//		total_ir=(abs(co_x)+abs(co_y)+abs(co_z))/3.0f;
		total_ir=sqrt(co_x*co_x+co_y*co_y+co_z*co_z);
		co_pitch=co_pitch/total_ir; 
		co_roll=co_roll/total_ir;

		if(co_z<0) // Des-ambigua invertidos
		{
//			co_pitch=2-co_pitch;
//			co_roll=2-co_roll;
		}
	}
	else
	{
		total_ir=modem_irmax;	//autopilotCfg.IR_max;	// From calibration

		co_pitch=co_pitch/total_ir; 
		co_roll=co_roll/total_ir;
	}

	if(autopilotCfg.IR_reverse_pitch)
		co_pitch=-co_pitch;

	if(autopilotCfg.IR_reverse_roll)
		co_roll=-co_roll;

	co_pitch *=90.0f; // -> grados
	co_roll *= 90.0f;

	if(co_pitch > 90.0f)
		co_pitch = 90.0f;
	else if(co_pitch < -90.0f)
		co_pitch = -90.0f;

	if(co_roll > 90.0f)
		co_roll = 90.0f;
	else if(co_roll < -90.0f)
		co_roll = -90.0f;

	ikarusInfo.Pitch=co_pitch;		
	ikarusInfo.Roll=co_roll;
}
#endif

void CoPilot() large
{
	// Estabilizacion del avion	
	if(autopilotCfg.copilot_use_PID_gains)
	{
		ikarusInfo.AutoPilot_ele = ControlP(elevatorPID.P, 0.0f, elevatorPID.DriveLimit, ikarusInfo.Pitch);
		ikarusInfo.AutoPilot_ail = ControlP(aileronsPID.P, 0.0f, aileronsPID.DriveLimit, ikarusInfo.Roll);
	}
	else
	{
		ikarusInfo.AutoPilot_ele = ControlP(autopilotCfg.copilot_pitch_P, 0.0f, 1.0f, ikarusInfo.Pitch);
		ikarusInfo.AutoPilot_ail = ControlP(autopilotCfg.copilot_roll_P, 0.0f, 1.0f, ikarusInfo.Roll);
	}	
}

void FullAutoPilot() large		// Con mezclas
{
	xdata float actitud_pitch, actitud_roll;
	xdata float servo_rumbo, servo_altura;
	xdata float servo_pitch, servo_roll;

	xdata float safe_alt;

	safe_alt = (gpsinfo.alt_filter-storedConfig.HomeAltitude)>=autopilotCfg.MotorSafeAlt;

	// Top Level Autopilot
	if(gpsinfo.conected && gpsinfo.pos_valid && gpsinfo.nmea_ok && safe_alt)
	{
		tailPID.set_point=ikarusInfo.navigator_rel_bearing;      
		servo_rumbo = ControlPID(&tailPID,0.0f);
		actitud_roll = limit(modem_ail * tailPID.resultado, -modem_ail, modem_ail);
		if(!autopilotCfg.mezclaAutopilot_roll_on)
			ikarusInfo.AutoPilot_tail = servo_rumbo;
		
	 	motorPID.set_point=ikarusInfo.navigator_altitude;
		servo_altura=ControlPID(&motorPID,getRelAltitude());
		actitud_pitch = limit(modem_ele * motorPID.resultado, -modem_ele, modem_ele);
		ikarusInfo.AutoPilot_thr = servo_altura;

	}
	else
	{
		actitud_roll = 0.0f;
		actitud_pitch = 0.0f;
		ikarusInfo.AutoPilot_tail=0.0f;
		if(safe_alt || !autopilotCfg.ServoMotorEnableAlt)
			ikarusInfo.AutoPilot_thr=-1.0f;
		else
			ikarusInfo.AutoPilot_thr=-2.0f;
	}

	// Low Level Autopilot	
	aileronsPID.set_point=actitud_roll;
	servo_roll = ControlPID(&aileronsPID, ikarusInfo.Roll);

	if(!autopilotCfg.mezclaAutopilot_roll_on)
	{
		ikarusInfo.AutoPilot_ail = servo_roll;
	}	
	else
	{
		ikarusInfo.AutoPilot_ail = (1 - autopilotCfg.mezclaAutopilot_roll_value) * servo_roll;
		ikarusInfo.AutoPilot_tail = autopilotCfg.mezclaAutopilot_roll_value * servo_roll;
	}

	elevatorPID.set_point=actitud_pitch;
	servo_pitch = ControlPID(&elevatorPID, ikarusInfo.Pitch);
	ikarusInfo.AutoPilot_ele=servo_pitch;

	ikarusInfo.AutoPilot_actitud_roll=actitud_roll;
	ikarusInfo.AutoPilot_actitud_pitch=actitud_pitch;
}

/*
void FullAutoPilot() large
{
	xdata float servo_tail, servo_roll, servo_motor, servo_pitch;

	// Top Level Autopilot
	if(gpsinfo.conected&&gpsinfo.pos_valid&&gpsinfo.nmea_ok)
	{
		tailPID.set_point=ikarusInfo.navigator_rel_bearing;      
		servo_tail = ControlPID(&tailPID,0.0f);
	
		if(tailPID.DriveLimit<0.1f)
			servo_roll=servo_tail*modem_ail;//*10.0f;	// *10 == /0.1
		else
			servo_roll=servo_tail*modem_ail/tailPID.DriveLimit;

		ikarusInfo.AutoPilot_tail=servo_tail;
		ikarusInfo.AutoPilot_actitud_roll=servo_roll;

	 	motorPID.set_point=ikarusInfo.navigator_altitude;
		servo_motor=ControlPID(&motorPID,getRelAltitude());

		if(motorPID.DriveLimit<0.1f)
			servo_pitch=servo_motor*modem_ele; //*10;		// *10 == /0.1
		else
			servo_pitch=servo_motor*modem_ele/motorPID.DriveLimit;
	
		if((gpsinfo.alt_filter-storedConfig.HomeAltitude)<autopilotCfg.MotorSafeAlt) // Por seguridad desactivar motor <20m
			servo_motor=-1.0f;

		ikarusInfo.AutoPilot_thr=servo_motor;
		ikarusInfo.AutoPilot_actitud_pitch=servo_pitch;
	}
	else
	{
		ikarusInfo.AutoPilot_tail=0.0f;
		ikarusInfo.AutoPilot_thr=-1.0f;
		ikarusInfo.AutoPilot_actitud_roll=0.0f;
		ikarusInfo.AutoPilot_actitud_pitch=0.0f;

	}

	// Low Level Autopilot
	// Estabilizacion del avion	
	aileronsPID.set_point=ikarusInfo.AutoPilot_actitud_roll;
	elevatorPID.set_point=ikarusInfo.AutoPilot_actitud_pitch;

	servo_pitch = ControlPID(&elevatorPID, ikarusInfo.Pitch);
	servo_roll = ControlPID(&aileronsPID, ikarusInfo.Roll);

	ikarusInfo.AutoPilot_ele=servo_pitch;
	ikarusInfo.AutoPilot_ail=servo_roll;
}
*/

/*
#if 1
void DriveByWire(float pitch, float roll) large
{
	float servo_pitch, servo_roll;

	// Estabilizacion del avion	
	aileronsPID.set_point=0.0f;
	elevatorPID.set_point=0.0f;

	servo_pitch = ControlP(&elevatorPID, ikarusInfo.Pitch); //, autopilotCfg.pid_pitch.rev, pitch);
	servo_roll = ControlP(&aileronsPID, ikarusInfo.Roll); //, autopilotCfg.pid_roll.rev, roll);

	servo_pitch+=pitch;
	servo_roll+=roll;

	if(autopilotCfg.pid_pitch.rev)
		servo_pitch=-servo_pitch;

	if(autopilotCfg.pid_roll.rev)
		servo_roll=-servo_roll;	

	ikarusInfo.AutoPilot_ele=servo_pitch;
	ikarusInfo.AutoPilot_ail=servo_roll;

}

#else

void DriveByWire(float pitch, float roll) large
{
	float servo_pitch, servo_roll;

	// Estabilizacion del avion	
	aileronsPID.set_point=45.0f*roll;
	elevatorPID.set_point=45.0f*pitch;

	servo_pitch = ControlP(&elevatorPID, ikarusInfo.Pitch); //, autopilotCfg.pid_pitch.rev, pitch);
	servo_roll = ControlP(&aileronsPID, ikarusInfo.Roll); //, autopilotCfg.pid_roll.rev, roll);

	if(autopilotCfg.pid_pitch.rev)
		servo_pitch=-servo_pitch;

	if(autopilotCfg.pid_roll.rev)
		servo_roll=-servo_roll;	

	ikarusInfo.AutoPilot_ele=servo_pitch;
	ikarusInfo.AutoPilot_ail=servo_roll;

}
#endif
*/

