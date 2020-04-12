using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PROYECTO_LFA_1251518
{
    public class TablesForm : Form
    {
        private IContainer components = (IContainer)null;
        private IBinaryTree<Node> expTree;
        private TreeGenerator treeGenerator;
        private int width, height, rows, acceptationStatus;
        private List<int>[] listFollows;
        private GrammarChecker grammarCheck;
        private Panel pnlTable;
        private DataGridView dgvFollow;
        private DataGridViewTextBoxColumn clmnNumber, clmnFollow, clmnSimbol, clmnFirst, clmnLast, clmnNullable, clmnStatus;
        private DataGridView dgvTable, dgvStatus;
        private Timer show;
        private ScrollablePanel panelScrollable;
        private NumericUpDown numericUpDown5, numericUpDown6;
        private Panel panel1;
        private Label lblTree, lblRegex, lblFLTitle, lblFollowTitle, lblStatusTitle;

        public TablesForm(GrammarChecker grammar, TreeGenerator generator, IBinaryTree<Node> tree, int simbols, string regex)
        {
            this.InitializeComponent();
            this.treeGenerator = generator;
            this.grammarCheck = grammar;
            
            this.lblRegex.Text = "Expresión Obtenida: " + regex;
            this.expTree = tree;
            this.rows = 0;
            this.width = simbols * 300;
            this.height = simbols * 110;
            this.panel1.Size = new Size(10000, this.height);
            this.listFollows = new List<int>[simbols - 1];
            for (int i = 0; i < simbols - 1; i++)
            {
                this.listFollows[i] = new List<int>();
                this.dgvFollow.Rows.Add();
            }
            this.dgvStatus.Rows.Add();
            this.expTree.postOrder(new TraversalTree<Node>(this.fillTableFirstLast));
            this.expTree.postOrder(new TraversalTree<Node>(this.fillFollow));
            this.fillFollowTable();
            this.generateStatusTable();
        }

        public void generateStatusTable()
        {
            for (int i = 0; i < this.treeGenerator.simbolSts.Count; i++)
            {
                DataGridViewTextBoxColumn viewTextBoxColumn = new DataGridViewTextBoxColumn();
                viewTextBoxColumn.HeaderText = this.treeGenerator.simbolSts.ToArray()[i];
                this.dgvStatus.Columns.Add((DataGridViewColumn)viewTextBoxColumn);
            }
            List<int> simbolList = new List<int>();
            for (int i = 0; i < this.expTree.Value.First.ToArray<int>().Length; i++)
                simbolList.Add(this.expTree.Value.First.ToArray<int>()[i]);
            simbolList.Sort();
            int j = -1;
            this.dgvStatus.Rows[0].Cells[0].Value = (object)this.getDGVStatus(simbolList);
            Simbol simbol = new Simbol();
            simbol.strSimbol = "#";
            simbol.intNumber = this.treeGenerator.simbolLst.Count + 1;
            this.acceptationStatus = this.treeGenerator.simbolLst.Count + 1;
            this.treeGenerator.simbolLst.Add(simbol);
            bool continueFlg = true;
            while (continueFlg)
            {
                j++;
                List<int> intList = this.getStatusGrid(this.dgvStatus.Rows[j].Cells[0].Value.ToString());
                List<int>[] intListArray = new List<int>[this.treeGenerator.simbolSts.Count];
                for (int k = 0; k < intListArray.Length; k++)
                    intListArray[k] = new List<int>();
                for (int k = 1; k <= this.treeGenerator.simbolSts.Count; k++)
                {
                    var headerText = this.dgvStatus.Columns[k].HeaderText;
                    foreach (int n in intList)
                    {
                        if (this.treeGenerator.simbolLst.ToArray()[n - 1].strSimbol.ToUpper().Equals(headerText))
                        {
                            foreach (int m in this.listFollows[n - 1])
                            {
                                if (!intListArray[k - 1].Contains(m))
                                    intListArray[k - 1].Add(m);
                            }
                        }
                    }
                    var status = this.getDGVStatus(intListArray[k - 1]);
                    if (!this.statusExists(status) && !status.Equals(" null "))
                    {
                        this.dgvStatus.Rows.Add();
                        this.dgvStatus.Rows[this.dgvStatus.Rows.Count - 1].Cells[0].Value = (object)status;
                        this.dgvStatus.Rows[j].Cells[k].Value = (object)status;
                    }
                    this.dgvStatus.Rows[j].Cells[k].Value = (object)status;
                }
                if (j + 1 == this.dgvStatus.Rows.Count)
                    continueFlg = false;
            }
        }

        private string getDGVStatus(List<int> status)
        {
            status.Sort();
            var element = string.Empty;
            foreach (int n in status)
                element = element + (object)n + ",";
            return element.Trim().Equals("") ? " null " : element.Remove(element.Length - 1);
        }

        private bool statusExists(string status)
        {
            for (int i = 0; i < this.dgvStatus.Rows.Count; i++)
            {
                if (this.dgvStatus.Rows[i].Cells[0].Value.ToString().Equals(status))
                    return true;
            }
            return false;
        }

        private List<int> getStatusGrid(string status)
        {
            string[] strArray = status.Split(',');
            List<int> intList = new List<int>();
            if (strArray.Length > 0)
            {
                for (int i = 0; i < strArray.Length; i++)
                    intList.Add(Convert.ToInt32(strArray[i]));
            }
            return intList;
        }

        private void drawTree(IBinaryTree<Node> tree, int x, int y, int interval)
        {
            if (tree == null)
                return;
            new DrawCircle(tree.Value.Simbol, x, y).Draw(this.panel1.CreateGraphics());
            if (tree.Right != null)
            {
                new DrawConector(x + 15, y + 15, x + interval + 25, y + 65).Draw(this.panel1.CreateGraphics());
                this.drawTree(tree.Right, x + interval, y + 70, 2 * interval / 3);
            }
            if (tree.Left != null)
            {
                new DrawConector(x + 15, y + 15, x - interval + 25, y + 65).Draw(this.panel1.CreateGraphics());
                this.drawTree(tree.Left, x - interval, y + 70, 2 * interval / 3);
            }
        }

        private void fillTableFirstLast(IBinaryTree<Node> tree)
        {
            this.dgvTable.Rows.Add();
            this.dgvTable.Rows[this.rows].Cells[0].Value = (object)tree.Value.Simbol;
            var value = string.Empty;
            for (int i = 0; i < tree.Value.First.Count; i++)
            {
                int n= tree.Value.First.ToArray<int>()[i];
                value = value + (object)n + ", ";
            }
            this.dgvTable.Rows[this.rows].Cells[1].Value = (object)value;
            var value2 = string.Empty;
            for (int i = 0; i < tree.Value.Last.Count; i++)
            {
                int n = tree.Value.Last.ToArray<int>()[i];
                value2 = value2 + (object)n + ", ";
            }
            this.dgvTable.Rows[this.rows].Cells[2].Value = (object)value2;
            this.dgvTable.Rows[this.rows].Cells[3].Value = (object)tree.Value.Nullable.ToString();
            this.rows++;
        }

        private void fillFollow(IBinaryTree<Node> tree)
        {
            if (tree.Value.Simbol.Equals("."))
            {
                for (int i = 0; i < tree.Left.Value.Last.Count; i++)
                {
                    int n = tree.Left.Value.Last.ToArray<int>()[i] - 1;
                    for (int j = 0; j < tree.Right.Value.First.Count; j++)
                    {
                        int m = tree.Right.Value.First.ToArray<int>()[j];
                        if (!this.listFollows[n].Contains(m))
                            this.listFollows[n].Add(m);
                    }
                }
            }
            else
            {
                if (!"*+".Contains(tree.Value.Simbol))
                    return;
                for (int i = 0; i < tree.Left.Value.Last.Count; i++)
                {
                    int n = tree.Left.Value.Last.ToArray<int>()[i] - 1;
                    for (int j = 0; j < tree.Left.Value.First.Count; j++)
                    {
                        int m = tree.Left.Value.First.ToArray<int>()[j];
                        if (!this.listFollows[n].Contains(m))
                            this.listFollows[n].Add(m);
                    }
                }
            }
        }

        private void fillFollowTable()
        {
            for (int i = 0; i < this.listFollows.Length; i++)
            {
                var value = string.Empty;
                this.listFollows[i].Sort();
                for (int j = 0; j < this.listFollows[i].Count; j++)
                    value = value + this.listFollows[i].ToArray()[j].ToString() + ", ";
                this.dgvFollow.Rows[i].Cells[0].Value = (object)(i + 1).ToString();
                this.dgvFollow.Rows[i].Cells[1].Value = (object)value;
            }
        }

        private void TablesForm_Activated(object sender, EventArgs e)
        {
            this.show.Enabled = true;
        }

        private void TablesForm_Load_1(object sender, EventArgs e)
        {
            this.show.Enabled = true;
            int num = (int)MessageBox.Show("El archivo es válido.", "Correcto !", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void show_Tick_1(object sender, EventArgs e)
        {
            this.drawTree((IBinaryTree<Node>)this.expTree, 3 * this.panel1.Width / 4, 120, 2200);
            this.show.Enabled = false;
        }

        private void panel1_ScrollHorizontal(object sender, ScrollEventArgs e)
        {
            this.numericUpDown5.Value = (Decimal)e.NewValue;
            this.drawTree((IBinaryTree<Node>)this.expTree, 3 * this.panel1.Width / 4, 120, 2200);
        }

        private void panel1_ScrollVertical(object sender, ScrollEventArgs e)
        {
            this.numericUpDown6.Value = (Decimal)e.NewValue;
            this.drawTree((IBinaryTree<Node>)this.expTree, 3 * this.panel1.Width / 4, 120, 2200);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            this.panelScrollable.AutoScrollHPos = Convert.ToInt32(this.numericUpDown5.Value);
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            this.panelScrollable.AutoScrollVPos = Convert.ToInt32(this.numericUpDown6.Value);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] directories = directoryInfo1.GetDirectories();
            if (!directoryInfo1.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);
            foreach (FileInfo file in directoryInfo1.GetFiles())
            {
                string destFileName = Path.Combine(destDirName, file.Name);
                file.CopyTo(destFileName, false);
            }
            if (!copySubDirs)
                return;
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
                TablesForm.DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.pnlTable = new Panel();
            this.dgvStatus = new DataGridView();
            this.clmnStatus = new DataGridViewTextBoxColumn();
            this.dgvFollow = new DataGridView();
            this.clmnNumber = new DataGridViewTextBoxColumn();
            this.clmnFollow = new DataGridViewTextBoxColumn();
            this.numericUpDown6 = new NumericUpDown();
            this.numericUpDown5 = new NumericUpDown();
            this.dgvTable = new DataGridView();
            this.clmnSimbol = new DataGridViewTextBoxColumn();
            this.clmnFirst = new DataGridViewTextBoxColumn();
            this.clmnLast = new DataGridViewTextBoxColumn();
            this.clmnNullable = new DataGridViewTextBoxColumn();
            this.show = new Timer(this.components);
            this.panelScrollable = new ScrollablePanel();
            this.lblTree = new Label();
            this.lblRegex = new Label();
            this.lblFLTitle = new Label();
            this.lblFollowTitle = new Label();
            this.lblStatusTitle = new Label();
            this.panel1 = new Panel();
            this.pnlTable.SuspendLayout();
            ((ISupportInitialize)this.dgvStatus).BeginInit();
            ((ISupportInitialize)this.dgvFollow).BeginInit();
            this.numericUpDown6.BeginInit();
            this.numericUpDown5.BeginInit();
            ((ISupportInitialize)this.dgvTable).BeginInit();
            this.panelScrollable.SuspendLayout();
            this.SuspendLayout();
            this.lblRegex.Location = new Point(1, 1);
            this.lblRegex.AutoSize = true;
            this.lblRegex.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblRegex.ForeColor = Color.DarkRed;
            this.lblRegex.Name = "lblRegex";
            this.lblRegex.Size = new Size(75, 24);
            this.lblFLTitle.Location = new Point(0, 0);
            this.lblFLTitle.AutoSize = true;
            this.lblFLTitle.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblFLTitle.ForeColor = Color.DarkRed;
            this.lblFLTitle.Text = "Tabla First, Last y Nullable";
            this.lblFollowTitle.Location = new Point(470, 0);
            this.lblFollowTitle.AutoSize = true;
            this.lblFollowTitle.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblFollowTitle.ForeColor = Color.DarkRed;
            this.lblFollowTitle.Text = "Tabla Follow";
            this.lblStatusTitle.Location = new Point(740, 0);
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblStatusTitle.ForeColor = Color.DarkRed;
            this.lblStatusTitle.Text = " Tabla Estados";
            this.pnlTable.BackColor = SystemColors.Control;
            this.pnlTable.Controls.Add((Control)this.lblFLTitle);
            this.pnlTable.Controls.Add((Control)this.lblFollowTitle);
            this.pnlTable.Controls.Add((Control)this.lblStatusTitle);
            this.pnlTable.Controls.Add((Control)this.dgvStatus);
            this.pnlTable.Controls.Add((Control)this.dgvFollow);
            this.pnlTable.Controls.Add((Control)this.numericUpDown6);
            this.pnlTable.Controls.Add((Control)this.numericUpDown5);
            this.pnlTable.Controls.Add((Control)this.dgvTable);
            this.pnlTable.Location = new Point(1, 25);
            this.pnlTable.Name = "pnlTabla";
            this.pnlTable.Size = new Size(1050, 190);
            this.pnlTable.TabIndex = 2;
            this.dgvStatus.AllowUserToAddRows = false;
            this.dgvStatus.AllowUserToDeleteRows = false;
            this.dgvStatus.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStatus.Columns.AddRange((DataGridViewColumn)this.clmnStatus);
            this.dgvStatus.Location = new Point(740, 20);
            this.dgvStatus.Name = "dgvEstados";
            this.dgvStatus.ReadOnly = true;
            this.dgvStatus.Size = new Size(300, 165);
            this.dgvStatus.TabIndex = 22;
            this.clmnStatus.HeaderText = "Estado";
            this.clmnStatus.Name = "clmStatus";
            this.clmnStatus.ReadOnly = true;
            this.dgvFollow.AllowUserToAddRows = false;
            this.dgvFollow.AllowUserToDeleteRows = false;
            this.dgvFollow.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFollow.Columns.AddRange((DataGridViewColumn)this.clmnNumber, (DataGridViewColumn)this.clmnFollow);
            this.dgvFollow.Location = new Point(470, 20);
            this.dgvFollow.Name = "dgvFollow";
            this.dgvFollow.ReadOnly = true;
            this.dgvFollow.Size = new Size(265, 165);
            this.dgvFollow.TabIndex = 1;
            this.clmnNumber.HeaderText = "Simbolo";
            this.clmnNumber.Name = "clmnNumeroSimbolo";
            this.clmnNumber.ReadOnly = true;
            this.clmnFollow.HeaderText = "Follow";
            this.clmnFollow.Name = "clmnFollow";
            this.clmnFollow.ReadOnly = true;
            this.numericUpDown6.Location = new Point(651, 108);
            this.numericUpDown6.Maximum = new Decimal(new int[4]{10000,0,0,0});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new Size(48, 20);
            this.numericUpDown6.TabIndex = 21;
            this.numericUpDown6.ValueChanged += new EventHandler(this.numericUpDown6_ValueChanged);
            this.numericUpDown5.Location = new Point(651, 82);
            this.numericUpDown5.Maximum = new Decimal(new int[4]{10000,0,0,0});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new Size(48, 20);
            this.numericUpDown5.TabIndex = 20;
            this.numericUpDown5.ValueChanged += new EventHandler(this.numericUpDown5_ValueChanged);
            this.dgvTable.AllowUserToAddRows = false;
            this.dgvTable.AllowUserToDeleteRows = false;
            this.dgvTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTable.Columns.AddRange((DataGridViewColumn)this.clmnSimbol, (DataGridViewColumn)this.clmnFirst, (DataGridViewColumn)this.clmnLast, (DataGridViewColumn)this.clmnNullable);
            this.dgvTable.Location = new Point(0, 20);
            this.dgvTable.Name = "dgvTabla";
            this.dgvTable.ReadOnly = true;
            this.dgvTable.Size = new Size(460, 165);
            this.dgvTable.TabIndex = 0;
            this.clmnSimbol.HeaderText = "Simbolo";
            this.clmnSimbol.Name = "clmnSimbolo";
            this.clmnSimbol.ReadOnly = true;
            this.clmnFirst.HeaderText = "First";
            this.clmnFirst.Name = "clmnFirst";
            this.clmnFirst.ReadOnly = true;
            this.clmnLast.HeaderText = "Last";
            this.clmnLast.Name = "clmnLast";
            this.clmnLast.ReadOnly = true;
            this.clmnNullable.HeaderText = "Nullable";
            this.clmnNullable.Name = "clmnNullable";
            this.clmnNullable.ReadOnly = true;
            this.show.Tick += new EventHandler(this.show_Tick_1);
            this.panelScrollable.AutoScroll = true;
            this.panelScrollable.AutoScrollHorizontalMaximum = 100;
            this.panelScrollable.AutoScrollHorizontalMinimum = 0;
            this.panelScrollable.AutoScrollHPos = 0;
            this.panelScrollable.AutoScrollVerticalMaximum = 100;
            this.panelScrollable.AutoScrollVerticalMinimum = 0;
            this.panelScrollable.AutoScrollVPos = 0;
            this.panelScrollable.BorderStyle = BorderStyle.Fixed3D;
            this.panelScrollable.Controls.Add((Control)this.lblTree);
            this.panelScrollable.Controls.Add((Control)this.panel1);
            this.panelScrollable.EnableAutoScrollHorizontal = true;
            this.panelScrollable.EnableAutoScrollVertical = true;
            this.panelScrollable.Location = new Point(1, 220);
            this.panelScrollable.Name = "panelScrollable";
            this.panelScrollable.Size = new Size(1044, 340);
            this.panelScrollable.TabIndex = 0;
            this.panelScrollable.VisibleAutoScrollHorizontal = true;
            this.panelScrollable.VisibleAutoScrollVertical = true;
            this.panelScrollable.ScrollHorizontal += new ScrollEventHandler(this.panel1_ScrollHorizontal);
            this.panelScrollable.ScrollVertical += new ScrollEventHandler(this.panel1_ScrollVertical);
            this.lblTree.AutoSize = true;
            this.lblTree.Text = "Representación Gráfica del Árbol de la Expresión Regular:";
            this.lblTree.Font = new Font("Arial", 15.75f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblTree.ForeColor = Color.DarkRed;
            this.lblTree.Location = new Point(9, 6);
            this.lblTree.Name = "lblTree";
            this.lblTree.Size = new Size(75, 24);
            this.lblTree.TabIndex = 1;
            this.panel1.Location = new Point(0, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(1030, 569);
            this.panel1.TabIndex = 0;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1050, 570);
            this.Controls.Add((Control)this.pnlTable);
            this.Controls.Add((Control)this.panelScrollable);
            this.Controls.Add((Control)this.lblRegex);
            this.Name = nameof(TablesForm);
            this.Text = "Proyecto_LFA";
            this.Load += new EventHandler(this.TablesForm_Load_1);
            this.pnlTable.ResumeLayout(false);
            ((ISupportInitialize)this.dgvStatus).EndInit();
            ((ISupportInitialize)this.dgvFollow).EndInit();
            this.numericUpDown6.EndInit();
            this.numericUpDown5.EndInit();
            ((ISupportInitialize)this.dgvTable).EndInit();
            this.panelScrollable.ResumeLayout(false);
            this.panelScrollable.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
