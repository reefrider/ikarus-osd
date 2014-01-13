namespace UAVConsole
{
    partial class FormScreen
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
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBoxCol = new System.Windows.Forms.TextBox();
            this.textBoxFila = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxScreenSlot = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxParam = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxShowSc = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button8 = new System.Windows.Forms.Button();
            this.checkBoxOpaco = new System.Windows.Forms.CheckBox();
            this.checkBoxServo = new System.Windows.Forms.CheckBox();
            this.checkBoxAbsoluto = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "Altimetro",
            "Autopilot",
            "Bearing",
            "Compas",
            "Distancia casa",
            "Distancia wpt",
            "Hora",
            "Horizonte Artificial",
            "Intensidad (A)",
            "Consumo total (mAh)",
            "Latitud",
            "Longitud",
            "Nombre HUD",
            "Nombre Piloto",
            "Nombre WayPoint",
            "Num Satelites GPS",
            "Posicion Antena",
            "Pos. Antena Vert.",
            "Rendimiento km/Ah",
            "RSSI",
            "Tasa planeo",
            "Tiempo vuelo",
            "Variometro",
            "Velocimetro",
            "Vmotor (Text)",
            "Vmotor (Bar)",
            "Vvideo (Text)",
            "Vvideo (Bar)",
            "Altitud máxima",
            "Distancia casa máxima",
            "Velocidad máxima",
            "Distancia recorrida",
            "Auxiliar"});
            this.listBox1.Location = new System.Drawing.Point(12, 39);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(167, 225);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // textBoxCol
            // 
            this.textBoxCol.Location = new System.Drawing.Point(433, 329);
            this.textBoxCol.Name = "textBoxCol";
            this.textBoxCol.Size = new System.Drawing.Size(100, 20);
            this.textBoxCol.TabIndex = 3;
            this.textBoxCol.TextChanged += new System.EventHandler(this.textBoxCol_TextChanged);
            // 
            // textBoxFila
            // 
            this.textBoxFila.Location = new System.Drawing.Point(261, 329);
            this.textBoxFila.Name = "textBoxFila";
            this.textBoxFila.Size = new System.Drawing.Size(100, 20);
            this.textBoxFila.TabIndex = 2;
            this.textBoxFila.TextChanged += new System.EventHandler(this.textBoxFila_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(220, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modo";
            // 
            // comboBoxScreenSlot
            // 
            this.comboBoxScreenSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScreenSlot.FormattingEnabled = true;
            this.comboBoxScreenSlot.Items.AddRange(new object[] {
            "Pantalla OSD1",
            "Pantalla OSD2",
            "Pantalla OSD3",
            "FailSafe",
            "Resumen"});
            this.comboBoxScreenSlot.Location = new System.Drawing.Point(12, 271);
            this.comboBoxScreenSlot.Name = "comboBoxScreenSlot";
            this.comboBoxScreenSlot.Size = new System.Drawing.Size(117, 21);
            this.comboBoxScreenSlot.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 298);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Subir Ikarus";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(104, 298);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Leer Ikarus";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxParam
            // 
            this.textBoxParam.Location = new System.Drawing.Point(433, 306);
            this.textBoxParam.Name = "textBoxParam";
            this.textBoxParam.Size = new System.Drawing.Size(100, 20);
            this.textBoxParam.TabIndex = 4;
            this.textBoxParam.TextChanged += new System.EventHandler(this.textBoxParam_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 332);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fila";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(372, 309);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Parámetro";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(372, 332);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Columna";
            // 
            // comboBoxShowSc
            // 
            this.comboBoxShowSc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShowSc.FormattingEnabled = true;
            this.comboBoxShowSc.Items.AddRange(new object[] {
            "Simulacion",
            "Capturadora"});
            this.comboBoxShowSc.Location = new System.Drawing.Point(12, 12);
            this.comboBoxShowSc.Name = "comboBoxShowSc";
            this.comboBoxShowSc.Size = new System.Drawing.Size(100, 21);
            this.comboBoxShowSc.TabIndex = 10;
            this.comboBoxShowSc.SelectedIndexChanged += new System.EventHandler(this.comboBoxShowSc_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 327);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Save File";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(104, 326);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 12;
            this.button4.Text = "Load File";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "PAL",
            "NTSC"});
            this.comboBox1.Location = new System.Drawing.Point(118, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(61, 21);
            this.comboBox1.TabIndex = 14;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
            this.comboBox3.Location = new System.Drawing.Point(262, 305);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(99, 21);
            this.comboBox3.TabIndex = 15;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(253, 356);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(71, 23);
            this.button5.TabIndex = 57;
            this.button5.Text = "Upload CS";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 356);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(167, 20);
            this.textBox1.TabIndex = 56;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(330, 356);
            this.progressBar1.Maximum = 256;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(215, 23);
            this.progressBar1.TabIndex = 58;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(185, 356);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(25, 23);
            this.button6.TabIndex = 59;
            this.button6.Text = "...";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(212, 356);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(38, 23);
            this.button7.TabIndex = 60;
            this.button7.Text = "Edit";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(196, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(356, 292);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(135, 271);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(44, 23);
            this.button8.TabIndex = 61;
            this.button8.Text = "Clear";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // checkBoxOpaco
            // 
            this.checkBoxOpaco.AutoSize = true;
            this.checkBoxOpaco.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkBoxOpaco.Location = new System.Drawing.Point(375, 296);
            this.checkBoxOpaco.Name = "checkBoxOpaco";
            this.checkBoxOpaco.Size = new System.Drawing.Size(43, 31);
            this.checkBoxOpaco.TabIndex = 62;
            this.checkBoxOpaco.Text = "Opaco";
            this.checkBoxOpaco.UseVisualStyleBackColor = true;
            this.checkBoxOpaco.Visible = false;
            this.checkBoxOpaco.CheckedChanged += new System.EventHandler(this.checkBoxOpaco_CheckedChanged);
            // 
            // checkBoxServo
            // 
            this.checkBoxServo.AutoSize = true;
            this.checkBoxServo.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkBoxServo.Location = new System.Drawing.Point(497, 296);
            this.checkBoxServo.Name = "checkBoxServo";
            this.checkBoxServo.Size = new System.Drawing.Size(39, 31);
            this.checkBoxServo.TabIndex = 63;
            this.checkBoxServo.Text = "Servo";
            this.checkBoxServo.UseVisualStyleBackColor = true;
            this.checkBoxServo.Visible = false;
            this.checkBoxServo.CheckedChanged += new System.EventHandler(this.checkBoxServo_CheckedChanged);
            // 
            // checkBoxAbsoluto
            // 
            this.checkBoxAbsoluto.AutoSize = true;
            this.checkBoxAbsoluto.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkBoxAbsoluto.Location = new System.Drawing.Point(438, 296);
            this.checkBoxAbsoluto.Name = "checkBoxAbsoluto";
            this.checkBoxAbsoluto.Size = new System.Drawing.Size(52, 31);
            this.checkBoxAbsoluto.TabIndex = 64;
            this.checkBoxAbsoluto.Text = "Absoluto";
            this.checkBoxAbsoluto.UseVisualStyleBackColor = true;
            this.checkBoxAbsoluto.Visible = false;
            this.checkBoxAbsoluto.CheckedChanged += new System.EventHandler(this.checkBoxAbsoluto_CheckedChanged);
            // 
            // FormScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 381);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBoxShowSc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxParam);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxCol);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxFila);
            this.Controls.Add(this.comboBoxScreenSlot);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBoxAbsoluto);
            this.Controls.Add(this.checkBoxServo);
            this.Controls.Add(this.checkBoxOpaco);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScreen";
            this.Text = "Screen Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormScreen_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBoxCol;
        private System.Windows.Forms.TextBox textBoxFila;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxScreenSlot;
        private System.Windows.Forms.TextBox textBoxParam;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxShowSc;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.CheckBox checkBoxOpaco;
        private System.Windows.Forms.CheckBox checkBoxServo;
        private System.Windows.Forms.CheckBox checkBoxAbsoluto;
    }
}