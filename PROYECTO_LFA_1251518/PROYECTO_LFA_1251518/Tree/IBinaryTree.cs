namespace PROYECTO_LFA_1251518
{
    public interface IBinaryTree<T>
    {
        T Value { get; set; }
        IBinaryTree<T> Left { get; set; }
        IBinaryTree<T> Right { get; set; }
        IBinaryTree<T> Parent { get; set; }
        void postOrder(TraversalTree<T> node);
    }

}