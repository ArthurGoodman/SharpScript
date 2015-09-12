using System;

namespace SharpScript {
    public static class Script {
        public static void RunFile(string source) {
            try {
                Engine.Instance.RunFile(source);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
