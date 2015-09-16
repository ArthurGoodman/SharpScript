using SharpScript.FiniteAutomata;
using System;
using System.Collections.Generic;

namespace SharpScript {
    public static class Program {
        static void Main(string[] args) {
            //Script.RunFile(args.Length > 0 ? args[0] : "../../script.sh");

            State.Initialize();

            State q0 = new State();
            q0.Start = true;
            State q1 = new State();
            State q2 = new State();
            q2.Final = true;
            State q3 = new State();
            State q4 = new State();
            q4.Final = true;

            q0.Connect(null, q1, null);
            q0.Connect(null, q3, null);
            q1.Connect('a', q2, 0);
            q3.Connect('b', q4, 1);

            StateMachine sm = new StateMachine(State.States);
            Console.WriteLine(sm);

            StateMachine reversed = sm.Reverse();
            Console.WriteLine(reversed);
        }
    }
}
