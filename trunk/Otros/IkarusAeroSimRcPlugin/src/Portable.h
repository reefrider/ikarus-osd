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


/*
 *	Lista de los ya portados:
 *
 *		Autopilot
 *		Controladores
 *		HUDs
 *		LibraryMAX7456
 *		Modem
 *		Navigation
 *		PID
 *		Utils
 *
 *  Lista de archivos que no merece la pena portar
 *
 *		Ikarus	
 *		MAX7456
 *		MenuConfig
 *		ParserNMEA
 *		ReadADC (eliminado)
 *		Servos
 */

#ifndef __PORTABLE_H
#define __PORTABLE_H

#ifndef __KEIL__

#define SIMULADOR

#define large
#define xdata
#define code
#define data

#define bit bool

#endif


#endif