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
            "false"
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
            ";"
        };

        public List<Token> Lex(string source) {
            this.source = source;
            pos = 0;
            line = column = 1;

            List<Token> tokens = new List<Token>();

            do {
                Scan();
                Console.WriteLine(token.Inspect());
                tokens.Add(token);
            } while (token.Id != Token.TokenId.End);

            return tokens;
        }

        private void Scan() {
            SkipSpaces();

            token = new Token();

            token.Position = new Position(pos, line, column);

            if (At(pos) == '\0')
                token.Id = Token.TokenId.End;
            else if (char.IsDigit(At(pos))) {
                while (char.IsDigit(At(pos)))
                    token.Text += At(pos++);

                if (At(pos) == '.') {
                    do
                        token.Text += At(pos++);
                    while (char.IsDigit(At(pos)));

                    token.Id = Token.TokenId.Float;
                } else
                    token.Id = Token.TokenId.Integer;

                if (char.ToLower(At(pos)) == 'e') {
                    token.Text += At(pos++);

                    if (At(pos) == '+' || At(pos) == '-')
                        token.Text += At(pos++);

                    while (char.IsDigit(At(pos)))
                        token.Text += At(pos++);

                    token.Id = Token.TokenId.Float;
                }

                while ((char.IsLetter(At(pos)) || At(pos) == '_'))
                    token.Text += At(pos++);

                if (token.Id == Token.TokenId.Float) {
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
                    token.Id = Token.TokenId.Keyword;
                else
                    token.Id = Token.TokenId.Identifier;
            } else {
                token.Id = Token.TokenId.Unknown;
                token.Text += At(pos++);
            }

            column += token.Text.Length;
        }

        private void SkipSpaces() {
            while (char.IsWhiteSpace(At(pos)))
                pos++;
        }

        private char At(int pos) {
            return pos < source.Length ? source[pos] : '\0';
        }

        private void Error(string message, int delta = 0) {
            throw new LexicalErrorException(message, token.Position.Shifted(delta));
        }
    }
}
