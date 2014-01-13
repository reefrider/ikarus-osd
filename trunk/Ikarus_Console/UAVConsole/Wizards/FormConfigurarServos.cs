/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  IKARUS_OSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IKARUS_OSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UAVConsole.ConfigClasses;
using UAVConsole.Intrumentos;
using UAVConsole.USBXpress;

namespace UAVConsole.Wizards
{
    public partial class FormConfigurarServos : Form
    {
        const int threshold = 150;
        enum Estados { Fijar_Ail, Fijar_Ele, Fijar_Thr, Fijar_Tail, Sleep, Inicio };
        enum Mandos{ AIL, ELE, THR, TAIL};
        bool EstadoTransicion = false;
        bool ConErrores;

        Estados estado = Estados.Inicio;

        Singleton me = Singleton.GetInstance();
        FlightPlanUSB ikarus = new FlightPlanUSB();

        IkarusAutopilotConfig cfg;
            
        int[] valores;
        int[] valores_reposo;
        CheckBox[,] buttons;

        Asignaciones[] canales;
            
        public FormConfigurarServos()
        {
            InitializeComponent();
            InitValores();

            canales = new Asignaciones[4];
            canales[0] = new Asignaciones((int)Mandos.AIL);
            canales[1] = new Asignaciones((int)Mandos.ELE);
            canales[2] = new Asignaciones((int)Mandos.THR);
            canales[3] = new Asignaciones((int)Mandos.TAIL);


            if (me.Idioma == 0)
                TextSpanish();
            else
                TextEnglish();

            
            timer1.Enabled = true;
        }

        void InitValores()
        {
            buttons = new CheckBox[11, 4];
            int y = radioButton1.Location.Y;
            int dy = (radioButton2.Location.Y - y) / 10;
            int x = radioButton1.Location.X;
            int dx = (radioButton2.Location.X - x) / 3;

            for(int i=0;i<11;i++)
                for (int j = 0; j < 4; j++)
                {
                    CheckBox btn = new CheckBox();
                    btn.AutoSize = true;
                    btn.Location = new System.Drawing.Point(x + j * dx, y + i * dy);
                    //btn.Name = "radioButton1";
                    btn.Size = new System.Drawing.Size(14, 13);
                    //btn.TabIndex = 47;
                    //btn.TabStop = true;
                    btn.UseVisualStyleBackColor = true;
                    //btn.Checked = true;
                    btn.Enabled = false;
                    this.Controls.Add(btn);
                    buttons[i, j] = btn;
                }
            valores_reposo = new int[11];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ikarus.IsOpen())
            {
                if (cfg == null)
                {
                    if (me.Idioma == 0)
                        MessageBox.Show("Debe ejecutar primero el asistente");
                    else
                        MessageBox.Show("I must run wizard first!");
                }
                else
                {
                    ikarus.WriteConfigAutopilot(cfg);
                    if (me.Idioma == 0)
                        MessageBox.Show("Configuración guardada en Ikarus OSD. Compruebe que es correcto.");
                    else
                        MessageBox.Show("Config saved to Ikarus OSD. Check it is correct.");
                }
            }
            else
            {
                if (me.Idioma == 0)
                    MessageBox.Show("Error abriendo conexión con Ikarus");
                else
                    MessageBox.Show("Error opening conection with Ikarus");
            }  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!ikarus.IsOpen())
            {
                ikarus = new FlightPlanUSB();
            }
            else
            {
                valores = ikarus.ReadServosRAW();
                UpdateValues();
            }
            
        }

        void CapturaReposo()
        {
            for (int i = 0; i < valores.Length && i < valores_reposo.Length; i++)
            {
                if (estado == Estados.Sleep|| estado==Estados.Inicio)
                {
                    valores_reposo[i] = valores[i];
                }
            }
        }

        Asignaciones CapturaCanales()
        {
            Asignaciones tmp = new Asignaciones((int)estado);
            
            for (int i = 0; i < valores.Length && i < valores_reposo.Length; i++)
            {
                switch (estado)
                {
                    case Estados.Fijar_Ail:
                        if (Math.Abs(valores[i] - valores_reposo[i]) > threshold)
                        {
                            bool reversed = (valores[i] < valores_reposo[i]);
                            tmp.Add(i, reversed);
                        }
                        break;

                    case Estados.Fijar_Ele:
                        if (Math.Abs(valores[i] - valores_reposo[i]) > threshold)
                        {
                            bool reversed = (valores[i] < valores_reposo[i]);
                            tmp.Add(i, reversed);
                        }
                        break;

                    case Estados.Fijar_Tail:
                        if (Math.Abs(valores[i] - valores_reposo[i]) > threshold)
                        {
                            bool reversed = (valores[i] < valores_reposo[i]);
                            tmp.Add(i, reversed);
                        }
                        break;

                    case Estados.Fijar_Thr:
                        if (Math.Abs(valores_reposo[i] - valores[i]) > threshold)
                        {
                            bool reversed = (valores[i] < valores_reposo[i]);
                            tmp.Add(i, reversed);
                        }
                        break;

                    default:
                        break;
                }
            }
        
            if (estado != Estados.Sleep && estado != Estados.Inicio)
            {
                return tmp;
            }

            return null;
        }

        void UpdateValues()
        {
            IndicadorSlider[] sliders = { indicadorSlider1, indicadorSlider2, indicadorSlider3, indicadorSlider4,
                                            indicadorSlider5, indicadorSlider6, indicadorSlider7, indicadorSlider8, 
                                            indicadorSlider9, indicadorSlider10, indicadorSlider11 };
            Label[] etiquetas = { label13, label14, label15, label16 };

            
            for (int i = 0; i < valores.Length && i < sliders.Length; i++)
            {
                sliders[i].PosFin = valores[i] / 1000.0f - 1;
                sliders[i].PosInicio = 0.5f;
                sliders[i].Texto = valores[i].ToString();
                sliders[i].Invalidate();
            }

            
            for (int i = 0; i < 4; i++)
            {
                if (i == ((int)estado))
                    etiquetas[i].ForeColor = Color.Red;
                else
                    etiquetas[i].ForeColor = Color.Black;
            }
            if (estado == Estados.Sleep)
            {
                if (me.Idioma == 0)
                    label8.Text = "Pulse Actualizar para terminar";
                else
                     label8.Text = "Press Update to end";
                
                CapturaReposo();
            }
            else if (estado == Estados.Inicio)
            {
                if (me.Idioma == 0)
                    label8.Text = "Pulse Inicio para empezar";
                else
                    label8.Text = "Press Init to start";
                
                CapturaReposo();
            }
            else
            {
                if (EstadoTransicion == false)
                {
                    Asignaciones tmp = CapturaCanales();

                    if (me.Idioma == 0)
                    {
                        switch (estado)
                        {
                            case Estados.Fijar_Ail:
                                label8.Text = "Mueve Alabeo Derecha";
                                break;
                            case Estados.Fijar_Ele:
                                label8.Text = "Tira mando profundidad";
                                break;
                            case Estados.Fijar_Thr:
                                label8.Text = "Mueve motor maximo";
                                break;
                            case Estados.Fijar_Tail:
                                label8.Text = "Mueve Cola Derecha";
                                break;
                        }
                    }
                    else
                    {
                        switch (estado)
                        {
                            case Estados.Fijar_Ail:
                                label8.Text = "Move Ailerons Right";
                                break;
                            case Estados.Fijar_Ele:
                                label8.Text = "Pull Elevator";
                                break;
                            case Estados.Fijar_Thr:
                                label8.Text = "Power ON Motor";
                                break;
                            case Estados.Fijar_Tail:
                                label8.Text = "Move Tail Right";
                                break;
                        }
                    }

                    for (int i = 0; i < buttons.GetLength(0); i++)
                    {
                        if (tmp.UsaChannel(i))
                            buttons[i, (int)estado].Checked = true;
                        else
                            buttons[i, (int)estado].Checked = false;

                        //buttons[i, (int)estado].Invalidate();
                    }
                    int numCanales = tmp.getNumCanales();

                    if (numCanales > 0 && numCanales <= 2)
                    {
                        canales[(int)estado] = tmp;
                        EstadoTransicion = true;
                    }
                }
                else
                {
                    int numCanales = CapturaCanales().getNumCanales();

                    if (me.Idioma == 0)
                    {
                        if (estado == Estados.Fijar_Thr)
                            label8.Text = "Corta mando motor";
                        else
                            label8.Text = "Suelta los mandos";
                    }
                    else
                    {
                        if (estado == Estados.Fijar_Thr)
                            label8.Text = "Power Motor Off";
                        else
                            label8.Text = "Release Controls";
                    }

                    if (numCanales == 0)
                    {
                        estado++;
                        EstadoTransicion = false;

                        if (estado == Estados.Sleep)
                        {
                            AnalizaDatos();
                        }
                    }
                    
                }
            }
        }

        void AnalizaDatos()
        {
            cfg = ikarus.ReadConfigAutopilot();
            int numCanales;

            ConErrores = false;
            textBox1.Text = "";
            cfg.tipo_mezcla = (byte)Singleton.Mezclas.Normal;

            // Motor
            numCanales =canales[(int)Mandos.THR].getNumCanales();
            if (numCanales != 1)
            {
                ConErrores = true;
                textBox1.Text += "El motor no puede tener "+numCanales+" canales"+Environment.NewLine;
            }
            else
            {
                cfg.thr_ch = (byte)(canales[(int)Mandos.THR].getCanales()[0]);
                cfg.servo_thr.reverse = (byte)((canales[(int)Mandos.THR].getReverses()[0]) ? 1 : 0);
               
                textBox1.Text += "Rev THR " + canales[(int)Mandos.THR].getReverses()[0] + Environment.NewLine;
            }

            // Alerones
            numCanales = canales[(int)Mandos.AIL].getNumCanales();
            if (numCanales == 1)
            {
                cfg.ail_ch = (byte)(canales[(int)Mandos.AIL].getCanales()[0]);
                cfg.servo_ail.reverse = (byte)((canales[(int)Mandos.AIL].getReverses()[0]) ? 1 : 0);
         
                textBox1.Text += "Rev AIL " + canales[(int)Mandos.AIL].getReverses()[0] + Environment.NewLine;
            }
            else if (numCanales != 2)
            {
                ConErrores = true;
                textBox1.Text += "El motor no puede tener " + numCanales + " canales"+Environment.NewLine;
            }
            else if (canales[(int)Mandos.AIL].Compare(canales[(int)Mandos.ELE]))    // Elevon
            {
                cfg.ail_ch = (byte)(canales[(int)Mandos.AIL].getCanales()[0]);
                cfg.ele_ch = (byte)(canales[(int)Mandos.AIL].getCanales()[1]);
                cfg.tipo_mezcla = (byte)Singleton.Mezclas.Elevon;

                Solucion sol = new Solucion(canales[(int)Mandos.AIL], canales[(int)Mandos.ELE]);
                
                cfg.rev_mezcla = (byte)(sol.rev_mix ? 1 : 0);
                cfg.servo_ail.reverse = (byte)(sol.rev_out1 ? 1 : 0);
                cfg.servo_ele.reverse = (byte)(sol.rev_out2 ? 1 : 0);
                
                textBox1.Text += "Elevon:"+Environment.NewLine;
                textBox1.Text += "REV Mix" + sol.rev_mix+Environment.NewLine;
                textBox1.Text += "REV Ail" + sol.rev_out1 + Environment.NewLine;
                textBox1.Text += "REV Ele" + sol.rev_out2 + Environment.NewLine;

            }
            else    // Es flaperon
            {
                cfg.ail_ch = (byte)(canales[(int)Mandos.AIL].getCanales()[0]);
                cfg.servo_ail.reverse = (byte)((canales[(int)Mandos.AIL].getReverses()[0]) ? 1 : 0);
                cfg.aux_ch = (byte)(canales[(int)Mandos.AIL].getCanales()[1]);
                cfg.servo_aux.reverse = (byte)((canales[(int)Mandos.AIL].getReverses()[1]) ? 1 : 0);
                cfg.CanalAuxMode = (byte)(Singleton.ModoCanalAux.AIL2);
               
                textBox1.Text += "Flaperones"+Environment.NewLine;

                textBox1.Text += "Rev AIL " + canales[(int)Mandos.AIL].getReverses()[0] + Environment.NewLine;
                textBox1.Text += "Rev AUX " + canales[(int)Mandos.AIL].getReverses()[1] + Environment.NewLine;
            }

            // Elevador
            numCanales = canales[(int)Mandos.ELE].getNumCanales();
            if (numCanales == 1)
            {
                cfg.ele_ch = (byte)(canales[(int)Mandos.ELE].getCanales()[0]);
                cfg.servo_ele.reverse = (byte)((canales[(int)Mandos.ELE].getReverses()[0]) ? 1 : 0);
               
                textBox1.Text += "Rev ELE " + canales[(int)Mandos.ELE].getReverses()[0] + Environment.NewLine;
            }
            else if (numCanales != 2)
            {
                ConErrores = true;
                textBox1.Text += "El elevador no puede tener " + numCanales + " canales"+Environment.NewLine;
            }
            else if (canales[(int)Mandos.ELE].Compare(canales[(int)Mandos.TAIL]))   // Es V-Tail
            {
                cfg.ele_ch = (byte)(canales[(int)Mandos.ELE].getCanales()[0]);
                cfg.tail_ch = (byte)(canales[(int)Mandos.ELE].getCanales()[1]);
                cfg.tipo_mezcla = (byte)Singleton.Mezclas.V_Tail;
                Solucion sol = new Solucion(canales[(int)Mandos.ELE], canales[(int)Mandos.TAIL]);
                
                cfg.rev_mezcla = (byte)(sol.rev_mix ? 1 : 0);
                cfg.servo_ele.reverse = (byte)(sol.rev_out1 ? 1 : 0);
                cfg.servo_tail.reverse = (byte)(sol.rev_out2 ? 1 : 0);

                textBox1.Text += "Elevon:" + Environment.NewLine;
                textBox1.Text += "REV Mix" + sol.rev_mix + Environment.NewLine;
                textBox1.Text += "REV Ele" + sol.rev_out1 + Environment.NewLine;
                textBox1.Text += "REV Tail" + sol.rev_out2 + Environment.NewLine;
            }
            else if (!canales[(int)Mandos.AIL].Compare(canales[(int)Mandos.ELE]))   // No es Elevon
            {
                ConErrores = true;
                textBox1.Text += "Mezcla Elevador no reconocida"+Environment.NewLine;
            }

            // Cola
            numCanales = canales[(int)Mandos.TAIL].getNumCanales();
            if (numCanales == 1)
            {
                cfg.tail_ch = (byte)(canales[(int)Mandos.TAIL].getCanales()[0]);
                cfg.servo_tail.reverse = (byte)((canales[(int)Mandos.TAIL].getReverses()[0]) ? 1 : 0);
            
                textBox1.Text += "Rev TAIL " + canales[(int)Mandos.TAIL].getReverses()[0] + Environment.NewLine;        
            }
            else if (numCanales != 2)
            {
                ConErrores = true;
                textBox1.Text += "La Cola no puede tener " + numCanales + " canales"+Environment.NewLine;
            }
            else if (!canales[(int)Mandos.ELE].Compare(canales[(int)Mandos.TAIL]))   // Si no es V-Tail
            {
                ConErrores = true;
                textBox1.Text += "Mezcla Cola no reconocida"+Environment.NewLine;
            }

            if (!ConErrores)
            {
                textBox1.Text += "OK"+Environment.NewLine;
            }
            else
            {
                textBox1.Text += "Errores!"+Environment.NewLine;
            }
        }
      

        private void FormConfigurarServos_FormClosing(object sender, FormClosingEventArgs e)
        {
            ikarus.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 11; i++)
                for (int j = 0; j < 4; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            estado = Estados.Fijar_Ail;
            EstadoTransicion = false;
        }

        void TextSpanish()
        {
            //label1.Text = "Control";
            //label2.Text = "Alerones";
            //label3.Text = "Elevador";
            //label4.Text = "Motor";
            //label5.Text = "Cola";

            //button2.Text = "Reinicia";
            button1.Text = "Actualizar";
            this.Text = "Calibrar Servos";
        }

        void TextEnglish()
        {
            //label1.Text = "Control";
            //label2.Text = "Ailerons";
            //label3.Text = "Elevator";
            //label4.Text = "Thrust";
            //label5.Text = "Tail";

            //button2.Text = "Reset";
            button1.Text = "Update";
            this.Text = "Calibrar Servos";
        }
    }
}

class Asignaciones
{
    int[] canales_afectados;
    bool[] canales_invertidos;
    int canal_index = 0;
    int control;

    public Asignaciones(int control)
    {
        canales_afectados = new int[4];
        canales_invertidos = new bool[4];
        this.control = control;
    }

    public bool Compare(Asignaciones a)
    {
        if (a.getNumCanales() != this.getNumCanales())
            return false;

        foreach (int canal in this.getCanales())
        {
            if (!a.UsaChannel(canal))
                return false;
        }
        return true;
    }

    public void Add(int canal, bool rev)
    {
        if (canal_index < canales_afectados.Length)
        {
            canales_afectados[canal_index] = canal;
            canales_invertidos[canal_index] = rev;
            canal_index++;
        }
    }

    public void Clear()
    {
        canal_index = 0;
    }

    public int getNumCanales()
    {
        return canal_index;
    }

    public bool UsaChannel(int ch)
    {
        for (int i = 0; i < canal_index; i++)
            if (canales_afectados[i] == ch)
                return true;
        return false;
    }

    public int[] getCanales()
    {
        int[] tmp = new int[canal_index];
        for (int i = 0; i < canal_index; i++)
            tmp[i] = canales_afectados[i];
        return tmp;
    }

    public bool[] getReverses()
    {
        bool[] tmp = new bool[canal_index];
        for (int i = 0; i < canal_index; i++)
            tmp[i] = canales_invertidos[i];
        return tmp;
    }
}

class Solucion
{
    public bool rev_mix;  // Alerones (elevon) o Elevador (cola-V)
    public bool rev_out1;  // Suma
    public bool rev_out2;  // Resta

    Asignaciones salida1, salida2;

    public Solucion(Asignaciones salida1, Asignaciones salida2)
    {
        this.salida1 = salida1;
        this.salida2 = salida2;
        Solve();
    }

    bool Solve()
    {
        for (int i=0; i<2; i++)
            for(int j=0;j<2;j++)
                for (int k = 0; k < 2; k++)
                {
                    this.rev_mix = (k == 0) ? false : true;
                    this.rev_out1 = (j == 0) ? false : true;
                    this.rev_out2 = (i == 0) ? false : true;
                    bool prueba1 = Probar(1, 0, salida1.getReverses()[0] ? -1 : 1, salida1.getReverses()[1] ? -1 : 1);
                    bool prueba2 = Probar(0, 1, salida2.getReverses()[0] ? -1 : 1, salida2.getReverses()[1] ? -1 : 1);
                    if (prueba1 && prueba2)
                        return true;

                }
        return false;
    }

    bool Probar(int entrada1, int entrada2, int salida1, int salida2)
    {
        int mix1, mix2;

        if (rev_mix)
        {
            mix1 = entrada1 + entrada2;
            mix2 = entrada1 - entrada2;
        }
        else
        {
            mix1 = entrada1 - entrada2;
            mix2 = entrada1 + entrada2;

        }
        if (rev_out1)
            mix1 = -mix1;
        if (rev_out2)
            mix2 = -mix2;

        if((Math.Sign(mix1)==Math.Sign(salida1))&&(Math.Sign(mix2)==Math.Sign(salida2)))
            return true;
        else
            return false;
    }
}


