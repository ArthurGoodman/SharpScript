namespace SharpScript {
    public class SyntaxErrorException : ErrorException {
        public SyntaxErrorException(string message, Position position)
            : base(message, position) {
        }

        public override string Message {
            get {
                return "syntax error: " + base.Message;
            }
        }
    }
}
