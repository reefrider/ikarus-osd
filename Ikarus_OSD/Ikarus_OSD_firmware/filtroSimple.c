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

#include "filtroSimple.h"
#include "Ikarus.h"

void init_filtroSimpleF(struct FiltroSimpleF *f, float valor) large
{
	unsigned char i;
	for(i=0;i<NUMVALS;i++)
		f->valores[i]=valor;
	f->salida=valor;
}

float filtroSimpleF(struct FiltroSimpleF *f, float valor) large
{
	float max,min;
	float total=0;
	unsigned char i;
	float v;
	max=f->valores[1];
	min=f->valores[1];
	for(i=0;i<NUMVALS;i++)
	{
		if(i<NUMVALS-1)
			v=f->valores[i+1];
		else
			v=valor;
		if(v>max)
			max=v;
		else if(v<min)
			min=v;
		
		total+=v;
		f->valores[i]=v;
	}
	total-=max;
	total-=min;
	f->salida=total/2.0f;
	return f->salida;
}

/*
void init_filtroIgual(struct FiltroIgualC *f, unsigned char valor) large
{
	unsigned char i;
	for(i=0;i<NUMVALS;i++)
		f->valores[i]=valor;
	f->salida=valor;
}
*/
char filtroIgual(struct FiltroIgualC *f, unsigned char valor) large
{
	unsigned char i;
	char bandera_iguales=1;

	for(i=0;i<NUMVALS;i++)
	{
		if(i<NUMVALS-1)
			f->valores[i]=f->valores[i+1];
		else
			f->valores[i]=valor;

		if(f->valores[i]!=valor)
			bandera_iguales=0;
	}

	if(bandera_iguales)
		f->salida = valor;

	return f->salida;
}

/*
void init_filtroSimple(struct FiltroSimple *f, int valor) large
{
	unsigned char i;
	for(i=0;i<NUMVALS;i++)
		f->valores[i]=valor;
	f->salida=valor;
}

int filtroSimple(struct FiltroSimple *f, int valor) large
{
	int max,min;
	int total=0;
	unsigned char i;
	float v;
	max=f->valores[1];
	min=f->valores[1];
	for(i=0;i<NUMVALS;i++)
	{
		if(i<NUMVALS-1)
			v=f->valores[i+1];
		else
			v=valor;
		if(v>max)
			max=v;
		else if(v<min)
			min=v;
		
		total+=v;
		f->valores[i]=v;
	}
	total-=max;
	total-=min;
	f->salida=total/2;
	return f->salida;
}
*/