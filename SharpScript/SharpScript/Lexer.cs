using System;
using System.Collections.Generic;

namespace SharpScript {
    public class Lexer : ILexer {
        private string source;
        private int pos, line, column;
        private Token token;

        private string[] keywords = new[] {
            "if",
            "else",
            "while",
            "for",
            "do",
            "true",
            "false",
            "null"
        };

        private string[] operators = new[] {
            "+",
            "-",
            "*",
            "/",
            "**",
            "&&",
            "||",
            "<",
            ">",
            "<=",
            ">=",
            "==",
            "!=",
            "!",
            "=",
            "(",
            ")",
            "{",
            "}",
            ";",
            "++",
            "--"
        };

        public List<Token> Lex(string source) {
            this.source = source;
            pos = 0;
            line = column = 1;

            List<Token> tokens = new List<Token>();

            do {
                Scan();
                //Console.WriteLine(token.Inspect());
                tokens.Add(token);
            } while (token.Id != Token.ID.End);

            return tokens;
        }

        private void Scan() {
            SkipSpaces();

            while (At(pos) == '/' && At(pos + 1) == '/') {
                while (At(pos) != '\0' && At(pos) != '\n')
                    pos++;

                SkipSpaces();
            }

            token = new Token();

            token.Position = new Position(pos, line, column);

            if (At(pos) == '\0')
                token.Id = Token.ID.End;
            else if (char.IsDigit(At(pos))) {
                while (char.IsDigit(At(pos)))
                    token.Text += At(pos++);

                if (At(pos) == '.') {
                    do
                        token.Text += At(pos++);
                    while (char.IsDigit(At(pos)));

                    token.Id = Token.ID.Float;
                } else
                    token.Id = Token.ID.Integer;

                if (char.ToLower(At(pos)) == 'e') {
                    token.Text += At(pos++);

                    if (At(pos) == '+' || At(pos) == '-')
                        token.Text += At(pos++);

                    while (char.IsDigit(At(pos)))
                        token.Text += At(pos++);

                    token.Id = Token.ID.Float;
                }

                while ((char.IsLetter(At(pos)) || At(pos) == '_'))
                    token.Text += At(pos++);

                if (token.Id == Token.ID.Float) {
                    double result;
                    if (!double.TryParse(token.Text, out result))
                        Error("invalid float constant");
                } else {
                    int result;
                    if (!int.TryParse(token.Text, out result))
                        Error("invalid integer constant");
                }
            } else if (char.IsLetter(At(pos)) || At(pos) == '_') {
                while ((char.IsLetter(At(pos)) || At(pos) == '_'))
                    token.Text += At(pos++);

                if (Array.Exists(keywords, (string s) => s == token.Text))
                    token.Id = Token.ID.Keyword;
                else
                    token.Id = Token.ID.Identifier;
            } else {
                int i = 0;

                while (i != operators.Length)
                    for (i = 0; i != operators.Length; i++)
                        if (operators[i].SubstringWrapper(0, token.Text.Length + 1) == token.Text + At(pos)) {
                            token.Text += At(pos++);
                            break;
                        }

                if (token.Text != "" && (i = Array.IndexOf(operators, token.Text)) != -1)
                    token.Id = Token.ID.Keyword;
                else {
                    if (token.Text == "")
                        token.Text += At(pos++);

                    token.Id = Token.ID.Unknown;
                }
            }

            column += token.Text.Length;
        }

        private void SkipSpaces() {
            while (char.IsWhiteSpace(At(pos))) {
                pos++;
                column++;

                if (At(pos - 1) == '\n') {
                    line++;
                    column = 1;
                }
            }
        }

        private char At(int pos) {
            return pos < source.Length ? source[pos] : '\0';
        }

        private void Error(string message, int delta = 0) {
            throw new LexicalErrorException(message, token.Position.Shifted(delta));
        }
    }
}
