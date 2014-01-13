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



void wait_for_ms(int t);

float Adjust180(float angle);
float Adjust360(float angle);

long get_timesecs(long f)large;
char get_secs(long f)large;
char get_mins(long f)large;
char get_hora(long f)large;
char secs2hora(long f) large;
char secs2mins(long f) large;
char secs2secs(long f) large;

void flash_clear(char volatile * pos) large;
void flash_write(char volatile * pos, char dato) large;
char flash_read(char volatile * pos) large;
void ram2flash(char volatile *dst, char *src, int len) large;
void flash2ram(char *dst, char volatile *src, int len) large;


void AutoUpdateSerie();

void flush_rom();
void write_rom(char volatile *dst, char dato);
char read_rom(char volatile *dst);
void ram2rom(char volatile *dst, char *src, int len) large;
void rom2ram(char *dst, char volatile *src, int len) large;
void ram2ram(char *dst, char *src, int len) large;


