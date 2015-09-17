using SharpScript.FiniteAutomata;
using System;
using System.Collections.Generic;

namespace SharpScript {
    public static class Program {
        static void Main(string[] args) {
            //Script.RunFile(args.Length > 0 ? args[0] : "../../script.sh");

            State.Initialize();

            State q0 = new State();
            State q1 = new State();
            State q2 = new State();
            State q3 = new State();

            q0.Start = true;
            q3.Final = true;

            q0.Connect('a', q1);
            q0.Connect('a', q2);
            q0.Connect('b', q2);
            q1.Connect('a', q2);
            q1.Connect('b', q3);
            q2.Connect('a', q1);
            q2.Connect('a', q2);
            q2.Connect('b', q3);

            StateMachine sm = new StateMachine(State.States);
            Console.WriteLine(sm);

            StateMachine minimized = sm.Minimize();
            minimized.Reindex();
            Console.WriteLine(minimized);
        }
    }
}
