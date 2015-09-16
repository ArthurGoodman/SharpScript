using SharpScript.FiniteAutomata;
using System;
using System.Collections.Generic;

namespace SharpScript {
    public static class Program {
        static void Main(string[] args) {
            //Script.RunFile(args.Length > 0 ? args[0] : "../../script.sh");

            State.Initialize();

            State s = new State();
            s.Start = true;
            State f = new State();
            f.Final = true;

            State _a1 = new State();
            State a1 = new State();
            State ab = new State();
            State abc = new State();

            s.Connect(null, _a1);
            
            _a1.Connect('a', a1, 0);
            a1.Connect('b', ab, 0);
            ab.Connect('c', abc, 0);
            
            abc.Connect(null, f);

            State _a2 = new State();
            State a2 = new State();
            State ad = new State();
            State adc = new State();

            s.Connect(null, _a2);

            _a2.Connect('a', a2, 1);
            a2.Connect('d', ad, 1);
            ad.Connect('c', adc, 1);

            adc.Connect(null, f);

            StateMachine sm = new StateMachine(State.States);
            Console.WriteLine(sm);

            StateMachine minimized = sm.Minimize();
            minimized.Reindex();
            Console.WriteLine(minimized);
        }
    }
}
