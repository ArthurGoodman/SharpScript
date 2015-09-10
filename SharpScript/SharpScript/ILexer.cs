using System.Collections.Generic;

namespace SharpScript {
    public interface ILexer {
        List<Token> Lex(string source);
    }
}
