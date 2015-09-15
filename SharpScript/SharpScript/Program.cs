using SharpScript.FiniteAutomata;
using System;
using System.Collections.Generic;

namespace SharpScript {
    public static class Program {
        static void Main(string[] args) {
            //Script.RunFile(args.Length > 0 ? args[0] : "../../script.sh");

            State q0 = new State();
            q0.Start = true;
            State q1 = new State();
            State q2 = new State();
            State q3 = new State();
            q3.Final = true;

            q0.Connect('a', q1, 0);
            q1.Connect('b', q2, 0);
            q2.Connect('c', q3, 0);

            ISet<State> q = new HashSet<State>();

            q.Add(q0);
            q.Add(q1);
            q.Add(q2);
            q.Add(q3);

            IStateMachine sm = new StateMachine(q);

            StateMachineMatch match = sm.Match("abcdef");

            Console.WriteLine(match.Inspect());
        }
    }
}
