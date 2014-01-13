/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  IKARUS_OSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IKARUS_OSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using UAVConsole.IkarusScreens;
using System.Reflection;
using System.IO;

namespace UAVConsole.ConfigClasses
{
    class IkarusScreenConfig : GenericConfigClass
    {
        public ElementoOSD Altimetro;
        public ElementoOSD Autopilot;
        public ElementoOSD Bearing;
        public ElementoOSD Compas;
        public ElementoOSD Dist_Home;
        public ElementoOSD Dist_Wpt;
        public ElementoOSD Hora;
        public ElementoOSD HorizonteArtificial;
        public ElementoOSD I;
        public ElementoOSD mAh;
        public ElementoOSD Lat;
        public ElementoOSD Lon;
        public ElementoOSD NombreHUD;
        public ElementoOSD NombrePiloto;
        public ElementoOSD WptName;
        public ElementoOSD NumSats;
        public ElementoOSD PosAntena;
        public ElementoOSD PosAntenaV;
        public ElementoOSD Coste_km_Ah;
        public ElementoOSD RSSI;
        public ElementoOSD TasaPlaneo;
        public ElementoOSD TiempoVuelo;
        public ElementoOSD Variometro;
        public ElementoOSD Velocimetro;
        public ElementoOSD V1_text;
        public ElementoOSD V1_bar;
        public ElementoOSD V2_text;
        public ElementoOSD V2_bar;
        public ElementoOSD MaxAlt;
        public ElementoOSD MaxDist;
        public ElementoOSD MaxVelo;
        public ElementoOSD TotalDist;
        public ElementoOSD BadRX;
        public ElementoOSD Auxiliary;

        public string StrNombreHUD;

        public enum Instrumento
        {
            Altimetro,Autopilot, Bearing,Compas,Dist_Home, Dist_Wpt, Hora,
            HorizonteArtificial, I, mAh, Lat, Lon, NombreHUD, NombrePiloto, WptName, NumSats, PosAntena, 
            PosAntenaV, coste_km_Ah,RSSI,TasaPlaneo, TiempoVuelo, Variometro,Velocimetro,
            V1_text, V1_bar, V2_text, V2_bar, MaxAlt, MaxDist, MaxVelo, TotalDist, BadRX, Auxiliary 
        };

        public IkarusScreenConfig()
        {
            foreach (FieldInfo field in this.GetType().GetFields())
            {
                if (field.FieldType == typeof(ElementoOSD))
                    field.SetValue(this, new ElementoOSD());
                else
                    field.SetValue(this, "");
            }
            size_bytes();
        }

        public void Load(string FileName)
        {
            int caracter;
            int i;
            if (FileName.Length == 0)
                return;

            FileStream handle = File.OpenRead(FileName);

            caracter = handle.ReadByte();
            for (i = 0; i < getNumElements()-1 && caracter >= 0; i++)
            {
                ElementoOSD elem = (ElementoOSD)getElement(i);
                elem.tipo = (byte)caracter;
                elem.fila = (sbyte)handle.ReadByte();
                elem.col = (sbyte)handle.ReadByte();
                elem.param = (byte)handle.ReadByte();
                caracter = handle.ReadByte();
            }

            caracter = handle.ReadByte();

            for (i = 0; i < 16 && caracter >= 0; i++)
            {
                StrNombreHUD += (char)caracter;
                caracter = handle.ReadByte();
            }

            handle.Close();
        }

        public byte getByte(int idx)
        {
            int numelem = getNumElements() - 1;
            int dir = idx / 4;
            if (dir >= numelem)
            {
                dir = idx - numelem * 4;
                if (dir < StrNombreHUD.Length)
                    return (byte)StrNombreHUD[dir];
                else
                    return 0;
            }
            else
            {
                ElementoOSD elem = (ElementoOSD)getElement(dir);
                switch (idx % 4)
                {
                    case 0:
                        return elem.tipo;

                    case 1:
                        return (byte)elem.fila;

                    case 2:
                        return (byte)elem.col;

                    case 3:
                        return elem.param;

                    default:
                        return 0;

                }
            }
              
        }

        public void setByte(int idx, byte dato)
        {
            int numelem=getNumElements()-1;
            int dir = idx / 4;
            if (dir >= numelem)
            {
                dir = idx - numelem * 4;
                if (dir < 16)
                {
                    if (dir < StrNombreHUD.Length)
                        StrNombreHUD += (char)dato;
                    else
                        StrNombreHUD = StrNombreHUD.Substring(0, dir) + (char)dato;
                }
                return;
            }
            else
            {
                ElementoOSD elem = (ElementoOSD)getElement(dir);
                switch (idx % 4)
                {
                    case 0:
                        elem.tipo = dato;
                        break;
                    case 1:
                        elem.fila = (sbyte)dato;
                        break;
                    case 2:
                        elem.col = (sbyte)dato;
                        break;
                    case 3:
                        elem.param = dato;
                        break;
                }
            }
        }
    }
}
