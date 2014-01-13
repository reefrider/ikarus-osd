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
#include "ikarus.h"
#include "ParserNMEA.h"
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


	modem_irxoff=autopilotCfg.x_off;
	modem_iryoff=autopilotCfg.y_off;

	modem_irmax=autopilotCfg.IR_max;

	modem_ail=autopilotCfg.rumbo_ail;
	modem_ele=autopilotCfg.altura_ele;

}

#ifdef SIMULADOR
extern const TDataFromAeroSimRC  *ptDataFromAeroSimRC;
void SimulaIR()
{
	ikarusInfo.Pitch=ptDataFromAeroSimRC->Model_fPitch*180.0f/3.1415f;		// -> grados
	ikarusInfo.Roll=-ptDataFromAeroSimRC->Model_fRoll*180.0f/3.1415f;		// -> grados
}
#else
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

	if(autopilotCfg.IR_Z_enabled)
	{
		co_z=adc_values[ADC_IR_Z]-autopilotCfg.z_off;
		total_ir=(abs(co_x)+abs(co_y)+abs(co_z))/3.0f;
//		total_ir=sqrt(co_x*co_x+co_y*co_y+co_z*co_z);
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

	ikarusInfo.Pitch=co_pitch*90.0f;		// -> grados
	ikarusInfo.Roll=co_roll*90.0f;
}

void Sensor_IMU()
{
	xdata long fp, fr, fc;
	
	fc=imu_ppm[0];
	fp=imu_ppm[1];
	fr=imu_ppm[2];

	ikarusInfo.Pitch=90*((750*fp/fc)-(500+750))/750.0f;
	ikarusInfo.Roll=180*((750*fr/fc)-(500+1500))/1500.0f;
}
#endif

void LowLevelAutoPilot() large
{
	float servo_pitch, servo_roll;

	// Estabilizacion del avion	
	aileronsPID.set_point=ikarusInfo.AutoPilot_roll;
	elevatorPID.set_point=ikarusInfo.AutoPilot_pitch;

	servo_pitch = ControlPID(&elevatorPID, ikarusInfo.Pitch);
	servo_roll = ControlPID(&aileronsPID, ikarusInfo.Roll);

	if(autopilotCfg.pid_pitch.rev)
		servo_pitch=-servo_pitch;

	if(autopilotCfg.pid_roll.rev)
		servo_roll=-servo_roll;	
	
	ikarusInfo.AutoPilot_ele=servo_pitch;
	ikarusInfo.AutoPilot_ail=servo_roll;
}


void TopLevelAutoPilot() large
{
	xdata float servo_tail, servo_roll, servo_motor, servo_pitch;

	if(gpsinfo.conected&&gpsinfo.pos_valid&&gpsinfo.nmea_ok)
	{

		tailPID.set_point=0.0f;      
		servo_tail = ControlPID(&tailPID,ikarusInfo.navigator_rel_bearing);

		servo_roll=-servo_tail*modem_ail; //autopilotCfg.rumbo_ail/tailPID.DriveLimit

		if(autopilotCfg.pid_tail.rev)
			servo_tail=-servo_tail;

		ikarusInfo.AutoPilot_tail=servo_tail;
		ikarusInfo.AutoPilot_roll=servo_roll;

		motorPID.set_point=ikarusInfo.navigator_altitude;
		servo_motor=ControlPID(&motorPID,getRelAltitude());

		servo_pitch=servo_motor*modem_ele; //autopilotCfg.altura_ele/motorPID.DriveLimit;

		if((gpsinfo.alt_filter-storedConfig.HomeAltitude)<20) // Por seguridad desactivar motor <20m
			servo_motor=-1.0f;

		if(autopilotCfg.pid_motor.rev)
			servo_motor=-servo_motor;

		ikarusInfo.AutoPilot_thr=servo_motor;
		ikarusInfo.AutoPilot_pitch=servo_pitch;
	}
	else
	{
		ikarusInfo.AutoPilot_tail=0.0f;
		ikarusInfo.AutoPilot_roll=0.0f;
		ikarusInfo.AutoPilot_thr=0.0f;
		ikarusInfo.AutoPilot_pitch=0.0f;
	}
}


