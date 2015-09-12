namespace SharpScript {
    public class Token {
        public enum TokenId {
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
        public TokenId Id { get; set; }

        public Token() {
            Text = "";
            Id = TokenId.None;
        }
    }
}
