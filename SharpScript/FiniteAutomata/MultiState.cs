using System.Collections.Generic;

namespace FiniteAutomata {
    public class MultiState {
        public HashSet<State> States { get; set; }
        public State State { get; set; }

        public bool Empty {
            get {
                return States.Count == 0;
            }
        }

        public HashSet<char> Alphabet {
            get {
                HashSet<char> alphabet = new HashSet<char>();

                foreach (State state in States)
                    alphabet.UnionWith(state.Alphabet);

                return alphabet;
            }
        }

        private static HashSet<MultiState> hash = new HashSet<MultiState>();

        public static void Initialize() {
            hash.Clear();
        }

        public static MultiState Create(HashSet<State> states) {
            MultiState newState = new MultiState(states);

            if (hash.Contains(newState))
                foreach (MultiState state in hash)
                    if (state.GetHashCode() == newState.GetHashCode() || state.Equals(newState))
                        return state;

            hash.Add(newState);

            return newState;
        }

        private MultiState(HashSet<State> states) {
            States = states;
            State = new State();

            foreach (State state in States)
                if (state.Final)
                    State.Final = true;
        }

        public MultiState Transit(char? symbol, ref int? label) {
            HashSet<State> result = new HashSet<State>();

            HashSet<int> labels = new HashSet<int>();

            foreach (State state in States) {
                int? l = null;

                result.UnionWith(state.Transit(symbol, ref l));

                if (l != null)
                    labels.Add((int)l);
            }

            if (labels.Count == 1) {
                HashSet<int>.Enumerator en = labels.GetEnumerator();
                en.MoveNext();
                label = en.Current;
            } else
                label = null;

            return Create(result);
        }

        public MultiState GetEpsilonClosure() {
            HashSet<State> closure = new HashSet<State>();

            foreach (State state in States)
                closure.UnionWith(state.EpsilonClosure);

            return Create(closure);
        }

        public override int GetHashCode() {
            int hash = 13;

            foreach (State state in States)
                hash = 7 * hash + state.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj) {
            if (!(obj is MultiState))
                return false;

            if (States.Count != ((MultiState)obj).States.Count)
                return false;

            foreach (State state in States)
                if (!((MultiState)obj).States.Contains(state))
                    return false;

            return true;
        }
    }
}
