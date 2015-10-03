namespace SharpScript {
    public class EmptyNode : Node {
        public override object Eval(Context context) {
            return null;
        }
    }
}
