using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class State {
        public ISet<Transition> Transitions { get; private set; }
        public ISet<State> EpsilonClosure { get; private set; }

        public bool Visited { get; set; }
        public bool Start { get; set; }
        public bool Final { get; set; }

        public State() {
            Transitions = new HashSet<Transition>();

            Visited = false;
            Start = false;
            Final = false;
        }

        public void Connect(char symbol, State state, int label) {
            Transitions.Add(new Transition(symbol, state, label));
        }
    }
}
