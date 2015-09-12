using System;
using System.Collections.Generic;

namespace SharpScript {
    public class Lexer : ILexer {
        private string source;
        private int pos;

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

            List<Token> tokens = new List<Token>();

            Token token;

            do {
                token = Scan();
                Console.WriteLine(token.Inspect());
                tokens.Add(token);
            } while (token.Id != Token.TokenId.End);

            return tokens;
        }

        private Token Scan() {
            SkipSpaces();

            Token token = new Token();

            if (pos == source.Length)
                token.Id = Token.TokenId.End;
            else if (char.IsLetter(source[pos]) || source[pos] == '_') {
                while (pos < source.Length && (char.IsLetter(source[pos]) || source[pos] == '_'))
                    token.Text += source[pos++];

                if (Array.Exists(keywords, (string s) => s == token.Text))
                    token.Id = Token.TokenId.Keyword;
                else
                    token.Id = Token.TokenId.Identifier;
            } else {
                token.Id = Token.TokenId.Unknown;
                token.Text += source[pos++];
            }

            return token;
        }

        private void SkipSpaces() {
            while (pos < source.Length && char.IsWhiteSpace(source[pos]))
                pos++;
        }
    }
}
