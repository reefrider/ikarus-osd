namespace UAVConsole
{
    partial class FormGestionRutas
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxWpts = new System.Windows.Forms.ListBox();
            this.textBoxWptName = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxWptLon = new System.Windows.Forms.TextBox();
            this.textBoxWptLat = new System.Windows.Forms.TextBox();
            this.textBoxWptAlt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.mapControl1 = new UAVConsole.GoogleMaps.IkarusMapControl();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxWpts
            // 
            this.listBoxWpts.FormattingEnabled = true;
            this.listBoxWpts.Location = new System.Drawing.Point(12, 12);
            this.listBoxWpts.Name = "listBoxWpts";
            this.listBoxWpts.Size = new System.Drawing.Size(132, 160);
            this.listBoxWpts.TabIndex = 13;
            this.listBoxWpts.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // textBoxWptName
            // 
            this.textBoxWptName.Location = new System.Drawing.Point(153, 30);
            this.textBoxWptName.Name = "textBoxWptName";
            this.textBoxWptName.Size = new System.Drawing.Size(131, 20);
            this.textBoxWptName.TabIndex = 14;
            this.textBoxWptName.Leave += new System.EventHandler(this.textBoxWptName_Leave);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(412, 14);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(91, 30);
            this.button6.TabIndex = 25;
            this.button6.Text = "Ikarus -> Ruta";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(510, 14);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 30);
            this.button4.TabIndex = 24;
            this.button4.Text = "Ruta -> Ikarus";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(442, 105);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 29);
            this.button3.TabIndex = 23;
            this.button3.Text = "Fijar Ruta";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBoxWptLon
            // 
            this.textBoxWptLon.Location = new System.Drawing.Point(152, 72);
            this.textBoxWptLon.Name = "textBoxWptLon";
            this.textBoxWptLon.Size = new System.Drawing.Size(131, 20);
            this.textBoxWptLon.TabIndex = 26;
            this.textBoxWptLon.Leave += new System.EventHandler(this.textBoxWptLon_Leave);
            // 
            // textBoxWptLat
            // 
            this.textBoxWptLat.Location = new System.Drawing.Point(289, 72);
            this.textBoxWptLat.Name = "textBoxWptLat";
            this.textBoxWptLat.Size = new System.Drawing.Size(131, 20);
            this.textBoxWptLat.TabIndex = 27;
            this.textBoxWptLat.Leave += new System.EventHandler(this.textBoxWptLat_Leave);
            // 
            // textBoxWptAlt
            // 
            this.textBoxWptAlt.Location = new System.Drawing.Point(153, 113);
            this.textBoxWptAlt.Name = "textBoxWptAlt";
            this.textBoxWptAlt.Size = new System.Drawing.Size(76, 20);
            this.textBoxWptAlt.TabIndex = 28;
            this.textBoxWptAlt.Leave += new System.EventHandler(this.textBoxWptAlt_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(150, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Nombre";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Longitud";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(286, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Latitud";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Altitud";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(237, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 29);
            this.button1.TabIndex = 33;
            this.button1.Text = "Borrar Wpt";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(154, 140);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 30);
            this.button2.TabIndex = 34;
            this.button2.Text = "Insertar Wpt";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(442, 141);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(76, 30);
            this.button5.TabIndex = 35;
            this.button5.Text = "Save Ruta";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(524, 141);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(76, 30);
            this.button7.TabIndex = 36;
            this.button7.Text = "Load Ruta";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(524, 104);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(76, 29);
            this.button8.TabIndex = 37;
            this.button8.Text = "Clear Ruta";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(319, 140);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(76, 29);
            this.button9.TabIndex = 38;
            this.button9.Text = "Modify Wpt";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "SATELLITE",
            "MAPA",
            "SAT+MAP",
            "TOPO"});
            this.comboBox1.Location = new System.Drawing.Point(442, 64);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(118, 21);
            this.comboBox1.TabIndex = 39;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // mapControl1
            // 
            this.mapControl1.AutoSize = true;
            this.mapControl1.Location = new System.Drawing.Point(12, 188);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(588, 350);
            this.mapControl1.TabIndex = 12;
            this.mapControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.mapControl1_Paint);
            this.mapControl1.MouseLeave += new System.EventHandler(this.mapControl1_MouseLeave);
            // 
            // button10
            // 
            this.button10.BackgroundImage = global::UAVConsole.Properties.Resources.ICO_dossier_home_ico_64x64;
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button10.Location = new System.Drawing.Point(566, 59);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(32, 29);
            this.button10.TabIndex = 40;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(319, 104);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(76, 29);
            this.button11.TabIndex = 41;
            this.button11.Text = "Añadir Wpt";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(352, 15);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(54, 29);
            this.button12.TabIndex = 42;
            this.button12.Text = "GPS";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // FormGestionRutas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 550);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxWptAlt);
            this.Controls.Add(this.textBoxWptLat);
            this.Controls.Add(this.textBoxWptLon);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBoxWptName);
            this.Controls.Add(this.listBoxWpts);
            this.Controls.Add(this.mapControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGestionRutas";
            this.Text = "FormGestionRutas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public UAVConsole.GoogleMaps.IkarusMapControl mapControl1;
        private System.Windows.Forms.ListBox listBoxWpts;
        private System.Windows.Forms.TextBox textBoxWptName;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxWptLon;
        private System.Windows.Forms.TextBox textBoxWptLat;
        private System.Windows.Forms.TextBox textBoxWptAlt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
    }
}