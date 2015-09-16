using System.Collections.Generic;

namespace SharpScript.FiniteAutomata {
    public class State {
        public List<Transition> Transitions { get; private set; }

        public HashSet<State> EpsilonClosure { get; set; }

        public int Id { get; set; }

        public bool Visited { get; set; }

        public bool Start { get; set; }
        public bool Final { get; set; }

        private static int idCounter = 0;
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

        public void Connect(char? symbol, State state, int? label) {
            Transitions.Add(new Transition(symbol, state, label));
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
