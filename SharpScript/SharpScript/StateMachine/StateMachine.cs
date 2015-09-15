using System.Collections.Generic;

namespace SharpScript.StateMachine {
    public class StateMachine : IStateMachine {
        public HashSet<State> Q { get; set; }

        public StateMachine() {
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
            return null;
        }
    }
}
