using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UAVConsole
{
    public partial class FormConfigurarTeclas : Form
    {
        bool asignando = false;

        Singleton me = Singleton.GetInstance();
        int[] asignaciones;
        public FormConfigurarTeclas()
        {
            InitializeComponent();
            asignaciones = new int[listBox1.Items.Count];
            if(asignaciones.Length!=me.asignaciones.Length)
                MessageBox.Show("Revisar el numero de teclas!");
            else
            {
                for(int i=0;i<asignaciones.Length;i++)
                    asignaciones[i]=me.asignaciones[i];
            }
        }

        private void FormConfigurarTeclas_KeyDown(object sender, KeyEventArgs e)
        {

            if (asignando)
            {
                if (e.KeyCode == Keys.Space)
                    MessageBox.Show(this, "Error: No se puede reasignar la tecla espacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
                {
                    bool encontrado = false;

                    foreach (int code in asignaciones)
                    {
                        if (code == (int)e.KeyData)
                        {
                            encontrado = true;
                            break;
                        }
                    }
                    if (encontrado)
                    {
                        MessageBox.Show(this, "Error: Esa combinación de teclas ya está asignada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        EnableOrDisable(true);
                        textBox2.Text = e.KeyData.ToString();
                        asignaciones[listBox1.SelectedIndex] = (int)e.KeyData;
                    }
                }
            }
        }

        void EnableOrDisable(bool state)
        {
            asignando = !state;
            listBox1.Enabled = state;
            button1.Enabled = state;
            button2.Enabled = state;
            button3.Enabled = state;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                EnableOrDisable(false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                textBox2.Text = ((Keys)(asignaciones[listBox1.SelectedIndex])).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                asignaciones[listBox1.SelectedIndex] = 0;
            textBox2.Text = ((Keys)0).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this,
                       "¿Realmente desea eliminar todas las asignaciones?",
                       "Limpiar Todo", MessageBoxButtons.YesNo);

            if (res == DialogResult.Yes)
            {
                for (int i = 0; i < asignaciones.Length; i++)
                    asignaciones[i] = 0;
                textBox2.Text = ((Keys)0).ToString();
            }
            
        }

        void ToSingleton()
        {
            if (asignaciones.Length != me.asignaciones.Length)
                MessageBox.Show("Revisar el numero de teclas!");
            else
            {
                for (int i = 0; i < asignaciones.Length; i++)
                    me.asignaciones[i] = asignaciones[i];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ToSingleton();
            me.ToRegistry();
            this.Close();
        }
    }
}
