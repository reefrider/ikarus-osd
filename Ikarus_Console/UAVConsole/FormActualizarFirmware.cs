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
using System.IO.Ports;
using System.Threading;
using System.IO;
using UAVConsole.USBXpress;

namespace UAVConsole
{
    public partial class FormActualizarFirmware : Form
    {
        public enum Devices { OSD, Uplink, AntTracker };
        public enum Modos { Serie, Usb };

        Modos modo = Modos.Serie;
        Devices device = Devices.OSD;
        UsbLayerFIRM usbFirm;

        Singleton me = Singleton.GetInstance();
        IntelHEX hex;
        SerialPort sp;
        int pages = 0;
        int currPage = 0;
        int currOffset = 0;
        byte crc = 0;

        bool _lock = false;

        public FormActualizarFirmware():this(Devices.OSD)
        {
        }

        public FormActualizarFirmware(Devices dev)
        {
            InitializeComponent();
            this.device = dev;

            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            buscaCOMs();
            if (dev == Devices.AntTracker || dev == Devices.Uplink)
                comboBoxCOM.Items.Insert(0, "USB");
                CheckIfUSB();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBoxCOM.SelectedIndex < 0)
            {
                if (me.Idioma == 0)
                    MessageBox.Show("Selecciona un puerto serie");
                else
                    MessageBox.Show("Choose Serial Port");
            }
            else
            {
                try
                {
                    hex = IntelHEX.FromBinFw(textBox1.Text);
                }
                catch (Exception)
                {
                }

                if (hex == null)
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Error abriendo fichero. Usando interno");
                    else
                        MessageBox.Show("Error opening file. Using build-in");
                    
                    MemoryStream stream;
                    switch (device)
                    {
                        case Devices.OSD:
                            stream = new MemoryStream(global::UAVConsole.Properties.Resources.firmware);
                            break;
                        case Devices.Uplink:
                            stream = new MemoryStream(global::UAVConsole.Properties.Resources.fw_uplink);
                            break;
                        case Devices.AntTracker:
                            stream = new MemoryStream(global::UAVConsole.Properties.Resources.fw_anttrack);
                            break;
                        default:
                            stream = null;
                            break;
                    }
                    hex = IntelHEX.FromBinFw(stream);
                }
                if (modo == Modos.Usb)
                {
                    pages = hex.lenght / 512;
                    currPage = 0;
                    currOffset = 0;
                   
                    if (hex.lenght % 512 == 0)
                    {
                        if (device == Devices.Uplink)
                            usbFirm = new UsbLayerFIRM("9100");
                        else
                            usbFirm = new UsbLayerFIRM("9200");

                        // configuramos la prgressbar
                        progressBar1.Maximum = pages;
                        progressBar1.Value = 0;
                        progressBar1.Invalidate();

                        timer1.Interval = 3000;
                        timer1.Start();
                    }
                    else
                    {
                        MessageBox.Show("El fichero no tiene la longitud adecuada. Debe ser múltiplo del tamaño de página.");
                    }
                }
                else
                {
                    string comm_name = comboBoxCOM.Items[comboBoxCOM.SelectedIndex].ToString();
                    sp = new SerialPort(comm_name, 115200, Parity.None, 8, StopBits.One);
                    pages = hex.lenght / 512;
                    currPage = 0;
                    crc = 0;

                    if (hex.lenght % 512 == 0)
                    {
                        sp.Open();
                        sp.Write("$");
                        sp.Write("A");
                        sp.Write("T");
                        sp.Write("C");

                        //enviamos el número de pages
                        sp.Write(new byte[] { (byte)pages }, 0, 1);

                        // configuramos la prgressbar
                        progressBar1.Maximum = pages;
                        progressBar1.Value = 0;
                        progressBar1.Invalidate();

                        timer1.Interval = 3000;
                        timer1.Start();
                    }
                    else
                    {
                        MessageBox.Show("El fichero no tiene la longitud adecuada. Debe ser múltiplo del tamaño de página.");
                    }
                }
                
            }
        }

        void buscaCOMs()
        {
            string[] comms = SerialPort.GetPortNames();
            if (comms.Length > 0)
            {
                int _selected_index = 0;
                comboBoxCOM.Items.Clear();
                for (int i = 0; i < comms.Length; i++)
                {
                    comboBoxCOM.Items.Add(comms[i]);
                    if (comms[i].Equals(me.commPort))
                        _selected_index = i;
                }
                comboBoxCOM.SelectedIndex = _selected_index;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".hex";
            dlg.AddExtension = true;
            dlg.Filter = "Firmware File (*.fw)|*.fw";
            dlg.ShowDialog();
            textBox1.Text = dlg.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (device == Devices.OSD)
            {
                FlightPlanUSB dev = new FlightPlanUSB();
                if (dev.IsOpen())
                {
                    dev.FirmwareUpdate();
                    dev.Close();
                }
                else
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Error abriendo USB");
                    else
                        MessageBox.Show("Error opening USB");
                }
            }
            else if (device == Devices.Uplink)
            {
                EmisoraUSB dev = new EmisoraUSB();
                if (dev.IsOpen())
                {
                    dev.UpdateFirmware();
                    dev.Close();
                }
                else
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Error abriendo USB");
                    else
                        MessageBox.Show("Error opening USB");
                }
            }
            else if (device == Devices.AntTracker)
            {
                AntenaTracker dev = new AntenaTracker();
                if (dev.IsOpen())
                {
                    dev.UpdateFirmware();
                    dev.Close();
                }
                else
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Error abriendo USB");
                    else
                        MessageBox.Show("Error opening USB");
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte []_buff = new byte[32];
            byte[] _buff2;
            System.Console.WriteLine("Entrada = " + DateTime.Now.Ticks);

            if (modo == Modos.Usb)
            {
                timer1.Interval = 150;
                if (!_lock)
                {
                    _lock = true;
                    if (currPage < pages)
                    {

                        for (int j = 0; j < 32; j++)
                            _buff[j] = hex[currPage * 512 + currOffset + j];
                        _buff2 = usbFirm.WriteData(currPage * 512 + currOffset, _buff);

                        for (int j = 0; j < 32; j++)
                            if (_buff[j] != _buff2[j])
                            {

                                break;
                            }

                        currOffset += 32;
                        if (currOffset >= 512)
                        {
                            currOffset = 0;
                            currPage++;
                            usbFirm.ClearPage(512 * currPage);

                        }
                        progressBar1.Value = (int)(currPage);
                        progressBar1.Invalidate();
                    }
                    else
                    {
                        timer1.Stop();
                        usbFirm.FirmwareUpdate(pages);
                    }
                    _lock = false;
                }
            }
            else
            {
                timer1.Interval = 50;
                if (currPage < pages)
                {
                    byte[] buff = new byte[512];
                    for (int j = 0; j < 512; j++)
                    {
                        byte b = hex[512 * currPage + j];
                        CRC.CCITT8(b, ref crc);
                        buff[j] = b;
                    }
                    sp.Write(buff, 0, 512);
                    currPage++;
                    progressBar1.Value = (int)(currPage);
                    progressBar1.Invalidate();
                    //this.Invalidate();
                }
                else
                {
                    sp.Write(new byte[] { crc }, 0, 1);
                    sp.Close();
                    timer1.Stop();
                }
            }

            System.Console.WriteLine("Salida = " + DateTime.Now.Ticks);
        }

        void TextSpanish()
        {
            label6.Text = "Puerto Serie";
            button4.Text = "Fichero";
            button1.Text = "Actualizar OSD";
            button2.Text = "Entrar Modo";
            switch (device)
            {
                case Devices.OSD:
                    this.Text = "Actualizar Firmware OSD";
                    button1.Text = "Actualizar OSD";
                    break;
                case Devices.Uplink:
                    this.Text = "Actualizar Módulo Uplink";
                    button1.Text = "Actualizar Uplink";
                    break;
                case Devices.AntTracker:
                    this.Text = "Actualizar Módulo AntTracker";
                    button1.Text = "Actualizar AntTracker";
                    break;
            }
        }


        void TextEnglish()
        {
            label6.Text = "Serial COM Port";
            button4.Text = "File";
            button1.Text = "Update Firmware";
            button2.Text = "Entry Mode";
            switch (device)
            {
                case Devices.OSD:
                    this.Text = "Update Ikarus OSD Firmware";
                    button1.Text = "Update OSD";
                    break;
                case Devices.Uplink:
                    this.Text = "Update Uplink Module";
                    button1.Text = "Update Uplink";
                    break;
                case Devices.AntTracker:
                    this.Text = "Update AntTracker Module";
                    button1.Text = "Update AntTracker";
                    break;
            }
            
        }

        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckIfUSB();
        }

        void CheckIfUSB()
        {
            if (comboBoxCOM.SelectedItem.ToString() == "USB")
            {
                button2.Enabled = false;
                checkBox1.Enabled = false;
                modo = Modos.Usb;
            }
            else
            {
                button2.Enabled = true;
                checkBox1.Enabled = true;
                modo = Modos.Serie;
            }
        }
    }
}