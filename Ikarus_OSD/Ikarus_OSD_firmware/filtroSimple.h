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

#define NUMVALS 4



struct FiltroSimple
{
	int valores[NUMVALS];
	int salida;
};
struct FiltroSimpleF
{
	float valores[NUMVALS];
	float salida;
};

struct FiltroIgualC
{
	unsigned char valores[NUMVALS];
	unsigned char salida;

};

void init_filtroSimple(struct FiltroSimple *f, int valor) large;
int filtroSimple(struct FiltroSimple *f, int valor) large;

void init_filtroSimpleF(struct FiltroSimpleF *f, float valor) large;
float filtroSimpleF(struct FiltroSimpleF *f, float valor) large;

void init_filtroIgual(struct FiltroIgualC *f, unsigned char valor) large;
char filtroIgual(struct FiltroIgualC *f, unsigned char valor) large;
