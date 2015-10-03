namespace SharpScript {
    public abstract class Node : INode {
        public Position Position { get; set; }
        public abstract object Eval(Context context);
    }
}
