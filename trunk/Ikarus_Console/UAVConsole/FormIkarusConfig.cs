using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UAVConsole.USBXpress;
using System.Threading;
using UAVConsole.GoogleMaps;

namespace UAVConsole
{
    public partial class FormIkarusConfig : Form
    {
        FlightPlanUSB planUSB = new FlightPlanUSB();
        StoredConfig sconfig;
        public FormIkarusMain formIk = null;

            
        public FormIkarusConfig()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
              timer1.Enabled = true;
        }

        private void FormIkarusConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            planUSB.Close();
        }

        private void updateForm()
        {
            if (formIk != null)
            {
                formIk.planeState.homeLon = sconfig.HomeLon;
                formIk.planeState.homeLat = sconfig.HomeLat;
                formIk.planeState.Lat = sconfig.HomeLat;
                formIk.planeState.Lon = sconfig.HomeLon;
                formIk.planeState.Alt = sconfig.HomeAltitude;

                formIk.homeWpt.Longitude = sconfig.HomeLon;
                formIk.homeWpt.Latitude = sconfig.HomeLat;
                formIk.homeWpt.Altitude = sconfig.HomeAltitude;

                formIk.planeWpt.Latitude = sconfig.HomeLat;
                formIk.planeWpt.Longitude = sconfig.HomeLon;
                formIk.planeWpt.Altitude = sconfig.HomeAltitude;

                formIk.mapControl1.Refresh();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            while (!planUSB.IsOpen())
            {
                planUSB = new FlightPlanUSB();
                Thread.Sleep(100);
            }
            if (planUSB.IsOpen())
            {
                labelStatus.Text = "Connected!";
                labelStatus.ForeColor = Color.Green;
                sconfig = planUSB.ReadConfig();
                //sconfig.offsetX = 0x2C;// &0x3f;
                //sconfig.offsetY = 0x15;//&0x1f;
                try
                {
                    numericUpDownOffsetX.Value = sconfig.offsetX;
                    numericUpDownOffsetY.Value = sconfig.offsetY;
                    numericUpDownBattCapacity.Value = (decimal)sconfig.total_mAh;

                    numericUpDownHomeAltitude.Value = (decimal)sconfig.HomeAltitude;
                    textBoxHomeLat.Text = sconfig.HomeLat.ToString();
                    textBoxHomeLon.Text = sconfig.HomeLon.ToString();

                    numericUpDownCellsBatt1.Value = sconfig.cellsBatt1;
                    numericUpDownCellsBatt2.Value = sconfig.cellsBatt2;
                    if (sconfig.cellAlarm < 3.0f)
                        sconfig.cellAlarm = 3.0f;
                    numericUpDownCellAlarm.Value = (decimal)sconfig.cellAlarm;

                    numericUpDownAlarmAltitude.Value = (decimal)sconfig.altitudeAlarm;
                    numericUpDownAlarmDistance.Value = (decimal)sconfig.distanceAlarm;
                    numericUpDownAlarmLowSpeed.Value = (decimal)sconfig.lowSpeedAlarm;
                    numericUpDownWptRange.Value = (decimal)sconfig.WptRange;
            
                    updateForm();
                } catch (Exception){}
                timer1.Enabled = false;
                panel1.Enabled = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            sconfig.offsetX = (byte)numericUpDownOffsetX.Value;
            sconfig.offsetY = (byte)numericUpDownOffsetY.Value;
            
            sconfig.total_mAh = (float)numericUpDownBattCapacity.Value;
            sconfig.cellsBatt1 = (byte)numericUpDownCellsBatt1.Value;
            sconfig.cellsBatt2 = (byte)numericUpDownCellsBatt2.Value;
            sconfig.cellAlarm = (float)numericUpDownCellAlarm.Value;
            
            sconfig.HomeAltitude=(float)numericUpDownHomeAltitude.Value;

            sconfig.altitudeAlarm = (float)numericUpDownAlarmAltitude.Value;
            sconfig.distanceAlarm = (float)numericUpDownAlarmDistance.Value;
            sconfig.lowSpeedAlarm = (float)numericUpDownAlarmLowSpeed.Value;
            sconfig.WptRange = (float)numericUpDownWptRange.Value;
            try
            {
                sconfig.HomeLon = float.Parse(textBoxHomeLon.Text);
                sconfig.HomeLat = float.Parse(textBoxHomeLat.Text);
            }
            catch (Exception) { }

            planUSB.WriteConfig(sconfig);
            updateForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WayPoint wpt=planUSB.ReadGPS();
            if (wpt != null)
            {
                textBoxHomeLat.Text = wpt.Latitude.ToString();
                textBoxHomeLon.Text = wpt.Longitude.ToString();
                numericUpDownHomeAltitude.Value = (decimal)wpt.Altitude;
                updateForm();
            }

        }
    }
}