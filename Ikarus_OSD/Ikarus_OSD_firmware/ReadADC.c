/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  FPVOSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FPVOSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <intrins.h>
#include "c8051f340.h"
#include "ParserNMEA.h"
#include "Ikarus.h"
#include "FiltroSimple.h"

#define CHANNELS 8

#define DT_ADC 0.0099f//0.01f		// 10ms

								// V2, I, V1, RSSI, temp , Co_X, Co_Y, Co_Z	
xdata float adc_values[CHANNELS]={0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
xdata int adc_indice=0;

							// V2, I, V1, RSSI, temp , Co_X, Co_Y, Co_Z	
code unsigned char adc_p[CHANNELS]={0x01, 0x02, 0x03,0x06,0x1e, 0x0E, 0x0F, 0x10};
code unsigned char adc_n[CHANNELS]={0x1f, 0x1f,0x1f,0x1f,0x1f, 0x1f, 0x1f, 0x1f};
code unsigned char lp_filter[CHANNELS]={32,64,32,64,128, 2, 2, 2};
 

extern xdata struct IkarusInfo ikarusInfo;
extern code struct StoredConfig storedConfig;

void ADC0_handle() interrupt 10
{
	xdata int valor16;
	xdata float valorf;

	//valor16=ADC0H;
	//valor16=(valor16<<8)|ADC0L;
	valor16=ADC0;
	valorf=valor16*3.3f/1023.0f;
	valorf=((lp_filter[adc_indice]-1)*adc_values[adc_indice]+valorf)/lp_filter[adc_indice];

	adc_values[adc_indice]=valorf;


	switch(adc_indice)
	{
		case ADC_V2:
				ikarusInfo.v2=valorf*storedConfig.gain_sensorV2 - storedConfig.offset_sensorV2;//(14+4.7)/4.7;
				break;

		case ADC_I:
				valorf=(valorf-storedConfig.offset_sensorI)*storedConfig.gain_sensorI;
				
				if( _chkfloat_(valorf)>1)
					valorf=0.0f;
				else if(valorf<0)
					valorf=0;
				
				ikarusInfo.currI=valorf;
				ikarusInfo.consumidos_mAh+=valorf*DT_ADC/3.6f; // *1000mA/1A * 1h/3600s
				
				break;

		case ADC_V1:
				ikarusInfo.v1=valorf*storedConfig.gain_sensorV1 - storedConfig.offset_sensorV1;//(33+4.7)/4.7;
				break;

		case ADC_RSSI:
				valorf=(valorf-storedConfig.min_rssi)/(storedConfig.max_rssi-storedConfig.min_rssi);
				if(valorf<0)
				  	valorf=0;
				else if(valorf>1)
					valorf=1;
				ikarusInfo.RSSI=100*valorf;	// Entre 0 y 100
				break;

		case ADC_temp: ikarusInfo.tempC=(1000.0f*valorf-776.0f)/2.86f;
				break;

		case ADC_IR_X:
				break;

		case ADC_IR_Y:
				break;

		case ADC_IR_Z:
				break;
	}

	adc_indice++;
	if(adc_indice>=CHANNELS)
		adc_indice=0;


	AMX0P=adc_p[adc_indice];
	AMX0N=adc_n[adc_indice];

	AD0INT=0;		// Clear interrupt flag
	if(adc_indice)
		AD0BUSY=1;		// Start new conversion
}