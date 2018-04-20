using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(BinaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
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

        public string VisitUnaryExpr(UnaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
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
