using System;
using System.IO;

namespace SharpScript {
    public class Engine : IEngine {
        public static IEngine Instance = new Engine();

        private ILexer lexer;

        public Engine() {
            Instance = this;

            lexer = new Lexer();
        }

        public void Run(string source) {
            try {
                lexer.Lex(source);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void RunFile(string fileName) {
            Run(File.ReadAllText(fileName));
        }
    }
}
