using System.Collections.Generic;

namespace SharpScript {
    public interface IParser {
        INode Parse(List<Token> tokens);
    }
}
