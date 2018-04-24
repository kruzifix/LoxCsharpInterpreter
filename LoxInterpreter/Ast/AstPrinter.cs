using System.Text;

namespace LoxInterpreter
{
    class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
        
        public string VisitAssignExpr(AssignExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitBinaryExpr(BinaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitCallExpr(CallExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitGetExpr(GetExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitGroupingExpr(GroupingExpr expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(LiteralExpr expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitLogicalExpr(LogicalExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitSetExpr(SetExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitSuperExpr(SuperExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitThisExpr(ThisExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string VisitUnaryExpr(UnaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitVariableExpr(VariableExpr expr)
        {
            return Parenthesize("var", expr);
        }

        private string Parenthesize(string name, params Expr[] expr)
        {
            var sb = new StringBuilder();

            sb.Append("(").Append(name);
            for (int i = 0; i < expr.Length; i++)
            {
                sb.Append(" ").Append(expr[i].Accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
