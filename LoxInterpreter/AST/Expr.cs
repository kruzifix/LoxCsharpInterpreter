namespace LoxInterpreter
{
    interface IExprVisitor<T>
    {
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);
        T VisitVariableExpr(VariableExpr expr);
    }
    abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }
    class BinaryExpr : Expr
    {
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }
        public BinaryExpr(Expr Left, Token Operator, Expr Right)
        {
            this.Left = Left;
            this.Operator = Operator;
            this.Right = Right;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
    class GroupingExpr : Expr
    {
        public Expr Expression { get; }
        public GroupingExpr(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
    class LiteralExpr : Expr
    {
        public object Value { get; }
        public LiteralExpr(object Value)
        {
            this.Value = Value;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
    class UnaryExpr : Expr
    {
        public Token Operator { get; }
        public Expr Right { get; }
        public UnaryExpr(Token Operator, Expr Right)
        {
            this.Operator = Operator;
            this.Right = Right;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
    class VariableExpr : Expr
    {
        public Token Name { get; }
        public VariableExpr(Token Name)
        {
            this.Name = Name;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}