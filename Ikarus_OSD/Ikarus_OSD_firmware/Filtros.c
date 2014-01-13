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
#define FILTRO_SIZE	5

struct Pol_Z
{
	 int size;
     float znum[FILTRO_SIZE];
	 float zden[FILTRO_SIZE];
};

struct Filtro
{
	int size;
    float delay[FILTRO_SIZE];
	struct Pol_Z * pol;
};


void FIR_init(struct Filtro * filtro, struct Pol_Z * pol, float init_val) large
{
	int i;
    filtro->pol = pol;
    filtro->size = pol->size;
	
	for (i = 0; i < pol->size; i++)
    {
        filtro->delay[i] = init_val;
    }
}

void IIR_init(struct Filtro * filtro, struct Pol_Z * pol, float init_val) large
{
	int i;
	float n=0,d=0;
	filtro->pol = pol;
    filtro->size = pol->size;

    for (i = 0; i < pol->size; i++)
    {
   		n+=pol->znum[i];
		d+=pol->zden[i];
    }
	for (i = 0; i < pol->size; i++)
    {
	    filtro->delay[i] = init_val*(1.0f+d/n);
	}
}

float FIR_calc(struct Filtro filtro, float invar) large
{
    float sumnum = 0.0f;
	int i;

    for (i = 0; i < filtro.size - 1; i++)
    {
        filtro.delay[i] = filtro.delay[i + 1];
        sumnum += filtro.delay[i] * filtro.pol->znum[i];
    }
    filtro.delay[filtro.size - 1] = invar;
    sumnum += filtro.delay[filtro.size - 1] * filtro.pol->znum[filtro.size - 1];
    return sumnum;
}

float IIR_calc(struct Filtro filtro, float invar) large
{
    float sumnum = 0.0f;
	float sumden = 0.0f;
	int i;

    for (i = 0; i < filtro.size - 1; i++)
    {
        filtro.delay[i] = filtro.delay[i + 1];
        sumnum += filtro.delay[i] * filtro.pol->znum[i];
		sumden += filtro.delay[i] * filtro.pol->zden[i];
    }
    filtro.delay[filtro.size - 1] = invar-sumden;
    sumnum += filtro.delay[filtro.size - 1] * filtro.pol->znum[filtro.size - 1];
    return sumnum;
}