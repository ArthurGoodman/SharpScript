using System.IO;

namespace SharpScript {
    public class Engine : IEngine {
        private ILexer lexer;

        public Engine() {
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
