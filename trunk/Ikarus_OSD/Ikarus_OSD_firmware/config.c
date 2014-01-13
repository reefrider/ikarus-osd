/////////////////////////////////////
//  Generated Initialization File  //
/////////////////////////////////////

#include "C8051F340.h"

// Peripheral specific initialization functions,
// Called from the Init_Device() function
void PCA_Init()
{
    PCA0CN    = 0x40;
    PCA0MD    &= ~0x40;
    PCA0MD    = 0x04;
    PCA0CPM0  = 0x31;
    PCA0CPL4  = 0x00;
    PCA0MD    |= 0x20;
}

void Timer_Init()
{
    TCON      = 0x55;
    TMOD      = 0x22;
    CKCON     = 0x05;
    TH0       = 0xD0;
    TH1       = 0x64;
    TMR2CN    = 0x04;
    TMR2RLL   = 0xC0;
    TMR2RLH   = 0x63;
    TMR2L     = 0xC0;
    TMR2H     = 0x63;
    TMR3CN    = 0x04;
    TMR3RLL   = 0xC0;
    TMR3RLH   = 0x63;
    TMR3L     = 0xC0;
    TMR3H     = 0x63;
}

void UART_Init()
{
    SCON0     = 0x10;
}

void SPI_Init()
{
    SPI0CFG   = 0x40;
    SPI0CN    = 0x01;
    SPI0CKR   = 0x02;
}

void ADC_Init()
{
    AMX0P     = 0x1E;
    AMX0N     = 0x1F;
    ADC0CN    = 0x80;
}

void Voltage_Reference_Init()
{
    REF0CN    = 0x0F;
}

void Port_IO_Init()
{
    // P0.0  -  SCK  (SPI0), Push-Pull,  Digital
    // P0.1  -  MISO (SPI0), Open-Drain, Digital
    // P0.2  -  MOSI (SPI0), Push-Pull,  Digital
    // P0.3  -  Skipped,     Push-Pull,  Digital
    // P0.4  -  TX0 (UART0), Push-Pull,  Digital
    // P0.5  -  RX0 (UART0), Open-Drain, Digital
    // P0.6  -  Skipped,     Open-Drain, Digital
    // P0.7  -  Skipped,     Open-Drain, Digital

    // P1.0  -  Skipped,     Open-Drain, Digital
    // P1.1  -  Skipped,     Open-Drain, Analog
    // P1.2  -  Skipped,     Open-Drain, Analog
    // P1.3  -  Skipped,     Open-Drain, Analog
    // P1.4  -  CEX0  (PCA), Open-Drain, Digital
    // P1.5  -  Skipped,     Open-Drain, Digital
    // P1.6  -  Skipped,     Open-Drain, Analog
    // P1.7  -  Skipped,     Push-Pull,  Digital

    // P2.0  -  Unassigned,  Open-Drain, Digital
    // P2.1  -  Unassigned,  Open-Drain, Digital
    // P2.2  -  Unassigned,  Open-Drain, Digital
    // P2.3  -  Unassigned,  Open-Drain, Digital
    // P2.4  -  Unassigned,  Open-Drain, Digital
    // P2.5  -  Unassigned,  Open-Drain, Digital
    // P2.6  -  Skipped,     Open-Drain, Analog
    // P2.7  -  Skipped,     Open-Drain, Analog

    // P3.0  -  Skipped,     Open-Drain, Analog

    P1MDIN    = 0xB1;
    P2MDIN    = 0x3F;
    P3MDIN    = 0xFE;
    P0MDOUT   = 0x1D;
    P1MDOUT   = 0x80;
    P0SKIP    = 0xC8;
    P1SKIP    = 0xEF;
    P2SKIP    = 0xC0;
    P3SKIP    = 0x01;
    XBR0      = 0x03;
    XBR1      = 0x41;
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
    IP        = 0x05;
    EIE1      = 0x18;
    IT01CF    = 0xFE;
    IE        = 0xB5;
}

// Initialization function for device,
// Call Init_Device() from your main program
void Init_Device(void)
{
    PCA_Init();
    Timer_Init();
    UART_Init();
    SPI_Init();
    ADC_Init();
    Voltage_Reference_Init();
    Port_IO_Init();
    Oscillator_Init();
    Interrupts_Init();
}
