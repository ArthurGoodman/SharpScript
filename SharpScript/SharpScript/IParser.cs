using System.Collections.Generic;
using System.Linq.Expressions;

namespace SharpScript {
    public interface IParser {
        Expression Parse(List<Token> tokens);
    }
}
