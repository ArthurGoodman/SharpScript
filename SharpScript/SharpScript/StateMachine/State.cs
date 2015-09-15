using System.Collections.Generic;

namespace SharpScript.StateMachine {
    public class State {
        public Transition[] Transitions { get; set; }
        public ISet<State> EpsilonCosure { get; set; }
        public bool Visited { get; set; }
        public bool Start { get; set; }
        public bool Final { get; set; }
    }
}
