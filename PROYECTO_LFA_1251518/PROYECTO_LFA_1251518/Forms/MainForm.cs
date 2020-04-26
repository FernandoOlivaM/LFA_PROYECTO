using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PROYECTO_LFA_1251518
{
    public class MainForm : Form
    {
        private IContainer components = (IContainer)null;
        private Label label1;
        private TextBox txbRoute;
        private Button btnOpen;
        private OpenFileDialog openFile;
        private GrammarChecker grammar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.txbRoute = new TextBox();
            this.btnOpen = new Button();
            this.openFile = new OpenFileDialog();
            this.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione el archivo que contiene la gramática";
            this.txbRoute.Enabled = false;
            this.txbRoute.Location = new Point(10, 30);
            this.txbRoute.Name = "txbRoute";
            this.txbRoute.Size = new Size(230, 20);
            this.txbRoute.TabIndex = 1;
            this.btnOpen.Location = new Point(90, 55);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new Size(75, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Examinar";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new EventHandler(this.btnOpen_Click);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(260, 100);
            this.Controls.Add((Control)this.btnOpen);
            this.Controls.Add((Control)this.txbRoute);
            this.Controls.Add((Control)this.label1);
            this.Name = nameof(MainForm);
            this.Text = "Proyecto LFA";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public MainForm()
        {
            this.InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.openFile.Title = "Seleccione el archivo que contiene la gramática";
            if (!this.openFile.ShowDialog().Equals((object)DialogResult.OK) || this.openFile.FileName.Equals(""))
                return;
            try
            {
                this.txbRoute.Text = this.openFile.FileName;
                string[] file = File.ReadAllLines(this.openFile.FileName);
                this.grammar = new GrammarChecker();
                this.grammar.correctFile(file);
            }
            catch (Exception ex)
            {
                string[] error = ex.Message.Split('|');
                if (error.Length == 3)
                {                    
                    int num = (int)MessageBox.Show(ErrorMessages.Message(ex.Message), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int num = (int)MessageBox.Show("Alguna sección no tiene el nombre correcto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

}
