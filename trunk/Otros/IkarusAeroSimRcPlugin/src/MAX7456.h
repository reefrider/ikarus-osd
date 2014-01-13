//-----------------------------------------------------------------------------
// MAX7456.h
//
// The Integrated Circuit MAX7456 is a monochrome on-screen display (OSD) generator.
//
// Key Features implemented in this emulation:
//   256 User-Defined Characters or Pictographs in Integrated EEPROM
//   12 x 18 Pixel Character Size
//   Displays 16 Rows x 30 Characters
//
// Datasheet can be downloaded from 
//   http://datasheets.maxim-ic.com/en/ds/MAX7456.pdf
//
//-----------------------------------------------------------------------------
#ifndef _MAX7456_h_
#define _MAX7456_h_

#define MAX7456_DISPLAY_CHAR_WIDTH  30
#define MAX7456_DISPLAY_CHAR_HEIGHT 16

#define MAX7456_OSD_PIXEL_WIDTH   360
#define MAX7456_OSD_PIXEL_HEIGHT  288

// Load a Character Memory file that contains the pixel colors for each character
void MAX7456_LoadCharacterMemory(const char *strFileName);

// Clear the Display Memory
void MAX7456_ClearDisplayMemory();
void OSD_Clear(unsigned char *pOSD_VideoBuffer);

// Write a character at given coordinates in the Display Memory
void MAX7456_DisplayMemoryWrite(unsigned int x, unsigned int y, unsigned char c);

// Write the Video Buffer with the contents of the Display Memory
void MAX7456_Render(unsigned char *pOSD_VideoBuffer, float fTimeStep);


#endif
