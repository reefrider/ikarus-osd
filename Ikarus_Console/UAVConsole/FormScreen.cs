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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UAVConsole.ConfigClasses;
using UAVConsole.IkarusScreens;
using DirectX.Capture;
using System.Threading;
using System.IO;
using UAVConsole.USBXpress;

namespace UAVConsole
{
    public partial class FormScreen : Form
    {
        Capture capture;
        Singleton me = Singleton.GetInstance();
        int Selected_Item = -1;
        IkarusScreenConfig hud = new IkarusScreenConfig();

        //
        float pitch = 10.0f;
        float roll = 30.0f;
        int rumbo = 0;
        int verticalSpeed = 10;
        int bearing = 30;
        float v1 = 11.3f;
        int cells_v1 = 3;
        float v2 = 8.22f;
        int cells_v2 = 2;
        int mAh = 2100;
        int cap_mAh = 4000;
        int kmph = 45;
        float IA = 3.2f;
        bool AutoPilot = true;
        string WptName = "HOME";
        int distancia_casa = 1200;
        int distancia_wpt = 520;
        int altitud = -100;
        int NumSats = 9;
        int RSSI = 13;
        float lon = -6.12345f;
        float lat = 37.23452f;
        int max_velo = 80;
        int max_alt = 12345;
        int max_dist = 1234;
        int total_dist = 2345;
        //

        public FormScreen()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            comboBoxShowSc.SelectedIndex = 0;
            comboBox1.SelectedIndex = (int)me.videosystem;
            comboBox1.Enabled = true;
            //pictureBox1.Width = 29 * 12 + 4;
            //pictureBox1.Height = 17 * 18 + 4;
            comboBoxScreenSlot.SelectedIndex = 0;
            textBoxCol.Enabled = false;
            textBoxFila.Enabled = false;
            textBoxParam.Enabled = false;
            comboBox3.Enabled = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //Bitmap bmp =new Bitmap(29 * 12+4, 17 * 18);
            
            Graphics g = Graphics.FromImage(bmp);
            Instrumentos ins;
            try
            {
                ins = new Instrumentos(g, new FileCharset(/*me.default_path + "\\" +*/ textBox1.Text));
            }
            catch (Exception)
            {
                MemoryStream stream = new MemoryStream(global::UAVConsole.Properties.Resources.Ikarus);
                ins = new Instrumentos(g, new FileCharset(new StreamReader(stream)));
            }

            if (comboBox1.SelectedIndex == 0)
                ins.SetPAL();
            else
                ins.SetNTSC();

            string cad;
                
            //Image fondo = Bitmap.FromFile("C:\\Documents and Settings\\All Users\\Documentos\\Mis imágenes\\Imágenes de muestra\\Puesta de sol.jpg");
            Image fondo;
            try
            {
                fondo = Bitmap.FromFile(me.default_path+"\\background.jpg");

            }
            catch (Exception)
            {
                fondo = global::UAVConsole.Properties.Resources.background;
            }

            g.DrawImage(fondo, 0, 0, pictureBox1.Width, pictureBox1.Height);
            //g.DrawImage(fondo, 0, 0, bmp.Width, bmp.Height);
          
            if (hud.V1_text.tipo!=0)
            {
                cad = string.Format("{0,4:0.0}", v1);
                ins.printAtStr(hud.V1_text.fila, hud.V1_text.col, cad);
                ins.writeAtChr(hud.V1_text.fila, hud.V1_text.col + cad.Length, 0xEA);
                //sprintf(cad, "%4.1fv", ikarusInfo.v1);
             }
            if (hud.V1_bar.tipo!=0)
            {
                ins.Bar(hud.V1_bar.fila, hud.V1_bar.col, hud.V1_bar.param, (int)((v1 / cells_v1 - 3.2f) * 255));
            }
            
            // Voltage V2
            if (hud.V2_text.tipo!=0)
            {    cad = string.Format("{0,4:0.0}", v2);
                ins.printAtStr(hud.V2_text.fila, hud.V2_text.col , cad);
           
                ins.writeAtChr(hud.V2_text.fila, hud.V2_text.col+cad.Length, 0xEB);
                //sprintf(cad, "%4.1fv", ikarusInfo.v1);
            }
            if (hud.V2_bar.tipo!=0)
            {
                ins.Bar(hud.V2_bar.fila, hud.V2_bar.col, hud.V2_bar.param, (int)((v2 / cells_v2 - 3.2f) * 255));
            }
            
            // Intensidad (A)
            if (hud.I.tipo!=0)
            {
                cad = string.Format("{0,5:0.0}", IA);
                ins.printAtStr(hud.I.fila, hud.I.col, cad);
           
                ins.writeAtChr(hud.I.fila, hud.I.col+cad.Length, 0xE9);
                //sprintf(cad, "%5.1fA", ikarusInfo.currI);
            }

            // Consumo (mAh?)
            if (hud.mAh.tipo == 1)
            {
                //sprintf(cad, "%5.0fmAh", ikarusInfo.consumidos_mAh);
                cad = string.Format("{0,5}mAh", mAh);
                ins.printAtStr(hud.mAh.fila, hud.mAh.col, cad);
            }
            else if (hud.mAh.tipo == 2)
            {
                ins.Bar(hud.mAh.fila, hud.mAh.col, hud.mAh.param, (int)((1 - (float)mAh / cap_mAh) * 255));
            }

            // RSSI
            if (hud.RSSI.tipo!=0)
            {
                cad = string.Format("{0,3}", RSSI);
                ins.printAtStr(hud.RSSI.fila, hud.RSSI.col, cad);
            
                ins.writeAtChr(hud.RSSI.fila, hud.RSSI.col+cad.Length, 0xD7);
                //sprintf(cad, "%3.0f", ikarusInfo.RSSI);
            }

            // Autopilot
            if (hud.Autopilot.tipo!=0)
            {
                if (AutoPilot)
                    cad="STAB";
                else
                    cad="    ";
                ins.printAtStr(hud.Autopilot.fila, hud.Autopilot.col, cad);
            }

            // Nombre destino
            if (hud.WptName.tipo!=0)
            {
                cad = WptName;
                ins.printAtStr(hud.WptName.fila, hud.WptName.col, cad);
            }
            
            //  DISTANCIA a CASA
            if (hud.Dist_Home.tipo != 0)
            {
                cad = string.Format("{0,5}", distancia_casa);
                ins.printAtStr(hud.Dist_Home.fila, hud.Dist_Home.col, cad);
                ins.writeAtChr(hud.Dist_Home.fila, hud.Dist_Home.col + cad.Length, 0xE7);
              
            }

            // DISTANCIA a WPT
            if (hud.Dist_Wpt.tipo != 0)
            {
                cad = string.Format("{0,5}", distancia_wpt);
                ins.printAtStr(hud.Dist_Wpt.fila, hud.Dist_Wpt.col, cad);
                ins.writeAtChr(hud.Dist_Wpt.fila, hud.Dist_Wpt.col + cad.Length, 0xE8);
            
            }
            
            // Número satelites
            if (hud.NumSats.tipo!=0)
            {
                if (NumSats >= 0 && NumSats < 20)
                    cad = string.Format("{0,2}", NumSats);
                else
                    cad = "EE";
                
                ins.printAtStr(hud.NumSats.fila, hud.NumSats.col, cad);
                ins.writeAtChr(hud.NumSats.fila, hud.NumSats.col + 2, 0xD8);
            }

            if (hud.PosAntena.tipo == 1)
            {
                ins.printAtStr(hud.PosAntena.fila, hud.PosAntena.col, "260");
                ins.writeAtChr(hud.PosAntena.fila, hud.PosAntena.col+3, 0xD6);
            }
            else if (hud.PosAntena.tipo == 2)
            {
                ins.printAtStr(hud.PosAntena.fila, hud.PosAntena.col, "-100");
                ins.writeAtChr(hud.PosAntena.fila, hud.PosAntena.col + 4, 0xD6);
            }

            if (hud.PosAntenaV.tipo == 1)
            {
                ins.printAtStr(hud.PosAntenaV.fila, hud.PosAntenaV.col, " 30");
                ins.writeAtChr(hud.PosAntenaV.fila, hud.PosAntenaV.col + 3, 0xD9);
            }

            // LON & LAT
            if (hud.Lon.tipo!=0)
            {
                cad=string.Format("Lon:{0,10:0.00000}", lon);
                ins.printAtStr(hud.Lon.fila, hud.Lon.col, cad);
            }
            if (hud.Lat.tipo!=0)
            {
                cad=string.Format("Lat:{0,9:0.00000}", lat);
                ins.printAtStr(hud.Lat.fila, hud.Lat.col, cad);
            }	

            // BEARING
            if (hud.Bearing.tipo == 1)
            {
                cad = string.Format("{0,4}", bearing);
                ins.printAtStr(hud.Bearing.fila, hud.Bearing.col, cad);
                ins.writeAtChr(hud.Bearing.fila, hud.Bearing.col + cad.Length, 0xe5);

            }
            else if (hud.Bearing.tipo == 2)
            {
                ins.COMPAS_chr(hud.Bearing.fila, hud.Bearing.col, bearing);
            }
            else if (hud.Bearing.tipo == 3)
            {
                ins.COMPAS_grp(hud.Bearing.fila, hud.Bearing.col, bearing);
            }
            else if (hud.Bearing.tipo == 4)
            {
                cad = string.Format("{0,3}", 270);
                ins.printAtStr(hud.Bearing.fila, hud.Bearing.col, cad);
                ins.writeAtChr(hud.Bearing.fila, hud.Bearing.col + cad.Length, 0xe5);

            }
            // DIBUJA ALTIMETRO
            if (hud.Altimetro.tipo == 1)
            {
                //sprintf(cad, "%4.0fm", var);
                cad = string.Format("{0,4}", altitud);
                ins.printAtStr(hud.Altimetro.fila, hud.Altimetro.col, cad);
                ins.writeAtChr(hud.Altimetro.fila, hud.Altimetro.col+cad.Length, 0xe6);
            }
            else if (hud.Altimetro.tipo == 2)
            {
                // DIBUJA ALTIMETRO FIGHTER
                ins.Altimetro(hud.Altimetro.fila, hud.Altimetro.col + 4, hud.Altimetro.param, altitud);
                //sprintf(cad, "%4.0f", var);
                cad = string.Format("{0,4}", altitud);
                ins.printAtStr2(hud.Altimetro.fila + hud.Altimetro.param / 2, hud.Altimetro.col, cad, 4);
                ins.printAtStr(hud.Altimetro.fila + hud.Altimetro.param / 2 + 1, hud.Altimetro.col + 2, "m");

            }
            
            // DIBUJA VELOCIMETRO
            if (hud.Velocimetro.tipo == 1)
            {
                //sprintf(cad, "%3.0f", gpsinfo.kmph);
                cad = string.Format("{0,3}", kmph);
                ins.printAtStr2(hud.Velocimetro.fila, hud.Velocimetro.col, cad, 3);
                ins.writeAtChr(hud.Velocimetro.fila, hud.Velocimetro.col + 3, 0xE1);
            }
            else if (hud.Velocimetro.tipo == 2)
            {
                // DIBUJA VELOCIMETRO FIGHTER
                ins.Velocimetro(hud.Velocimetro.fila, hud.Velocimetro.col, hud.Velocimetro.param, kmph);
                //sprintf(cad, "%3.0f", gpsinfo.kmph);
                cad = string.Format("{0,3}", kmph);
                ins.printAtStr2(hud.Velocimetro.fila + hud.Velocimetro.param / 2, hud.Velocimetro.col + 1, cad, 3);
                ins.printAtStr(hud.Velocimetro.fila + hud.Velocimetro.param / 2 + 1, hud.Velocimetro.col + 1, "kmh");
            }
           
            // Compas
            if (hud.Compas.tipo == 1)
            {
                //sprintf(cad, "%3.0f", gpsinfo.rumbo);
                cad = string.Format("{0,3}", rumbo);
                ins.printAtStr(hud.Compas.fila, hud.Compas.col, cad);
                ins.writeAtChr(hud.Compas.fila, hud.Compas.col + 3, 0xe4);
            }
            else if (hud.Compas.tipo == 2)
                ins.Compas(hud.Compas.fila, hud.Compas.col, hud.Compas.param, rumbo, bearing);

            // DIBUJA VARIOMETRO
            if (hud.Variometro.tipo == 1)
            {
                ins.Variometro1(hud.Variometro.fila, hud.Variometro.col, verticalSpeed);
            }
            else if (hud.Variometro.tipo == 2)
            {
                ins.Variometro2(hud.Variometro.fila, hud.Variometro.param, hud.Variometro.col, verticalSpeed);
            }

            if (hud.MaxAlt.tipo!=0)
            {
                cad=string.Format("Altitud MAX: {0,5:0.}m", max_alt);
                ins.printAtStr(hud.MaxAlt.fila, hud.MaxAlt.col, cad);
            }

            if (hud.MaxVelo.tipo != 0)
            {
                cad = string.Format("Velocidad Max: {0,3:0.}km/h", max_velo);
                ins.printAtStr(hud.MaxVelo.fila, hud.MaxVelo.col, cad);
            }

            if (hud.MaxDist.tipo != 0)
            {
                //sprintf(cad, "Distancia Max: %5.0fm", max_dist);
                cad = string.Format("Distancia Max: {0,5:0.}m", max_dist);
                ins.printAtStr(hud.MaxDist.fila, hud.MaxDist.col, cad);
            }
             
            if (hud.TotalDist.tipo == 1)
            {
                cad = string.Format("{0,5:0.}m", total_dist);
                ins.printAtStr(hud.TotalDist.fila, hud.TotalDist.col, cad);
            }
            else if (hud.TotalDist.tipo == 2)
            {
                cad = string.Format("Dist. Recorrida: {0,5:0.}m", total_dist);
                ins.printAtStr(hud.TotalDist.fila, hud.TotalDist.col, cad);
            }


            if (hud.Hora.tipo!=0)
            {
                if (hud.Hora.tipo == 1)
                {
                    ins.printAtStr(hud.Hora.fila, hud.Hora.col, "18:41");
                }
                else
                {
                    ins.printAtStr(hud.Hora.fila, hud.Hora.col, "18:41:08");
                }
            }

            if (hud.TiempoVuelo.tipo!=0)
            {
                if (hud.TiempoVuelo.tipo == 1)
                {
                    ins.printAtStr(hud.TiempoVuelo.fila, hud.TiempoVuelo.col, "00:17");
                }
                else if (hud.TiempoVuelo.tipo == 2)
                {
                    ins.printAtStr(hud.TiempoVuelo.fila, hud.TiempoVuelo.col, "00:17:41");
                }
                else if (hud.TiempoVuelo.tipo == 3)
                {
                    ins.printAtStr(hud.TiempoVuelo.fila, hud.TiempoVuelo.col, "Tiempo Vuelo: 00:17:41");
                }
            }

            // tasa de planeo
            if (hud.TasaPlaneo.tipo!=0)
            {
                ins.printAtStr(hud.TasaPlaneo.fila, hud.TasaPlaneo.col, "17:1");
            }

            // consumo km/Ah o m/mAh (es lo mismo)
            if (hud.Coste_km_Ah.tipo!=0)
            {
                ins.printAtStr(hud.Coste_km_Ah.fila, hud.Coste_km_Ah.col, "4.2km/Ah");
            }

            if (hud.HorizonteArtificial.tipo == 1 || hud.HorizonteArtificial.tipo == 2)
            {
                ins.HorizonteArtificial(hud.HorizonteArtificial.fila, hud.HorizonteArtificial.col,
                    hud.HorizonteArtificial.param, pitch, roll, hud.HorizonteArtificial.tipo-1);
            }

            if (hud.Auxiliary.tipo == 1)
            {
               // ins.printAtStr(hud.Auxiliary.fila, hud.Auxiliary.col, "Debug Info Here");
               // ins.printAtStr(hud.Auxiliary.fila+1, hud.Auxiliary.col, "and prob. here too");
                sbyte fila = hud.Auxiliary.fila;
                sbyte col = hud.Auxiliary.col;
                float bear = 0.0f;
                int tmp;

                if ((hud.Auxiliary.param & 2) != 0)
                    ins.CharAttrBackGr();

                ins.printAtStr(fila++, col, "ALT 1200/2000 PITCH -10/-30");
                ins.printAtStr(fila++, col, "HDG -120/-200 ROLL  -20/-30");
                
                if ((hud.Auxiliary.param & 1)!=0)
                {
                    ins.printAtStr(fila++, col, "A 1500 E 1500 M 1200 T 1500");
                }
                else
                {
                    ins.printAtStr(fila++, col, "A 0.00 E 0.00 M-0.20 T 0.00");
                }

                ins.CharAttrNoBackGr();
						
            }

            // Nombre piloto
            if (hud.NombrePiloto.tipo!=0)
            {
                ins.printAtStr(hud.NombrePiloto.fila, hud.NombrePiloto.col, "IkarusOSD_plt");
            }

            if (hud.NombreHUD.tipo != 0)
            {
                ins.printAtStr2(hud.NombreHUD.fila, hud.NombreHUD.col, hud.StrNombreHUD,16);
            }

            if (hud.BadRX.tipo != 0)
            {
                ins.printAtStr(hud.BadRX.fila, hud.BadRX.col, "BAD RX 09");
            }
            e.Graphics.DrawImage(bmp, 0, 0);//, pictureBox1.Width+11, pictureBox1.Height+11);
      
        }

        private void comboBoxShowSc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxShowSc.SelectedIndex == 0)      // simulation
            {
                if (capture != null)
                {
                    capture.PreviewWindow = null;
                    capture.Dispose();
                }
                comboBox1.SelectedIndex = (int)me.videosystem;
                comboBox1.Enabled = true;
            }
            else
            {
                try
                {
                    InitCapture();
                    if (capture != null)
                    {
                        capture.PreviewWindow = pictureBox1;
                    }
                    comboBox1.SelectedIndex = (int) me.videosystem;
                    comboBox1.Enabled = false;
        
                }
                catch (Exception)
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Error abriendo dispositivo de captura");
                    else
                        MessageBox.Show("Error opening capture device");
                    comboBoxShowSc.SelectedIndex = 0;
                }
            }
        }

        private void InitCapture()
        {
            Filters filters=null;
            
            try
            {
                filters = new Filters();
            }
            catch (Exception)
            {
            }

            if (filters != null && filters.VideoInputDevices.Count > 0)
            {
                foreach (Filter f in filters.VideoInputDevices)
                {
                    if (f.Name.Equals(me.videoCaptureStr))
                    {
                        capture = new Capture(f, null, false);
                        break;
                    }
                }
                
                //capture = new Capture(filters.VideoInputDevices[0], null, false);
                if (capture != null)
                {
                    if (me.videosystem == Singleton.VideoSystem.PAL)
                    {
                        capture.dxUtils.VideoStandard = DShowNET.AnalogVideoStandard.PAL_B;
                        try
                        {
                            capture.FrameSize = new Size(720, 576);
                            capture.PreviewFrameSize = new Size(720, 576);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        capture.dxUtils.VideoStandard = DShowNET.AnalogVideoStandard.NTSC_M;
                        try
                        {
                            capture.FrameSize = new Size(720, 480);
                        }
                        catch (Exception) { }
                    }
          
                    capture.AllowSampleGrabber = true;

                    //capture.PreviewWindow = pictureBox1;
                }
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg=new SaveFileDialog();
            dlg.DefaultExt = ".osd";
            dlg.AddExtension = true;
            dlg.Filter = "Pantalla (*.osd)|*.osd|Binary (*.bin)|*.bin";
            dlg.ShowDialog();
            
            if (dlg.FileName != null&&dlg.FileName.Length>0)
            {
                if(dlg.FilterIndex==1)
                {
                    hud.SaveToXml(dlg.FileName);
                }
                else
                {
                    byte[] buffer = new byte[hud.size_bytes()];
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = hud.getByte(i);
                    File.WriteAllBytes(dlg.FileName, buffer);
                }
                    
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".osd";
            dlg.AddExtension = true;
            dlg.Filter = "Pantalla (*.osd)|*.osd";
            dlg.ShowDialog();
            if (dlg.FileName != null)
            {
                try
                {
                    hud.LoadFromXml(dlg.FileName);
                }
                catch (Exception)
                {
                    hud.Load(dlg.FileName);
                }
            }
            UpdateTextBoxes();
            pictureBox1.Invalidate();
        }
                
        void UpdateTextBoxes()
        {
            if (Selected_Item >= 0)
            {
                comboBox3.Enabled = true;
                comboBox3.Items.Clear();
                if (me.Idioma == 0)
                {
                    if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Bearing)
                        comboBox3.Items.AddRange(new object[] { "Disable", "Texto", "On (1x2)", "On (2x4)", "Absoluto (TXT)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Variometro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (1ch)", "On (2ch)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Compas)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Velocimetro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Altimetro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.mAh)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Barra)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Hora)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (HH:MM)", "On (HH:MM:SS)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.TiempoVuelo)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (HH:MM)", "ON (HH:MM:SS)", "On (texto)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.HorizonteArtificial)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Modo 1)", "ON (Modo2)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.PosAntena)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Modo 1)", "ON (Modo2)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.TotalDist)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Breve)", "ON (Resumen)" });
                    else
                        comboBox3.Items.AddRange(new object[] { "Disable", "Enable" });
                }
                else
                {
                    if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Bearing)
                        comboBox3.Items.AddRange(new object[] { "Disable", "Texto", "On (1x2)", "On (2x4)", "Absolute (TXT)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Variometro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (1ch)", "On (2ch)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Compas)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Velocimetro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Altimetro)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Caza)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.mAh)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (texto)", "On (Barra)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Hora)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (HH:MM)", "On (HH:MM:SS)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.TiempoVuelo)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (HH:MM)", "ON (HH:MM:SS)", "On (texto)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.HorizonteArtificial)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Modo 1)", "ON (Modo2)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.PosAntena)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Modo 1)", "ON (Modo2)" });
                    else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.TotalDist)
                        comboBox3.Items.AddRange(new object[] { "Disable", "On (Breve)", "ON (Resumen)" });
                    else
                        comboBox3.Items.AddRange(new object[] { "Disable", "Enable" });
                }
                EnableInstrumentos();
                ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

                if (elem.tipo < comboBox3.Items.Count)
                    comboBox3.SelectedIndex = elem.tipo;
                else
                {
                    comboBox3.SelectedIndex = 0;
                    elem.tipo = 0;
                }
                
                textBoxFila.Text = "" + elem.fila;
                textBoxCol.Text = "" + elem.col;
                //textBoxParam.Enabled = false;

                if (Selected_Item == (int)IkarusScreenConfig.Instrumento.NombreHUD)
                {
                    textBoxParam.Text = hud.StrNombreHUD;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Auxiliary)
                {
                    checkBoxAbsoluto.Checked=((elem.param & 1) != 0);
                    checkBoxOpaco.Checked = ((elem.param & 2) != 0);
                    checkBoxServo.Checked = ((elem.param & 4) != 0);
                }
                else
                    textBoxParam.Text = "" + elem.param;
            }
        }

        void EnableInstrumentos()
        {
            ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

            if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Auxiliary)
            {
                checkBoxAbsoluto.Visible = true;
                checkBoxOpaco.Visible = true;
                checkBoxServo.Visible = true;
                label3.Visible = false;
                textBoxParam.Visible = false;
            }
            else
            {
                checkBoxAbsoluto.Visible = false;
                checkBoxOpaco.Visible = false;
                checkBoxServo.Visible = false;
                label3.Visible = true;
                textBoxParam.Visible = true;
            }

            if (elem.tipo == 0)
            {
                textBoxCol.Enabled = false;
                textBoxFila.Enabled = false;
                textBoxParam.Enabled = false;
                //label3.Text = "Parametro";

                checkBoxAbsoluto.Enabled = false;
                checkBoxOpaco.Enabled = false;
                checkBoxServo.Enabled = false;
                label3.Enabled = false;
                textBoxParam.Enabled = false;
                label2.Enabled = false;
                label4.Enabled = false;
            }
            else
            {
                textBoxCol.Enabled = true;
                textBoxFila.Enabled = true;
                label2.Enabled = true;
                label4.Enabled = true;

                if (Selected_Item == (int)IkarusScreenConfig.Instrumento.V1_bar && elem.tipo == 1)
                {
                    label3.Text = "Ancho";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.V2_bar && elem.tipo == 1)
                {
                    label3.Text = "Ancho";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Variometro && elem.tipo == 2)
                {
                    label3.Text = "Fila 2";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Compas && elem.tipo == 2)
                {
                    label3.Text = "Ancho";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Velocimetro && elem.tipo == 2)
                {
                    label3.Text = "Alto";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Altimetro && elem.tipo == 2)
                {
                    label3.Text = "Alto";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.mAh && elem.tipo == 2)
                {
                    label3.Text = "Ancho";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.HorizonteArtificial)
                {
                    label3.Text = "Tamaño";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.NombreHUD)
                {
                    label3.Text = "HUD Str";
                    label3.Enabled = true;
                    textBoxParam.Enabled = true;
                }
                else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Auxiliary)
                {
                    checkBoxAbsoluto.Enabled = true;
                    checkBoxOpaco.Enabled = true;
                    checkBoxServo.Enabled = true;
                
                }
                else
                {
                    label3.Text = "Parametro";
                    textBoxParam.Enabled = false;
                    checkBoxAbsoluto.Enabled = false;
                    checkBoxOpaco.Enabled = false;
                    checkBoxServo.Enabled = false;
                
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected_Item = listBox1.SelectedIndex;
            UpdateTextBoxes();
        }
        
        private void textBoxFila_TextChanged(object sender, EventArgs e)
        {
            int i;
            ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

            if (int.TryParse(textBoxFila.Text, out i))
               elem.fila = (sbyte)i;
            pictureBox1.Invalidate();
        }

        private void textBoxCol_TextChanged(object sender, EventArgs e)
        {
            int i;
            ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

            if (int.TryParse(textBoxCol.Text, out i))
                elem.col = (sbyte)i;
            pictureBox1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                pictureBox1.Height = 16 * 18 + 4;
                pictureBox1.Top = 4;

                //196; 4
            }
            else
            {
                pictureBox1.Height = 13 * 18 + 4;
                pictureBox1.Top = 31;//4 + 18*1.5 = 27+4 = 31
            }
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {
                fp.WriteScreen(comboBoxScreenSlot.SelectedIndex,hud);
                fp.Close();
            }
            else
                MessageBox.Show("Not conected!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {
                hud = fp.ReadScreen(comboBoxScreenSlot.SelectedIndex);
                fp.Close();
                UpdateTextBoxes();
                pictureBox1.Invalidate();
            }
            else
                MessageBox.Show("Not conected!");
           
        }

        private void FormScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(capture!=null)
                capture.Dispose();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Selected_Item >= 0)
            {
                ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

                elem.tipo = (byte)comboBox3.SelectedIndex;
                pictureBox1.Invalidate();
                EnableInstrumentos();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                fc = new FileCharset(textBox1.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error abriendo fichero. Usando buildin");
                MemoryStream stream = new MemoryStream(global::UAVConsole.Properties.Resources.Ikarus);
                fc= new FileCharset(new StreamReader(stream));
            }

            fp = new FlightPlanUSB();
            timer1.Interval = 200;
            t_i = 0;
            if (fp.IsOpen())
                timer1.Enabled = true;
            else
                MessageBox.Show("Not conected!");
        }

        int t_i = 0;
        FileCharset fc = null;
        FlightPlanUSB fp = null;

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] buff;

            buff = fc.getChar((byte)t_i);
            fp.WriteCharSet(t_i, buff);
            t_i++;
            progressBar1.Value = t_i;
            progressBar1.Invalidate();

            if (t_i > 255)
            {
                timer1.Enabled = false;
                fp.Close();

                progressBar1.Value = 0;
                progressBar1.Invalidate();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FormEditCharset().Show(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            if (dialog.FileName != "")
                textBox1.Text = dialog.FileName;
        }

        void TextSpanish()
        {
            label1.Text = "Modo";
            label2.Text = "Fila";
            label3.Text = "Parametro";
            label4.Text = "Columna";
            this.Text = "Gestor de Pantallas";

            button1.Text = "Subir Ikarus";
            button2.Text = "Leer Ikarus";

            comboBoxShowSc.Items.Clear();
            comboBoxShowSc.Items.AddRange(new object[] {
            "Simulacion",
            "Capturadora"});

            listBox1.Items.Clear();
            listBox1.Items.AddRange(new object[] {
            "Altimetro",
            "Autopilot",
            "Bearing",
            "Compas",
            "Distancia casa",
            "Distancia wpt",
            "Hora",
            "Horizonte Artificial",
            "Intensidad (A)",
            "Consumo total (mAh)",
            "Latitud",
            "Longitud",
            "Nombre HUD",
            "Nombre Piloto",
            "Nombre WayPoint",
            "Num Satelites GPS",
            "Posicion Antena",
            "Pos. Antena Vert",
            "Rendimiento km/Ah",
            "RSSI",
            "Tasa planeo",
            "Tiempo vuelo",
            "Variometro",
            "Velocimetro",
            "Vmotor (Text)",
            "Vmotor (Bar)",
            "Vvideo (Text)",
            "Vvideo (Bar)",
            "Altitud máxima",
            "Distancia casa máxima",
            "Velocidad máxima",
            "Distancia recorrida",
            "Bad RX",
            "Auxiliar"});

            comboBoxScreenSlot.Items.Clear();
            comboBoxScreenSlot.Items.AddRange(new object[] {
            "Pantalla OSD1",
            "Pantalla OSD2",
            "Pantalla OSD3",
            "FailSafe",
            "Resumen"});

        }
        void TextEnglish()
        {

            label1.Text = "Mode";
            label2.Text = "Row";
            label3.Text = "Param";
            label4.Text = "Colum";
            this.Text = "Screen Manager";

            button1.Text = "Load Ikarus";
            button2.Text = "Read Ikarus";

            comboBoxShowSc.Items.Clear();
            comboBoxShowSc.Items.AddRange(new object[] {
            "Simulation",
            "Video Capture"});

            listBox1.Items.Clear();
            listBox1.Items.AddRange(new object[] {
            "Altimeter",
            "Autopilot",
            "Bearing",
            "Compas",
            "Distance (Home)",
            "Distance (Waypoint)",
            "Time",
            "Artificial Horizont",
            "Intensity (A)",
            "Battery Level (mAh)",
            "Latitude",
            "Longitude",
            "HUD Name",
            "Pilot Name",
            "WayPoint Name",
            "Num GPS Sats",
            "Ant Tracker",
            "Ant Tracker Vert",
            "Milles per Ah",
            "RSSI",
            "Plane Rate",
            "Fly Time",
            "Vertical Speed",
            "Ground Speed",
            "Vmotor (Text)",
            "Vmotor (Bar)",
            "Vvideo (Text)",
            "Vvideo (Bar)",
            "Max. Altitude",
            "Max. Home Distance",
            "Max. Speed",
            "Total Traveled",
            "Bad RX",
            "Auxiliar"});

            comboBoxScreenSlot.Items.Clear();
            comboBoxScreenSlot.Items.AddRange(new object[] {
            "OSD Screen 1",
            "OSD Screen 2",
            "OSD Screen 3",
            "FailSafe",
            "Summary"});

        }

        private void button8_Click(object sender, EventArgs e)
        {
            hud = new IkarusScreenConfig();
            UpdateTextBoxes();
            pictureBox1.Invalidate();
        }

        void ParamChanged()
        {
            ElementoOSD elem = (ElementoOSD)hud.getElement(Selected_Item);

            if (Selected_Item == (int)IkarusScreenConfig.Instrumento.NombreHUD)
            {
                hud.StrNombreHUD = textBoxParam.Text;
            }
            else if (Selected_Item == (int)IkarusScreenConfig.Instrumento.Auxiliary)
            {
                elem.param = (byte)0;
                if (checkBoxAbsoluto.Checked)
                    elem.param |= 1;
                if (checkBoxOpaco.Checked)
                    elem.param |= 2;
                if (checkBoxServo.Checked)
                    elem.param |= 4;
            }
            else
            {
                int i = 0;
                if (int.TryParse(textBoxParam.Text, out i))
                    elem.param = (byte)i;
            }
            pictureBox1.Invalidate();
        }

        private void textBoxParam_TextChanged(object sender, EventArgs e)
        {
            ParamChanged();
        }
        private void checkBoxOpaco_CheckedChanged(object sender, EventArgs e)
        {
            ParamChanged();
        }

        private void checkBoxServo_CheckedChanged(object sender, EventArgs e)
        {
            ParamChanged();
        }

        private void checkBoxAbsoluto_CheckedChanged(object sender, EventArgs e)
        {
            ParamChanged();
        }
    }
}