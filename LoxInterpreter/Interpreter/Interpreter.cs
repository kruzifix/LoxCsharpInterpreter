﻿using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    class Interpreter : IExprVisitor<object>, IStmtVisitor
    {
        private Environment environment;
        private Dictionary<Expr, int> locals;

        public Environment Globals { get; }

        public Interpreter()
        {
            Globals = new Environment();
            Globals.Define("clock", new ClockFunction());

            locals = new Dictionary<Expr, int>();
            environment = Globals;
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

        public void VisitBreakStmt(BreakStmt stmt)
        {
            throw new Break();
        }

        public void VisitClassStmt(ClassStmt stmt)
        {
            environment.Define(stmt.Name.Lexeme, null);

            object superClass = null;
            if (stmt.SuperClass != null)
            {
                superClass = Evaluate(stmt.SuperClass);
                if (!(superClass is LoxClass))
                    throw new RuntimeError(stmt.SuperClass.Name, "Superclass must be class.");

                environment = new Environment(environment);
                environment.Define("super", superClass);
            }

            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in stmt.Methods)
            {
                var function = new LoxFunction(method, environment, method.Name.Lexeme.Equals("init"));

                methods.Add(method.Name.Lexeme, function);
            }

            var klass = new LoxClass(stmt.Name.Lexeme, (LoxClass)superClass, methods);

            if (stmt.SuperClass != null)
                environment = environment.Enclosing;

            environment.Assign(stmt.Name, klass);
        }

        public void VisitExecuteStmt(ExecuteStmt stmt)
        {
            var path = Evaluate(stmt.Value);

            if (!(path is string))
            {
                throw new RuntimeError(stmt.Keyword, "Execute path must be a string.");
            }

            Lox.RunFile(path as string);
        }

        public void VisitExpressionStmt(ExpressionStmt stmt)
        {
            Evaluate(stmt.Expression);
        }

        public void VisitFunctionStmt(FunctionStmt stmt)
        {
            var function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.Name.Lexeme, function);
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

        public void VisitReturnStmt(ReturnStmt stmt)
        {
            object value = null;
            if (stmt.Value != null)
                value = Evaluate(stmt.Value);

            throw new Return(value);
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

        public void VisitWhileStmt(WhileStmt stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                // break/continue statement
                // throw abort/continue loop exception
                // catch here!
                try
                {
                    Execute(stmt.Body);
                }
                catch (Break)
                {
                    return;
                }
            }
        }

        #endregion

        #region Expression Visitor

        public object VisitAssignExpr(AssignExpr expr)
        {
            var value = Evaluate(expr.Value);

            if (locals.ContainsKey(expr))
                environment.AssignAt(locals[expr], expr.Name, value);
            else
                Globals.Assign(expr.Name, value);

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

        public object VisitCallExpr(CallExpr expr)
        {
            var callee = Evaluate(expr.Callee);

            var arguments = new List<object>();
            foreach (var arg in expr.Arguments)
            {
                arguments.Add(Evaluate(arg));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            var function = (ICallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, string.Format("Expected {0} arguments but got {1}.", function.Arity(), arguments.Count));
            }
            return function.Call(this, arguments);
        }

        public object VisitGetExpr(GetExpr expr)
        {
            var obj = Evaluate(expr.Object);
            if (obj is LoxInstance)
            {
                return ((LoxInstance)obj).Get(expr.Name);
            }

            throw new RuntimeError(expr.Name, "Only instances have properties.");
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(LogicalExpr expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Operator.Type == TokenType.Or)
            {
                if (IsTruthy(left))
                    return left;
            }
            else
            {
                if (!IsTruthy(left))
                    return left;
            }

            return Evaluate(expr.Right);
        }

        public object VisitSetExpr(SetExpr expr)
        {
            var obj = Evaluate(expr.Object);

            if (!(obj is LoxInstance))
                throw new RuntimeError(expr.Name, "Only instances have fields.");

            var value = Evaluate(expr.Value);
            (obj as LoxInstance).Set(expr.Name, value);
            return value;
        }

        public object VisitSuperExpr(SuperExpr expr)
        {
            int distance = locals[expr];
            var superClass = (LoxClass)environment.GetAt(distance, "super");
            var obj = (LoxInstance)environment.GetAt(distance - 1, "this");

            var method = superClass.FindMethod(obj, expr.Method.Lexeme);

            if (method == null)
                throw new RuntimeError(expr.Method, "Undefined property '" + expr.Method.Lexeme + "'.");

            return method;
        }

        public object VisitThisExpr(ThisExpr expr)
        {
            return LookUpVariable(expr.Keyword, expr);
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
            return LookUpVariable(expr.Name, expr);
        }

        #endregion

        public void Resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            if (locals.ContainsKey(expr))
            {
                return environment.GetAt(locals[expr], name.Lexeme);
            }
            return Globals.Get(name);
        }

        public object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
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
