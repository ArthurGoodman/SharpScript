using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class StateMachine : IStateMachine {
        public HashSet<State> Q { get; set; }

        public StateMachine(HashSet<State> q) {
            Q = q;
        }

        public StateMachine Reverse() {
            Dictionary<State, State> stateMap = new Dictionary<State, State>();

            State.Initialize();

            foreach (State state in Q) {
                State copy = new State();

                copy.Start = state.Final;
                copy.Final = state.Start;

                stateMap.Add(state, copy);
            }

            foreach (State state in Q)
                foreach (Transition transition in state.Transitions)
                    stateMap[transition.State].Transitions.Add(new Transition(transition.Symbol, stateMap[state], transition.Label));

            return new StateMachine(State.States);
        }

        public StateMachine Determinize() {
            return this;
        }

        public StateMachine Minimize() {
            return Reverse().Determinize().Reverse().Determinize();
        }

        public StateMachineMatch Match(string str) {
            return new StateMachineMatch();
        }

        public override string ToString() {
            string str = "";

            foreach (State state in Q)
                str += state.Inspect();

            return str;
        }
    }
}
