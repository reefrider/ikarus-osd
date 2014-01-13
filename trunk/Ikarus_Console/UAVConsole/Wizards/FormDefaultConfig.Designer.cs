namespace UAVConsole.Wizards
{
    partial class FormDefaultConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxGPSBaudRate = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxVideoSystem = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.comboBoxTipoMezcla = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.comboBoxModoPPM = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.comboBoxCanalPPM = new System.Windows.Forms.ComboBox();
            this.checkBoxActualizarCharSet = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxActualizarHUDs = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(38, 80);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 13);
            this.label18.TabIndex = 57;
            this.label18.Text = "GPS baud rate";
            // 
            // comboBoxGPSBaudRate
            // 
            this.comboBoxGPSBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGPSBaudRate.FormattingEnabled = true;
            this.comboBoxGPSBaudRate.Items.AddRange(new object[] {
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600"});
            this.comboBoxGPSBaudRate.Location = new System.Drawing.Point(125, 77);
            this.comboBoxGPSBaudRate.Name = "comboBoxGPSBaudRate";
            this.comboBoxGPSBaudRate.Size = new System.Drawing.Size(108, 21);
            this.comboBoxGPSBaudRate.TabIndex = 56;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(36, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 13);
            this.label16.TabIndex = 55;
            this.label16.Text = "Video System";
            // 
            // comboBoxVideoSystem
            // 
            this.comboBoxVideoSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVideoSystem.FormattingEnabled = true;
            this.comboBoxVideoSystem.Items.AddRange(new object[] {
            "PAL",
            "NTSC"});
            this.comboBoxVideoSystem.Location = new System.Drawing.Point(125, 48);
            this.comboBoxVideoSystem.Name = "comboBoxVideoSystem";
            this.comboBoxVideoSystem.Size = new System.Drawing.Size(54, 21);
            this.comboBoxVideoSystem.TabIndex = 54;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(39, 167);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(41, 13);
            this.label28.TabIndex = 131;
            this.label28.Text = "Mezcla";
            // 
            // comboBoxTipoMezcla
            // 
            this.comboBoxTipoMezcla.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTipoMezcla.FormattingEnabled = true;
            this.comboBoxTipoMezcla.Items.AddRange(new object[] {
            "Normal",
            "Elevon (AIL+ELE)",
            "V-Tail (ELE+TAIL)"});
            this.comboBoxTipoMezcla.Location = new System.Drawing.Point(125, 164);
            this.comboBoxTipoMezcla.Name = "comboBoxTipoMezcla";
            this.comboBoxTipoMezcla.Size = new System.Drawing.Size(102, 21);
            this.comboBoxTipoMezcla.TabIndex = 130;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(38, 109);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(80, 13);
            this.label26.TabIndex = 133;
            this.label26.Text = "Entrada Control";
            // 
            // comboBoxModoPPM
            // 
            this.comboBoxModoPPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModoPPM.FormattingEnabled = true;
            this.comboBoxModoPPM.Items.AddRange(new object[] {
            "Normal",
            "PPM"});
            this.comboBoxModoPPM.Location = new System.Drawing.Point(125, 106);
            this.comboBoxModoPPM.Name = "comboBoxModoPPM";
            this.comboBoxModoPPM.Size = new System.Drawing.Size(108, 21);
            this.comboBoxModoPPM.TabIndex = 132;
            this.comboBoxModoPPM.SelectedIndexChanged += new System.EventHandler(this.comboBoxModoPPM_SelectedIndexChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(38, 138);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(60, 13);
            this.label27.TabIndex = 135;
            this.label27.Text = "Canal PPM";
            // 
            // comboBoxCanalPPM
            // 
            this.comboBoxCanalPPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCanalPPM.FormattingEnabled = true;
            this.comboBoxCanalPPM.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.comboBoxCanalPPM.Location = new System.Drawing.Point(125, 135);
            this.comboBoxCanalPPM.Name = "comboBoxCanalPPM";
            this.comboBoxCanalPPM.Size = new System.Drawing.Size(108, 21);
            this.comboBoxCanalPPM.TabIndex = 134;
            // 
            // checkBoxActualizarCharSet
            // 
            this.checkBoxActualizarCharSet.AutoSize = true;
            this.checkBoxActualizarCharSet.Checked = true;
            this.checkBoxActualizarCharSet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxActualizarCharSet.Location = new System.Drawing.Point(42, 218);
            this.checkBoxActualizarCharSet.Name = "checkBoxActualizarCharSet";
            this.checkBoxActualizarCharSet.Size = new System.Drawing.Size(156, 17);
            this.checkBoxActualizarCharSet.TabIndex = 136;
            this.checkBoxActualizarCharSet.Text = "Actualizar Tabla Caracteres";
            this.checkBoxActualizarCharSet.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 23);
            this.button1.TabIndex = 137;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.Location = new System.Drawing.Point(23, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 20);
            this.label1.TabIndex = 138;
            this.label1.Text = "Default Config";
            // 
            // checkBoxActualizarHUDs
            // 
            this.checkBoxActualizarHUDs.AutoSize = true;
            this.checkBoxActualizarHUDs.Checked = true;
            this.checkBoxActualizarHUDs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxActualizarHUDs.Location = new System.Drawing.Point(42, 195);
            this.checkBoxActualizarHUDs.Name = "checkBoxActualizarHUDs";
            this.checkBoxActualizarHUDs.Size = new System.Drawing.Size(145, 17);
            this.checkBoxActualizarHUDs.TabIndex = 139;
            this.checkBoxActualizarHUDs.Text = "Actualizar Pantallas HUD";
            this.checkBoxActualizarHUDs.UseVisualStyleBackColor = true;
            // 
            // FormDefaultConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.checkBoxActualizarHUDs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBoxActualizarCharSet);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.comboBoxCanalPPM);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.comboBoxModoPPM);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.comboBoxTipoMezcla);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.comboBoxGPSBaudRate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.comboBoxVideoSystem);
            this.Name = "FormDefaultConfig";
            this.Text = "FormDefaultConfig";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox comboBoxGPSBaudRate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBoxVideoSystem;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox comboBoxTipoMezcla;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox comboBoxModoPPM;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.ComboBox comboBoxCanalPPM;
        private System.Windows.Forms.CheckBox checkBoxActualizarCharSet;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxActualizarHUDs;

    }
}