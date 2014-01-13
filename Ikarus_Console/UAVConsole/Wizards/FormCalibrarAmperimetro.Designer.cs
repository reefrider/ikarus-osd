namespace UAVConsole.Wizards
{
    partial class FormCalibrarAmperimetro
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.textBoxMotorI2 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.numericUpDownMotorI2 = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMotorI1 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.numericUpDownMotorI1 = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button8 = new System.Windows.Forms.Button();
            this.textBoxMotorV2 = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.numericUpDownMotorV2 = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxMotorV1 = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.numericUpDownMotorV1 = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownVideoV1 = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxVideoV1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownVideoV2 = new System.Windows.Forms.NumericUpDown();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxVideoV2 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorI2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorI1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorV2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorV1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoV1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoV2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.textBoxMotorI2);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.numericUpDownMotorI2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBoxMotorI1);
            this.groupBox2.Controls.Add(this.button7);
            this.groupBox2.Controls.Add(this.numericUpDownMotorI1);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(12, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(488, 65);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I Motor";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(425, 24);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(57, 25);
            this.button5.TabIndex = 46;
            this.button5.Text = "Save";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBoxMotorI2
            // 
            this.textBoxMotorI2.Enabled = false;
            this.textBoxMotorI2.Location = new System.Drawing.Point(330, 26);
            this.textBoxMotorI2.Name = "textBoxMotorI2";
            this.textBoxMotorI2.Size = new System.Drawing.Size(46, 20);
            this.textBoxMotorI2.TabIndex = 45;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(382, 23);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(40, 25);
            this.button6.TabIndex = 44;
            this.button6.Text = "Set";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // numericUpDownMotorI2
            // 
            this.numericUpDownMotorI2.Location = new System.Drawing.Point(274, 26);
            this.numericUpDownMotorI2.Name = "numericUpDownMotorI2";
            this.numericUpDownMotorI2.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownMotorI2.TabIndex = 43;
            this.numericUpDownMotorI2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMotorI2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMotorI2.ValueChanged += new System.EventHandler(this.numericUpDownMotorI2_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(229, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "Read 2";
            // 
            // textBoxMotorI1
            // 
            this.textBoxMotorI1.Enabled = false;
            this.textBoxMotorI1.Location = new System.Drawing.Point(116, 25);
            this.textBoxMotorI1.Name = "textBoxMotorI1";
            this.textBoxMotorI1.Size = new System.Drawing.Size(46, 20);
            this.textBoxMotorI1.TabIndex = 41;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(166, 22);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(40, 25);
            this.button7.TabIndex = 40;
            this.button7.Text = "Set";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // numericUpDownMotorI1
            // 
            this.numericUpDownMotorI1.Location = new System.Drawing.Point(60, 24);
            this.numericUpDownMotorI1.Name = "numericUpDownMotorI1";
            this.numericUpDownMotorI1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownMotorI1.TabIndex = 39;
            this.numericUpDownMotorI1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMotorI1.ValueChanged += new System.EventHandler(this.numericUpDownMotorI1_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 38;
            this.label9.Text = "Read 1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button8);
            this.groupBox3.Controls.Add(this.textBoxMotorV2);
            this.groupBox3.Controls.Add(this.button9);
            this.groupBox3.Controls.Add(this.numericUpDownMotorV2);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textBoxMotorV1);
            this.groupBox3.Controls.Add(this.button10);
            this.groupBox3.Controls.Add(this.numericUpDownMotorV1);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(12, 83);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(488, 65);
            this.groupBox3.TabIndex = 47;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "V Motor";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(425, 24);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(57, 25);
            this.button8.TabIndex = 46;
            this.button8.Text = "Save";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textBoxMotorV2
            // 
            this.textBoxMotorV2.Enabled = false;
            this.textBoxMotorV2.Location = new System.Drawing.Point(330, 26);
            this.textBoxMotorV2.Name = "textBoxMotorV2";
            this.textBoxMotorV2.Size = new System.Drawing.Size(46, 20);
            this.textBoxMotorV2.TabIndex = 45;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(382, 23);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(40, 25);
            this.button9.TabIndex = 44;
            this.button9.Text = "Set";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // numericUpDownMotorV2
            // 
            this.numericUpDownMotorV2.DecimalPlaces = 2;
            this.numericUpDownMotorV2.Increment = new decimal(new int[] {
            42,
            0,
            0,
            65536});
            this.numericUpDownMotorV2.Location = new System.Drawing.Point(274, 26);
            this.numericUpDownMotorV2.Maximum = new decimal(new int[] {
            252,
            0,
            0,
            65536});
            this.numericUpDownMotorV2.Name = "numericUpDownMotorV2";
            this.numericUpDownMotorV2.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownMotorV2.TabIndex = 43;
            this.numericUpDownMotorV2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMotorV2.ValueChanged += new System.EventHandler(this.numericUpDownMotorV2_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(229, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Read 2";
            // 
            // textBoxMotorV1
            // 
            this.textBoxMotorV1.Enabled = false;
            this.textBoxMotorV1.Location = new System.Drawing.Point(116, 25);
            this.textBoxMotorV1.Name = "textBoxMotorV1";
            this.textBoxMotorV1.Size = new System.Drawing.Size(46, 20);
            this.textBoxMotorV1.TabIndex = 41;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(166, 22);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(40, 25);
            this.button10.TabIndex = 40;
            this.button10.Text = "Set";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // numericUpDownMotorV1
            // 
            this.numericUpDownMotorV1.DecimalPlaces = 2;
            this.numericUpDownMotorV1.Increment = new decimal(new int[] {
            42,
            0,
            0,
            65536});
            this.numericUpDownMotorV1.Location = new System.Drawing.Point(60, 24);
            this.numericUpDownMotorV1.Maximum = new decimal(new int[] {
            252,
            0,
            0,
            65536});
            this.numericUpDownMotorV1.Name = "numericUpDownMotorV1";
            this.numericUpDownMotorV1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownMotorV1.TabIndex = 39;
            this.numericUpDownMotorV1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMotorV1.Value = new decimal(new int[] {
            126,
            0,
            0,
            65536});
            this.numericUpDownMotorV1.ValueChanged += new System.EventHandler(this.numericUpDownMotorV1_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 38;
            this.label11.Text = "Read 1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Read 1";
            // 
            // numericUpDownVideoV1
            // 
            this.numericUpDownVideoV1.DecimalPlaces = 2;
            this.numericUpDownVideoV1.Increment = new decimal(new int[] {
            42,
            0,
            0,
            65536});
            this.numericUpDownVideoV1.Location = new System.Drawing.Point(60, 24);
            this.numericUpDownVideoV1.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            65536});
            this.numericUpDownVideoV1.Name = "numericUpDownVideoV1";
            this.numericUpDownVideoV1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownVideoV1.TabIndex = 39;
            this.numericUpDownVideoV1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownVideoV1.Value = new decimal(new int[] {
            126,
            0,
            0,
            65536});
            this.numericUpDownVideoV1.ValueChanged += new System.EventHandler(this.numericUpDownVideoV1_ValueChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(166, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 25);
            this.button2.TabIndex = 40;
            this.button2.Text = "Set";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxVideoV1
            // 
            this.textBoxVideoV1.Enabled = false;
            this.textBoxVideoV1.Location = new System.Drawing.Point(116, 25);
            this.textBoxVideoV1.Name = "textBoxVideoV1";
            this.textBoxVideoV1.Size = new System.Drawing.Size(46, 20);
            this.textBoxVideoV1.TabIndex = 41;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(229, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "Read 2";
            // 
            // numericUpDownVideoV2
            // 
            this.numericUpDownVideoV2.DecimalPlaces = 2;
            this.numericUpDownVideoV2.Increment = new decimal(new int[] {
            42,
            0,
            0,
            65536});
            this.numericUpDownVideoV2.Location = new System.Drawing.Point(274, 26);
            this.numericUpDownVideoV2.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            65536});
            this.numericUpDownVideoV2.Name = "numericUpDownVideoV2";
            this.numericUpDownVideoV2.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownVideoV2.TabIndex = 43;
            this.numericUpDownVideoV2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownVideoV2.ValueChanged += new System.EventHandler(this.numericUpDownVideoV2_ValueChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(382, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(40, 25);
            this.button3.TabIndex = 44;
            this.button3.Text = "Set";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBoxVideoV2
            // 
            this.textBoxVideoV2.Enabled = false;
            this.textBoxVideoV2.Location = new System.Drawing.Point(330, 26);
            this.textBoxVideoV2.Name = "textBoxVideoV2";
            this.textBoxVideoV2.Size = new System.Drawing.Size(46, 20);
            this.textBoxVideoV2.TabIndex = 45;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(425, 24);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(57, 25);
            this.button4.TabIndex = 46;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.textBoxVideoV2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.numericUpDownVideoV2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxVideoV1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.numericUpDownVideoV1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 65);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "V Video";
            // 
            // FormCalibrarAmperimetro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 231);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormCalibrarAmperimetro";
            this.Text = "FormCalibrarAmperimetro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCalibrarAmperimetro_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorI2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorI1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorV2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMotorV1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoV1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoV2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBoxMotorI2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.NumericUpDown numericUpDownMotorI2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMotorI1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.NumericUpDown numericUpDownMotorI1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textBoxMotorV2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.NumericUpDown numericUpDownMotorV2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxMotorV1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.NumericUpDown numericUpDownMotorV1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownVideoV1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBoxVideoV1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownVideoV2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxVideoV2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}