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
        private int width, height, rows, acceptationStatus=0;
        private List<int>[] listFollows;
        private Panel pnlTable;
        private DataGridView dgvFollow;
        private DataGridViewTextBoxColumn clmnNumber, clmnFollow, clmnSimbol, clmnFirst, clmnLast, clmnNullable, clmnStatus;
        private DataGridView dgvTableFL, dgvStatus;
        private Timer show;
        private ScrollablePanel panelScrollable;
        private NumericUpDown numericUpDown5, numericUpDown6;
        private Panel panel1;
        private Label lblTree, lblRegex, lblFLTitle, lblFollowTitle, lblStatusTitle;
        private GrammarChecker grammar;
        private List<AutomatStatus> automat;
        private Button btnGenerate;

        public TablesForm(GrammarChecker grammarReceived,TreeGenerator generator, IBinaryTree<Node> tree, int simbols, string regex)
        {
            this.InitializeComponent();
            this.treeGenerator = generator;
            this.grammar = grammarReceived;
            this.automat = new List<AutomatStatus>();
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
            this.generateAutomat();
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var path = string.Empty;
            using (var fldrDlg = new FolderBrowserDialog())
            {
                if (fldrDlg.ShowDialog() == DialogResult.OK)
                {
                    path = fldrDlg.SelectedPath + "\\evaluar.cs";
                }
            }

            using (var writeStream = new FileStream(path, FileMode.Truncate))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    writer.Write(System.Text.Encoding.Unicode.GetBytes(this.codeGenerator()));
                }
            }
            int num = (int)MessageBox.Show("El programa se generó correctamente", "Correcto !", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


        }
        private string codeGenerator()
        {
            var content = string.Empty;
            var dictionary = generateDictionarys();
            var function = generateFunction();
            foreach (AutomatStatus auto in this.automat)
            {               
                var auxContent = string.Empty;
                auxContent += "bool acceptation =" + Convert.ToString(auto.acceptation).ToLower() + ";\r\naceptado=acceptation;\r\nvar transitions = new List<Transition>();\r\n"; 
                foreach (var item in auto.transitionsList)
                {
                    string re = Convert.ToString(Convert.ToChar(34));
                    var simbol = item.Simbol.Replace(re, Convert.ToChar(34) + "+Convert.ToChar(34)+" + Convert.ToChar(34));
                    var status = item.Status;
                    auxContent += "transitions.Add(new Transition("+ Convert.ToChar(34)+simbol + Convert.ToChar(34) + ","+status+ "));\r\n";
                }
                auxContent += "foreach (var item in transitions)\r\n{\r\nif (item.Simbol.Replace(" + Convert.ToChar(34) + "'" + Convert.ToChar(34) + "," + Convert.ToChar(34) + Convert.ToChar(34)+ ") == token)";
                auxContent += "\r\n{\r\nestado = item.Status - 1;\r\ntkn.Add(token);\r\n}\r\nelse if (buscarEnSets(token) == item.Simbol.Replace("+ Convert.ToChar(34) + "'" + Convert.ToChar(34) + "," + Convert.ToChar(34) + Convert.ToChar(34) + "))";
                auxContent += "\r\n{\r\nestado = item.Status - 1;\r\ntkn.Add(buscarEnSets(token));\r\n}\r\n}\r\n";
                content = content + "case " + (object)auto.current + ": { "+auxContent+"\r\n} break; \r\n";

            }
            return Generator.generate(content,dictionary);
        }
        private string generateFunction() 
        {
            var function = "public string buscarEnSets(string token)\r\n{\r\nforeach (var item in diccionarioSets)\r\n{\r\n";
            function += "var aux = item.Value.Replace(" + Convert.ToChar(34) + "'" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + ".." + Convert.ToChar(34) + ", " + Convert.ToChar(34) + "-" + Convert.ToChar(34) + "); ";
            function += "\r\nstring[] aux2 = aux.Split('+');\r\nfor (int i = 0; i < aux2.Count(); i++)\r\n{\r\n";
            function += "if (aux2[i].Contains(" + Convert.ToChar(34) + "-" + Convert.ToChar(34) + "))\r\n{\r\nint min = aux2[i][0];\r\nint max = aux2[i][2]; ";
            function += "if((int)Convert.ToChar(token) > min-1 && (int)Convert.ToChar(token) < max+1)\r\n{\r\nvar value = diccionarioSets.FirstOrDefault(x => x.Value == item.Value).Key;\r\nreturn value;\r\n}\r\n}\r\n";
            function += "else if (aux2[i].Length == 1)\r\n{\r\nif ((int)Convert.ToChar(token) == Convert.ToChar(aux2[i]))\r\n{\r\nvar value = diccionarioSets.FirstOrDefault(x => x.Value == item.Value).Key;\r\n return value;\r\n}\r\n}\r\n}\r\n";
            function += "}\r\nreturn " + Convert.ToChar(34) + Convert.ToChar(34) + ";\r\n}\r\n";
            return function;
        }
        private string generateDictionarys()
        {

            string dictionary = "static Dictionary<int, string> diccionarioTokensActions = new Dictionary<int, string>();\r\npublic void llenarTokensActions(){\r\ndiccionarioTokensActions.Clear();\r\n";
            foreach (var item in GrammarChecker.dictionaryTokensActions)
            {
                string re = Convert.ToString(Convert.ToChar(34));
                var aux = item.Value.Replace(re, Convert.ToChar(34) + "+Convert.ToChar(34)+" + Convert.ToChar(34));
                string element = "diccionarioTokensActions.Add(" + item.Key+","+ Convert.ToChar(34) + aux+ Convert.ToChar(34) + "); \r\n";
                dictionary += element;
            }
            dictionary += "}\r\nstatic Dictionary<string, string> diccionarioSets = new Dictionary<string, string>(); \r\npublic void llenarSets(){\r\ndiccionarioSets.Clear();\r\n";
            foreach (var item in GrammarChecker.dictionarySets)
            {
                string re = Convert.ToString(Convert.ToChar(34));
                var aux = item.Value.Replace(re, Convert.ToChar(34) + "+Convert.ToChar(34)+" + Convert.ToChar(34));
                string element = "diccionarioSets.Add(" + item.Key + "," + Convert.ToChar(34) + aux + Convert.ToChar(34) + "); \r\n";
                dictionary += element;
            }
            return dictionary + "}\r\n" + generateFunction();
        }
        private void generateAutomat()
        {
            for (int i = 0; i < this.dgvStatus.Rows.Count; i++)
            {
                AutomatStatus Status = new AutomatStatus();
                Status.current = i;
                Status.acceptation = this.isAcceptationStatus(this.dgvStatus.Rows[i].Cells[0].Value.ToString());
                for (int j = 1; j < this.dgvStatus.Columns.Count; j++)
                {
                    var header = this.dgvStatus.Columns[j].HeaderText;
                    var statusToFind = this.dgvStatus.Rows[i].Cells[j].Value.ToString();
                    if (!statusToFind.Equals(" null "))
                        Status.transitionsList.Add(new Transition(header, this.findStatus(statusToFind)));
                }
                this.automat.Add(Status);
            }
            foreach (Simbol simbolList in this.treeGenerator.simbolLst)
            {
                Ranges range = new Ranges();
                if (!this.existsInRanges(simbolList.strSimbol) && !simbolList.strSimbol.Contains("#"))
                {
                    range.simbol = simbolList.strSimbol;
                    this.grammar.isBasic(simbolList.strSimbol, range, false, false);
                    this.grammar.ranges.Add(range);
                }
            }
        }

        private bool existsInRanges(string simbol)
        {
            foreach (Ranges range in this.grammar.ranges)
            {
                if (range.simbol.Equals(simbol))
                    return true;
            }
            return false;
        }

        private bool isAcceptationStatus(string status)
        {
            char[] chArray = new char[1] { ',' };
            foreach (var item in status.Split(chArray))
            {
                if (this.acceptationStatus == Convert.ToInt32(item))
                    return true;
            }
            return false;
        }

        private int findStatus(string status)
        {
            for (int i = 0; i < this.dgvStatus.Rows.Count; i++)
            {
                if (status.Equals(this.dgvStatus.Rows[i].Cells[0].Value.ToString()))
                    return i;
            }
            return -1;
        }


        private void fillTableFirstLast(IBinaryTree<Node> tree)
        {
            this.dgvTableFL.Rows.Add();
            this.dgvTableFL.Rows[this.rows].Cells[0].Value = (object)tree.Value.Simbol;
            var value = string.Empty;
            for (int i = 0; i < tree.Value.First.Count; i++)
            {
                int n = tree.Value.First.ToArray<int>()[i];
                value = value + (object)n + ", ";
            }
            this.dgvTableFL.Rows[this.rows].Cells[1].Value = (object)value;
            var value2 = string.Empty;
            for (int i = 0; i < tree.Value.Last.Count; i++)
            {
                int n = tree.Value.Last.ToArray<int>()[i];
                value2 = value2 + (object)n + ", ";
            }
            this.dgvTableFL.Rows[this.rows].Cells[2].Value = (object)value2;
            this.dgvTableFL.Rows[this.rows].Cells[3].Value = (object)tree.Value.Nullable.ToString();
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
            this.treeGenerator.simbolLst.Add(simbol);
            this.acceptationStatus = this.treeGenerator.simbolLst.Count;
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
            this.dgvTableFL = new DataGridView();
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

            this.btnGenerate = new Button(); this.btnGenerate.Location = new Point(900, 0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new Size(130, 22);
            this.btnGenerate.Text = "Generar Programa";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new EventHandler(this.btnGenerate_Click);
            this.Controls.Add((Control)this.btnGenerate);

            ((ISupportInitialize)this.dgvStatus).BeginInit();
            ((ISupportInitialize)this.dgvFollow).BeginInit();
            this.numericUpDown6.BeginInit();
            this.numericUpDown5.BeginInit();
            ((ISupportInitialize)this.dgvTableFL).BeginInit();
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
            this.pnlTable.Controls.Add((Control)this.dgvTableFL);
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
            this.numericUpDown6.Maximum = new Decimal(new int[4] { 10000, 0, 0, 0 });
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new Size(48, 20);
            this.numericUpDown6.TabIndex = 21;
            this.numericUpDown6.ValueChanged += new EventHandler(this.numericUpDown6_ValueChanged);
            this.numericUpDown5.Location = new Point(651, 82);
            this.numericUpDown5.Maximum = new Decimal(new int[4] { 10000, 0, 0, 0 });
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new Size(48, 20);
            this.numericUpDown5.TabIndex = 20;
            this.numericUpDown5.ValueChanged += new EventHandler(this.numericUpDown5_ValueChanged);
            this.dgvTableFL.AllowUserToAddRows = false;
            this.dgvTableFL.AllowUserToDeleteRows = false;
            this.dgvTableFL.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableFL.Columns.AddRange((DataGridViewColumn)this.clmnSimbol, (DataGridViewColumn)this.clmnFirst, (DataGridViewColumn)this.clmnLast, (DataGridViewColumn)this.clmnNullable);
            this.dgvTableFL.Location = new Point(0, 20);
            this.dgvTableFL.Name = "dgvTabla";
            this.dgvTableFL.ReadOnly = true;
            this.dgvTableFL.Size = new Size(460, 165);
            this.dgvTableFL.TabIndex = 0;
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
            ((ISupportInitialize)this.dgvTableFL).EndInit();
            this.panelScrollable.ResumeLayout(false);
            this.panelScrollable.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
