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

#include "Utils.h"
#include "Uplink.h"
#include "Modem.h"


xdata struct Packet slots[MAX_MODEM_SLOTS];
xdata struct Sequence seq;

unsigned char slot_idx;
unsigned char modem_idx;
unsigned int modem_servo=1500;

void modem_init()
{
	slot_idx=0;
	modem_idx=0;
	modem_servo=1500;
	seq.clk_div=1;
	seq.len=0;
	seq.modem_ch=-1;
}

unsigned int modem_get() 
{
	return modem_servo;
}

unsigned int modem_update() large
{
	if(seq.len==0)
		modem_servo=1500;
	else
	{
		modem_servo=slots[seq.seq[slot_idx]].pkt[modem_idx];

		modem_idx++;
		if(modem_idx>=slots[seq.seq[slot_idx]].len)		// IDLE
		{
			modem_idx=0;

			slot_idx++;
			if(slot_idx>=seq.len)
				slot_idx=0;
		}
	}
	return modem_servo;
}
