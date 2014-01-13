using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UAVConsole.USBXpress;
using UAVConsole.ConfigClasses;

namespace UAVConsole
{
    public partial class FormConfigurarAntracker : Form
    {
        Singleton me = Singleton.GetInstance();

        AntTrackerConfig antTrackConfig;
        NumericUpDown control_pan;
        NumericUpDown control_tilt;

        bool firstTime = true;

        
        public FormConfigurarAntracker()
        {
            InitializeComponent();
            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            comboBoxNumCellsAntTracker.SelectedIndex = 0;

            timer1.Enabled = true;
        }
        
        void UpdateControles()
        {
            
            try
            {   
                checkBoxEnableGPS.Checked = (antTrackConfig.useInternalGPS == 1);
                numericUpDownAlturaSinGPS.Value = (decimal)antTrackConfig.AltitudSinGPS;

                checkBoxEnableCompas.Checked = (antTrackConfig.useInternalCompas == 1);
                numericUpDownOffsetBrujula.Value = (decimal)antTrackConfig.offsetCompas;
                checkBoxGPSoverrideCompas.Checked = (antTrackConfig.enableCompasOverride == 1);
                numericUpDownGPSoverrideCompasSpeed.Value = (decimal)antTrackConfig.speedCompasOverride;

                checkBoxLipoAntTracker.Checked = (antTrackConfig.useLipo == 1);
                comboBoxNumCellsAntTracker.SelectedIndex = antTrackConfig.numCellsLipo - 2;
                numericUpDownVminAntTracker.Value = (decimal)antTrackConfig.Vmin;
                numericUpDownVmaxAntTracker.Value = (decimal)antTrackConfig.Vmax;
                numericUpDownValarmAntTracker.Value = (decimal)antTrackConfig.Valarm;

                numericUpDownOffsetPAN.Value = (decimal)antTrackConfig.offsetPan;

                checkBoxDecodeTelemetry.Checked = (antTrackConfig.decodeTelemetry == 1);

                checkBoxServo360.Checked = (antTrackConfig.useServo360 == 1);

                numericUpDownServoPANmin2.Value = antTrackConfig.GradosPANleft2.servo;
                numericUpDownServoPANmin.Value = antTrackConfig.GradosPANleft.servo;
                numericUpDownServoPANcenter.Value = antTrackConfig.GradosPANcenter.servo;
                numericUpDownServoPANmax.Value = antTrackConfig.GradosPANright.servo;
                numericUpDownServoPANmax2.Value = antTrackConfig.GradosPANright2.servo;

                numericUpDownGradosPANmin2.Value = (decimal)antTrackConfig.GradosPANleft2.grados;
                numericUpDownGradosPANmin.Value = (decimal)antTrackConfig.GradosPANleft.grados;
                numericUpDownGradosPANcenter.Value = (decimal)antTrackConfig.GradosPANcenter.grados;
                numericUpDownGradosPANmax.Value = (decimal)antTrackConfig.GradosPANright.grados;
                numericUpDownGradosPANmax2.Value = (decimal)antTrackConfig.GradosPANright2.grados;


                numericUpDownServoTILTmin.Value = antTrackConfig.GradosTILTdown.servo;
                numericUpDownServoTILTcenter.Value = antTrackConfig.GradosTILTcenter.servo;
                numericUpDownServoTILTmax.Value = antTrackConfig.GradosTILTup.servo;

                numericUpDownGradosTILTmin.Value = (decimal)antTrackConfig.GradosTILTdown.grados;
                numericUpDownGradosTILTcenter.Value = (decimal)antTrackConfig.GradosTILTcenter.grados;
                numericUpDownGradosTILTmax.Value = (decimal)antTrackConfig.GradosTILTup.grados;

                updateAntTrackerBattery();
            }
            catch (Exception)
            {
                MessageBox.Show("Error reading some values!");
            }
        }

        void UpdateStruct()
        {
            
            try
            {
                
                antTrackConfig.useInternalGPS = (byte)(checkBoxEnableGPS.Checked ? 1 : 0);
                antTrackConfig.AltitudSinGPS = (float) numericUpDownAlturaSinGPS.Value;

                antTrackConfig.useInternalCompas = (byte)(checkBoxEnableCompas.Checked ? 1 : 0);
                antTrackConfig.offsetCompas = (float) numericUpDownOffsetBrujula.Value;
                antTrackConfig.enableCompasOverride = (byte)(checkBoxGPSoverrideCompas.Checked ? 1 : 0);
                antTrackConfig.speedCompasOverride = (float)numericUpDownGPSoverrideCompasSpeed.Value;

                antTrackConfig.useLipo = (byte)(checkBoxLipoAntTracker.Checked ? 1 : 0);
                antTrackConfig.numCellsLipo = (byte)(comboBoxNumCellsAntTracker.SelectedIndex + 2);
                antTrackConfig.Vmin = (float) numericUpDownVminAntTracker.Value;
                antTrackConfig.Vmax = (float) numericUpDownVmaxAntTracker.Value;
                antTrackConfig.Valarm = (float)numericUpDownValarmAntTracker.Value;

                antTrackConfig.offsetPan = (float) numericUpDownOffsetPAN.Value;

                antTrackConfig.decodeTelemetry = (byte)(checkBoxDecodeTelemetry.Checked ? 1 : 0);

                antTrackConfig.useServo360 = (byte)(checkBoxServo360.Checked ? 1 : 0);

                antTrackConfig.GradosPANleft2.servo = (short)numericUpDownServoPANmin2.Value;
                antTrackConfig.GradosPANleft.servo = (short)numericUpDownServoPANmin.Value;
                antTrackConfig.GradosPANcenter.servo = (short)numericUpDownServoPANcenter.Value;
                antTrackConfig.GradosPANright.servo = (short)numericUpDownServoPANmax.Value;
                antTrackConfig.GradosPANright2.servo = (short)numericUpDownServoPANmax2.Value;

                antTrackConfig.GradosPANleft2.grados = (float)numericUpDownGradosPANmin2.Value;
                antTrackConfig.GradosPANleft.grados = (float)numericUpDownGradosPANmin.Value;
                antTrackConfig.GradosPANcenter.grados = (float)numericUpDownGradosPANcenter.Value;
                antTrackConfig.GradosPANright.grados = (float)numericUpDownGradosPANmax.Value;
                antTrackConfig.GradosPANright2.grados = (float)numericUpDownGradosPANmax2.Value;


                antTrackConfig.GradosTILTdown.servo = (short)numericUpDownServoTILTmin.Value;
                antTrackConfig.GradosTILTcenter.servo = (short)numericUpDownServoTILTcenter.Value;
                antTrackConfig.GradosTILTup.servo = (short)numericUpDownServoTILTmax.Value;

                antTrackConfig.GradosTILTdown.grados = (float)numericUpDownGradosTILTmin.Value;
                antTrackConfig.GradosTILTcenter.grados = (float)numericUpDownGradosTILTcenter.Value;
                antTrackConfig.GradosTILTup.grados = (float)numericUpDownGradosTILTmax.Value;
            }
            catch (Exception)
            {
            }
            
        }
        private void checkBoxLipoAntTracker_CheckedChanged(object sender, EventArgs e)
        {
            updateAntTrackerBattery();
        }

        private void updateAntTrackerBattery()
        {
            if (checkBoxLipoAntTracker.Checked)
            {
                comboBoxNumCellsAntTracker.Enabled = true;
                numericUpDownVminAntTracker.Enabled = false;
                numericUpDownVmaxAntTracker.Enabled = false;
                label14.Enabled = false;
                label15.Enabled = false;
            }
            else
            {
                comboBoxNumCellsAntTracker.Enabled = false;
                numericUpDownVminAntTracker.Enabled = true;
                numericUpDownVmaxAntTracker.Enabled = true;
                label14.Enabled = true;
                label15.Enabled = true;
            }
        }

        private void comboBoxNumCellsAntTracker_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num_cells;
            if (checkBoxLipoAntTracker.Enabled)
            {
                if (comboBoxNumCellsAntTracker.SelectedIndex == 0)
                    num_cells = 2;
                else
                    num_cells = 3;

                numericUpDownVminAntTracker.Value = (decimal)(3.2f * num_cells);
                numericUpDownVmaxAntTracker.Value = (decimal)(4.2f * num_cells);
                numericUpDownValarmAntTracker.Value = (decimal)(3.6f * num_cells);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            AntenaTracker antenaTracker = new AntenaTracker();

            if (antenaTracker.IsOpen())
            {
                if (firstTime)
                {
                    antTrackConfig = antenaTracker.ReadConfig();
                    UpdateControles();
                    panel1.Enabled = true;
                    firstTime = false;
                    control_pan = numericUpDownServoPANcenter;
                    control_tilt = numericUpDownServoTILTcenter;
                }
                labelStatus.Text = "Connected!";
                labelStatus.ForeColor = Color.Green;
                AntTrackerDebug dbg = new AntTrackerDebug();
                dbg.pan = (short)control_pan.Value;
                dbg.tilt = (short)control_tilt.Value;
                dbg.EnableDebug = 0x05;
                antenaTracker.WriteDebugInfo(dbg);
                antenaTracker.Close();
            }
            else
            {
                labelStatus.Text = "Not connected.";
                labelStatus.ForeColor = Color.Red;
            }
        }

        void TextSpanish()
        {
        }

        void TextEnglish()
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Config (*.xml)|*.xml";
            dlg.ShowDialog(this);
            if (!dlg.FileName.Equals(""))
            {
                UpdateStruct();
                antTrackConfig.SaveToXml(dlg.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.AddExtension = true;
            dlg.Filter = "Config (*.xml)|*.xml";
            dlg.ShowDialog(this);

            if (!dlg.FileName.Equals(""))
            {
                antTrackConfig.LoadFromXml(dlg.FileName);
                UpdateControles();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            labelStatus.Text = "Closed!";
            labelStatus.ForeColor = Color.DarkRed;

            FormActualizarFirmware form = new FormActualizarFirmware(FormActualizarFirmware.Devices.AntTracker);
            form.Show(this);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            UpdateStruct();
            AntenaTracker antenaTracker = new AntenaTracker();
            antenaTracker.WriteConfig(antTrackConfig);
            antenaTracker.Close();
            if (me.Idioma == (int)Singleton.Idiomas.Spanish)
                MessageBox.Show("Configuración Actualizada");
            else
                MessageBox.Show("Config Updated");
            timer1.Enabled = true;
        }

        private void FormConfigurarAntracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            AntenaTracker antenaTracker = new AntenaTracker();
            if (antenaTracker.IsOpen())
            {
                AntTrackerDebug dbg = new AntTrackerDebug();
                dbg.EnableDebug = 0x00;
                antenaTracker.WriteDebugInfo(dbg);
                antenaTracker.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AntenaTracker antenaTracker = new AntenaTracker();
            antTrackConfig = antenaTracker.ReadConfig();
            antenaTracker.Close();
            
            UpdateControles();    
        }

        private void checkBoxEnableCompas_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGPSCompasOptions();
        }

        private void UpdateGPSCompasOptions()
        {
            if (checkBoxEnableCompas.Checked)
            {
                label11.Enabled = true;
                numericUpDownOffsetBrujula.Enabled = true;
             }
            else
            {
                label11.Enabled = false;
                numericUpDownOffsetBrujula.Enabled = false;
                numericUpDownGPSoverrideCompasSpeed.Enabled = false;
                checkBoxGPSoverrideCompas.Enabled = false;
            }

            if (checkBoxEnableGPS.Checked)
            {
                label17.Enabled = false;
                numericUpDownAlturaSinGPS.Enabled = false;
            }
            else
            {
                label17.Enabled = true;
                numericUpDownAlturaSinGPS.Enabled = true;
            }

            if (checkBoxEnableGPS.Checked && checkBoxEnableCompas.Checked)
            {
                numericUpDownGPSoverrideCompasSpeed.Enabled = true;
                checkBoxGPSoverrideCompas.Enabled = true;
            }
            else
            {
                numericUpDownGPSoverrideCompasSpeed.Enabled = false;
                checkBoxGPSoverrideCompas.Enabled = false;
            }
        }

        private void checkBoxEnableGPS_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGPSCompasOptions();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxServo360.Checked)
            {
                numericUpDownGradosPANmax2.Visible = true;
                numericUpDownServoPANmax2.Visible = true;

                numericUpDownGradosPANmin2.Visible = true;
                numericUpDownServoPANmin2.Visible = true;

                numericUpDownGradosPANmax.Value = 180;
                numericUpDownGradosPANmin.Value = -180;
            }
            else
            {
                numericUpDownGradosPANmax2.Visible = false;
                numericUpDownServoPANmax2.Visible = false;

                numericUpDownGradosPANmin2.Visible = false;
                numericUpDownServoPANmin2.Visible = false;

                numericUpDownGradosPANmax.Value = 90;
                numericUpDownGradosPANmin.Value = -90;
        
            }
        }

        private void numericUpDownServoTILTcenter_Enter(object sender, EventArgs e)
        {
            control_tilt = numericUpDownServoTILTcenter;
        }

        private void numericUpDownServoTILTmin_Enter(object sender, EventArgs e)
        {
            control_tilt = numericUpDownServoTILTmin;
        }

        private void numericUpDownServoTILTmax_Enter(object sender, EventArgs e)
        {
            control_tilt = numericUpDownServoTILTmax;
        }

        private void numericUpDownServoPANmin2_Enter(object sender, EventArgs e)
        {
            control_pan = numericUpDownServoPANmin2;
        }

        private void numericUpDownServoPANmin_Enter(object sender, EventArgs e)
        {
            control_pan = numericUpDownServoPANmin;
        }

        private void numericUpDownServoPANcenter_Enter(object sender, EventArgs e)
        {
            control_pan = numericUpDownServoPANcenter;
        }

        private void numericUpDownServoPANmax_Enter(object sender, EventArgs e)
        {
            control_pan = numericUpDownServoPANmax;
        }

        private void numericUpDownServoPANmax2_Enter(object sender, EventArgs e)
        {
            control_pan = numericUpDownServoPANmax2;
        }


    }
}
