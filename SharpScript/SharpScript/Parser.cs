using System.Collections.Generic;
using System.Linq.Expressions;

namespace SharpScript {
    public class Parser : IParser {
        private List<Token> tokens;
        private List<Token>.Enumerator token;

        private Dictionary<string, Stack<ParameterExpression>> variables = new Dictionary<string, Stack<ParameterExpression>>();
        private Stack<HashSet<ParameterExpression>> scopes = new Stack<HashSet<ParameterExpression>>();

        public Expression Parse(List<Token> tokens) {
            this.tokens = tokens;
            token = tokens.GetEnumerator();

            GetToken();

            scopes.Push(new HashSet<ParameterExpression>());

            List<Expression> nodes = new List<Expression>();

            while (!Check(Token.ID.End))
                nodes.Add(Oper());

            Expression e = nodes.Count == 0 ? Expression.Constant(null) : (Expression)Expression.Block(scopes.Peek(), nodes);

            scopes.Pop();

            return Expression.Convert(e, typeof(object));
        }

        private Expression Oper() {
            Expression e = Expr();

            Accept(";");

            return e;
        }

        private Expression Expr() {
            Expression e = LogicOr();

            return e;
        }

        private Expression LogicOr() {
            Expression e = LogicAnd();

            while (true) {
                if (Accept("||"))
                    e = Expression.Or(e, LogicAnd());
                else
                    break;
            }

            return e;
        }

        private Expression LogicAnd() {
            Expression e = Equality();

            while (true) {
                if (Accept("&&"))
                    e = Expression.And(e, Equality());
                else
                    break;
            }

            return e;
        }

        private Expression Equality() {
            Expression e = Relation();

            while (true) {
                if (Accept("=="))
                    e = Expression.Equal(e, Relation());
                else if (Accept("!="))
                    e = Expression.NotEqual(e, Relation());
                break;
            }

            return e;
        }

        private Expression Relation() {
            Expression e = AddSub();

            while (true) {
                if (Accept("<"))
                    e = Expression.LessThan(e, AddSub());
                else if (Accept(">"))
                    e = Expression.GreaterThan(e, AddSub());
                else if (Accept("<="))
                    e = Expression.LessThanOrEqual(e, AddSub());
                else if (Accept(">="))
                    e = Expression.GreaterThanOrEqual(e, AddSub());
                break;
            }

            return e;
        }

        private Expression AddSub() {
            Expression e = MulDiv();

            while (true) {
                if (Accept("+"))
                    e = Expression.Add(e, Expression.Convert(MulDiv(), e.Type));
                else if (Accept("-"))
                    e = Expression.Subtract(e, Expression.Convert(MulDiv(), e.Type));
                break;
            }

            return e;
        }

        private Expression MulDiv() {
            Expression e = Power();

            while (true) {
                if (Accept("*"))
                    e = Expression.Multiply(e, Expression.Convert(Power(), e.Type));
                else if (Accept("/"))
                    e = Expression.Divide(e, Expression.Convert(Power(), e.Type));
                break;
            }

            return e;
        }

        private Expression Power() {
            Expression e = Preffix();

            while (true) {
                if (Accept("**"))
                    e = Expression.Power(e, Expression.Convert(Preffix(), e.Type));
                break;
            }

            return e;
        }

        private Expression Preffix() {
            Expression e = null;

            if (Accept("+"))
                e = Expression.UnaryPlus(Suffix());
            else if (Accept("-"))
                e = Expression.Negate(Suffix());
            else if (Accept("!"))
                e = Expression.Not(Suffix());
            else if (Accept("++"))
                e = Expression.PreIncrementAssign(Suffix());
            else if (Accept("--"))
                e = Expression.PreDecrementAssign(Suffix());
            else
                e = Suffix();

            return e;
        }

        private Expression Suffix() {
            Expression e = Term();

            if (Accept("++"))
                e = Expression.PostDecrementAssign(e);
            else if (Accept("--"))
                e = Expression.PostIncrementAssign(e);

            return e;
        }

        private Expression Term() {
            Expression e = null;

            if (Check(Token.ID.Identifier)) {
                string name = token.Current.Text;
                GetToken();

                e = variables.ContainsKey(name) ? variables[name].Peek() : Expression.Parameter(typeof(object), name);

                if (Accept("=")) {
                    if (!variables.ContainsKey(name)) {
                        variables.Add(name, new Stack<ParameterExpression>());
                        variables[name].Push((ParameterExpression)e);

                        scopes.Peek().Add((ParameterExpression)e);
                    }

                    e = Expression.Assign(e, Expression.Convert(Expr(), typeof(object)));
                }
            } else if (Check(Token.ID.Integer)) {
                e = Expression.Constant(int.Parse(token.Current.Text));
                GetToken();
            } else if (Check(Token.ID.Float)) {
                e = Expression.Constant(double.Parse(token.Current.Text));
                GetToken();
            } else if (Check(Token.ID.String)) {
                e = Expression.Constant(token.Current.Text);
                GetToken();
            } else if (Check(Token.ID.Character)) {
                e = Expression.Constant(token.Current.Text[0]);
                GetToken();
            } else if (Accept("true"))
                e = Expression.Constant(true);
            else if (Accept("false"))
                e = Expression.Constant(false);
            else if (Accept("null"))
                e = Expression.Constant(null);
            else if (Accept("(")) {
                e = Expr();

                if (!Accept(")"))
                    Error("unmatched parentheses");
            } else if (Accept("{")) {
                scopes.Push(new HashSet<ParameterExpression>());

                List<Expression> nodes = ParseBlock();
                e = nodes.Count == 0 ? Expression.Constant(null) : (Expression)Expression.Block(scopes.Peek(), nodes);

                foreach (ParameterExpression pe in scopes.Peek()) {
                    variables[pe.Name].Pop();

                    if (variables[pe.Name].Count == 0)
                        variables.Remove(pe.Name);
                }

                scopes.Pop();
            } else if (Accept(";"))
                e = Expression.Constant(null);
            else if (Check(Token.ID.End))
                Error("unexpected end of program");
            else if (Check(Token.ID.Unknown))
                Error("unknown token '" + token.Current.Text + "'");
            else
                Error("unexpected token '" + token.Current.Text + "'");

            return e;
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

        private List<Expression> ParseBlock() {
            List<Expression> nodes = new List<Expression>();

            while (!Check("}") && !Check(Token.ID.End))
                nodes.Add(Oper());

            if (!Accept("}"))
                Error("unmatched braces");

            return nodes;
        }

        private List<Expression> ParseList() {
            List<Expression> nodes = new List<Expression>();

            do
                nodes.Add(LogicOr());
            while (Accept(","));

            return nodes;
        }
    }
}
