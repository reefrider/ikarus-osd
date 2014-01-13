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
using UAVConsole.GoogleMaps;
using UAVConsole.ConfigClasses;
using System.Threading;
using System.IO;
using UAVConsole.USBXpress;
using System.Globalization;

namespace UAVConsole
{
    public partial class FormGestionRutas : Form
    {
        public List<WayPoint> ruta;
        WayPoint wpt;
        Singleton me = Singleton.GetInstance();
         public FormGestionRutas()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            ruta = me.Ruta;
            mapControl1.ruta = ruta;
            listBoxUpdate();
            formUpdate();
            comboBox1.SelectedIndex = 0;
        }

        void listBoxUpdate()
        {
            listBoxWpts.Items.Clear();
            foreach (WayPoint wpt in ruta)
            {
                listBoxWpts.Items.Add(wpt.name);
            }
            listBoxWpts.Invalidate();
            formUpdate();
        }

        void formUpdate()
        {
            if (listBoxWpts.SelectedIndex >= 0)
            {
                WayPoint wpt = ruta[listBoxWpts.SelectedIndex];
                textBoxWptName.Text = wpt.name;
                textBoxWptLon.Text = wpt.Longitude.ToString();
                textBoxWptLat.Text = wpt.Latitude.ToString();
                textBoxWptAlt.Text = wpt.Altitude.ToString();
                
                textBoxWptName.Enabled = true;
                textBoxWptLon.Enabled = true;
                textBoxWptLat.Enabled = true;
                textBoxWptAlt.Enabled = true;

                button1.Enabled = true;
                button2.Enabled = true;
                button9.Enabled = true;

                mapControl1.target = wpt;
                
            }
            else
            {
                textBoxWptName.Text = "";
                textBoxWptLon.Text = "";
                textBoxWptLat.Text = "";
                textBoxWptAlt.Text = "";
                textBoxWptName.Enabled = false;
                textBoxWptLon.Enabled = false;
                textBoxWptLat.Enabled = false;
                textBoxWptAlt.Enabled = false;

                button1.Enabled = false;
                button2.Enabled = false;
                button9.Enabled = false;

                mapControl1.target = null;
            }

            if (listBoxWpts.Items.Count > 0)
                button11.Enabled = true;
            else
                button11.Enabled = false;
            
        }

        void EnableControls(bool state, object sender)
        {
            label1.Enabled = state;
            label2.Enabled = state;
            label3.Enabled = state;
            label4.Enabled = state;
            
            listBoxWpts.Enabled = state;
            textBoxWptAlt.Enabled = state;
            textBoxWptLat.Enabled = state;
            textBoxWptLon.Enabled = state;
            textBoxWptName.Enabled = state;

            button1.Enabled = state;
            button10.Enabled = state;
            button2.Enabled = state;
            button4.Enabled = state;
            button5.Enabled = state;
            button6.Enabled = state;
            button7.Enabled = state;
            button8.Enabled = state;
            button9.Enabled = state;

            if((Button)sender != button3 || state == true)
                button3.Enabled = state;
            if ((Button)sender != button11 || state == true)
                button11.Enabled = state;
            


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (me.Idioma == 0)
            {
                if (bt.Text == "Fijar Ruta")
                {
                    EnableControls(false, sender);
                    bt.Text = "Fin Ruta";
                    mapControl1.listeners += Listener_FijarRuta;
                    mapControl1.ruta.Clear();
                }
                else
                {
                    EnableControls(true, sender);
                    bt.Text = "Fijar Ruta";
                    mapControl1.listeners -= Listener_FijarRuta;
                }
            }
            else
            {
                if (bt.Text == "Set Route")
                {
                    EnableControls(false, sender);
                    bt.Text = "End Route";
                    mapControl1.listeners += Listener_FijarRuta;
                    mapControl1.ruta.Clear();
                }
                else
                {
                    EnableControls(true, sender);
                    bt.Text = "Set Route";
                    mapControl1.listeners -= Listener_FijarRuta;
                }
            }
            
            listBoxUpdate();
            mapControl1.Invalidate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (me.Idioma == 0)
            {
                if (bt.Text == "Añadir Wpt")
                {
                    EnableControls(false, sender);
                    bt.Text = "Fin Añadir";
                    mapControl1.listeners += Listener_FijarRuta;
                }
                else
                {
                    EnableControls(true, sender);
                    bt.Text = "Añadir Wpt";
                    mapControl1.listeners -= Listener_FijarRuta;
                }
            }
            else
            {
                if (bt.Text == "Add Wpt")
                {
                    EnableControls(false, sender);
                    bt.Text = "End Add";
                    mapControl1.listeners += Listener_FijarRuta;
                }
                else
                {
                    EnableControls(true, sender);
                    bt.Text = "Add Wpt";
                    mapControl1.listeners -= Listener_FijarRuta;
                }
            }

            listBoxUpdate();
            mapControl1.Invalidate();
        }
        void Listener_FijarRuta(WayPoint waypoint, MouseButtons btn)
        {
            if (btn == MouseButtons.Left)
            {
                waypoint.name = "WPT " + ruta.Count;
                ruta.Add(waypoint);
                listBoxUpdate();
                mapControl1.Invalidate();      
            }
            else if (btn == MouseButtons.None && ruta.Count>0)
            {
                WayPoint wpt = ruta[ruta.Count-1];
                float dist=wpt.getDistance(waypoint);
                mapControl1.mensaje = "Dist "+dist.ToString(".000");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<WayPoint> miRuta = mapControl1.ruta;
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {
                if (miRuta.Count >= 0)
                {
                    Thread.Sleep(500);
                    for (int i = 0; i < miRuta.Count; i++)
                    {
                        WayPoint wpt = miRuta[i];
                        fp.WriteWpt(i, wpt.name, (float)wpt.Longitude, (float)wpt.Latitude, wpt.Altitude);
                        Thread.Sleep(500);
                    }
                    fp.WriteMaxWpt(miRuta.Count);
                    fp.Flush();
                }
                fp.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<WayPoint> miRuta = mapControl1.ruta;
            FlightPlanUSB fp = new FlightPlanUSB();
            if (fp.IsOpen())
            {
                int max = fp.ReadMaxWpts();
                miRuta.Clear();

                for (int i = 0; i < max && max < 0xff; i++)
                {
                    miRuta.Add(fp.ReadWpt(i));
                }
                mapControl1.Refresh();
                fp.Close();
                listBoxUpdate();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxWpts.SelectedIndex >= 0)
            {
                wpt = ruta[listBoxWpts.SelectedIndex];
                mapControl1.Invalidate();
                formUpdate();
                //    mapControl1.listeners += Listener_ModificarWaypoint;
            }
            
        }
        void Listener_ModificarWaypoint(WayPoint waypoint, MouseButtons btn)
        {
            if (btn == MouseButtons.Left)
            {
                wpt.Latitude = waypoint.Latitude;
                wpt.Longitude = waypoint.Longitude;
                mapControl1.listeners -= Listener_ModificarWaypoint;
                mapControl1.Invalidate();
                formUpdate();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int sel = listBoxWpts.SelectedIndex;
            if (sel >= 0)
            {
                ruta.RemoveAt(sel);
                listBoxUpdate();
                listBoxWpts.SelectedIndex = sel - 1;
            }
            
            formUpdate();
            mapControl1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mapControl1.listeners += Listener_InsertarWaypoint;
        }


        void Listener_InsertarWaypoint(WayPoint waypoint, MouseButtons btn)
        {
            if (btn == MouseButtons.Left)
            {
                int sel = listBoxWpts.SelectedIndex;
                wpt = new WayPoint("new");
                wpt.Longitude = waypoint.Longitude;
                wpt.Latitude = waypoint.Latitude;
                if (sel >= 0)
                    ruta.Insert(sel, wpt);
                else
                    ruta.Add(wpt);
                listBoxUpdate();
                listBoxWpts.SelectedIndex = sel;
                
                mapControl1.listeners -= Listener_InsertarWaypoint;
                mapControl1.Invalidate();
                formUpdate();
            }
        }

        private void mapControl1_Click(object sender, EventArgs e)
        {
            formUpdate();
        }

        private void mapControl1_Paint(object sender, PaintEventArgs e)
        {
            formUpdate();
        }

        private void textBoxWptName_Leave(object sender, EventArgs e)
        {
            wpt.name = textBoxWptName.Text;
            listBoxWpts.Items[listBoxWpts.SelectedIndex] = wpt.name;
        }

        private void textBoxWptAlt_Leave(object sender, EventArgs e)
        {
            try
            {
                wpt.Altitude = float.Parse(textBoxWptAlt.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
            catch(Exception)
            {
            }
        }

        private void textBoxWptLon_Leave(object sender, EventArgs e)
        {
            try
            {
                wpt.Longitude = float.Parse(textBoxWptLon.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                mapControl1.Invalidate();
            }
            catch (Exception)
            {
            }
        }

        private void textBoxWptLat_Leave(object sender, EventArgs e)
        {
            try
            {
                wpt.Latitude = float.Parse(textBoxWptLat.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                mapControl1.Invalidate();
            }
            catch (Exception)
            {
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".ikarus";
            dlg.AddExtension = true;
            dlg.Filter = "Rutas (*.ikarus)|*.ikarus";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {

                StreamWriter fout = File.CreateText(dlg.FileName);
                fout.WriteLine(ruta.Count);
                for (int i = 0; i < ruta.Count; i++)
                {
                    fout.WriteLine(ruta[i].name);
                    fout.WriteLine(ruta[i].Longitude.ToString(CultureInfo.InvariantCulture));
                    fout.WriteLine(ruta[i].Latitude.ToString(CultureInfo.InvariantCulture));
                    fout.WriteLine(ruta[i].Altitude.ToString(CultureInfo.InvariantCulture));
                }
                fout.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".ikarus";
            dlg.AddExtension = true;
            dlg.Filter = "Rutas (*.ikarus)|*.ikarus";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                StreamReader fin = File.OpenText(dlg.FileName);
                int numwpts = 0;
                if (int.TryParse(fin.ReadLine(), out numwpts))
                {
                    ruta.Clear();
                    for (int i = 0; i < numwpts; i++)
                    {
                        WayPoint wpt = new WayPoint();
                        wpt.name = fin.ReadLine();
                        // Poner que soporte tambien en cultura nativa

                        wpt.Longitude = float.Parse(fin.ReadLine().Replace(',', '.'), CultureInfo.InvariantCulture);
                        wpt.Latitude = float.Parse(fin.ReadLine().Replace(',', '.'), CultureInfo.InvariantCulture);
                        wpt.Altitude = float.Parse(fin.ReadLine().Replace(',', '.'), CultureInfo.InvariantCulture);

                        ruta.Add(wpt);
                    }
                }
                listBoxUpdate();
                formUpdate();
                mapControl1.Invalidate();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ruta.Clear();
            listBoxUpdate();
            formUpdate();
            mapControl1.Invalidate();
        }

        private void button9_Click(object sender, EventArgs e)
        {   // modificar wpt
            if(listBoxWpts.SelectedIndex>=0)
                mapControl1.listeners += Listener_ModificarWaypoint;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            int i = cbo.SelectedIndex;
            switch (i)
            {
                case 0: mapControl1.SetMode(Modes.SAT);
                    break;
                case 1: mapControl1.SetMode(Modes.MAP);
                    break;
                case 2: mapControl1.SetMode(Modes.MAP_SAT);
                    break;
                case 3: mapControl1.SetMode(Modes.TOPO);
                    break;
                default:
                    break;
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            mapControl1.listeners += Listener_FijarCasa;
        }

        void Listener_FijarCasa(WayPoint waypoint, MouseButtons btn)
        {
            if (btn == MouseButtons.Left)
            {

                mapControl1.home.Latitude = waypoint.Latitude;
                mapControl1.home.Longitude = waypoint.Longitude;

                Singleton.GetInstance().HomeLat = (float)waypoint.Latitude;
                Singleton.GetInstance().HomeLon = (float)waypoint.Longitude;
                Singleton.GetInstance().HomeAlt = (float)waypoint.Altitude;

                mapControl1.listeners -= Listener_FijarCasa;
                mapControl1.Invalidate();
            }

        }

        void TextSpanish()
        {
            label1.Text = "Nombre Wpt";
            label2.Text = "Longitud";
            label3.Text = "Latitud";
            button2.Text = "Insertar Wpt";
            button1.Text = "Borrar Wpt";
            button9.Text = "Modif. Wpt";
            button3.Text = "Fijar Ruta";
            button8.Text = "Borrar Ruta";
            button5.Text = "Salvar Ruta";
            button7.Text = "Cargar Ruta";
            button11.Text = "Añadir Wpt";
            button4.Text = "Ruta -> Ikarus";
            button6.Text = "Ikarus -> Ruta";
            button3.Text = "Fijar Ruta";
            this.Text = "Gestión de Rutas";
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[] {
            "SATELITE",
            "MAPA",
            "SAT+MAP",
            "TOPO"});
        }

        void TextEnglish()
        {
            label1.Text = "Wpt Name";
            label2.Text = "Longitude";
            label3.Text = "Latitude";
            button2.Text = "Insert Wpt";
            button1.Text = "Erase Wpt";
            button9.Text = "Modify Wpt";
            button3.Text = "Set Route";
            button8.Text = "Clear Route";
            button5.Text = "Save Route";
            button7.Text = "Load Route";
            button11.Text = "Add Wpt";
            button4.Text = "Route -> Ikarus";
            button6.Text = "Ikarus -> Route";
            button3.Text = "Set Route";
            this.Text = "Route Manager";
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[] {
            "SATELLITE",
            "MAP",
            "SAT+MAP",
            "TOPO"});
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FlightPlanUSB planUSB = new FlightPlanUSB();
            if (planUSB.IsOpen())
            {
                WayPoint wpt = planUSB.ReadGPS();
                if (wpt != null)
                {
                    Singleton.GetInstance().HomeLat = (float)wpt.Latitude;
                    Singleton.GetInstance().HomeLon = (float)wpt.Longitude;
                    Singleton.GetInstance().HomeAlt = (float)wpt.Altitude;

                    mapControl1.home.Latitude = wpt.Latitude;
                    mapControl1.home.Longitude = wpt.Longitude;
                    mapControl1.Invalidate();
                }
                else
                {
                    IkarusBasicConfig cfg = planUSB.ReadConfig();

                    Singleton.GetInstance().HomeLat = (float)cfg.HomeLat;
                    Singleton.GetInstance().HomeLon = (float)cfg.HomeLon;
                    Singleton.GetInstance().HomeAlt = (float)cfg.HomeAltitude;

                    mapControl1.home.Latitude = cfg.HomeLat;
                    mapControl1.home.Longitude = cfg.HomeLon;
                    mapControl1.Invalidate();

                }
                planUSB.Close();
            }
            else if (me.Idioma == 0)
            {
                MessageBox.Show("No se puede conectar!");
            }
            else
            {
                MessageBox.Show("Cannot connect!");
            }

        }

        private void mapControl1_MouseLeave(object sender, EventArgs e)
        {
            mapControl1.mensaje = null;
        }

  
    }
}