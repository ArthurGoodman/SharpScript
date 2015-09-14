namespace SharpScript.StateMachine {
    public class State {
        public Transition[] Transitions { get; set; }
        public int[] EpsilonCosure { get; set; }
        public bool Visited { get; set; }
        public bool Start { get; set; }
        public bool Final { get; set; }
    }
}
