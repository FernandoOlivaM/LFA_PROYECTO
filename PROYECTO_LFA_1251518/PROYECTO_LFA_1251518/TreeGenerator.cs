using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518
{
    public class TreeGenerator
    {
        private Queue<BynaryTree<Node>> expresionQueue;
        private int simbolCount;
        private List<Simbol> simbolList;
        public void generate(string regex)
        {
            this.buildQueue(regex);
        }
        private void buildQueue(string regex)
        {
            regex = "(" + regex + ")#";
            for (int i = 0; i < regex.Length; ++i)
            {
                char ch = regex[i];
                var simb = ch.ToString().Trim();
                if (simb.Equals("("))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals(")"))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("*"))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("+"))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("?"))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("."))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("|"))
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(new Node(simb)));
                else if (simb.Equals("#"))
                {
                    Node node = new Node(simb);
                    node.First.AddLast(this.simbolCount);
                    node.Last.AddLast(this.simbolCount);
                    node.Nullable = false;
                    ++this.simbolCount;
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(node));
                }
                else if (simb.Equals("'"))
                {
                    var simb2 = simb + (object)regex[i + 1] + (object)regex[i + 2];
                    i += 2;
                    Node dato = new Node(simb2);
                    dato.First.AddLast(this.simbolCount);
                    dato.Last.AddLast(this.simbolCount);
                    dato.Nullable = false;
                    this.simbolList.Add(new Simbol()
                    {
                        strSimbol = simb2,
                        intNumber = this.simbolCount
                    });
                    ++this.simbolCount;
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(dato));
                }
                else if (simb.Equals("\""))
                {
                    var simb2 = simb + (object)regex[i + 1] + (object)regex[i + 2];
                    i += 2;
                    Node node = new Node(simb2);
                    node.First.AddLast(this.simbolCount);
                    node.Last.AddLast(this.simbolCount);
                    node.Nullable = false;
                    this.simbolList.Add(new Simbol()
                    {
                        strSimbol = simb2,
                        intNumber = this.simbolCount
                    });
                    ++this.simbolCount;
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(node));
                }
                else if (!simb.Equals(""))
                {
                    var simb2 = string.Empty;
                    for (; !"()*+?.|'\" ".Contains(simb) || i >= regex.Length; simb = ch.ToString())
                    {
                        simb2 += simb;
                        ++i;
                        ch = regex[i];
                    }
                    if (simb2.ToUpper().Equals("CHR"))
                    {
                        for (; !")".Contains(simb) || i >= regex.Length; simb = ch.ToString())
                        {
                            simb2 += simb;
                            ++i;
                            ch = regex[i];
                        }
                        ch = regex[i];
                        string str = ch.ToString();
                        simb2 += str;
                        ++i;
                    }
                    Node node = new Node(simb2);
                    node.First.AddLast(this.simbolCount);
                    node.Last.AddLast(this.simbolCount);
                    node.Nullable = false;
                    this.simbolList.Add(new Simbol()
                    {
                        strSimbol = simb2,
                        intNumber = this.simbolCount
                    });
                    ++this.simbolCount;
                    this.expresionQueue.Enqueue(new BynaryTree<Node>(node));
                    --i;
                }
            }
        }

    }
}
