namespace LoxInterpreter
{
    interface IStmtVisitor<T>
    {
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitPrintStmt(PrintStmt stmt);
    }
    abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }
    class ExpressionStmt : Stmt
    {
        public Expr Expression { get; }
        public ExpressionStmt(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
    class PrintStmt : Stmt
    {
        public Expr Expression { get; }
        public PrintStmt(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
}