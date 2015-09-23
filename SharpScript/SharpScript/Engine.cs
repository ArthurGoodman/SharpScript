using System;
using System.Collections.Generic;
using System.IO;

namespace SharpScript {
    public class Engine : IEngine {
        public static IEngine Instance = new Engine();

        private ILexer lexer;

        private Stack<string> sources = new Stack<string>();
        private Stack<string> fileNames = new Stack<string>();

        private string FileName {
            get {
                return fileNames.Peek();
            }
        }

        private string Source {
            get {
                return sources.Peek();
            }
        }

        public Engine() {
            Instance = this;

            lexer = new Lexer();
        }

        public void Run(string source) {
            sources.Push(source);

            try {
                lexer.Lex(source);
            } catch (ErrorException e) {
                Console.WriteLine(FileName + ":" + (e.Position.Valid ? e.Position + ": " : " ") + e.Message);
                if (e.Position.Valid) {
                    // quote
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            sources.Pop();
        }

        public void RunFile(string fileName) {
            fileNames.Push(fileName);
            Run(File.ReadAllText(fileName));
            fileNames.Pop();
        }
    }
}
