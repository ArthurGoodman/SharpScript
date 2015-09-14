namespace SharpScript.StateMachine {
    public struct Transition {
        public char Symbol { get; set; }
        public int State { get; set; }
        public int Label { get; set; }
    }
}
