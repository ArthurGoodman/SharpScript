using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class StateMachine : IStateMachine {
        public HashSet<State> States { get; set; }

        public StateMachine(HashSet<State> q) {
            States = q;
        }

        private void ResetFlags() {
            foreach (State state in States)
                state.Visited = false;
        }

        private void ComputeEpsilonClosures() {
            foreach (State state in States) {
                ResetFlags();
                state.ComputeEpsilonClosure(state.EpsilonClosure = new HashSet<State>());
            }
        }

        public StateMachine Reverse() {
            Dictionary<State, State> stateMap = new Dictionary<State, State>();

            State.Initialize();

            foreach (State state in States) {
                State copy = new State();

                copy.Start = state.Final;
                copy.Final = state.Start;

                stateMap.Add(state, copy);
            }

            foreach (State state in States)
                foreach (Transition transition in state.Transitions)
                    stateMap[transition.State].Connect(transition.Symbol, stateMap[state], transition.Label);

            return new StateMachine(State.States);
        }

        public StateMachine Determinize() {
            ComputeEpsilonClosures();

            MultiState.Initialize();
            State.Initialize();

            LinkedList<MultiState> multiStates = new LinkedList<MultiState>();
            HashSet<State> states = new HashSet<State>();

            HashSet<State> startStates = new HashSet<State>();

            foreach (State state in States)
                if (state.Start)
                    startStates.Add(state);

            MultiState start = MultiState.Create(startStates).GetEpsilonClosure();
            start.State.Start = true;
            multiStates.AddLast(start);

            while (multiStates.Count != 0) {
                MultiState multiState = multiStates.First.Value;
                multiStates.RemoveFirst();

                if (states.Contains(multiState.State))
                    continue;

                states.Add(multiState.State);

                foreach (char symbol in multiState.Alphabet) {
                    int? label = null;

                    MultiState newState = multiState.Transit(symbol, ref label).GetEpsilonClosure();

                    if (newState.Empty)
                        continue;

                    multiStates.AddLast(newState);

                    multiState.State.Connect(symbol, newState.State, label);
                }
            }

            return new StateMachine(states);
        }

        public StateMachine Minimize() {
            return Reverse().Determinize().Reverse().Determinize();
        }

        public void Reindex() {
            int c = 0;

            foreach (State state in States)
                state.Id = c++;
        }

        public StateMachineMatch Match(string str) {
            return new StateMachineMatch();
        }

        public override string ToString() {
            string str = "";

            foreach (State state in States)
                str += state.Inspect();

            return str;
        }
    }
}
