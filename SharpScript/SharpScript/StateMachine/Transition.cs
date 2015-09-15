namespace SharpScript.StateMachine {
    public class Transition {
        public char Symbol { get; set; }
        public State State { get; set; }
        public int Label { get; set; }
    }
}
