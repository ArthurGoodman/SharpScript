namespace SharpScript.StateMachine {
    public interface IStateMachine {
        IStateMachine Reverse();
        IStateMachine Determinize();
        IStateMachine Minimize();
    }
}
