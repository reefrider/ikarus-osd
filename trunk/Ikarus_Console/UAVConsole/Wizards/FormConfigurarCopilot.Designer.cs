namespace UAVConsole.Wizards
{
    partial class FormConfigurarCopilot
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelP = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonSetOffset = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonSetPitchSensor = new System.Windows.Forms.Button();
            this.lblPitch1 = new System.Windows.Forms.Label();
            this.lblPitch2 = new System.Windows.Forms.Label();
            this.lblRoll2 = new System.Windows.Forms.Label();
            this.lblRoll1 = new System.Windows.Forms.Label();
            this.buttonSetRollSensor = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonFIN = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Image = global::UAVConsole.Properties.Resources.Copilot;
            this.pictureBox1.Location = new System.Drawing.Point(311, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(174, 171);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint_1);
            // 
            // labelP
            // 
            this.labelP.AutoSize = true;
            this.labelP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelP.Location = new System.Drawing.Point(372, 186);
            this.labelP.Name = "labelP";
            this.labelP.Size = new System.Drawing.Size(58, 20);
            this.labelP.TabIndex = 1;
            this.labelP.Text = "P: 1.66";
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelR.Location = new System.Drawing.Point(246, 87);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(60, 20);
            this.labelR.TabIndex = 2;
            this.labelR.Text = "R: 1.66";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.Location = new System.Drawing.Point(12, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Offsets";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "P:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Location = new System.Drawing.Point(36, 52);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1.TabIndex = 12;
            this.numericUpDown1.Value = new decimal(new int[] {
            166,
            0,
            0,
            131072});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DecimalPlaces = 2;
            this.numericUpDown2.Location = new System.Drawing.Point(122, 52);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown2.TabIndex = 14;
            this.numericUpDown2.Value = new decimal(new int[] {
            166,
            0,
            0,
            131072});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(99, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "R:";
            // 
            // buttonSetOffset
            // 
            this.buttonSetOffset.Enabled = false;
            this.buttonSetOffset.Location = new System.Drawing.Point(211, 50);
            this.buttonSetOffset.Name = "buttonSetOffset";
            this.buttonSetOffset.Size = new System.Drawing.Size(32, 25);
            this.buttonSetOffset.TabIndex = 15;
            this.buttonSetOffset.Text = "Set";
            this.buttonSetOffset.UseVisualStyleBackColor = true;
            this.buttonSetOffset.Click += new System.EventHandler(this.button2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label7.Location = new System.Drawing.Point(12, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 20);
            this.label7.TabIndex = 16;
            this.label7.Text = "Pitch Down";
            // 
            // buttonSetPitchSensor
            // 
            this.buttonSetPitchSensor.Enabled = false;
            this.buttonSetPitchSensor.Location = new System.Drawing.Point(211, 122);
            this.buttonSetPitchSensor.Name = "buttonSetPitchSensor";
            this.buttonSetPitchSensor.Size = new System.Drawing.Size(32, 25);
            this.buttonSetPitchSensor.TabIndex = 17;
            this.buttonSetPitchSensor.Text = "Set";
            this.buttonSetPitchSensor.UseVisualStyleBackColor = true;
            this.buttonSetPitchSensor.Click += new System.EventHandler(this.buttonSetPitchSensor_Click);
            // 
            // lblPitch1
            // 
            this.lblPitch1.AutoSize = true;
            this.lblPitch1.BackColor = System.Drawing.SystemColors.Window;
            this.lblPitch1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPitch1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblPitch1.Location = new System.Drawing.Point(36, 122);
            this.lblPitch1.Name = "lblPitch1";
            this.lblPitch1.Size = new System.Drawing.Size(49, 22);
            this.lblPitch1.TabIndex = 18;
            this.lblPitch1.Text = "Right";
            // 
            // lblPitch2
            // 
            this.lblPitch2.AutoSize = true;
            this.lblPitch2.BackColor = System.Drawing.SystemColors.Window;
            this.lblPitch2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPitch2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblPitch2.Location = new System.Drawing.Point(122, 122);
            this.lblPitch2.Name = "lblPitch2";
            this.lblPitch2.Size = new System.Drawing.Size(49, 22);
            this.lblPitch2.TabIndex = 19;
            this.lblPitch2.Text = "Right";
            // 
            // lblRoll2
            // 
            this.lblRoll2.AutoSize = true;
            this.lblRoll2.BackColor = System.Drawing.SystemColors.Window;
            this.lblRoll2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRoll2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblRoll2.Location = new System.Drawing.Point(122, 201);
            this.lblRoll2.Name = "lblRoll2";
            this.lblRoll2.Size = new System.Drawing.Size(49, 22);
            this.lblRoll2.TabIndex = 23;
            this.lblRoll2.Text = "Right";
            // 
            // lblRoll1
            // 
            this.lblRoll1.AutoSize = true;
            this.lblRoll1.BackColor = System.Drawing.SystemColors.Window;
            this.lblRoll1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRoll1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblRoll1.Location = new System.Drawing.Point(36, 201);
            this.lblRoll1.Name = "lblRoll1";
            this.lblRoll1.Size = new System.Drawing.Size(49, 22);
            this.lblRoll1.TabIndex = 22;
            this.lblRoll1.Text = "Right";
            // 
            // buttonSetRollSensor
            // 
            this.buttonSetRollSensor.Enabled = false;
            this.buttonSetRollSensor.Location = new System.Drawing.Point(211, 198);
            this.buttonSetRollSensor.Name = "buttonSetRollSensor";
            this.buttonSetRollSensor.Size = new System.Drawing.Size(32, 25);
            this.buttonSetRollSensor.TabIndex = 21;
            this.buttonSetRollSensor.Text = "Set";
            this.buttonSetRollSensor.UseVisualStyleBackColor = true;
            this.buttonSetRollSensor.Click += new System.EventHandler(this.buttonSetRollSensor_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label12.Location = new System.Drawing.Point(12, 172);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 20);
            this.label12.TabIndex = 20;
            this.label12.Text = "Roll Right";
            // 
            // buttonFIN
            // 
            this.buttonFIN.Enabled = false;
            this.buttonFIN.Location = new System.Drawing.Point(407, 214);
            this.buttonFIN.Name = "buttonFIN";
            this.buttonFIN.Size = new System.Drawing.Size(78, 25);
            this.buttonFIN.TabIndex = 24;
            this.buttonFIN.Text = "FIN";
            this.buttonFIN.UseVisualStyleBackColor = true;
            this.buttonFIN.Click += new System.EventHandler(this.buttonFIN_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 333;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormConfigurarCopilot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 252);
            this.Controls.Add(this.buttonFIN);
            this.Controls.Add(this.lblRoll2);
            this.Controls.Add(this.lblRoll1);
            this.Controls.Add(this.buttonSetRollSensor);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblPitch2);
            this.Controls.Add(this.lblPitch1);
            this.Controls.Add(this.buttonSetPitchSensor);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonSetOffset);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelR);
            this.Controls.Add(this.labelP);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigurarCopilot";
            this.Text = "FormConfigurarCopilot";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelP;
        private System.Windows.Forms.Label labelR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonSetOffset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonSetPitchSensor;
        private System.Windows.Forms.Label lblPitch1;
        private System.Windows.Forms.Label lblPitch2;
        private System.Windows.Forms.Label lblRoll2;
        private System.Windows.Forms.Label lblRoll1;
        private System.Windows.Forms.Button buttonSetRollSensor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonFIN;
        private System.Windows.Forms.Timer timer1;
    }
}