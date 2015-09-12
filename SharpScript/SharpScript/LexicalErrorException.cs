namespace SharpScript {
    public class LexicalErrorException : ErrorException {
        public LexicalErrorException(string message, Position position)
            : base(message, position) {
        }

        public override string Message {
            get {
                return "lexical error: " + base.Message;
            }
        }
    }
}
