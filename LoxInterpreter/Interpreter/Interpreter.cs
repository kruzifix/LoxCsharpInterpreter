using System;

namespace LoxInterpreter
{
    class Interpreter : IVisitor<object>
    {
        public object VisitBinaryExpr(BinaryExpr expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            throw new NotImplementedException();
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            throw new NotImplementedException();
        }
    }
}
