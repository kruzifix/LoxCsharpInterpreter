namespace LoxInterpreter
{
    interface IStmtVisitor
    {
        void VisitExpressionStmt(ExpressionStmt stmt);
        void VisitPrintStmt(PrintStmt stmt);
    }
    abstract class Stmt
    {
        public abstract void Accept(IStmtVisitor visitor);
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
}