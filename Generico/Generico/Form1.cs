using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Generico
{
    public class Form1 : Form
    {
        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private OpenFileDialog openFileDialog1;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.textBox1 = new TextBox();
            this.button1 = new Button();
            this.openFileDialog1 = new OpenFileDialog();
            this.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ingrese el lenguaje";
            this.textBox1.Location = new Point(10, 30);
            this.textBox1.Name = "txbRoute";
            this.textBox1.Size = new Size(230, 20);
            this.textBox1.TabIndex = 1;
            this.button1.Location = new Point(90, 55);
            this.button1.Name = "btnOpen";
            this.button1.Size = new Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Verificar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(260, 100);
            this.Controls.Add((Control)this.button1);
            this.Controls.Add((Control)this.textBox1);
            this.Controls.Add((Control)this.label1);
            this.Name = nameof(Form1);
            this.Text = "Proyecto LFA";
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            evaluar eva = new evaluar();
            if(textBox1.Text.Length > 0)
            {
                eva.llenarSets();
                eva.llenarTokensActions();
                bool v = eva.verificar(textBox1.Text);
                if (v)
                {
                    int num = (int)MessageBox.Show("La Cadena ingresada es válida", "Correcto!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    var texto = eva.buscar(textBox1.Text);
                    int num2 = (int)MessageBox.Show(texto.Replace("enter", Convert.ToString(Convert.ToChar(13))), "Correcto!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                }
                else
                {
                    int num = (int)MessageBox.Show("La Cadena ingresada no es válida", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                int num = (int)MessageBox.Show("La Cadena esta vacía", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        

    }
}
