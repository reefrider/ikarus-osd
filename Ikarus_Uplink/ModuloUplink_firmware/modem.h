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

#define MAX_MODEM_SLOTS	16
#define MAX_MODEM_LEN	32
#define MAX_SEQ_SIZE	32

struct Packet
{
	unsigned char len;
	unsigned int pkt[MAX_MODEM_LEN];
};

struct Sequence
{
	char modem_ch;
	unsigned char clk_div;
	unsigned char len;
	unsigned char seq[MAX_SEQ_SIZE];

};

void modem_init();
unsigned int modem_get();
unsigned int modem_update() large;
