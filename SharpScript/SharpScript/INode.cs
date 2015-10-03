namespace SharpScript {
    public interface INode {
        Position Position { get; set; }
        object Eval(Context context);
    }
}
