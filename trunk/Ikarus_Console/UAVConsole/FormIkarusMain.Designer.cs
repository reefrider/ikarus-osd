namespace UAVConsole
{
    partial class FormIkarusMain
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.mapControl1 = new UAVConsole.GoogleMaps.IkarusMapControl();
            this.panel7 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxTeam = new System.Windows.Forms.ComboBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.button15 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.checkBoxHomeRX = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.medidorBaterias3 = new UAVConsole.Intrumentos.MedidorBaterias();
            this.medidorBaterias4 = new UAVConsole.Intrumentos.MedidorBaterias();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.medidorBaterias2 = new UAVConsole.Intrumentos.MedidorBaterias();
            this.medidorBaterias1 = new UAVConsole.Intrumentos.MedidorBaterias();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.button14 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonHUD = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.buttonAUTO = new System.Windows.Forms.Button();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button10 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.buttonCAM = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.buttonRUTA = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.knob_anttracker = new UAVConsole.Intrumentos.Instrumento_knob();
            this.label3 = new System.Windows.Forms.Label();
            this.instrumento_VerticalSpeed1 = new UAVConsole.Intrumentos.Instrumento_VerticalSpeed();
            this.led1 = new UAVConsole.Intrumentos.Led();
            this.medidorRSSI = new UAVConsole.Intrumentos.MedidorVertical();
            this.dG808_AirSpeed1 = new UAVConsole.Intrumentos.Instrumento_AirSpeed();
            this.instrumento_Altimeter1 = new UAVConsole.Intrumentos.Instrumento_Altimeter();
            this.instrumento_DirectionalGyro1 = new UAVConsole.Intrumentos.Instrumento_DirectionalGyro();
            this.instrumento_HorizonteArtificial = new UAVConsole.Intrumentos.Instrumento_Attitude();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(1003, 511);
            this.splitContainer1.SplitterDistance = 318;
            this.splitContainer1.TabIndex = 25;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel1);
            this.splitContainer2.Panel1.Resize += new System.EventHandler(this.splitContainer2_Panel1_Resize);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel2MinSize = 0;
            this.splitContainer2.Size = new System.Drawing.Size(1003, 318);
            this.splitContainer2.SplitterDistance = 544;
            this.splitContainer2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 318);
            this.panel1.TabIndex = 12;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.mapControl1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.panel7);
            this.splitContainer3.Panel2.Controls.Add(this.comboBoxTeam);
            this.splitContainer3.Panel2.Controls.Add(this.button6);
            this.splitContainer3.Panel2.Controls.Add(this.button7);
            this.splitContainer3.Panel2MinSize = 23;
            this.splitContainer3.Size = new System.Drawing.Size(455, 318);
            this.splitContainer3.SplitterDistance = 293;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 12;
            // 
            // mapControl1
            // 
            this.mapControl1.AutoSize = true;
            this.mapControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl1.Location = new System.Drawing.Point(0, 0);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(455, 293);
            this.mapControl1.TabIndex = 11;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.comboBox1);
            this.panel7.Controls.Add(this.button2);
            this.panel7.Controls.Add(this.button3);
            this.panel7.Controls.Add(this.button1);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(207, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(248, 23);
            this.panel7.TabIndex = 28;
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
            this.comboBox1.Location = new System.Drawing.Point(161, 1);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(87, 21);
            this.comboBox1.TabIndex = 22;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(2, -1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 25);
            this.button2.TabIndex = 21;
            this.button2.Text = "PLANE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button2_MouseDown);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(54, -1);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(52, 25);
            this.button3.TabIndex = 24;
            this.button3.Text = "TRACK";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(108, -1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 25);
            this.button1.TabIndex = 20;
            this.button1.Text = "HOME";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxTeam
            // 
            this.comboBoxTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTeam.FormattingEnabled = true;
            this.comboBoxTeam.Location = new System.Drawing.Point(39, 1);
            this.comboBoxTeam.Name = "comboBoxTeam";
            this.comboBoxTeam.Size = new System.Drawing.Size(120, 21);
            this.comboBoxTeam.TabIndex = 50;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(-1, -1);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(38, 25);
            this.button6.TabIndex = 51;
            this.button6.Text = "View";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(162, -1);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(44, 25);
            this.button7.TabIndex = 51;
            this.button7.Text = "Track";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click_1);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1003, 189);
            this.panel2.TabIndex = 24;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.button15);
            this.panel6.Controls.Add(this.button12);
            this.panel6.Controls.Add(this.checkBoxHomeRX);
            this.panel6.Controls.Add(this.button4);
            this.panel6.Controls.Add(this.checkBox2);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Controls.Add(this.medidorBaterias3);
            this.panel6.Controls.Add(this.medidorBaterias4);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.medidorBaterias2);
            this.panel6.Controls.Add(this.medidorBaterias1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(832, 25);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(171, 164);
            this.panel6.TabIndex = 28;
            // 
            // button15
            // 
            this.button15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button15.Image = global::UAVConsole.Properties.Resources.video;
            this.button15.Location = new System.Drawing.Point(117, 5);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(26, 23);
            this.button15.TabIndex = 49;
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button12
            // 
            this.button12.Image = global::UAVConsole.Properties.Resources.camera;
            this.button12.Location = new System.Drawing.Point(144, 5);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(26, 23);
            this.button12.TabIndex = 48;
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click_1);
            // 
            // checkBoxHomeRX
            // 
            this.checkBoxHomeRX.AutoSize = true;
            this.checkBoxHomeRX.Checked = true;
            this.checkBoxHomeRX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHomeRX.Location = new System.Drawing.Point(3, 33);
            this.checkBoxHomeRX.Name = "checkBoxHomeRX";
            this.checkBoxHomeRX.Size = new System.Drawing.Size(72, 17);
            this.checkBoxHomeRX.TabIndex = 27;
            this.checkBoxHomeRX.Text = "Home RX";
            this.checkBoxHomeRX.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(66, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(50, 23);
            this.button4.TabIndex = 25;
            this.button4.Text = "FullScr";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(75, 33);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(61, 17);
            this.checkBox2.TabIndex = 31;
            this.checkBox2.Text = "Historia";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(-1, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 16);
            this.label6.TabIndex = 30;
            this.label6.Text = "Ant";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(0, 109);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 16);
            this.label7.TabIndex = 29;
            this.label7.Text = "TX";
            // 
            // medidorBaterias3
            // 
            this.medidorBaterias3.AutoCalculate = true;
            this.medidorBaterias3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.medidorBaterias3.Enabled = false;
            this.medidorBaterias3.Location = new System.Drawing.Point(31, 131);
            this.medidorBaterias3.Name = "medidorBaterias3";
            this.medidorBaterias3.num_cells = 0;
            this.medidorBaterias3.Size = new System.Drawing.Size(140, 26);
            this.medidorBaterias3.strip = true;
            this.medidorBaterias3.TabIndex = 28;
            this.medidorBaterias3.valor = -3.200001F;
            this.medidorBaterias3.volts = 0F;
            this.medidorBaterias3.volts_max = 8.4F;
            this.medidorBaterias3.volts_min = 6.4F;
            // 
            // medidorBaterias4
            // 
            this.medidorBaterias4.AutoCalculate = true;
            this.medidorBaterias4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.medidorBaterias4.Enabled = false;
            this.medidorBaterias4.Location = new System.Drawing.Point(31, 104);
            this.medidorBaterias4.Name = "medidorBaterias4";
            this.medidorBaterias4.num_cells = 0;
            this.medidorBaterias4.Size = new System.Drawing.Size(140, 27);
            this.medidorBaterias4.strip = true;
            this.medidorBaterias4.TabIndex = 27;
            this.medidorBaterias4.valor = -3.200001F;
            this.medidorBaterias4.volts = 0F;
            this.medidorBaterias4.volts_max = 8.4F;
            this.medidorBaterias4.volts_min = 6.4F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(-2, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 16);
            this.label2.TabIndex = 23;
            this.label2.Text = "Vid";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-1, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Thr";
            // 
            // medidorBaterias2
            // 
            this.medidorBaterias2.AutoCalculate = true;
            this.medidorBaterias2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.medidorBaterias2.Location = new System.Drawing.Point(30, 78);
            this.medidorBaterias2.Name = "medidorBaterias2";
            this.medidorBaterias2.num_cells = 0;
            this.medidorBaterias2.Size = new System.Drawing.Size(140, 26);
            this.medidorBaterias2.strip = true;
            this.medidorBaterias2.TabIndex = 21;
            this.medidorBaterias2.valor = -3.200001F;
            this.medidorBaterias2.volts = 0F;
            this.medidorBaterias2.volts_max = 8.4F;
            this.medidorBaterias2.volts_min = 6.4F;
            // 
            // medidorBaterias1
            // 
            this.medidorBaterias1.AutoCalculate = true;
            this.medidorBaterias1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.medidorBaterias1.Location = new System.Drawing.Point(30, 51);
            this.medidorBaterias1.Name = "medidorBaterias1";
            this.medidorBaterias1.num_cells = 0;
            this.medidorBaterias1.Size = new System.Drawing.Size(140, 27);
            this.medidorBaterias1.strip = true;
            this.medidorBaterias1.TabIndex = 20;
            this.medidorBaterias1.valor = -3.200001F;
            this.medidorBaterias1.volts = 0F;
            this.medidorBaterias1.volts_max = 8.4F;
            this.medidorBaterias1.volts_min = 6.4F;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel8);
            this.panel4.Controls.Add(this.buttonHUD);
            this.panel4.Controls.Add(this.button11);
            this.panel4.Controls.Add(this.buttonAUTO);
            this.panel4.Controls.Add(this.comboBox3);
            this.panel4.Controls.Add(this.numericUpDown1);
            this.panel4.Controls.Add(this.button10);
            this.panel4.Controls.Add(this.comboBox2);
            this.panel4.Controls.Add(this.buttonCAM);
            this.panel4.Controls.Add(this.numericUpDown2);
            this.panel4.Controls.Add(this.buttonRUTA);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.textBox1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1003, 25);
            this.panel4.TabIndex = 41;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.button14);
            this.panel8.Controls.Add(this.button13);
            this.panel8.Controls.Add(this.button9);
            this.panel8.Controls.Add(this.button8);
            this.panel8.Controls.Add(this.checkBox1);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(736, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(267, 25);
            this.panel8.TabIndex = 12;
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(7, 0);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(44, 25);
            this.button14.TabIndex = 54;
            this.button14.Text = "Flash";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(54, 0);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(43, 25);
            this.button13.TabIndex = 55;
            this.button13.Text = "CASA";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(163, 0);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(50, 25);
            this.button9.TabIndex = 54;
            this.button9.Text = "Max IR";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click_1);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(101, 0);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(60, 25);
            this.button8.TabIndex = 52;
            this.button8.Text = "Center IR";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(218, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(46, 17);
            this.checkBox1.TabIndex = 28;
            this.checkBox1.Text = "JOY";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonHUD
            // 
            this.buttonHUD.Location = new System.Drawing.Point(3, 0);
            this.buttonHUD.Name = "buttonHUD";
            this.buttonHUD.Size = new System.Drawing.Size(60, 25);
            this.buttonHUD.TabIndex = 44;
            this.buttonHUD.Text = "SCR 0";
            this.buttonHUD.UseVisualStyleBackColor = true;
            this.buttonHUD.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonHUD_MouseDown);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(667, 0);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(38, 25);
            this.button11.TabIndex = 47;
            this.button11.Text = "Up";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // buttonAUTO
            // 
            this.buttonAUTO.Location = new System.Drawing.Point(189, 0);
            this.buttonAUTO.Name = "buttonAUTO";
            this.buttonAUTO.Size = new System.Drawing.Size(60, 25);
            this.buttonAUTO.TabIndex = 43;
            this.buttonAUTO.Text = "Auto";
            this.buttonAUTO.UseVisualStyleBackColor = true;
            this.buttonAUTO.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonAUTO_MouseDown);
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Pitch",
            "Roll",
            "Tail",
            "Thrust",
            "Otros"});
            this.comboBox3.Location = new System.Drawing.Point(472, 3);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(59, 21);
            this.comboBox3.TabIndex = 49;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.debug_combo_chg);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(412, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1.TabIndex = 46;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown1.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(343, 0);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(45, 25);
            this.button10.TabIndex = 45;
            this.button10.Text = "IR A";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "P gain",
            "I gain",
            "D gain",
            "I limit",
            "Drive Lim"});
            this.comboBox2.Location = new System.Drawing.Point(535, 3);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(73, 21);
            this.comboBox2.TabIndex = 28;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.debug_combo_chg2);
            // 
            // buttonCAM
            // 
            this.buttonCAM.Location = new System.Drawing.Point(65, 0);
            this.buttonCAM.Name = "buttonCAM";
            this.buttonCAM.Size = new System.Drawing.Size(60, 25);
            this.buttonCAM.TabIndex = 47;
            this.buttonCAM.Text = "CAM 1";
            this.buttonCAM.UseVisualStyleBackColor = true;
            this.buttonCAM.Click += new System.EventHandler(this.button12_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(296, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown2.TabIndex = 52;
            this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // buttonRUTA
            // 
            this.buttonRUTA.Location = new System.Drawing.Point(127, 0);
            this.buttonRUTA.Name = "buttonRUTA";
            this.buttonRUTA.Size = new System.Drawing.Size(60, 25);
            this.buttonRUTA.TabIndex = 42;
            this.buttonRUTA.Text = "Casa";
            this.buttonRUTA.UseVisualStyleBackColor = true;
            this.buttonRUTA.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonRUTA_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(254, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 53;
            this.label5.Text = "WPTID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(389, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "Alt";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(611, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(54, 20);
            this.textBox1.TabIndex = 48;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.knob_anttracker);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.instrumento_VerticalSpeed1);
            this.panel5.Controls.Add(this.led1);
            this.panel5.Controls.Add(this.medidorRSSI);
            this.panel5.Controls.Add(this.dG808_AirSpeed1);
            this.panel5.Controls.Add(this.instrumento_Altimeter1);
            this.panel5.Controls.Add(this.instrumento_DirectionalGyro1);
            this.panel5.Controls.Add(this.instrumento_HorizonteArtificial);
            this.panel5.Location = new System.Drawing.Point(0, 28);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(827, 167);
            this.panel5.TabIndex = 44;
            // 
            // knob_anttracker
            // 
            this.knob_anttracker.Location = new System.Drawing.Point(790, 123);
            this.knob_anttracker.Manual = false;
            this.knob_anttracker.Name = "knob_anttracker";
            this.knob_anttracker.Size = new System.Drawing.Size(36, 36);
            this.knob_anttracker.TabIndex = 44;
            this.knob_anttracker.Valor = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(781, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 18);
            this.label3.TabIndex = 42;
            this.label3.Text = "RSSI";
            // 
            // instrumento_VerticalSpeed1
            // 
            this.instrumento_VerticalSpeed1.Location = new System.Drawing.Point(639, 3);
            this.instrumento_VerticalSpeed1.Name = "instrumento_VerticalSpeed1";
            this.instrumento_VerticalSpeed1.Size = new System.Drawing.Size(156, 157);
            this.instrumento_VerticalSpeed1.TabIndex = 9;
            this.instrumento_VerticalSpeed1.Value = 0F;
            // 
            // led1
            // 
            this.led1.Location = new System.Drawing.Point(305, 5);
            this.led1.Name = "led1";
            this.led1.Size = new System.Drawing.Size(30, 30);
            this.led1.TabIndex = 40;
            // 
            // medidorRSSI
            // 
            this.medidorRSSI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.medidorRSSI.Location = new System.Drawing.Point(797, 21);
            this.medidorRSSI.Name = "medidorRSSI";
            this.medidorRSSI.Size = new System.Drawing.Size(25, 97);
            this.medidorRSSI.TabIndex = 39;
            this.medidorRSSI.valor = 1F;
            // 
            // dG808_AirSpeed1
            // 
            this.dG808_AirSpeed1.Location = new System.Drawing.Point(3, 1);
            this.dG808_AirSpeed1.Name = "dG808_AirSpeed1";
            this.dG808_AirSpeed1.Size = new System.Drawing.Size(158, 158);
            this.dG808_AirSpeed1.TabIndex = 9;
            this.dG808_AirSpeed1.Value = 0F;
            // 
            // instrumento_Altimeter1
            // 
            this.instrumento_Altimeter1.Altitude = 0F;
            this.instrumento_Altimeter1.Calibration = 0F;
            this.instrumento_Altimeter1.Location = new System.Drawing.Point(162, 3);
            this.instrumento_Altimeter1.Name = "instrumento_Altimeter1";
            this.instrumento_Altimeter1.Size = new System.Drawing.Size(158, 156);
            this.instrumento_Altimeter1.TabIndex = 4;
            // 
            // instrumento_DirectionalGyro1
            // 
            this.instrumento_DirectionalGyro1.Location = new System.Drawing.Point(319, 0);
            this.instrumento_DirectionalGyro1.Name = "instrumento_DirectionalGyro1";
            this.instrumento_DirectionalGyro1.Size = new System.Drawing.Size(163, 162);
            this.instrumento_DirectionalGyro1.TabIndex = 3;
            this.instrumento_DirectionalGyro1.Value = 0F;
            // 
            // instrumento_HorizonteArtificial
            // 
            this.instrumento_HorizonteArtificial.Location = new System.Drawing.Point(479, -3);
            this.instrumento_HorizonteArtificial.Name = "instrumento_HorizonteArtificial";
            this.instrumento_HorizonteArtificial.pitch = 0F;
            this.instrumento_HorizonteArtificial.roll = 0F;
            this.instrumento_HorizonteArtificial.Size = new System.Drawing.Size(164, 164);
            this.instrumento_HorizonteArtificial.TabIndex = 43;
            // 
            // FormIkarusMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1003, 511);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "FormIkarusMain";
            this.Text = "Ikarus UAV Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIkarusMain_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormIkarusMain_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormIkarusMain_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormIkarusMain_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private UAVConsole.Intrumentos.Instrumento_DirectionalGyro instrumento_DirectionalGyro1;
        private UAVConsole.Intrumentos.Instrumento_VerticalSpeed instrumento_VerticalSpeed1;
        private UAVConsole.Intrumentos.Instrumento_Altimeter instrumento_Altimeter1;
        private System.Windows.Forms.Timer timer1;
        public UAVConsole.GoogleMaps.IkarusMapControl mapControl1;
        private UAVConsole.Intrumentos.MedidorBaterias medidorBaterias1;
        private UAVConsole.Intrumentos.MedidorBaterias medidorBaterias2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private UAVConsole.Intrumentos.Instrumento_AirSpeed dG808_AirSpeed1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private UAVConsole.Intrumentos.MedidorVertical medidorRSSI;
        private UAVConsole.Intrumentos.Led led1;
        private System.Windows.Forms.CheckBox checkBoxHomeRX;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonHUD;
        private System.Windows.Forms.Button buttonAUTO;
        private System.Windows.Forms.Button buttonRUTA;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private UAVConsole.Intrumentos.Instrumento_Attitude instrumento_HorizonteArtificial;
        private System.Windows.Forms.Button buttonCAM;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBoxTeam;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private Intrumentos.MedidorBaterias medidorBaterias3;
        private Intrumentos.MedidorBaterias medidorBaterias4;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Panel panel8;
        private Intrumentos.Instrumento_knob knob_anttracker;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button15;


    }
}

