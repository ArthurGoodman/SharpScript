namespace SharpScript.StateMachine {
    public class StateMachine : IStateMachine {
        public int[] S { get; set; }
        public int[] F { get; set; }
        public Transition[][] Q { get; set; }

        public StateMachine() {
        }

        public IStateMachine Reverse() {
            return this;
        }

        public IStateMachine Determinize() {
            return this;
        }

        public IStateMachine Minimize() {
            return this;
        }
    }
}
