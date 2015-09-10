using System;

namespace SharpScript {
    public class Script {
        private static IEngine engine = new Engine();

        public static void RunFile(string source) {
            try {
                engine.RunFile(source);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
