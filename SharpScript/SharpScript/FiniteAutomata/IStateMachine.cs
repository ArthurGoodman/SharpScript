namespace SharpScript.FiniteAutomata {
    public interface IStateMachine {
        StateMachineMatch Match(string str);
    }
}
