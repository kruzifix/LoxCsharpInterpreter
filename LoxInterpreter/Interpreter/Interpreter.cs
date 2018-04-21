using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    class Interpreter : IExprVisitor<object>, IStmtVisitor
    {
        private Environment environment;

        public Interpreter()
        {
            environment = new Environment();
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError e)
            {
                Lox.RuntimeError(e);
            }
        }

        #region Statement Visitor

        public void VisitBlockStmt(BlockStmt stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
        }

        public void VisitExpressionStmt(ExpressionStmt stmt)
        {
            Evaluate(stmt.Expression);
        }

        public void VisitIfStmt(IfStmt stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
        }

        public void VisitPrintStmt(PrintStmt stmt)
        {
            var value = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(value));
        }

        public void VisitVarStmt(VarStmt stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            environment.Define(stmt.Name.Lexeme, value);
        }

        #endregion

        #region Expression Visitor

        public object VisitAssignExpr(AssignExpr expr)
        {
            var value = Evaluate(expr.Value);
            environment.Assign(expr.Name, value);
            return value;
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

        public object VisitVariableExpr(VariableExpr expr)
        {
            return environment.Get(expr.Name);
        }

        #endregion

        public object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                this.environment = previous;
            }
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

        public static string Stringify(object value)
        {
            if (value == null)
                return "nil";
            if (value is bool)
                return value.ToString().ToLower();

            return value.ToString();
        }
    }
}
