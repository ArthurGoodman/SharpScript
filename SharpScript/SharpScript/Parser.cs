using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SharpScript {
    public class Parser : IParser {
        private List<Token> tokens;
        private List<Token>.Enumerator token;

        private Dictionary<string, Stack<ParameterExpression>> variables = new Dictionary<string, Stack<ParameterExpression>>();
        private Stack<HashSet<ParameterExpression>> scopes = new Stack<HashSet<ParameterExpression>>();

        private Stack<LabelTarget> breakLabels = new Stack<LabelTarget>();
        private Stack<LabelTarget> continueLabels = new Stack<LabelTarget>();

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

            return e.Type == typeof(void) ? (Expression)Expression.Block(e, (Expression)Expression.Constant(null)) : Expression.Convert(e, typeof(object));
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
                if (Accept("||")) {
                    Expression right = LogicAnd();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Or(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression LogicAnd() {
            Expression e = Equality();

            while (true) {
                if (Accept("&&")) {
                    Expression right = Equality();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.And(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression Equality() {
            Expression e = Relation();

            while (true) {
                if (Accept("==")) {
                    Expression right = Relation();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Equal(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept("!=")) {
                    Expression right = Relation();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.NotEqual(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression Relation() {
            Expression e = AddSub();

            while (true) {
                if (Accept("<")) {
                    Expression right = AddSub();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.LessThan(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept(">")) {
                    Expression right = AddSub();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.GreaterThan(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept("<=")) {
                    Expression right = AddSub();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.LessThanOrEqual(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept(">=")) {
                    Expression right = AddSub();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.GreaterThanOrEqual(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression AddSub() {
            Expression e = MulDiv();

            while (true) {
                if (Accept("+")) {
                    Expression right = MulDiv();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Add(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept("-")) {
                    Expression right = MulDiv();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Subtract(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression MulDiv() {
            Expression e = Power();

            while (true) {
                if (Accept("*")) {
                    Expression right = Power();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Multiply(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept("/")) {
                    Expression right = Power();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Divide(Expression.Convert(e, type), Expression.Convert(right, type));
                } else if (Accept("%")) {
                    Expression right = Power();
                    Type type = Coerce(e.Type, right.Type);
                    e = Expression.Modulo(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
                    break;
            }

            return e;
        }

        private Expression Power() {
            Expression e = Preffix();

            while (true) {
                if (Accept("**")) {
                    Expression right = Preffix();
                    Type type = typeof(double);
                    e = Expression.Power(Expression.Convert(e, type), Expression.Convert(right, type));
                } else
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
                e = Expression.PostIncrementAssign(e);
            else if (Accept("--"))
                e = Expression.PostDecrementAssign(e);

            return e;
        }

        private Expression Term() {
            Expression e = null;

            if (Accept("print"))
                e = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(object) }), Expression.Convert(Expr(), typeof(object)));
            else if (Check(Token.ID.Identifier)) {
                string name = token.Current.Text;
                GetToken();

                e = variables.ContainsKey(name) ? variables[name].Peek() : null;

                if (Accept("=")) {
                    Expression value = Expr();

                    e = Expression.Parameter(value.Type, name);

                    if (!variables.ContainsKey(name)) {
                        variables.Add(name, new Stack<ParameterExpression>());
                        variables[name].Push((ParameterExpression)e);

                        scopes.Peek().Add((ParameterExpression)e);
                    }

                    e = Expression.Assign(e, value);
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
            else if (Accept("if")) {
                Expression condition = Oper();
                Expression body = Expr();

                if (Accept("else"))
                    e = Expression.IfThenElse(condition, body, Expr());
                else
                    e = Expression.IfThen(condition, body);
            } else if (Accept("while")) {
                breakLabels.Push(Expression.Label());
                continueLabels.Push(Expression.Label());

                Expression condition = Oper();
                Expression body = Expr();

                e = Expression.Loop(Expression.IfThenElse(condition, body, Expression.Break(breakLabels.Peek())), breakLabels.Peek(), continueLabels.Peek());

                breakLabels.Pop();
                continueLabels.Pop();
            } else if (Accept("break"))
                e = Expression.Break(breakLabels.Peek());
            else if (Accept("continue"))
                e = Expression.Continue(continueLabels.Peek());
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

        private Type Coerce(Type a, Type b) {
            Type[] types = new Type[] { typeof(char), typeof(int), typeof(double) };
            return types[Math.Max(Array.IndexOf(types, a), Array.IndexOf(types, b))];
        }
    }
}
