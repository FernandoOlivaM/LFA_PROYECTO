namespace PROYECTO_LFA_1251518
{
    public class BynaryTree<T> : IBinaryTree<T>
    {
        private IBinaryTree<T> right = (IBinaryTree<T>)null;
        private IBinaryTree<T> left = (IBinaryTree<T>)null;
        private IBinaryTree<T> parent = (IBinaryTree<T>)null;
        private T value;

        public BynaryTree(T value) : this(value, (IBinaryTree<T>)null, (IBinaryTree<T>)null)
        {
        }

        public BynaryTree(T value, IBinaryTree<T> left, IBinaryTree<T> right)
        {
            this.Value = value;
            this.Left = left;
            this.Right = right;
            this.Parent = (IBinaryTree<T>)null;
        }

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public IBinaryTree<T> Left
        {
            get
            {
                return this.left;
            }
            set
            {
                this.left = value;
            }
        }

        public IBinaryTree<T> Right
        {
            get
            {
                return this.right;
            }
            set
            {
                this.right = value;
            }
        }

        public IBinaryTree<T> Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }


        public void inOrder(TraversalTree<T> node)
        {
            if (this.Left != null)
                this.Left.inOrder(node);
            node((IBinaryTree<T>)this);
            if (this.Right == null)
                return;
            this.Right.inOrder(node);
        }
        public void postOrder(TraversalTree<T> node)
        {
            if (this.Left != null)
                this.Left.postOrder(node);
            if (this.Right != null)
                this.Right.postOrder(node);
            node((IBinaryTree<T>)this);
        }

    }

}
