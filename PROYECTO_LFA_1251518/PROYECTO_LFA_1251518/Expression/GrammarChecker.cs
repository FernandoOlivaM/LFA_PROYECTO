using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518
{
    public class GrammarChecker
    {
        public static Dictionary<int, string> dictionaryTokensActions = new Dictionary<int, string>();
        public static Dictionary<string, string> dictionarySets = new Dictionary<string, string>();

        private string spectedWord, lastWord;
        private int row, column;
        private int setCount, tokenCount, actionCount, errorCount;
        public List<Ranges> ranges;
        private List<string> setList;
        private List<int> tokenList;
        private Queue<string> regex;
        private List<string> actionList;
        private TreeGenerator tree;
        public GrammarChecker()
        {
            this.row = 0;
            this.column = 0;
            this.setCount = 0;
            this.tokenCount = 0;
            this.actionCount = 0;
            this.errorCount = 0;
            this.regex = new Queue<string>();
            this.actionList = new List<string>();
            this.setList = new List<string>();
            this.tokenList = new List<int>();
            this.ranges = new List<Ranges>();
        }
        public bool correctFile(string[] file)
        {
            this.checkSets(file);
            this.checkTokens(file);
            this.checkActions(file);
            this.checkError(file);
            this.buildTree(this.regex);
            return true;
        }
        public void buildTree(Queue<string> tokens)
        {
            var regex = string.Empty;
            while (tokens.Count > 0)
                regex = regex + tokens.Dequeue() + " ";
            this.tree = new TreeGenerator();
            this.tree.generate(regex);
            int num = (int)new TablesForm(this, this.tree, this.tree.expTree, this.tree.simbolQuantity, regex).ShowDialog();

        }
        private void checkSets(string[] file)
        {
            this.spectedWord = "SETS";
            this.lastWord = this.getWord(file);
            if (this.lastWord == null)
                return;
            if (this.lastWord.ToUpper().Equals(this.spectedWord))
            {
                this.lastWord = this.getWord(file);
                this.correctSet(file);
            }
            else
            {
                if (!this.lastWord.Equals(""))
                    return;
                this.checkSets(file);
            }
        }
        private void checkTokens(string[] file)
        {
            this.spectedWord = "TOKENS";
            if (this.lastWord == null)
                throw new Exception("1|" + (object)this.row + "|" + (object)this.column);
            if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.checkTokens(file);
            }
            else
            {
                if (!this.lastWord.ToUpper().Equals(this.spectedWord))
                    throw new Exception("1|" + (object)this.row + "|" + (object)this.column);
                this.lastWord = this.getWord(file);
                this.correctToken(file);
            }
        }
        private void checkActions(string[] file)
        {
            this.spectedWord = "ACTIONS";
            if (this.lastWord == null)
                return;
            if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.checkActions(file);
            }
            else
            {
                if (!this.lastWord.ToUpper().Equals(this.spectedWord))
                    return;
                this.correctActions(file);
            }
        }
        private string getWord(string[] file)
        {
            if (this.row == file.Length)
                return (string)null;
            var str1 = file[this.row];
            var str2 = string.Empty;
            while (this.column < str1.Length)
            {
                if (!str1[this.column].Equals(' ') && !str1.Equals((object)'\t'))
                {
                    str2 += str1[this.column].ToString().Trim();
                    this.column++;
                    if (this.column == str1.Length)
                        return str2.Trim();
                }
                else
                {
                    this.column++;
                    return str2.Trim();
                }
            }
            this.row++;
            this.column = 0;
            return this.getWord(file);
        }
        private void checkError(string[] file)
        {
            this.spectedWord = "ERROR";
            if (this.lastWord == null)
            {
                if (this.errorCount == 0)
                    throw new Exception("0|" + (object)this.row + "|" + (object)this.column);
            }
            else if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.checkError(file);
            }
            else if (this.lastWord.ToUpper().Contains(this.spectedWord))
                this.correctErrors(file);
            else
                throw new Exception("0|" + (object)this.row + "|" + (object)this.column);
        }
        private void correctErrors(string[] file)
        {
            if (this.lastWord == null)
            {
                if (this.errorCount == 0)
                    throw new Exception("0|" + (object)this.row + "|" + (object)this.column);
            }
            else if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.correctErrors(file);
            }
            else if (this.lastWord.ToUpper().Equals("ERROR"))
            {
                var input = string.Empty;
                for (this.lastWord = this.getWord(file); this.lastWord != null && !this.lastWord.ToUpper().Contains("ERROR"); this.lastWord = this.getWord(file))
                    input += this.lastWord;
                input.Trim();
                if (!Regex.IsMatch(input, "^=(\\s)*(\\d)+$", RegexOptions.Compiled))
                    throw new Exception("2|" + (object)this.row + "|" + (object)this.column);
                
                this.errorCount++;
                this.correctErrors(file);
            }
            else if (this.lastWord.ToUpper().Equals("ERROR="))
            {
                var input = string.Empty;
                do
                {
                    this.lastWord = this.getWord(file);
                    input += this.lastWord;
                }
                while (this.lastWord != null && !this.lastWord.ToUpper().Equals("ERROR"));
                input.Trim();
                if (!Regex.IsMatch(input, "^(\\d)+$", RegexOptions.Compiled))
                    throw new Exception("2|" + (object)this.row + "|" + (object)this.column);
                this.errorCount++;
                this.correctErrors(file);
            }
            else
            {
                if (!Regex.IsMatch(this.lastWord.ToUpper(), "^ERROR=(\\d)+$", RegexOptions.Compiled))
                    throw new Exception("2|" + (object)this.row + "|" + (object)this.column);
                this.lastWord = this.getWord(file);
                this.errorCount++;
                this.correctErrors(file);
            }
        }
        private void correctActions(string[] file)
        {
            var key = string.Empty; 
            var value = string.Empty;
            if (this.lastWord == null)
            {
                if (this.actionCount == 0)
                    throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
            }
            else if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.correctActions(file);
            }
            else if (this.lastWord.ToUpper().Contains("ERROR"))
            {
                if (this.actionCount == 0)
                    throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
            }
            else
            {
                Queue<string> actionsQueue = new Queue<string>();
                while (!this.lastWord.Equals("{"))
                {
                    this.lastWord = this.getWord(file);
                    if (this.lastWord == null)
                        throw new Exception("4|" + (object)this.row + "|" + (object)this.column);
                    if (!this.lastWord.Equals("") && !this.lastWord.Equals("{"))
                        actionsQueue.Enqueue(this.lastWord);
                }
                var action = string.Empty;
                while (actionsQueue.Count > 0)
                    action += actionsQueue.Dequeue();
                if (this.isAction(action))
                {
                    if (this.lastWord == null)
                        throw new Exception("5|" + (object)this.row + "|" + (object)this.column);
                    while (!this.lastWord.Equals("}"))
                    {
                        this.lastWord = this.getWord(file);
                        if (this.lastWord == null)
                            throw new Exception("5|" + (object)this.row + "|" + (object)this.column);
                        if (!this.lastWord.Equals("") && !this.lastWord.Equals("}"))
                        {
                            if (Regex.IsMatch(this.lastWord, "^(\\d)+$", RegexOptions.Compiled))
                            {
                                do
                                {
                                    key = this.lastWord;
                                    this.lastWord = this.getWord(file);
                                    if (this.lastWord == null)
                                        throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                                }
                                while (this.lastWord.Equals(""));
                                if (Regex.IsMatch(this.lastWord, "^=$", RegexOptions.Compiled))
                                {
                                    do
                                    {
                                        this.lastWord = this.getWord(file);
                                        value = this.lastWord;
                                        if (this.lastWord == null)
                                            throw new Exception("5|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    while (this.lastWord.Equals(""));
                                    if (!Regex.IsMatch(this.lastWord, "^'(\\w)+'$", RegexOptions.Compiled))
                                        throw new Exception("7|" + (object)this.row + "|" + (object)this.column);
                                }
                                else if (!Regex.IsMatch(this.lastWord, "^=('(\\w)+')$", RegexOptions.Compiled))
                                    throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                            }
                            else if (Regex.IsMatch(this.lastWord, "^(\\d)+=$", RegexOptions.Compiled))
                            {
                                do
                                {
                                    this.lastWord = this.getWord(file);
                                    if (this.lastWord == null)
                                        throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                                }
                                while (this.lastWord.Equals(""));
                                if (!Regex.IsMatch(this.lastWord, "^'(\\w)+'$", RegexOptions.Compiled))
                                    throw new Exception("7|" + (object)this.row + "|" + (object)this.column);
                            }
                            else if (!Regex.IsMatch(this.lastWord, "^(\\d)+='(\\w)+'$", RegexOptions.Compiled))
                                throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
                        }
                        if (!dictionaryTokensActions.ContainsKey(Convert.ToInt32(key)))
                            dictionaryTokensActions.Add(Convert.ToInt32(key), value);
                    }
                    this.lastWord = this.getWord(file);
                    this.actionCount++;
                    this.correctActions(file);
                }
                else
                    throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
            }
        }
        private void correctToken(string[] file)
        {
            if (this.lastWord != null)
            {
                if (!this.lastWord.Equals(""))
                {
                    if (this.lastWord.ToUpper().Equals("TOKEN"))
                    {
                        this.lastWord = string.Empty;
                        var rows = string.Empty;
                        bool status1 = false;
                        while (!this.lastWord.Equals("="))
                        {
                            if (this.row >= file.Length)
                                throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                            var fileRow = file[this.row];
                            this.lastWord = fileRow[this.column].ToString();
                            if (this.isDigit(fileRow[this.column]))
                            {
                                if (!status1)
                                    rows += fileRow[this.column].ToString();
                                else
                                    throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                            }
                            else
                            {
                                if (!this.lastWord.Trim().Equals(""))
                                {
                                    if (this.lastWord.Equals("=") && rows.Length > 0)
                                    {
                                        if (true)
                                        {
                                            if (this.lastWord.Equals("="))
                                            {
                                                if (!this.tokenList.Contains(Convert.ToInt32(rows)))
                                                {
                                                    dictionaryTokensActions.Add(Convert.ToInt32(rows), fileRow.Substring(fileRow.IndexOf('=')+1));
                                                    this.tokenList.Add(Convert.ToInt32(rows));
                                                    this.column++;
                                                    break;
                                                }
                                                throw new Exception("9|" + (object)this.row + "|" + (object)this.column);
                                            }
                                            throw new Exception("6|" + (object)this.row + "|" + (object)this.column);
                                        }
                                        throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                                }
                                if (rows.Length > 0)
                                    status1 = true;
                            }
                            //
                            this.column++;
                            if (this.column >= file[this.row].Length)
                            {
                                this.column = 0;
                                this.row++;
                            }
                        }
                        bool status2 = false;
                        int n = 0;
                        while (!this.lastWord.ToUpper().Equals("TOKEN") && !this.lastWord.ToUpper().Equals("ACTIONS") && !this.lastWord.ToUpper().Contains("ERROR"))
                        {
                            if (this.row >= file.Length)
                            {
                                if (this.tokenCount == 0)
                                    throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                                if (status2)
                                    break;
                                throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                            }
                            var fileRow = file[this.row];
                            this.lastWord = string.Empty;
                            while (this.column < fileRow.Length)
                            {
                                if (this.row >= file.Length)
                                {
                                    if (this.tokenCount != 0)
                                        return;
                                    throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                                }
                                fileRow = file[this.row];
                                char ch = fileRow[this.column];
                                var characthre = ch.ToString();
                                Exception exception;
                                if (characthre.Equals("("))
                                {
                                    this.regex.Enqueue(characthre);
                                    n++;
                                    status2 = false;
                                    ++this.column;
                                }
                                else if (characthre.Equals(")"))
                                {
                                    this.regex.Enqueue(characthre);
                                    n--;
                                    if (!status2)
                                        throw new Exception("11|" + (object)this.row + "|" + (object)this.column);
                                    if (n < 0)
                                        throw new Exception("12|" + (object)this.row + "|" + (object)this.column);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (characthre.Equals("{"))
                                {
                                    this.column++;
                                    string wordRow;
                                    do { } while ((wordRow = this.getWord(file)).Trim().Equals(""));
                                    if (this.isAction(wordRow.Trim()))
                                    {
                                        this.actionList.Add(wordRow);
                                        string wrd;
                                        do { } while ((wrd = this.getWord(file)).Trim().Equals(""));
                                        if (!wrd.Equals("}"))
                                            throw new Exception("19|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    else
                                        throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
                                }
                                else if (characthre.Equals("|"))
                                {
                                    this.regex.Enqueue(characthre);
                                    if (!status2)
                                        throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                    status2 = false;
                                    this.column++;
                                }
                                else if (characthre.Equals("."))
                                {
                                    if (!status2)
                                        throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                    status2 = false;
                                    this.column++;
                                }
                                else if (characthre.Equals("*"))
                                {
                                    this.regex.Enqueue(characthre);
                                    if (!status2)
                                        throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (characthre.Equals("+"))
                                {
                                    this.regex.Enqueue(characthre);
                                    if (!status2)
                                        throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (characthre.Equals("?"))
                                {
                                    this.regex.Enqueue(characthre);
                                    if (!status2)
                                        throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (characthre.Equals("'"))
                                {
                                    bool status3 = false;
                                    var wordRow = characthre;
                                    string auxWord;
                                    string auxWord2;
                                    try
                                    {
                                        this.column++;
                                        var auxWordRow = wordRow;
                                        ch = fileRow[this.column];
                                        string str8 = ch.ToString();
                                        auxWord = auxWordRow + str8;
                                        status3 = true;
                                        this.column++;
                                        ch = fileRow[this.column];
                                        auxWord2 = ch.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        exception = ex;
                                        if (!status3)
                                            throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                                        throw new Exception("14|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    if (!auxWord2.Equals("'"))
                                        throw new Exception("14|" + (object)this.row + "|" + (object)this.column);
                                    this.regex.Enqueue(auxWord + auxWord2);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (characthre.Equals("\""))
                                {
                                    var wordRow = characthre;
                                    bool status3 = false;
                                    string auxWord;
                                    string auxWord2;
                                    try
                                    {
                                        this.column++;
                                        var auxWordRow = wordRow;
                                        ch = fileRow[this.column];
                                        var str8 = ch.ToString();
                                        auxWord = auxWordRow + str8;
                                        status3 = true;
                                        this.column++;
                                        ch = fileRow[this.column];
                                        auxWord2 = ch.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        exception = ex;
                                        if (!status3)
                                            throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                                        throw new Exception("14|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    if (!auxWord2.Equals("\""))
                                        throw new Exception("14|" + (object)this.row + "|" + (object)this.column);
                                    this.regex.Enqueue(auxWord + auxWord2);
                                    status2 = true;
                                    this.column++;
                                }
                                else if (!characthre.Trim().Equals(""))
                                {
                                    var wordRow = string.Empty;
                                    while (true)
                                    {
                                        int m;
                                        if (this.column < fileRow.Length)
                                        {
                                            ch = fileRow[this.column];
                                            m = !"()+*? \t'\"|.".Contains(ch.ToString()) ? 1 : 0;
                                        }
                                        else
                                            m = 0;
                                        if (m != 0)
                                        {
                                            ch = fileRow[this.column];
                                            string str5 = ch.ToString();
                                            wordRow += str5;
                                            this.column++;
                                        }
                                        else
                                            break;
                                    }
                                    var word = wordRow.Trim();
                                    int p;
                                    if (this.column < fileRow.Length)
                                    {
                                        ch = fileRow[this.column];
                                        p = !ch.ToString().Equals("(") ? 1 : 0;
                                    }
                                    else
                                        p = 1;
                                    if (p == 0)
                                    {
                                        if (word.ToUpper().Equals("CHR"))
                                        {
                                            string auxWord = word;
                                            ch = fileRow[this.column];
                                            string auxWord2 = ch.ToString();
                                            string auxWordRow = auxWord + auxWord2;
                                            this.column++;
                                            var data = string.Empty;
                                            while (true)
                                            {
                                                int num2;
                                                if (this.column < fileRow.Length)
                                                {
                                                    ch = fileRow[this.column];
                                                    num2 = !")".Equals(ch.ToString()) ? 1 : 0;
                                                }
                                                else
                                                    num2 = 0;
                                                if (num2 != 0)
                                                {
                                                    string auxData = data;
                                                    ch = fileRow[this.column];
                                                    string dataChar = ch.ToString();
                                                    data = auxData + dataChar;
                                                    ++this.column;
                                                }
                                                else
                                                    break;
                                            }
                                            if (this.column >= fileRow.Length)
                                                throw new Exception("11|" + (object)this.row + "|" + (object)this.column);
                                            if (!this.isNumber(data))
                                                throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                                            this.regex.Enqueue(auxWordRow + data + (object)fileRow[this.column]);
                                            this.column++;
                                            status2 = true;
                                        }
                                    }
                                    else if (word.ToUpper().Equals("TOKEN") || word.ToUpper().Equals("ACTIONS") || word.ToUpper().Contains("ERROR"))
                                    {
                                        this.lastWord = word;
                                        if (word.ToUpper().Equals("TOKEN") && this.regex.Count > 0)
                                        {
                                            if (!status2)
                                                throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                            if (n != 0)
                                                throw new Exception("11|" + (object)this.row + "|" + (object)this.column);
                                            this.regex.Enqueue("|");
                                            this.tokenCount++;
                                            this.correctToken(file);
                                            return;
                                        }
                                        if (word.ToUpper().Equals("ACTIONS") || word.ToUpper().Contains("ERROR"))
                                        {
                                            if (!status2)
                                                throw new Exception("10|" + (object)this.row + "|" + (object)this.column);
                                            if (n != 0)
                                                throw new Exception("11|" + (object)this.row + "|" + (object)this.column);
                                            this.tokenCount++;
                                            return;
                                        }
                                        this.lastWord = word;
                                    }
                                    else if (this.identifier(word))
                                    {
                                        if (this.setList.Contains(word.ToUpper()))
                                        {
                                            this.regex.Enqueue(word);
                                            status2 = true;
                                        }
                                        else
                                            throw new Exception("15|" + (object)this.row + "|" + (object)this.column);
                                    }
                                    else
                                        throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
                                }
                                else
                                    this.column++;
                            }
                            this.row++;
                            this.column = 0;
                        }
                    }
                    else if (this.tokenCount == 0)
                        throw new Exception("16|" + (object)this.row + "|" + (object)this.column);
                }
                else
                {
                    this.lastWord = this.getWord(file);
                    this.correctToken(file);
                }
            }
            else if (this.tokenCount == 0)
                throw new Exception("16|" + (object)this.row + "|" + (object)this.column);
        }
        private void correctSet(string[] file)
        {
            if (this.lastWord == null)
            {
                if (this.setCount == 0)
                    throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
            }
            else if (this.lastWord.Equals(""))
            {
                this.lastWord = this.getWord(file);
                this.correctSet(file);
            }
            else if (this.lastWord.ToUpper().Equals("TOKENS"))
            {
                if (this.setCount <= 0)
                    throw new Exception("17|" + (object)this.row + "|" + (object)this.column);
            }
            else
            {
                int num;
                if (!this.identifier(this.lastWord))
                    num = this.lastWord.Split('=').Length > 2 ? 1 : 0;
                else
                    num = 0;
                if (num == 0)
                {
                    Queue<string> setQueue = new Queue<string>();
                    setQueue.Enqueue(this.lastWord);
                    for (this.lastWord = this.getWord(file); !this.identifier(this.lastWord) && this.lastWord != null; this.lastWord = this.getWord(file))
                    {
                        if (this.lastWord == null && this.setCount == 0)
                            throw new Exception("17|" + (object)this.row + "|" + (object)this.column);
                        if (!this.lastWord.Equals(""))
                            setQueue.Enqueue(this.lastWord);
                    }
                    var valueSet = string.Empty;
                    foreach (var item in setQueue)
                    {
                        valueSet += item;
                    }
                    dictionarySets.Add(Convert.ToChar(34)+(valueSet.Substring(0, valueSet.IndexOf("=")))+ Convert.ToChar(34), (valueSet.Substring(valueSet.IndexOf("=")+1)));
                    this.checkSetExpression(setQueue);
                    this.setCount++;
                    this.correctSet(file);
                }
                else
                    throw new Exception("3|" + (object)this.row + "|" + (object)this.column);
            }
        }
        private void checkSetExpression(Queue<string> set)
        {
            var element = string.Empty;
            while (set.Count > 0)
                element += set.Dequeue();
            string[] strArray1 = element.Split('=');
            if (this.identifier(strArray1[0]))
            {
                Ranges rang = new Ranges();
                if (!this.setList.Contains(strArray1[0].ToUpper()))
                {
                    this.setList.Add(strArray1[0].ToUpper());
                    rang.simbol = strArray1[0];
                    if (strArray1[1].Contains("+"))
                    {
                        string[] strArray2 = strArray1[1].Split('+');
                        for (int i = 0; i < strArray2.Length; i++)
                        {
                            if (strArray2[i].Contains(".."))
                            {
                                string[] strArray3 = strArray2[i].Split(new string[1] {".."}, StringSplitOptions.RemoveEmptyEntries);
                                for (int j = 0; j < strArray3.Length; j++)
                                {
                                    if (!this.isBasic(strArray3[j], rang, true, j % 2 == 0))
                                        throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                                }
                            }
                            else if (!this.isBasic(strArray2[i], rang, false, false))
                                throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                        }
                    }
                    else if (strArray1[1].Contains(".."))
                    {
                        string[] strArray2 = strArray1[1].Split(new string[1] {".."}, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < strArray2.Length; i++)
                        {
                            if (!this.isBasic(strArray2[i], rang, true, i % 2 == 0))
                                throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                        }
                    }
                    else
                    {
                        if (this.isBasic(strArray1[1], rang, false, false))
                            return;
                        throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
                    }
                    this.ranges.Add(rang);
                }
                else
                    throw new Exception("18|" + (object)this.row + "|" + (object)this.column);
            }
            else
                throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
        }
        private bool identifier(string word)
        {
            if (word == null || word.Trim().Length == 0 || !this.isLettre(word[0]))
                return false;
            for (int i = 0; i < word.Length; i++)
            {
                if (!this.isLettre(word[i]) && !this.isDigit(word[i]))
                    return false;
            }
            return true;
        }
        private bool isLettre(char lettre)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".Contains<char>(lettre);
        }
        private bool isDigit(char digit)
        {
            return "0123456789".Contains<char>(digit);
        }
        private bool isNumber(string number)
        {
            return Regex.IsMatch(number, "^(\\d)+$", RegexOptions.Compiled);
        }
        private bool isAction(string chain)
        {
            return new Regex("^[a-zA-Z_]+(\\w)*(\\s)*\\(\\)$(\\s)*").IsMatch(chain);
        }
        public bool isBasic(string word, Ranges range, bool isRange, bool isLow)
        {
            if ('\''.Equals(word[0]))
            {
                if (word.Length == 3 && '\''.Equals(word[2]))
                {
                    if (isRange)
                    {
                        if (isLow)
                            range.low.Add((int)word[1]);
                        else
                            range.high.Add((int)word[1]);
                    }
                    else
                        range.single.Add((int)word[1]);
                    return true;
                }
                throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
            }
            char charactre = '"';
            if (charactre.Equals(word[0]))
            {
                int number;
                if (word.Length == 3)
                {
                    charactre = '"';
                    number = !charactre.Equals(word[2]) ? 1 : 0;
                }
                else
                    number = 1;
                if (number == 0)
                {
                    if (isRange)
                    {
                        if (isLow)
                            range.low.Add((int)word[1]);
                        else
                            range.high.Add((int)word[1]);
                    }
                    else
                        range.single.Add((int)word[1]);
                    return true;
                }
                throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
            }
            if (word.ToUpper().Contains("CHR("))
            {
                charactre = ')';
                if (charactre.Equals(word[word.Length - 1]))
                {
                    word = word.Replace("CHR(", "").Replace(")", "");
                    if (this.isNumber(word.Trim()))
                    {
                        if (isRange)
                        {
                            if (isLow)
                                range.low.Add(Convert.ToInt32(word));
                            else
                                range.high.Add(Convert.ToInt32(word));
                        }
                        else
                            range.single.Add(Convert.ToInt32(word));
                        return true;
                    }
                    throw new Exception("8|" + (object)this.row + "|" + (object)this.column);
                }
                throw new Exception("11|" + (object)this.row + "|" + (object)this.column);
            }
            throw new Exception("13|" + (object)this.row + "|" + (object)this.column);
        }

    }
}
