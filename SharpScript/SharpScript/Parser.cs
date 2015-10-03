using System.Collections.Generic;

namespace SharpScript {
    public class Parser : IParser {
        private List<Token> tokens;
        private List<Token>.Enumerator token;

        public INode Parse(List<Token> tokens) {
            this.tokens = tokens;
            token = tokens.GetEnumerator();

            GetToken();

            Position p = token.Current.Position;

            List<INode> nodes = new List<INode>();

            while (!Check(Token.ID.End))
                nodes.Add(Oper());

            INode node = new EmptyNode();
            node.Position = p;

            return node;
        }

        private INode Oper() {
            INode node = Expr();

            Accept(";");

            return node;
        }

        private INode Expr() {
            INode node = LogicOr();

            return node;
        }

        private INode LogicOr() {
            INode node = LogicAnd();

            return node;
        }

        private INode LogicAnd() {
            INode node = Equality();

            return node;
        }

        private INode Equality() {
            INode node = Relation();

            return node;
        }

        private INode Relation() {
            INode node = AddSub();

            return node;
        }

        private INode AddSub() {
            INode node = MulDiv();

            return node;
        }

        private INode MulDiv() {
            INode node = Power();

            return node;
        }

        private INode Power() {
            INode node = Preffix();

            return node;
        }

        private INode Preffix() {
            INode node = null;

            node = Suffix();

            return node;
        }

        private INode Suffix() {
            INode node = Term();

            return node;
        }

        private INode Term() {
            INode node = null;

            GetToken();
            node = new EmptyNode();

            return node;
        }

        private void Error(string message, int delta = 0) {
            throw new ErrorException(message, token.Current.Position.Shifted(delta));
        }

        private bool Check(Token.ID id) {
            return token.Current.Id == id;
        }

        private bool Check(string text) {
            return token.Current.Text == text;
        }

        private bool Accept(string text) {
            if (token.Current.Text == text) {
                GetToken();
                return true;
            }

            return false;
        }

        private void GetToken() {
            token.MoveNext();
        }

        private List<INode> ParseBlock() {
            List<INode> nodes = new List<INode>();

            while (!Check("}") && !Check(Token.ID.End))
                nodes.Add(Oper());

            if (!Accept("}"))
                Error("unmatched braces");

            return nodes;
        }

        private List<INode> ParseList() {
            List<INode> nodes = new List<INode>();

            do
                nodes.Add(LogicOr());
            while (Accept(","));

            return nodes;
        }
    }
}
