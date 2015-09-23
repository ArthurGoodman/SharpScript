namespace FiniteAutomata {
    public class Transition {
        public char? Symbol { get; set; }
        public State State { get; set; }
        public int? Label { get; set; }

        public Transition(char? symbol, State state, int? label = null) {
            Symbol = symbol;
            State = state;
            Label = label;
        }

        public string Inspect() {
            return "--" + (Symbol == null ? "\x3b5" : Symbol.ToString()) + "-> " + State + (Label == null ? "" : " (" + Label + ")");
        }
    }
}
