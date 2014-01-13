#pragma src

/* 
 * (c) 2009 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of FPVOSD.
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
 *  along with FPVOSD.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "c8051f340.h"
#include "ParserNMEA.h"

struct Telemetry_PKT pkt;

sbit TELEMETRY = P1^0;


/*
void SendTelemetry()
{
	#pragma asm
		
		CLR EA
	
		MOV R1, #070
w_ini:	DJNZ R1, w_ini


		; Modo push-pull

		SETB TELEMETRY
		MOV  	A,#LOW (pkt)
		MOV  	DPL,A
		MOV  	A,#HIGH (pkt)
		MOV  	DPH,A

		MOVX 	A,@DPTR
		MOV	 R1, A
		MOV  R2, #8

		ORL  	P1MDOUT,#01H		;01h
ST2:	SETB 	TELEMETRY		;; bit de start a 1
		NOP
		NOP
		NOP
		NOP
	;	NOP



ST1:	RRC  	A			;; RLC for bigendian, RRC for litleendian
		JC salto
		CLR TELEMETRY		;; Si el siguiente es 0, bajamos TELEMETRY un poco antes
salto:	MOV 	TELEMETRY,C			;//
		DJNZ R2, ST1
		NOP
		NOP
		
		NOP
		NOP
		NOP
		NOP
		;NOP
		MOV  R2, #8
		
		INC DPTR
		
		MOVX  	A,@DPTR
		CLR TELEMETRY		;; Bit de stop a 0
	
		DJNZ R1, ST2

		; Alta impedancia (OC+1)
		SETB  	TELEMETRY
		ANL P1MDOUT, #0FEH
		SETB EA

	#pragma endasm
}/* */

/*
void SendTelemetry()
{
	#pragma asm
		
		CLR EA
	
		MOV R1, #090
w_ini:	DJNZ R1, w_ini


		; Modo push-pull

		SETB TELEMETRY
		MOV  	A,#LOW (pkt)
		MOV  	DPL,A
		MOV  	A,#HIGH (pkt)
		MOV  	DPH,A

		MOVX 	A,@DPTR
		MOV	 R1, A
		MOV  R2, #8

		ORL  	P1MDOUT,#01H		;01h

ST1:	CPL TELEMETRY
		NOP
		NOP
		NOP
		;NOP
ST2:	RRC  	A			;; RLC for bigendian, RRC for litleendian
		MOV 	TELEMETRY,C			;//
		DJNZ R2, ST1
		MOV  R2, #8
		CPL TELEMETRY
		INC DPTR
		MOVX  	A,@DPTR
		DJNZ R1, ST2

		; Alta impedancia (OC+1)
		SETB  	TELEMETRY
		ANL P1MDOUT, #0FEH
		SETB EA

	#pragma endasm
}
/* */

