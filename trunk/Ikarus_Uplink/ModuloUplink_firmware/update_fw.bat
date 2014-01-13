oh51 ModuloUplink HEXFILE(FW_UPLINK.HEX)
hex2bin -e bin fw_uplink.hex
lfsr32 fw_uplink.bin fw_uplink.fw 120
copy fw_uplink.fw "..\UAVConsole - devel\UAVConsole\Resources"

