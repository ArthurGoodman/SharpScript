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
            lexer.Lex(source);
        }

        public void RunFile(string fileName) {
            Run(File.ReadAllText(fileName));
        }
    }
}
