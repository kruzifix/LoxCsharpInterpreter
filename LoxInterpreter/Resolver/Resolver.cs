using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    class Resolver : IExprVisitor, IStmtVisitor
    {
        #region Expressions

        public void VisitAssignExpr(AssignExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitBinaryExpr(BinaryExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitCallExpr(CallExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitGroupingExpr(GroupingExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitLiteralExpr(LiteralExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitLogicalExpr(LogicalExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitUnaryExpr(UnaryExpr expr)
        {
            throw new NotImplementedException();
        }

        public void VisitVariableExpr(VariableExpr expr)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Statements

        public void VisitBlockStmt(BlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitExpressionStmt(ExpressionStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitFunctionStmt(FunctionStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitIfStmt(IfStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitPrintStmt(PrintStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitReturnStmt(ReturnStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitVarStmt(VarStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void VisitWhileStmt(WhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
