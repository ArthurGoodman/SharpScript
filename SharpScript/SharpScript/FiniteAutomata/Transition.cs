namespace SharpScript.FiniteAutomata {
    public class Transition {
        public char? Symbol { get; set; }
        public State State { get; set; }
        public int? Label { get; set; }

        public Transition(char? symbol, State state, int? label) {
            Symbol = symbol;
            State = state;
            Label = label;
        }

        public string Inspect() {
            string arrow = "--" + (Symbol == null ? "-" : Symbol.ToString()) + "-->";
            return arrow + " " + State + (Label == null ? "" : " (" + Label + ")");
        }
    }
}
