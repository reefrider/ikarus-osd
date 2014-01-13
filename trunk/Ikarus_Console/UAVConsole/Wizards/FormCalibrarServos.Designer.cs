namespace UAVConsole.Wizards
{
    partial class FormCalibrarServos
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.labelCtrlMin = new System.Windows.Forms.Label();
            this.labelAilMin = new System.Windows.Forms.Label();
            this.labelEleMin = new System.Windows.Forms.Label();
            this.labelThrMin = new System.Windows.Forms.Label();
            this.labelTailMin = new System.Windows.Forms.Label();
            this.labelTailMax = new System.Windows.Forms.Label();
            this.labelThrMax = new System.Windows.Forms.Label();
            this.labelEleMax = new System.Windows.Forms.Label();
            this.labelAilMax = new System.Windows.Forms.Label();
            this.labelCtrlMax = new System.Windows.Forms.Label();
            this.indicadorSlider7 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider6 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider5 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider4 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider3 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider2 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.indicadorSlider1 = new UAVConsole.Intrumentos.IndicadorSlider();
            this.labelAuxMin = new System.Windows.Forms.Label();
            this.labelAuxMax = new System.Windows.Forms.Label();
            this.labelPanMax = new System.Windows.Forms.Label();
            this.labelPanMin = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Control";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Alerones";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Elevador";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Motor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Cola";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(19, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Pan";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(19, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "Aux";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(257, 249);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 26);
            this.button1.TabIndex = 17;
            this.button1.Text = "Update";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(22, 249);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 26);
            this.button2.TabIndex = 18;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelCtrlMin
            // 
            this.labelCtrlMin.AutoSize = true;
            this.labelCtrlMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCtrlMin.Location = new System.Drawing.Point(100, 12);
            this.labelCtrlMin.Name = "labelCtrlMin";
            this.labelCtrlMin.Size = new System.Drawing.Size(40, 18);
            this.labelCtrlMin.TabIndex = 19;
            this.labelCtrlMin.Text = "1000";
            // 
            // labelAilMin
            // 
            this.labelAilMin.AutoSize = true;
            this.labelAilMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAilMin.Location = new System.Drawing.Point(100, 39);
            this.labelAilMin.Name = "labelAilMin";
            this.labelAilMin.Size = new System.Drawing.Size(40, 18);
            this.labelAilMin.TabIndex = 20;
            this.labelAilMin.Text = "1000";
            // 
            // labelEleMin
            // 
            this.labelEleMin.AutoSize = true;
            this.labelEleMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEleMin.Location = new System.Drawing.Point(100, 66);
            this.labelEleMin.Name = "labelEleMin";
            this.labelEleMin.Size = new System.Drawing.Size(40, 18);
            this.labelEleMin.TabIndex = 21;
            this.labelEleMin.Text = "1000";
            // 
            // labelThrMin
            // 
            this.labelThrMin.AutoSize = true;
            this.labelThrMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelThrMin.Location = new System.Drawing.Point(100, 93);
            this.labelThrMin.Name = "labelThrMin";
            this.labelThrMin.Size = new System.Drawing.Size(40, 18);
            this.labelThrMin.TabIndex = 22;
            this.labelThrMin.Text = "1000";
            // 
            // labelTailMin
            // 
            this.labelTailMin.AutoSize = true;
            this.labelTailMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTailMin.Location = new System.Drawing.Point(100, 120);
            this.labelTailMin.Name = "labelTailMin";
            this.labelTailMin.Size = new System.Drawing.Size(40, 18);
            this.labelTailMin.TabIndex = 23;
            this.labelTailMin.Text = "1000";
            // 
            // labelTailMax
            // 
            this.labelTailMax.AutoSize = true;
            this.labelTailMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTailMax.Location = new System.Drawing.Point(286, 121);
            this.labelTailMax.Name = "labelTailMax";
            this.labelTailMax.Size = new System.Drawing.Size(40, 18);
            this.labelTailMax.TabIndex = 28;
            this.labelTailMax.Text = "2000";
            // 
            // labelThrMax
            // 
            this.labelThrMax.AutoSize = true;
            this.labelThrMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelThrMax.Location = new System.Drawing.Point(286, 94);
            this.labelThrMax.Name = "labelThrMax";
            this.labelThrMax.Size = new System.Drawing.Size(40, 18);
            this.labelThrMax.TabIndex = 27;
            this.labelThrMax.Text = "2000";
            // 
            // labelEleMax
            // 
            this.labelEleMax.AutoSize = true;
            this.labelEleMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEleMax.Location = new System.Drawing.Point(286, 67);
            this.labelEleMax.Name = "labelEleMax";
            this.labelEleMax.Size = new System.Drawing.Size(40, 18);
            this.labelEleMax.TabIndex = 26;
            this.labelEleMax.Text = "2000";
            // 
            // labelAilMax
            // 
            this.labelAilMax.AutoSize = true;
            this.labelAilMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAilMax.Location = new System.Drawing.Point(286, 40);
            this.labelAilMax.Name = "labelAilMax";
            this.labelAilMax.Size = new System.Drawing.Size(40, 18);
            this.labelAilMax.TabIndex = 25;
            this.labelAilMax.Text = "2000";
            // 
            // labelCtrlMax
            // 
            this.labelCtrlMax.AutoSize = true;
            this.labelCtrlMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCtrlMax.Location = new System.Drawing.Point(286, 13);
            this.labelCtrlMax.Name = "labelCtrlMax";
            this.labelCtrlMax.Size = new System.Drawing.Size(40, 18);
            this.labelCtrlMax.TabIndex = 24;
            this.labelCtrlMax.Text = "2000";
            // 
            // indicadorSlider7
            // 
            this.indicadorSlider7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider7.Location = new System.Drawing.Point(146, 172);
            this.indicadorSlider7.Name = "indicadorSlider7";
            this.indicadorSlider7.PosFin = 0.5F;
            this.indicadorSlider7.PosInicio = 0.5F;
            this.indicadorSlider7.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider7.TabIndex = 13;
            this.indicadorSlider7.Texto = "1500";
            // 
            // indicadorSlider6
            // 
            this.indicadorSlider6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider6.Location = new System.Drawing.Point(146, 145);
            this.indicadorSlider6.Name = "indicadorSlider6";
            this.indicadorSlider6.PosFin = 0.5F;
            this.indicadorSlider6.PosInicio = 0.5F;
            this.indicadorSlider6.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider6.TabIndex = 11;
            this.indicadorSlider6.Texto = "1500";
            // 
            // indicadorSlider5
            // 
            this.indicadorSlider5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider5.Location = new System.Drawing.Point(146, 118);
            this.indicadorSlider5.Name = "indicadorSlider5";
            this.indicadorSlider5.PosFin = 0.5F;
            this.indicadorSlider5.PosInicio = 0.5F;
            this.indicadorSlider5.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider5.TabIndex = 9;
            this.indicadorSlider5.Texto = "1500";
            // 
            // indicadorSlider4
            // 
            this.indicadorSlider4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider4.Location = new System.Drawing.Point(146, 91);
            this.indicadorSlider4.Name = "indicadorSlider4";
            this.indicadorSlider4.PosFin = 0.5F;
            this.indicadorSlider4.PosInicio = 0.5F;
            this.indicadorSlider4.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider4.TabIndex = 7;
            this.indicadorSlider4.Texto = "1500";
            // 
            // indicadorSlider3
            // 
            this.indicadorSlider3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider3.Location = new System.Drawing.Point(146, 64);
            this.indicadorSlider3.Name = "indicadorSlider3";
            this.indicadorSlider3.PosFin = 0.5F;
            this.indicadorSlider3.PosInicio = 0.5F;
            this.indicadorSlider3.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider3.TabIndex = 5;
            this.indicadorSlider3.Texto = "1500";
            // 
            // indicadorSlider2
            // 
            this.indicadorSlider2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider2.Location = new System.Drawing.Point(146, 37);
            this.indicadorSlider2.Name = "indicadorSlider2";
            this.indicadorSlider2.PosFin = 0.5F;
            this.indicadorSlider2.PosInicio = 0.5F;
            this.indicadorSlider2.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider2.TabIndex = 3;
            this.indicadorSlider2.Texto = "1500";
            // 
            // indicadorSlider1
            // 
            this.indicadorSlider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indicadorSlider1.Location = new System.Drawing.Point(146, 10);
            this.indicadorSlider1.Name = "indicadorSlider1";
            this.indicadorSlider1.PosFin = 0.5F;
            this.indicadorSlider1.PosInicio = 0.5F;
            this.indicadorSlider1.Size = new System.Drawing.Size(130, 22);
            this.indicadorSlider1.TabIndex = 1;
            this.indicadorSlider1.Texto = "1500";
            // 
            // labelAuxMin
            // 
            this.labelAuxMin.AutoSize = true;
            this.labelAuxMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAuxMin.Location = new System.Drawing.Point(100, 174);
            this.labelAuxMin.Name = "labelAuxMin";
            this.labelAuxMin.Size = new System.Drawing.Size(40, 18);
            this.labelAuxMin.TabIndex = 29;
            this.labelAuxMin.Text = "1000";
            // 
            // labelAuxMax
            // 
            this.labelAuxMax.AutoSize = true;
            this.labelAuxMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAuxMax.Location = new System.Drawing.Point(286, 175);
            this.labelAuxMax.Name = "labelAuxMax";
            this.labelAuxMax.Size = new System.Drawing.Size(40, 18);
            this.labelAuxMax.TabIndex = 30;
            this.labelAuxMax.Text = "2000";
            // 
            // labelPanMax
            // 
            this.labelPanMax.AutoSize = true;
            this.labelPanMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPanMax.Location = new System.Drawing.Point(286, 148);
            this.labelPanMax.Name = "labelPanMax";
            this.labelPanMax.Size = new System.Drawing.Size(40, 18);
            this.labelPanMax.TabIndex = 31;
            this.labelPanMax.Text = "2000";
            // 
            // labelPanMin
            // 
            this.labelPanMin.AutoSize = true;
            this.labelPanMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPanMin.Location = new System.Drawing.Point(100, 148);
            this.labelPanMin.Name = "labelPanMin";
            this.labelPanMin.Size = new System.Drawing.Size(40, 18);
            this.labelPanMin.TabIndex = 32;
            this.labelPanMin.Text = "1000";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(128, 249);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(95, 26);
            this.button3.TabIndex = 33;
            this.button3.Text = "Update Centers";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FormCalibrarServos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 287);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.labelPanMin);
            this.Controls.Add(this.labelPanMax);
            this.Controls.Add(this.labelAuxMax);
            this.Controls.Add(this.labelAuxMin);
            this.Controls.Add(this.labelTailMax);
            this.Controls.Add(this.labelThrMax);
            this.Controls.Add(this.labelEleMax);
            this.Controls.Add(this.labelAilMax);
            this.Controls.Add(this.labelCtrlMax);
            this.Controls.Add(this.labelTailMin);
            this.Controls.Add(this.labelThrMin);
            this.Controls.Add(this.labelEleMin);
            this.Controls.Add(this.labelAilMin);
            this.Controls.Add(this.labelCtrlMin);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.indicadorSlider7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.indicadorSlider6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.indicadorSlider5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.indicadorSlider4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.indicadorSlider3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.indicadorSlider2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.indicadorSlider1);
            this.Name = "FormCalibrarServos";
            this.Text = "FormConfigurarServos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigurarServos_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider2;
        private System.Windows.Forms.Label label3;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider3;
        private System.Windows.Forms.Label label4;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider4;
        private System.Windows.Forms.Label label5;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider5;
        private System.Windows.Forms.Label label6;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider6;
        private System.Windows.Forms.Label label7;
        private UAVConsole.Intrumentos.IndicadorSlider indicadorSlider7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelCtrlMin;
        private System.Windows.Forms.Label labelAilMin;
        private System.Windows.Forms.Label labelEleMin;
        private System.Windows.Forms.Label labelThrMin;
        private System.Windows.Forms.Label labelTailMin;
        private System.Windows.Forms.Label labelTailMax;
        private System.Windows.Forms.Label labelThrMax;
        private System.Windows.Forms.Label labelEleMax;
        private System.Windows.Forms.Label labelAilMax;
        private System.Windows.Forms.Label labelCtrlMax;
        private System.Windows.Forms.Label labelAuxMin;
        private System.Windows.Forms.Label labelAuxMax;
        private System.Windows.Forms.Label labelPanMax;
        private System.Windows.Forms.Label labelPanMin;
        private System.Windows.Forms.Button button3;
    }
}