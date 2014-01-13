//-----------------------------------------------------------------------------
// MAX7456.cpp
//-----------------------------------------------------------------------------
#include "MAX7456.h"
#include <stdio.h>
#include <memory.h>
#include <assert.h>

#define VIDEO_BUFFER_PIXEL_WIDTH 512
#define MAX7456_NUM_CHARS_IN_FONT 256

#define MAX7456_CHAR_PIXEL_WIDTH   12
#define MAX7456_CHAR_PIXEL_HEIGHT  18
//-------------------------------------------------
// Character Memory
// Contains 256 characters
// Character size is 18x12 pixels
// 4 bytes per pixel: R G B A (A is transparency: 0=transparent, 255=opaque)
//-------------------------------------------------
unsigned char aucCharacterMemory[MAX7456_NUM_CHARS_IN_FONT][MAX7456_CHAR_PIXEL_HEIGHT][MAX7456_CHAR_PIXEL_WIDTH][4];

//-------------------------------------------------
// Display Memory
// 16 rows
// 30 columns
//-------------------------------------------------
unsigned char aucDisplayMemory[MAX7456_DISPLAY_CHAR_HEIGHT][MAX7456_DISPLAY_CHAR_WIDTH];


float s_fBlinkingTime = 0.2f;  // Character blink time in seconds


///------------------------------------------------
// Atributes

#define MAX7456_ATTRIB_BLINK	1

unsigned char currAttribute;
unsigned char aucDisplayMemoryAttribs[MAX7456_DISPLAY_CHAR_HEIGHT][MAX7456_DISPLAY_CHAR_WIDTH];

void CharAttrNoBlink()
{
	currAttribute&=~MAX7456_ATTRIB_BLINK;
}

void CharAttrBlink()
{
	currAttribute|=MAX7456_ATTRIB_BLINK;
}



////


//-----------------------------------------------------------------------------
//  MAX7456 CharacterMemory file .MCM is a text file like this:
//
//  MAX7456
//  01010101
//  01010100
//  00101000
//  00010101
//  01010010
//  10101010
//  10000101
//  01010010
//  10101010
//  10000101
//  ...
//  ...
//
//  Where:
//    00 is a black opaque pixel
//    10 is a white opaque pixel
//    x1 is a transparent pixel
//
//  Characters are 12x18 = 216 pixels
//    After 216 pixels thera are 01010101 to fill 256 pixels per character
//    Hence 256-216 pixels must be skipped
//
//  There are 256 characters in the file
//
//-----------------------------------------------------------------------------
void MAX7456_LoadCharacterMemory(const char *strFileName)
{
  // Clear Character Memory
  memset(aucCharacterMemory, 0, sizeof(aucCharacterMemory));

  FILE *pFile = fopen(strFileName, "rb");

  if(pFile)
  {
    bool bDone = false;
    int nPaddingPixelsSkipCount = 0;
    int nPixelsInCharCount = 0;
    unsigned char cBitFirst  = 0;
    unsigned char cBitSecond = 0;
    int n = 0;

    while(!bDone)
    {
      unsigned char c;
      int nCount = fread(&c, 1, 1, pFile);
      if(nCount == 0)
      {
        bDone = true;
        break;
      }
      else
      switch(c) // parse byte
      {
        case '1':
        case '0':
          if(cBitFirst == 0)
          {
            // Store first bit
            cBitFirst = c;
            break;
          }
          else
          if(cBitSecond == 0)
          {
            // Store second bit
            cBitSecond = c;
            
            if(nPaddingPixelsSkipCount)
            {
              // Padding pixels are transparent
              assert(cBitFirst  == '0');
              assert(cBitSecond == '1');
              nPaddingPixelsSkipCount--;
            }
            else
            {
              nPixelsInCharCount++;

              // Parse the pair of bits that have been read from file
              if(cBitSecond == '1')
              {
                // Transparent pixel
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
              }
              else
              if(cBitFirst == '1')
              {
                // White Opaque pixel
                ((unsigned char *)aucCharacterMemory)[n++] = 255; 
                ((unsigned char *)aucCharacterMemory)[n++] = 255; 
                ((unsigned char *)aucCharacterMemory)[n++] = 255; 
                ((unsigned char *)aucCharacterMemory)[n++] = 255; 
              }
              else
              if(cBitFirst == '0')
              {
                // Black Opaque pixel
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 0;
                ((unsigned char *)aucCharacterMemory)[n++] = 255;
              }
            }

            cBitFirst  = 0;
            cBitSecond = 0;
            
            // Skip padding pixels between 216 and 256
            if(nPixelsInCharCount == 216)
            {
              nPixelsInCharCount = 0;
              nPaddingPixelsSkipCount = 256 - 216;
            }
          }

          // Check if reached end of aucCharacterMemory
          if(n > sizeof(aucCharacterMemory) - 4)
          {
            bDone = true;
          }
          break;
      }
    }

    fclose(pFile);
  }
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
void MAX7456_ClearDisplayMemory()
{
  memset(aucDisplayMemory, 0, sizeof(aucDisplayMemory));
  memset(aucDisplayMemoryAttribs, 0, sizeof(aucDisplayMemoryAttribs));		// Atributos
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
void MAX7456_DisplayMemoryWrite(unsigned int x, unsigned int y, unsigned char c)
{
  if(x <= MAX7456_DISPLAY_CHAR_WIDTH && y <= MAX7456_DISPLAY_CHAR_HEIGHT)
  {
    aucDisplayMemory[y][x] = c;
	aucDisplayMemoryAttribs[y][x]=currAttribute;
  }
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
void OSD_Clear(unsigned char *pOSD_VideoBuffer)
{
  memset(pOSD_VideoBuffer, 0, 512*512*4);	// Creo que vale con MAX7456_ClearDisplayMemory+MAX7456_Render
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------

void MAX7456_Render(unsigned char *pOSD_VideoBuffer, float fTimeStep)
{
  static float sfTime = 0.0f;
  static bool sbBlinkStatus = false;

  sfTime += fTimeStep;
  if(sfTime > s_fBlinkingTime)
  {
    sfTime -= s_fBlinkingTime;
    sbBlinkStatus = !sbBlinkStatus;
  }

  for(int y=0; y<MAX7456_DISPLAY_CHAR_HEIGHT; y++)
  {
    for(int x=0; x<MAX7456_DISPLAY_CHAR_WIDTH; x++)
    {
      // Character to draw
      unsigned c      = aucDisplayMemory       [y][x];
      unsigned attrib = aucDisplayMemoryAttribs[y][x];

      if(sbBlinkStatus || !(attrib & MAX7456_ATTRIB_BLINK))
      {
        // Copy character rows to Video Buffer
        for(int i=0; i<MAX7456_CHAR_PIXEL_HEIGHT; i++)
        {
          // Copy row of pixels from CharMem to Video Buffer
          unsigned char *pVideoBuffer =
            pOSD_VideoBuffer +
            (x * MAX7456_CHAR_PIXEL_WIDTH * 4) + 
            (y * MAX7456_CHAR_PIXEL_HEIGHT + i) * (VIDEO_BUFFER_PIXEL_WIDTH * 4);

          unsigned char *pCharMem = &aucCharacterMemory[c][i][0][0];

          memcpy(pVideoBuffer, pCharMem, MAX7456_CHAR_PIXEL_WIDTH * 4);
        }
      }
    }
  }
}

/*
void MAX7456_Render(unsigned char *pOSD_VideoBuffer, float fTimeStep)
{
	static bool drawblink=false;
	drawblink=!drawblink;

  for(int y=0; y<MAX7456_DISPLAY_CHAR_HEIGHT; y++)
  {
    for(int x=0; x<MAX7456_DISPLAY_CHAR_WIDTH; x++)
    {
      // Character to draw
      unsigned c= aucDisplayMemory[y][x];
	  
      // Copy character rows to Video Buffer
      for(int i=0; i<MAX7456_CHAR_PIXEL_HEIGHT; i++)
      {
        // Copy row of pixels from CharMem to Video Buffer
        unsigned char *pVideoBuffer =
          pOSD_VideoBuffer +
          (x * MAX7456_CHAR_PIXEL_WIDTH * 4) + 
          (y * MAX7456_CHAR_PIXEL_HEIGHT + i) * (VIDEO_BUFFER_PIXEL_WIDTH * 4);

        unsigned char *pCharMem = &aucCharacterMemory[c][i][0][0];
 
		if((aucDisplayMemoryAttribs[y][x]&MAX7456_ATTRIB_BLINK)&&!drawblink)
			memset(pVideoBuffer,0,MAX7456_CHAR_PIXEL_WIDTH * 4);
		else
			memcpy(pVideoBuffer, pCharMem, MAX7456_CHAR_PIXEL_WIDTH * 4);
      }
    }
  }
}
*/




