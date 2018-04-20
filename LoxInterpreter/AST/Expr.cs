namespace LoxInterpreter
{
	abstract class Expr
	{
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
	}
	class GroupingExpr : Expr
	{
		public Expr Expression { get; }
		public GroupingExpr(Expr Expression)
		{
			this.Expression = Expression;
		}
	}
	class LiteralExpr : Expr
	{
		public object Value { get; }
		public LiteralExpr(object Value)
		{
			this.Value = Value;
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
	}

}