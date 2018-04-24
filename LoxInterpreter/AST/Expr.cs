using System.Collections.Generic;
namespace LoxInterpreter
{
    interface IExprVisitor
    {
        void VisitAssignExpr(AssignExpr expr);
        void VisitBinaryExpr(BinaryExpr expr);
        void VisitCallExpr(CallExpr expr);
        void VisitGetExpr(GetExpr expr);
        void VisitGroupingExpr(GroupingExpr expr);
        void VisitLiteralExpr(LiteralExpr expr);
        void VisitLogicalExpr(LogicalExpr expr);
        void VisitSetExpr(SetExpr expr);
        void VisitUnaryExpr(UnaryExpr expr);
        void VisitVariableExpr(VariableExpr expr);
    }
    interface IExprVisitor<T>
    {
        T VisitAssignExpr(AssignExpr expr);
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitCallExpr(CallExpr expr);
        T VisitGetExpr(GetExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitLogicalExpr(LogicalExpr expr);
        T VisitSetExpr(SetExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);
        T VisitVariableExpr(VariableExpr expr);
    }
    abstract class Expr
    {
        public abstract void Accept(IExprVisitor visitor);
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }
    class AssignExpr : Expr
    {
        public Token Name { get; }
        public Expr Value { get; }
        public AssignExpr(Token Name, Expr Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitAssignExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
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
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitBinaryExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
    class CallExpr : Expr
    {
        public Expr Callee { get; }
        public Token Paren { get; }
        public List<Expr> Arguments { get; }
        public CallExpr(Expr Callee, Token Paren, List<Expr> Arguments)
        {
            this.Callee = Callee;
            this.Paren = Paren;
            this.Arguments = Arguments;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitCallExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitCallExpr(this);
        }
    }
    class GetExpr : Expr
    {
        public Expr Object { get; }
        public Token Name { get; }
        public GetExpr(Expr Object, Token Name)
        {
            this.Object = Object;
            this.Name = Name;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitGetExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetExpr(this);
        }
    }
    class GroupingExpr : Expr
    {
        public Expr Expression { get; }
        public GroupingExpr(Expr Expression)
        {
            this.Expression = Expression;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitGroupingExpr(this);
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
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitLiteralExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
    class LogicalExpr : Expr
    {
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }
        public LogicalExpr(Expr Left, Token Operator, Expr Right)
        {
            this.Left = Left;
            this.Operator = Operator;
            this.Right = Right;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitLogicalExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }
    class SetExpr : Expr
    {
        public Expr Object { get; }
        public Token Name { get; }
        public Expr Value { get; }
        public SetExpr(Expr Object, Token Name, Expr Value)
        {
            this.Object = Object;
            this.Name = Name;
            this.Value = Value;
        }
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitSetExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitSetExpr(this);
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
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitUnaryExpr(this);
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
        public override void Accept(IExprVisitor visitor)
        {
            visitor.VisitVariableExpr(this);
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}