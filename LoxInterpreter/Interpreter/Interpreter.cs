using System;

namespace LoxInterpreter
{
    class Interpreter : IExprVisitor<object>
    {
        public void Interpret(Expr expr)
        {
            try
            {
                var value = Evaluate(expr);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError e)
            {
                Lox.RuntimeError(e);
            }
        }

        public object VisitBinaryExpr(BinaryExpr expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);

                case TokenType.Minus:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.Plus:
                    if (left is double && right is double)
                        return (double)left + (double)right;
                    if (left is string && right is string)
                        return (string)left + (string)right;
                    if (left is string)
                        return (string)left + Stringify(right);
                    if (right is string)
                        return Stringify(left) + (string)right;
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or at least one string.");
                case TokenType.Slash:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
            }

            // unreachable
            return null;
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Bang:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            // unreachable
            return null;
        }
        
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object value)
        {
            if (value == null)
                return false;
            if (value is bool)
                return (bool)value;
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            // nil is only equal to nil.
            if (left == null && right == null)
                return true;
            if (left == null)
                return false;

            return left.Equals(right);
        }

        private string Stringify(object value)
        {
            if (value == null)
                return "nil";
            if (value is bool)
                return value.ToString().ToLower();

            return value.ToString();
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
                return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object right, object left)
        {
            if (right is double && left is double)
                return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }
    }
}
