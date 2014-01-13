namespace UAVConsole.Wizards
{
    partial class FormCalibrarRSSI
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
            this.buttonSetOffset = new System.Windows.Forms.Button();
            this.numericUpDownRSSImin = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownRSSImax = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonFIN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRSSImin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRSSImax)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSetOffset
            // 
            this.buttonSetOffset.Location = new System.Drawing.Point(192, 54);
            this.buttonSetOffset.Name = "buttonSetOffset";
            this.buttonSetOffset.Size = new System.Drawing.Size(40, 25);
            this.buttonSetOffset.TabIndex = 21;
            this.buttonSetOffset.Text = "Set";
            this.buttonSetOffset.UseVisualStyleBackColor = true;
            this.buttonSetOffset.Click += new System.EventHandler(this.buttonSetOffset_Click);
            // 
            // numericUpDownRSSImin
            // 
            this.numericUpDownRSSImin.DecimalPlaces = 3;
            this.numericUpDownRSSImin.Location = new System.Drawing.Point(91, 129);
            this.numericUpDownRSSImin.Name = "numericUpDownRSSImin";
            this.numericUpDownRSSImin.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownRSSImin.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Valor MIN:";
            // 
            // numericUpDownRSSImax
            // 
            this.numericUpDownRSSImax.DecimalPlaces = 3;
            this.numericUpDownRSSImax.Location = new System.Drawing.Point(94, 58);
            this.numericUpDownRSSImax.Name = "numericUpDownRSSImax";
            this.numericUpDownRSSImax.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownRSSImax.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Valor MAX:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.Location = new System.Drawing.Point(27, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "Emisora Encendida";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.Location = new System.Drawing.Point(27, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 20);
            this.label1.TabIndex = 22;
            this.label1.Text = "Emisora Apagada";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(192, 125);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 25);
            this.button1.TabIndex = 23;
            this.button1.Text = "Set";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonFIN
            // 
            this.buttonFIN.Location = new System.Drawing.Point(91, 176);
            this.buttonFIN.Name = "buttonFIN";
            this.buttonFIN.Size = new System.Drawing.Size(78, 25);
            this.buttonFIN.TabIndex = 25;
            this.buttonFIN.Text = "FIN";
            this.buttonFIN.UseVisualStyleBackColor = true;
            this.buttonFIN.Click += new System.EventHandler(this.buttonFIN_Click);
            // 
            // FormCalibrarRSSI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 224);
            this.Controls.Add(this.buttonFIN);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSetOffset);
            this.Controls.Add(this.numericUpDownRSSImin);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownRSSImax);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Name = "FormCalibrarRSSI";
            this.Text = "FormCalibrarRSSI";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRSSImin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRSSImax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSetOffset;
        private System.Windows.Forms.NumericUpDown numericUpDownRSSImin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownRSSImax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonFIN;
    }
}