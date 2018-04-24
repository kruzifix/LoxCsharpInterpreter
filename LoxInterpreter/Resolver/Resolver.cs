using System.Collections.Generic;

namespace LoxInterpreter
{
    class Resolver : IExprVisitor, IStmtVisitor
    {
        class VariableData
        {
            public bool Defined { get; set; }
            public int Line { get; set; }
            public bool Used { get; set; }
        }

        private Interpreter interpreter;
        private Stack<Dictionary<string, VariableData>> scopes;
        private FunctionType currentFunction;
        private ClassType currentClass;
        private int loopDepth;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            scopes = new Stack<Dictionary<string, VariableData>>();
            currentFunction = FunctionType.None;
            currentClass = ClassType.None;
            loopDepth = 0;
        }

        public void Resolve(List<Stmt> statements)
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
            int depth = 0;
            foreach (var scope in scopes)
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    scope[name.Lexeme].Used = true;
                    interpreter.Resolve(expr, depth);
                    return;
                }
                depth++;
            }
        }

        private void ResolveFunction(FunctionStmt function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, VariableData>());
        }

        private void EndScope()
        {
            var scope = scopes.Pop();

            foreach (var kvp in scope)
            {
                if (kvp.Value.Used == false)
                {
                    Lox.Warning(kvp.Value.Line, "Variable " + kvp.Key + " declared but never used.");
                }
            }
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0)
                return;
            var scope = scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Variable with this name already declared in this scope.");
                return;
            }

            scope.Add(name.Lexeme, new VariableData {
                Defined = false,
                Line = name.Line,
                Used = false
            });
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0)
                return;
            scopes.Peek()[name.Lexeme].Defined = true;
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

        public void VisitGetExpr(GetExpr expr)
        {
            Resolve(expr.Object);
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

        public void VisitSetExpr(SetExpr expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
        }

        public void VisitThisExpr(ThisExpr expr)
        {
            if (currentClass == ClassType.None)
            {
                Lox.Error(expr.Keyword, "Cannot use 'this' ouside of class.");
                return;
            }
            ResolveLocal(expr, expr.Keyword);
        }

        public void VisitUnaryExpr(UnaryExpr expr)
        {
            Resolve(expr.Right);
        }

        public void VisitVariableExpr(VariableExpr expr)
        {
            if (scopes.Count > 0)
            {
                var scope = scopes.Peek();
                if (scope.ContainsKey(expr.Name.Lexeme) && scope[expr.Name.Lexeme].Defined == false)
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

        public void VisitBreakStmt(BreakStmt stmt)
        {
            if (loopDepth == 0) // not inside loop
                Lox.Error(stmt.Keyword, "Cannot break outside of loop.");
        }

        public void VisitClassStmt(ClassStmt stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            var enclosingClass = currentClass;
            currentClass = ClassType.Class;

            BeginScope();
            scopes.Peek().Add("this", new VariableData {
                Defined = true,
                Line = stmt.Name.Line,
                Used = true
            });

            foreach (var method in stmt.Methods)
            {
                var declaration = FunctionType.Method;

                if (method.Name.Lexeme.Equals("init"))
                    declaration = FunctionType.Initializer;

                ResolveFunction(method, declaration);
            }
            EndScope();

            currentClass = enclosingClass;
        }

        public void VisitExpressionStmt(ExpressionStmt stmt)
        {
            Resolve(stmt.Expression);
        }

        public void VisitFunctionStmt(FunctionStmt stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.Function);
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
            if (currentFunction == FunctionType.None)
                Lox.Error(stmt.Keyword, "Cannot return from top-level code.");
            if (stmt.Value != null)
            {
                if (currentFunction == FunctionType.Initializer)
                    Lox.Error(stmt.Keyword, "Cannot return a value from an initializer.");
                Resolve(stmt.Value);
            }
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
            loopDepth++;
            Resolve(stmt.Body);
            loopDepth--;
            if (loopDepth < 0)
                throw new System.Exception("loopDepth < 0 !?!");
        }

        #endregion
    }
}
