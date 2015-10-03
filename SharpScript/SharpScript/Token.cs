namespace SharpScript {
    public class Token {
        public enum ID {
            None = -1,

            Integer,
            Float,
            String,
            Character,
            Keyword,
            Operator,
            Identifier,
            Symbol,
            End,
            Unknown
        }

        public string Text { get; set; }
        public ID Id { get; set; }
        public Position Position { get; set; }

        public Token() {
            Text = "";
            Id = ID.None;
        }
    }
}
