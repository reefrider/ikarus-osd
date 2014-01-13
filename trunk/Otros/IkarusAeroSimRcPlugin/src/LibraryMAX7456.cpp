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
#include <string.h>
#include <Math.h>

#include "LibraryMAX7456.h"
#include "ikarus.h"
#include "huds.h"
#include "MAX7456.h"

#define PI			3.14159265
#define DEG2RAD(a)	((a)*PI/180.0)
#define MAXCOLS 30
#define MAXROWS 16

extern int scanline;
extern code struct StoredConfig storedConfig;

#ifndef SIMULADOR
void InitMAX7456()
{
	// Set reset bit	
	sendValue(0x00, 0x02);
	while (readValue(0xA0)&0x40);
	
	AutoDetectNTSC_PAL();
	if(storedConfig.Video_PAL==-1)	// EEPROM BORRADA?
		{
		sendValue(0x2,42);		//hor offset
		sendValue(0x3,21); 		//ver offset
		}
	else
		{
		sendValue(0x2,storedConfig.offsetX);		//hor offset
		sendValue(0x3,storedConfig.offsetY); 		//ver offset
		}
}

char AutoDetectNTSC_PAL()
{
	xdata char old_v=-1;
	char st, r;
	old_v=(storedConfig.Video_PAL==0)?0x08:0x48;
	st=readValue(0xA0);

	r=readValue(0x80)&0xBF;
	if(st&0x04)	// No video
		sendValue(0x0,r|old_v);		// Por defecto
	else if(st&0x02)	// NTSC
		{
		sendValue(0x0,r|0x08);
		old_v=0;
		}
	else if(st&0x01)	// PAL
		{
		sendValue(0x0,r|0x48);		//0x48			PAL & OSD on, 0x08 NTSC & OSD on	
		old_v=0x40;
		}
	return (st&0x03);

}

void ClrScr()
{
	CharAttrNoBlink();
	sendValue(0x4,readValue(0x84)|0x4);	
	printAtChr(0,0,' ');		// A ver si lo arregla
}

void writeAtChr(int fila, int col, char c)
{
	xdata int dir;
	xdata int filas;
	if(storedConfig.Video_PAL==1)
		{
		filas=16;
		while(scanline>20&&scanline<290); //250 NTSC
		}
	else if(storedConfig.Video_PAL==2)
		{
		filas=15;
		while(scanline>20&&scanline<290); //250 NTSC
		}
	else
		{
		filas=13;
		while(scanline>16&&scanline<250); //250 NTSC
		}

	if(fila<0)
		fila=filas+fila;
	
	if(fila>filas)
		return;

	dir=fila*30+col;
	sendValue(0x5,(dir>>8)&1);
	sendValue(0x6,dir&0xff);	//dir
	sendValue(0x7,c);	//car
}

#else
void ClrScr()
{
	MAX7456_ClearDisplayMemory();
}

void writeAtChr(int fila, int col, char c)
{
	int dir;
	int filas;
	
	filas=16;

	if(fila<0)
		fila=filas+fila;
	
	if(fila>filas)
		return;

	MAX7456_DisplayMemoryWrite(col, fila,c);
}
#endif

void Bar(int fila, int col, unsigned char width, int v) large
{
	xdata int i,j;
	xdata int steps=(width-2)*4;
	xdata char c;

	if(v<0)
		v=0;
	else if(v>255)
		v=255;

	j=steps*v/255;

	writeAtChr(fila, col, 0xF7);
	
	for(i=1;i<width-1;i++)
	{
		if (j>3)
			c=0xF8;
		else if(j>2)
			c=0xF9;
		else if(j>1)
			c=0xF0;
		else if(j>0)
			c=0xF1;
		else
			c=0xF2;

		if(j>0)
			j-=4;

		writeAtChr(fila, col+i, c);
	}

	writeAtChr(fila, col+i, 0xF3);
}

void HorizonteArtificial(int fila, int col, int width, float pitch, float roll, char mode) large
{
	#define MAX_WIDTH 5
	static int oldy[2*MAX_WIDTH+1];

   // 12 es el ancho del caracter, 18 el alto.
    int maxx;
    int maxy;
	int i, c, pos;
	float aux;

	if(width >MAX_WIDTH)
		width = MAX_WIDTH;

	maxx = width;
	maxy = 12 * width + 12 / 2;

    for (i = -maxx; i <= maxx; i++)
    {
		if((mode==0)||(i==maxx||i==-maxx))
	    {
	        float y = (float)(12*i * tan(DEG2RAD(roll)));
	        y -= (float)(sin(DEG2RAD(pitch)) * maxy);
        
	        if (y > maxy)
	            y = maxy;
	        else if (y < -maxy)
	            y = -maxy;

	        aux = y + 18 / 2;
	        pos = (int)floor(aux / 18);        // No vale con hacer casting. Floor da la vida
	        c = (int)((aux - (18 * pos)) / 3);
		
		    if (oldy[maxx + i] != pos)
	        {
	            writeAtChr(fila - oldy[maxx + i], col + i, 0);
	            oldy[maxx + i] = pos;
	        }
		
			writeAtChr(fila - pos, col + i, 0xFA + c);
		}
    }
	
    writeAtChr(fila, col + maxx + 1, 0xFA + 2);
    writeAtChr(fila, col - maxx - 1, 0xFA + 2);
}

void COMPAS_grp(int fila, int col, float valor) large
{
	xdata int v;
	xdata unsigned char ch;

	valor=valor+90.0f-180.0f/16.0f;
	while(valor<0)
		valor+=360.0f;
	while(valor>=360.0f)
		valor-=360.0f;
	
	valor=valor*16.0/360.0;
	v=(int)valor;

	writeAtChr(fila,col,0x90);
	writeAtChr(fila,col+3,0x91);
	writeAtChr(fila+1,col,0x92);
	writeAtChr(fila+1,col+3,0x93);

	if(v<8)
		ch=0x50+0x0E-2*v;
	else
		ch=0x70+0x0E-2*(v-8);
	writeAtChr(fila,col+1,ch);
	writeAtChr(fila,col+2,ch+1);
	writeAtChr(fila+1,col+1,ch+0x10);
	writeAtChr(fila+1,col+2,ch+0x11);
}

void COMPAS_chr(int fila, int col,float valor)large
{
	valor=valor+45.0f-180.0f/8.0f;
	while(valor<0)
		valor+=360.0f;
	while(valor>=360.0f)
		valor-=360.0f;
	
	valor=valor*8.0/360.0;

	writeAtChr(fila,col,0xB0+2*((int)valor));
	writeAtChr(fila,col+1,0xB1+2*((int)valor));

}

void Compas(int fila, int col, int width, float heading, float bearing)large
{
	xdata int i;
	xdata int center;
	xdata int steps=4;
	xdata int entero,resto, entero2;
	xdata float t;
	
	t=bearing-heading;
	while(t>180.0)
		t-=360.0;
	while(t<-180.0)
		t+=360.0;
	
	if (t>0)
	{
		writeAtChr(fila,col+width-1,CH_ROWR);
		writeAtChr(fila,col, 0x00);

	}
	else
	{
		writeAtChr(fila,col, CH_ROWL);
		writeAtChr(fila,col+width-1,0x00);
	}		
	width-=2;
	col+=1;

	center=width/2;
	heading=-heading+center*4*360/ 192.0f;
	
	while(heading<0)
		heading+=360.0f;
	while(heading>=360.0f)
		heading-=360.0f;
	heading=heading*192.0/360.0;

	bearing=-bearing;

	if (bearing<0)
		bearing+=360.0f;
	else if (bearing >=360.0f)
		bearing-=360.0f;
	bearing=bearing*192.0/360.0;
	entero2=(int)bearing;
	entero2=entero2/steps;

	entero=heading/steps;
	resto=(int)heading%steps;
	
	for(i=0;i<width;i++)
	{
		xdata int ie=i-entero;
		xdata int ie2=i-entero+entero2;
		if(ie<0)
			ie+=48;
		if(ie2<0)
			ie2+=48;
		else if(ie2>=48)
			ie2-=48;


		printAtStr(fila+1,col+i," ");

		if(ie2%48==0)
			writeAtChr(fila+1, col+i, CH_ROWU);
		else if((ie)%12==0)
		{
			int t=ie/12;
			t=t%4;
			if(t==0)
				printAtStr(fila+1,col+i,"N");
			else if(t==1)
				printAtStr(fila+1,col+i,"E");
			else if(t==2)
				printAtStr(fila+1,col+i,"S");
			else if(t==3)
				printAtStr(fila+1,col+i,"W");
		}
		
		if((ie)%3==0)
		{
			switch(resto)
			{
				case 0:
					writeAtChr(fila,col+i,0xD2);
					break;
				case 1:
					writeAtChr(fila,col+i,0xD4);
					break;
				case 2:
					writeAtChr(fila,col+i,0xD3);
					break;
				case 3:
					writeAtChr(fila,col+i,0xD5);
					break;
			}
		}
		else
		{
			if(resto==0||resto==2)
				writeAtChr(fila, col+i, 0xD0);
			else
				writeAtChr(fila, col+i, 0xD1);

		}
	}
}

void Velocimetro(int fila, int col, int height, float valor)large
{
	xdata int i;
	xdata int steps=6;
	xdata int entero,resto;
		

	valor=(valor+1)*12.0/5;
	entero=valor/steps;
	resto=(int)valor%steps;
	
	if(resto<0)
		resto=6+resto;

	for(i=0;i<height;i++)
	{
		int ie=i-entero;
		
		if((ie)%2==0)
		{
			switch(resto)
			{
				case 0:
					writeAtChr(fila+i,col,0xC2);
					break;
				case 1:
					writeAtChr(fila+i,col,0xC5);
					break;
				case 2:
					writeAtChr(fila+i,col,0xC3);
					break;
				case 3:
					writeAtChr(fila+i,col,0xC6);
					break;
				case 4:
					writeAtChr(fila+i,col,0xC4);
					break;
				case 5:
					writeAtChr(fila+i,col,0xC7);
					break;

			}
		}
		else
		{
			if(resto==0||resto==2||resto==4)
				writeAtChr(fila+i, col, 0xC0);
			else
				writeAtChr(fila+i, col, 0xC1);

		}
	}
}

void Altimetro(int fila, int col, int height, float valor)large
{
	xdata int i;
	xdata int steps=6;
	xdata int entero,resto;
		

	valor=(valor+1)*12.0/5;
	entero=valor/steps;
	resto=(int)valor%steps;

	if(resto<0)
		resto=6+resto;
	
	for(i=0;i<height;i++)
	{
		int ie=i-entero;
		
		if((ie)%2==0)
		{
			switch(resto)
			{
				case 0:
					writeAtChr(fila+i,col,0xCA);
					break;
				case 1:
					writeAtChr(fila+i,col,0xCD);
					break;
				case 2:
					writeAtChr(fila+i,col,0xCB);
					break;
				case 3:
					writeAtChr(fila+i,col,0xCE);
					break;
				case 4:
					writeAtChr(fila+i,col,0xCC);
					break;
				case 5:
					writeAtChr(fila+i,col,0xCF);
					break;

			}
		}
		else
		{
			if(resto==0||resto==2||resto==4)
				writeAtChr(fila+i, col, 0xC8);
			else
				writeAtChr(fila+i, col, 0xC9);

		}
	}
}


void Variometro1( int fila, int col, int valor)large
{
	xdata int valores=7;
	xdata int rango=256/valores;
	xdata int result;
	code const char var1[]={0x9E,0x9D,0x9C,0x98,0x99,0x9A,0x9B};

	if(valor<-3)
		valor=-3;
	else if(valor>3)
		valor=3;
	
	result=valor;
	writeAtChr(fila, col, var1[result+3]);
}

void Variometro2( int fila1, int fila2, int col, int valor)large
{
	xdata int valores=16;
	xdata int rango=256/valores;
	xdata int result;

	if(valor<-7)
		valor=-7;
	else if(valor>7)
		valor=7;
	
	result=valor;///rango;
	if(result<0)
	{
		writeAtChr(fila1, col, 0xA0);
		writeAtChr(fila2, col, 0xAF+result);
	}
	else
	{
		writeAtChr(fila1, col, 0xA0+result);
		writeAtChr(fila2, col, 0xAF);
	}

}



/*
void writeAtStr(int fila, int col, char cad[])
{
	while(*cad!=0)
	{
		writeAtChr(fila,col,*cad);
		col++;
		cad++;
	}
}
*/

void printAtChr(int fila, int col, char c)
{
	xdata char t;
	
	if(c=='0')
		t=0x0a;
	else if(c>='1'&&c<='9')
		t=c-'1'+1;
	else if(c>='A'&&c<='Z')
		t=c-'A'+0x0B;
	else if(c>='a'&&c<='z')
		t=c-'a'+0x25;
	else if (c=='(')
		t=0x3f;
	else if (c==')')
		t=0x40;
	else if (c=='.')
		t=0x41;
	else if (c=='?')
		t=0x42;
	else if (c==';')
		t=0x43;
	else if (c==':')
		t=0x44;
	else if (c==',')
		t=0x45;
	else if (c=='\'')
		t=0x46;
	else if (c=='/')
		t=0x47;
	else if (c=='\"')
		t=0x48;
	else if (c=='-')
		t=0x49;
	else if (c=='<')
		t=0x4A;
	else if (c=='>')
		t=0x4B;
	else if (c=='@')
		t=0x4C;
	else if (c=='=')
		t=0x4D;
	else if (c=='Ñ')
		t=0x4E;
	else if (c=='ñ')
		t=0x4F;

	else
		t=0;
	writeAtChr(fila,col,t);
}

void printAtStr(int fila, int col, char cad[])
{
	while(*cad!=0)
	{
		printAtChr(fila,col,*cad);
		col++;
		cad++;
	}	
}

void printAtStr2(int fila, int col, char cad[], char len)
{
	int i;
	for(i=0;i<len&&cad[i]!=0;i++)
		printAtChr(fila, col++, cad[i]);
}


void printCenteredAtStr(int fila, char cad[])
{
	printAtStr(fila,(MAXCOLS-strlen(cad))/2,cad);
}

