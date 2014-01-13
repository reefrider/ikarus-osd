/////////////////////////////////////
//  Generated Initialization File  //
/////////////////////////////////////

#include "C8051F340.h"

// Peripheral specific initialization functions,
// Called from the Init_Device() function
void Reset_Sources_Init()
{
    int i = 0;
    VDM0CN    = 0x80;
    for (i = 0; i < 350; i++);  // Wait 100us for initialization
    RSTSRC    = 0x02;
}

void PCA_Init()
{
    PCA0CN    = 0x40;
    PCA0MD    &= ~0x40;
    PCA0MD    = 0x04;
}

void Timer_Init()
{
    TCON      = 0x55;
    TMOD      = 0x22;
    CKCON     = 0x34;
    TH0       = 0xD0;
    TH1       = 0x64;
    TMR2CN    = 0x0C;
    TMR2RLL   = 0x60;
    TMR2RLH   = 0x60;
    TMR2L     = 0x60;
    TMR2H     = 0x60;
    TMR3CN    = 0x04;
    TMR3RLL   = 0xC0;
    TMR3RLH   = 0x63;
    TMR3L     = 0xC0;
    TMR3H     = 0x63;
}

void ADC_Init()
{
    AMX0P     = 0x09;
    AMX0N     = 0x1F;
    ADC0CN    = 0x80;
}

void Comparator_Init()
{
    CPT0MD    = 0x00;
}

void Voltage_Reference_Init()
{
    REF0CN    = 0x0F;
}

void Port_IO_Init()
{
    // P0.0  -  Skipped,     Open-Drain, Digital
    // P0.1  -  Skipped,     Open-Drain, Digital
    // P0.2  -  Skipped,     Open-Drain, Digital
    // P0.3  -  Skipped,     Open-Drain, Digital
    // P0.4  -  TX0 (UART0), Push-Pull,  Digital
    // P0.5  -  RX0 (UART0), Open-Drain, Digital
    // P0.6  -  Skipped,     Open-Drain, Digital
    // P0.7  -  Skipped,     Open-Drain, Digital

    // P1.0  -  Skipped,     Open-Drain, Digital
    // P1.1  -  Skipped,     Open-Drain, Digital
    // P1.2  -  Skipped,     Open-Drain, Digital
    // P1.3  -  CEX0  (PCA), Push-Pull,  Digital
    // P1.4  -  CEX1  (PCA), Push-Pull,  Digital
    // P1.5  -  Skipped,     Open-Drain, Digital
    // P1.6  -  Skipped,     Open-Drain, Digital
    // P1.7  -  Skipped,     Open-Drain, Digital

    // P2.0  -  Unassigned,  Push-Pull,  Digital
    // P2.1  -  Skipped,     Open-Drain, Analog
    // P2.2  -  Unassigned,  Push-Pull,  Digital
    // P2.3  -  Unassigned,  Open-Drain, Digital
    // P2.4  -  Unassigned,  Open-Drain, Digital
    // P2.5  -  Unassigned,  Open-Drain, Digital
    // P2.6  -  Unassigned,  Open-Drain, Digital
    // P2.7  -  Unassigned,  Open-Drain, Digital

    // P3.0  -  Unassigned,  Open-Drain, Digital

    P2MDIN    = 0xFD;
    P0MDOUT   = 0x10;
    P1MDOUT   = 0x18;
    P2MDOUT   = 0x05;
    P0SKIP    = 0xCF;
    P1SKIP    = 0xE7;
    P2SKIP    = 0x02;
    XBR0      = 0x01;
    XBR1      = 0x42;
}

void Oscillator_Init()
{
    int i = 0;
    FLSCL     = 0x90;
    CLKMUL    = 0x80;
    for (i = 0; i < 20; i++);    // Wait 5us for initialization
    CLKMUL    |= 0xC0;
    while ((CLKMUL & 0x20) == 0);
    CLKSEL    = 0x03;
    OSCICN    = 0x83;
}

void Interrupts_Init()
{
    EIE1      = 0x98;
    IT01CF    = 0xFE;
    IE        = 0x80;
}

// Initialization function for device,
// Call Init_Device() from your main program
void Init_Device(void)
{
    Reset_Sources_Init();
    PCA_Init();
    Timer_Init();
    ADC_Init();
    Comparator_Init();
    Voltage_Reference_Init();
    Port_IO_Init();
    Oscillator_Init();
    Interrupts_Init();
}
