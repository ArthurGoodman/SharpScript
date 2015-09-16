using System;
using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class State {
        public List<Transition> Transitions { get; private set; }

        public HashSet<State> EpsilonClosure { get; set; }

        public int Id { get; set; }

        public bool Visited { get; set; }

        public bool Start { get; set; }
        public bool Final { get; set; }

        public HashSet<char> Alphabet {
            get {
                HashSet<char> alphabet = new HashSet<char>();

                foreach (Transition transition in Transitions)
                    if (transition.Symbol != null)
                        alphabet.Add((char)transition.Symbol);

                return alphabet;
            }
        }

        private static int idCounter;

        public static HashSet<State> States { get; private set; }

        public static void Initialize() {
            idCounter = 0;
            States = new HashSet<State>();
        }

        public State() {
            Id = idCounter++;
            States.Add(this);

            Transitions = new List<Transition>();

            Visited = false;
            Start = false;
            Final = false;
        }

        public void ComputeEpsilonClosure(HashSet<State> closure) {
            closure.Add(this);

            if (Visited)
                return;

            Visited = true;

            foreach (Transition transition in Transitions)
                if (transition.Symbol == null)
                    transition.State.ComputeEpsilonClosure(closure);
        }

        public void Connect(char? symbol, State state, int? label = null) {
            Transitions.Add(new Transition(symbol, state, label));
        }

        public HashSet<State> Transit(char? symbol, ref int? label) {
            HashSet<State> result = new HashSet<State>();

            HashSet<int> labels = new HashSet<int>();

            foreach (Transition transition in Transitions)
                if (transition.Symbol == symbol) {
                    if (transition.Label != null)
                        labels.Add((int)transition.Label);

                    result.Add(transition.State);
                }

            if (labels.Count == 1) {
                HashSet<int>.Enumerator en = labels.GetEnumerator();
                en.MoveNext();
                label = en.Current;
            } else
                label = null;

            return result;
        }

        public string Inspect() {
            string str = "";

            foreach (Transition transition in Transitions)
                str += this + " " + transition.Inspect() + "\n";

            return str;
        }

        public override string ToString() {
            return (Start ? "*" : " ") + Id + (Final ? "*" : " ");
        }
    }
}
