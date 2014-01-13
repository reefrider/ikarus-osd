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

#include "Portable.h"

void fb_refresh(unsigned char n);

void ClrScr();
void InitMAX7456();
char AutoDetectNTSC_PAL();

void CharColorWhite();
void CharColorBlack();
void CharAttrBlink();
void CharAttrNoBlink();

void WriteCharMemory(unsigned char c, unsigned char buff[64])large;
void WriteCharMemory2(unsigned char c, unsigned char len, unsigned char buff[64])large;
void ReadCharMemory(unsigned char c, unsigned char buff[64])large;

void writeAtChr(int fila, int col, char c);
void writeAtStr(int fila, int col, char cad[]);
void printAtChr(int fila, int col, char c);
void printAtStr(int fila, int col, char cad[]);
void printAtStr2(int fila, int col, char cad[], char len);
void printCenteredAtStr(int fila, char cad[]);

char sendByte(char byte);
void sendValue(unsigned char dir, unsigned char value);
void sendValue16(unsigned char dir, unsigned char msb, unsigned char lsb);
char readValue(unsigned char dir);

void Bar(int fila, int col, unsigned char width, int v)large;
void Variometro1( int fila, int col, int valor)large;
void Variometro2( int fila1, int fila2, int col, int valor)large;
void Compas(int fila, int col, int width, float heading, float bearing)large;
void Altimetro(int fila, int col, int height, float valor)large;
void Velocimetro(int fila, int col, int height, float valor)large;
void HorizonteArtificial(int fila, int col, int width, float pitch, float roll, char mode) large;

void COMPAS_grp(int fila, int col, float valor)large;
void COMPAS_chr(int fila, int col,float valor)large;


