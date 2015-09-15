using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class StateMachine : IStateMachine {
        public ISet<State> Q { get; set; }

        public StateMachine(ISet<State> q) {
            Q = q;
        }

        public StateMachine Reverse() {
            return this;
        }

        public StateMachine Determinize() {
            return this;
        }

        public StateMachine Minimize() {
            return this;
        }

        public StateMachineMatch Match(string str) {
            return new StateMachineMatch();
        }
    }
}
