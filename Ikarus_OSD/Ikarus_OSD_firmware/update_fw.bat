oh51 Ikarus_OSD HEXFILE(FIRMWARE.HEX)
hex2bin -e bin firmware.hex
lfsr32 firmware.bin firmware.fw 120
copy firmware.fw "..\..\UAVConsole - devel\UAVConsole\Resources"

