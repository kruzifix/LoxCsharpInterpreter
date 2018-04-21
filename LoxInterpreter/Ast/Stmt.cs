using System.Collections.Generic;
namespace LoxInterpreter
{
    interface IStmtVisitor
    {
        void VisitBlockStmt(BlockStmt stmt);
        void VisitExpressionStmt(ExpressionStmt stmt);
        void VisitIfStmt(IfStmt stmt);
        void VisitPrintStmt(PrintStmt stmt);
        void VisitVarStmt(VarStmt stmt);
        void VisitWhileStmt(WhileStmt stmt);
    }
    abstract class Stmt
    {
        public abstract void Accept(IStmtVisitor visitor);
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
    }
}