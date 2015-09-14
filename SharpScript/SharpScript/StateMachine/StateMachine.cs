namespace SharpScript.StateMachine {
    public class StateMachine : IStateMachine {
        public State[] Q { get; set; }

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
            throw new System.NotImplementedException();
        }
    }
}
