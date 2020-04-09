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
        private BynaryTree<Node> expresionTree;
        private List<string> simbolStatus;
        public int simbolQuantity
        {
            get{return this.simbolCount;}
        }

        public List<Simbol> simbolLst
        {
            get{return this.simbolList;}
        }

        public List<string> simbolSts
        {
            get{return this.simbolStatus;}
        }
        public void generate(string regex)
        {
            this.buildQueue(regex);
            this.expresionTree = new BynaryTree<Node>(new Node("."));
            this.expTree.Left = (IBinaryTree<Node>)this.generateExpression(ref this.expresionQueue);
            this.expTree.Right = (IBinaryTree<Node>)this.expresionQueue.Dequeue();
            this.expTree.postOrder(new TraversalTree<Node>(this.getFirstLast));
            for (int i = 0; i < this.simbolList.Count; ++i)
            {
                if (!this.simbolStatus.Contains(this.simbolList.ToArray()[i].strSimbol))
                    this.simbolStatus.Add(this.simbolList.ToArray()[i].strSimbol);
            }
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
        public BynaryTree<Node> expTree
        {
            get
            {
                return this.expresionTree;
            }
        }
        private BynaryTree<Node> generateExpression(ref Queue<BynaryTree<Node>> expresionQueue)
        {
            Stack<BynaryTree<Node>> bynaryTrees = new Stack<BynaryTree<Node>>();
            if (expresionQueue == null)
                throw new Exception("No existe expresión regular para evaluar.");
            while (expresionQueue.Count > 0)
            {
                if (expresionQueue.Peek().Value.Simbol.Equals("#"))
                {
                    if (bynaryTrees.Count <= 1)
                        return bynaryTrees.Pop();
                    return new BynaryTree<Node>(new Node("."))
                    {
                        Right = (IBinaryTree<Node>)bynaryTrees.Pop(),
                        Left = (IBinaryTree<Node>)bynaryTrees.Pop()
                    };
                }
                BynaryTree<Node> bynaryTree1 = expresionQueue.Dequeue();
                if (this.operatorSimbol(bynaryTree1))
                {
                    bynaryTrees.Push(bynaryTree1);
                    if (bynaryTrees.Count >= 3)
                    {
                        BynaryTree<Node> binaryTree2 = bynaryTrees.Pop();
                        BynaryTree<Node> binaryTree3 = bynaryTrees.Pop();
                        BynaryTree<Node> binaryTree4 = bynaryTrees.Pop();
                        bynaryTrees.Push(new BynaryTree<Node>(new Node("."))
                        {
                            Right = (IBinaryTree<Node>)binaryTree3,
                            Left = (IBinaryTree<Node>)binaryTree4
                        });
                        bynaryTrees.Push(binaryTree2);
                    }
                }
                else
                {
                    if (!this.isOperator(bynaryTree1))
                        throw new Exception("Error en la expresion regular");
                    if ("+*?".Contains(bynaryTree1.Value.Simbol))
                    {
                        bynaryTree1.Left = (IBinaryTree<Node>)bynaryTrees.Pop();
                        bynaryTrees.Push(bynaryTree1);
                    }
                    else if ("(".Equals(bynaryTree1.Value.Simbol))
                    {
                        bynaryTrees.Push(this.generateExpression(ref expresionQueue));
                        if (bynaryTrees.Count >= 3)
                        {
                            BynaryTree<Node> arbolBinarioBase2 = bynaryTrees.Pop();
                            BynaryTree<Node> arbolBinarioBase3 = bynaryTrees.Pop();
                            BynaryTree<Node> arbolBinarioBase4 = bynaryTrees.Pop();
                            bynaryTrees.Push(new BynaryTree<Node>(new Node("."))
                            {
                                Right = (IBinaryTree<Node>)arbolBinarioBase3,
                                Left = (IBinaryTree<Node>)arbolBinarioBase4
                            });
                            bynaryTrees.Push(arbolBinarioBase2);
                        }
                    }
                    else
                    {
                        if (")".Equals(bynaryTree1.Value.Simbol))
                        {
                            if (bynaryTrees.Count <= 1)
                                return bynaryTrees.Pop();
                            return new BynaryTree<Node>(new Node("."))
                            {
                                Right = (IBinaryTree<Node>)bynaryTrees.Pop(),
                                Left = (IBinaryTree<Node>)bynaryTrees.Pop()
                            };
                        }
                        if ("|".Equals(bynaryTree1.Value.Simbol))
                        {
                            if (bynaryTrees.Count <= 1)
                            {
                                bynaryTree1.Left = (IBinaryTree<Node>)bynaryTrees.Pop();
                                bynaryTree1.Right = (IBinaryTree<Node>)this.generateOr(ref expresionQueue);
                                bynaryTrees.Push(bynaryTree1);
                            }
                            else
                            {
                                bynaryTree1.Left = (IBinaryTree<Node>)new BynaryTree<Node>(new Node("."))
                                {
                                    Right = (IBinaryTree<Node>)bynaryTrees.Pop(),
                                    Left = (IBinaryTree<Node>)bynaryTrees.Pop()
                                };
                                bynaryTree1.Right = (IBinaryTree<Node>)this.generateOr(ref expresionQueue);
                                bynaryTrees.Push(bynaryTree1);
                            }
                        }
                    }
                }
            }
            return bynaryTrees.Pop();
        }
        private bool operatorSimbol(BynaryTree<Node> node)
        {
            return !"()*+?.|".Contains(node.Value.Simbol);
        }

        private bool isOperator(BynaryTree<Node> node)
        {
            return !this.operatorSimbol(node);
        }
        private BynaryTree<Node> generateOr(ref Queue<BynaryTree<Node>> expressionQueue)
        {
            Stack<BynaryTree<Node>> arbolBinarioBaseStack = new Stack<BynaryTree<Node>>();
            if (expressionQueue == null)
                throw new Exception("Error en la expresion regular");
            while (expressionQueue.Count > 0)
            {
                BynaryTree<Node> arbolBinarioBase1 = expressionQueue.Peek();
                if (this.operatorSimbol(arbolBinarioBase1))
                {
                    BynaryTree<Node> arbolBinarioBase2 = expressionQueue.Dequeue();
                    arbolBinarioBaseStack.Push(arbolBinarioBase2);
                    if (arbolBinarioBaseStack.Count >= 3)
                    {
                        BynaryTree<Node> arbolBinarioBase3 = arbolBinarioBaseStack.Pop();
                        BynaryTree<Node> arbolBinarioBase4 = arbolBinarioBaseStack.Pop();
                        BynaryTree<Node> arbolBinarioBase5 = arbolBinarioBaseStack.Pop();
                        arbolBinarioBaseStack.Push(new BynaryTree<Node>(new Node("."))
                        {
                            Right = (IBinaryTree<Node>)arbolBinarioBase4,
                            Left = (IBinaryTree<Node>)arbolBinarioBase5
                        });
                        arbolBinarioBaseStack.Push(arbolBinarioBase3);
                    }
                }
                else
                {
                    if (!this.isOperator(arbolBinarioBase1))
                        throw new Exception("Se encontro un simbolo no conocido");
                    if ("+*?".Contains(arbolBinarioBase1.Value.Simbol))
                    {
                        BynaryTree<Node> arbolBinarioBase2 = expressionQueue.Dequeue();
                        arbolBinarioBase2.Left = (IBinaryTree<Node>)arbolBinarioBaseStack.Pop();
                        arbolBinarioBaseStack.Push(arbolBinarioBase2);
                    }
                    else if ("(".Equals(arbolBinarioBase1.Value.Simbol))
                    {
                        expressionQueue.Dequeue();
                        arbolBinarioBaseStack.Push(this.generateExpression(ref expressionQueue));
                        if (arbolBinarioBaseStack.Count >= 3)
                        {
                            BynaryTree<Node> arbolBinarioBase2 = arbolBinarioBaseStack.Pop();
                            BynaryTree<Node> arbolBinarioBase3 = arbolBinarioBaseStack.Pop();
                            BynaryTree<Node> arbolBinarioBase4 = arbolBinarioBaseStack.Pop();
                            arbolBinarioBaseStack.Push(new BynaryTree<Node>(new Node("."))
                            {
                                Right = (IBinaryTree<Node>)arbolBinarioBase3,
                                Left = (IBinaryTree<Node>)arbolBinarioBase4
                            });
                            arbolBinarioBaseStack.Push(arbolBinarioBase2);
                        }
                    }
                    else
                    {
                        if (")".Equals(arbolBinarioBase1.Value.Simbol))
                        {
                            if (arbolBinarioBaseStack.Count <= 1)
                                return arbolBinarioBaseStack.Pop();
                            return new BynaryTree<Node>(new Node("."))
                            {
                                Right = (IBinaryTree<Node>)arbolBinarioBaseStack.Pop(),
                                Left = (IBinaryTree<Node>)arbolBinarioBaseStack.Pop()
                            };
                        }
                        if ("|".Equals(arbolBinarioBase1.Value.Simbol))
                        {
                            if (arbolBinarioBaseStack.Count <= 1)
                                return arbolBinarioBaseStack.Pop();
                            return new BynaryTree<Node>(new Node("."))
                            {
                                Right = (IBinaryTree<Node>)arbolBinarioBaseStack.Pop(),
                                Left = (IBinaryTree<Node>)arbolBinarioBaseStack.Pop()
                            };
                        }
                    }
                }
            }
            return arbolBinarioBaseStack.Pop();
        }
        private void getFirstLast(IBinaryTree<Node> tree)
        {
            if (tree.Value.Simbol.Equals("|"))
            {
                for (int i = 0; i < tree.Left.Value.First.Count; ++i)
                {
                    int value = tree.Left.Value.First.ToArray<int>()[i];
                    if (!tree.Value.First.Contains(value))
                        tree.Value.First.AddLast(value);
                }
                for (int i = 0; i < tree.Right.Value.First.Count; ++i)
                {
                    int value = tree.Right.Value.First.ToArray<int>()[i];
                    if (!tree.Value.First.Contains(value))
                        tree.Value.First.AddLast(value);
                }
                for (int i = 0; i < tree.Left.Value.Last.Count; ++i)
                {
                    int value = tree.Left.Value.Last.ToArray<int>()[i];
                    if (!tree.Value.Last.Contains(value))
                        tree.Value.Last.AddLast(value);
                }
                for (int i = 0; i < tree.Right.Value.Last.Count; ++i)
                {
                    int value = tree.Right.Value.Last.ToArray<int>()[i];
                    if (!tree.Value.Last.Contains(value))
                        tree.Value.Last.AddLast(value);
                }
                tree.Value.Nullable = tree.Right.Value.Nullable || tree.Left.Value.Nullable;
            }
            if (tree.Value.Simbol.Equals("."))
            {
                if (tree.Left.Value.Nullable)
                {
                    for (int i = 0; i < tree.Left.Value.First.Count; ++i)
                    {
                        int value = tree.Left.Value.First.ToArray<int>()[i];
                        if (!tree.Value.First.Contains(value))
                            tree.Value.First.AddLast(value);
                    }
                    for (int i = 0; i < tree.Right.Value.First.Count; ++i)
                    {
                        int value = tree.Right.Value.First.ToArray<int>()[i];
                        if (!tree.Value.First.Contains(value))
                            tree.Value.First.AddLast(value);
                    }
                }
                else
                {
                    for (int i = 0; i < tree.Left.Value.First.Count; ++i)
                    {
                        int value = tree.Left.Value.First.ToArray<int>()[i];
                        if (!tree.Value.First.Contains(value))
                            tree.Value.First.AddLast(value);
                    }
                }
                if (tree.Right.Value.Nullable)
                {
                    for (int i = 0; i < tree.Left.Value.Last.Count; ++i)
                    {
                        int value = tree.Left.Value.Last.ToArray<int>()[i];
                        if (!tree.Value.Last.Contains(value))
                            tree.Value.Last.AddLast(value);
                    }
                    for (int i = 0; i < tree.Right.Value.Last.Count; ++i)
                    {
                        int value = tree.Right.Value.Last.ToArray<int>()[i];
                        if (!tree.Value.Last.Contains(value))
                            tree.Value.Last.AddLast(value);
                    }
                }
                else
                {
                    for (int i = 0; i < tree.Right.Value.Last.Count; ++i)
                    {
                        int value = tree.Right.Value.Last.ToArray<int>()[i];
                        if (!tree.Value.Last.Contains(value))
                            tree.Value.Last.AddLast(value);
                    }
                }
                tree.Value.Nullable = tree.Right.Value.Nullable && tree.Left.Value.Nullable;
            }
            if (tree.Value.Simbol.Equals("*"))
            {
                for (int i = 0; i < tree.Left.Value.First.Count; ++i)
                {
                    int value = tree.Left.Value.First.ToArray<int>()[i];
                    if (!tree.Value.First.Contains(value))
                        tree.Value.First.AddLast(value);
                }
                for (int i = 0; i < tree.Left.Value.Last.Count; ++i)
                {
                    int value = tree.Left.Value.Last.ToArray<int>()[i];
                    if (!tree.Value.Last.Contains(value))
                        tree.Value.Last.AddLast(value);
                }
                tree.Value.Nullable = true;
            }
            if (tree.Value.Simbol.Equals("+"))
            {
                for (int i = 0; i < tree.Left.Value.First.Count; ++i)
                {
                    int value = tree.Left.Value.First.ToArray<int>()[i];
                    if (!tree.Value.First.Contains(value))
                        tree.Value.First.AddLast(value);
                }
                for (int i = 0; i < tree.Left.Value.Last.Count; ++i)
                {
                    int value = tree.Left.Value.Last.ToArray<int>()[i];
                    if (!tree.Value.Last.Contains(value))
                        tree.Value.Last.AddLast(value);
                }
                tree.Value.Nullable = tree.Left.Value.Nullable;
            }
            if (!tree.Value.Simbol.Equals("?"))
                return;
            for (int i = 0; i < tree.Left.Value.First.Count; ++i)
            {
                int value = tree.Left.Value.First.ToArray<int>()[i];
                if (!tree.Value.First.Contains(value))
                    tree.Value.First.AddLast(value);
            }
            for (int i = 0; i < tree.Left.Value.Last.Count; ++i)
            {
                int value = tree.Left.Value.Last.ToArray<int>()[i];
                if (!tree.Value.Last.Contains(value))
                    tree.Value.Last.AddLast(value);
            }
            tree.Value.Nullable = true;
        }

    }
}
