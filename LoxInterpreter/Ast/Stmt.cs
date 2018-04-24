using System.Collections.Generic;
namespace LoxInterpreter
{
    interface IStmtVisitor
    {
        void VisitBlockStmt(BlockStmt stmt);
        void VisitBreakStmt(BreakStmt stmt);
        void VisitClassStmt(ClassStmt stmt);
        void VisitExecuteStmt(ExecuteStmt stmt);
        void VisitExpressionStmt(ExpressionStmt stmt);
        void VisitFunctionStmt(FunctionStmt stmt);
        void VisitIfStmt(IfStmt stmt);
        void VisitPrintStmt(PrintStmt stmt);
        void VisitReturnStmt(ReturnStmt stmt);
        void VisitVarStmt(VarStmt stmt);
        void VisitWhileStmt(WhileStmt stmt);
    }
    interface IStmtVisitor<T>
    {
        T VisitBlockStmt(BlockStmt stmt);
        T VisitBreakStmt(BreakStmt stmt);
        T VisitClassStmt(ClassStmt stmt);
        T VisitExecuteStmt(ExecuteStmt stmt);
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitFunctionStmt(FunctionStmt stmt);
        T VisitIfStmt(IfStmt stmt);
        T VisitPrintStmt(PrintStmt stmt);
        T VisitReturnStmt(ReturnStmt stmt);
        T VisitVarStmt(VarStmt stmt);
        T VisitWhileStmt(WhileStmt stmt);
    }
    abstract class Stmt
    {
        public abstract void Accept(IStmtVisitor visitor);
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }
    class BlockStmt : Stmt
    {
        public List<Stmt> Statements { get; }
        public BlockStmt(List<Stmt> Statements)
        {
            this.Statements = Statements;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitBlockStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
    class BreakStmt : Stmt
    {
        public Token Keyword { get; }
        public BreakStmt(Token Keyword)
        {
            this.Keyword = Keyword;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitBreakStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitBreakStmt(this);
        }
    }
    class ClassStmt : Stmt
    {
        public Token Name { get; }
        public VariableExpr SuperClass { get; }
        public List<FunctionStmt> Methods { get; }
        public ClassStmt(Token Name, VariableExpr SuperClass, List<FunctionStmt> Methods)
        {
            this.Name = Name;
            this.SuperClass = SuperClass;
            this.Methods = Methods;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitClassStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitClassStmt(this);
        }
    }
    class ExecuteStmt : Stmt
    {
        public Token Keyword { get; }
        public Token Path { get; }
        public ExecuteStmt(Token Keyword, Token Path)
        {
            this.Keyword = Keyword;
            this.Path = Path;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitExecuteStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExecuteStmt(this);
        }
    }
    class ExpressionStmt : Stmt
    {
        public Expr Expression { get; }
        public ExpressionStmt(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitExpressionStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
    class FunctionStmt : Stmt
    {
        public Token Name { get; }
        public List<Token> Parameters { get; }
        public List<Stmt> Body { get; }
        public FunctionStmt(Token Name, List<Token> Parameters, List<Stmt> Body)
        {
            this.Name = Name;
            this.Parameters = Parameters;
            this.Body = Body;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitFunctionStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }
    class IfStmt : Stmt
    {
        public Expr Condition { get; }
        public Stmt ThenBranch { get; }
        public Stmt ElseBranch { get; }
        public IfStmt(Expr Condition, Stmt ThenBranch, Stmt ElseBranch)
        {
            this.Condition = Condition;
            this.ThenBranch = ThenBranch;
            this.ElseBranch = ElseBranch;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitIfStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }
    class PrintStmt : Stmt
    {
        public Expr Expression { get; }
        public PrintStmt(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitPrintStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
    class ReturnStmt : Stmt
    {
        public Token Keyword { get; }
        public Expr Value { get; }
        public ReturnStmt(Token Keyword, Expr Value)
        {
            this.Keyword = Keyword;
            this.Value = Value;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitReturnStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }
    class VarStmt : Stmt
    {
        public Token Name { get; }
        public Expr Initializer { get; }
        public VarStmt(Token Name, Expr Initializer)
        {
            this.Name = Name;
            this.Initializer = Initializer;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitVarStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }
    class WhileStmt : Stmt
    {
        public Expr Condition { get; }
        public Stmt Body { get; }
        public WhileStmt(Expr Condition, Stmt Body)
        {
            this.Condition = Condition;
            this.Body = Body;
        }
        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitWhileStmt(this);
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}