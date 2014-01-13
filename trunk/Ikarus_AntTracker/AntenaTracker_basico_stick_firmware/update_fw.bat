oh51 AntenaTracker HEXFILE(FW_ANTTRACK.HEX)
hex2bin -e bin fw_anttrack.hex
lfsr32 fw_anttrack.bin fw_anttrack.fw 56
copy fw_anttrack.fw "..\..\UAVConsole - devel\UAVConsole\Resources"

