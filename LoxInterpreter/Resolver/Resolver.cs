using System.Collections.Generic;

namespace LoxInterpreter
{
    class Resolver : IExprVisitor, IStmtVisitor
    {
        private Interpreter interpreter;
        private Stack<Dictionary<string, bool>> scopes;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            scopes = new Stack<Dictionary<string, bool>>();
        }

        private void Resolve(List<Stmt> statements)
        {
            statements.ForEach(s => Resolve(s));
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            int scopeDistance = 0;
            foreach (var scope in scopes)
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, scopeDistance);
                    return;
                }
                scopeDistance++;
            }
        }

        private void ResolveFunction(FunctionStmt function)
        {
            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0)
                return;
            var scope = scopes.Peek();
            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0)
                return;
            scopes.Peek()[name.Lexeme] = true;
        }

        #region Expressions

        public void VisitAssignExpr(AssignExpr expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
        }

        public void VisitBinaryExpr(BinaryExpr expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
        }

        public void VisitCallExpr(CallExpr expr)
        {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments)
                Resolve(argument);
        }

        public void VisitGroupingExpr(GroupingExpr expr)
        {
            Resolve(expr.Expression);
        }

        public void VisitLiteralExpr(LiteralExpr expr)
        {
        }

        public void VisitLogicalExpr(LogicalExpr expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
        }

        public void VisitUnaryExpr(UnaryExpr expr)
        {
            Resolve(expr.Right);
        }

        public void VisitVariableExpr(VariableExpr expr)
        {
            if (scopes.Count > 0 && scopes.Peek()[expr.Name.Lexeme] == false)
            {
                Lox.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
        }

        #endregion

        #region Statements

        public void VisitBlockStmt(BlockStmt stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
        }

        public void VisitExpressionStmt(ExpressionStmt stmt)
        {
            Resolve(stmt.Expression);
        }

        public void VisitFunctionStmt(FunctionStmt stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt);
        }

        public void VisitIfStmt(IfStmt stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null)
                Resolve(stmt.ElseBranch);
        }

        public void VisitPrintStmt(PrintStmt stmt)
        {
            Resolve(stmt.Expression);
        }

        public void VisitReturnStmt(ReturnStmt stmt)
        {
            if (stmt.Value != null)
                Resolve(stmt.Value);
        }

        public void VisitVarStmt(VarStmt stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
        }

        public void VisitWhileStmt(WhileStmt stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
        }

        #endregion
    }
}
