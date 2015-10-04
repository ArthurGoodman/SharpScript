using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace SharpScript {
    public class Engine : IEngine {
        public static IEngine Instance = new Engine();

        private ILexer lexer;
        private IParser parser;

        private Stack<Source> sources = new Stack<Source>();
        private Stack<string> fileNames = new Stack<string>();

        private string FileName {
            get {
                return fileNames.Peek();
            }
        }

        private Source Source {
            get {
                return sources.Peek();
            }
        }

        public Engine() {
            Instance = this;

            lexer = new Lexer();
            parser = new Parser();
        }

        public void Run(string source) {
            source = ExpandTabs(source);

            sources.Push(new Source(source));

            try {
                Console.WriteLine(Expression.Lambda(parser.Parse(lexer.Lex(source))).Compile().DynamicInvoke());
            } catch (ErrorException e) {
                Console.WriteLine(FileName + ":" + (e.Position.Valid ? e.Position + ": " : " ") + e.Message);
                if (e.Position.Valid)
                    Console.WriteLine(Source.Quote(e.Position));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            sources.Pop();
        }

        public void RunFile(string fileName) {
            fileNames.Push(Path.GetFullPath(fileName));
            Run(File.ReadAllText(fileName));
            fileNames.Pop();
        }

        private static string ExpandTabs(string str) {
            const int tabLength = 4;

            string[] text = str.Split('\t');
            string result = "";

            foreach (string s in text)
                result += s + new string(' ', tabLength - s.Length % tabLength);

            return result;
        }
    }
}
