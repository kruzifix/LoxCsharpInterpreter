using System.Collections.Generic;

namespace LoxInterpreter
{
    class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.Class))
                    return ClassDeclaration();
                if (Match(TokenType.Execute))
                    return ExecuteDeclaration();
                if (Match(TokenType.Fun))
                    return Function("function");
                if (Match(TokenType.Var))
                    return VarDeclaration();

                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt ClassDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected class name.");

            VariableExpr superClass = null;
            if (Match(TokenType.Less))
            {
                Consume(TokenType.Identifier, "Expected superclass name.");
                superClass = new VariableExpr(Previous());
            }

            Consume(TokenType.LeftBrace, "Expected '{' before class body.");

            var methods = new List<FunctionStmt>();
            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                methods.Add(Function("method"));
            }

            Consume(TokenType.RightBrace, "Expected '}' after class body.");

            return new ClassStmt(name, superClass, methods);
        }

        private Stmt ExecuteDeclaration()
        {
            var keyword = Previous();
            var value = Consume(TokenType.String, "Expected string after 'execute'.");

            Consume(TokenType.Semicolon, "Expected ';' after string.");

            return new ExecuteStmt(keyword, value);
        }

        private FunctionStmt Function(string kind)
        {
            var name = Consume(TokenType.Identifier, "Expected " + kind + " name.");

            Consume(TokenType.LeftParen, "Expected '(' after " + kind + " name.");
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (parameters.Count >= 8)
                    {
                        Error(Peek(), "Cannot have more than 8 parameters.");
                    }
                    parameters.Add(Consume(TokenType.Identifier, "Expected parameter name."));
                } while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen, "Expected ')' after parameters.");

            Consume(TokenType.LeftBrace, "Expect '{' before " + kind + " body.");
            List<Stmt> body = Block();
            return new FunctionStmt(name, parameters, body);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected variable name.");

            Expr initializer = null;
            if (Match(TokenType.Equal))
                initializer = Expression();

            Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(TokenType.For))
                return ForStatement();
            if (Match(TokenType.If))
                return IfStatement();
            if (Match(TokenType.Print))
                return PrintStatement();
            if (Match(TokenType.Break))
                return BreakStatement();
            if (Match(TokenType.Return))
                return ReturnStatement();
            if (Match(TokenType.While))
                return WhileStatement();
            if (Match(TokenType.LeftBrace))
                return new BlockStmt(Block());
            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'for'.");

            Stmt initializer;
            if (Match(TokenType.Semicolon))
                initializer = null;
            else if (Match(TokenType.Var))
                initializer = VarDeclaration();
            else
                initializer = ExpressionStatement();

            Expr condition = null;
            if (!Check(TokenType.Semicolon))
                condition = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after loop condition.");

            Expr increment = null;
            if (!Check(TokenType.RightParen))
                increment = Expression();
            Consume(TokenType.RightParen, "Expect ')' after for clauses.");

            var body = Statement();

            if (increment != null)
                body = new BlockStmt(new List<Stmt> { body, new ExpressionStmt(increment) });

            if (condition == null)
                condition = new LiteralExpr(true);
            body = new WhileStmt(condition, body);

            if (initializer != null)
                body = new BlockStmt(new List<Stmt> { initializer, body });

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LeftParen, "Expected '(' after 'if'.");
            var condition = Expression();
            Consume(TokenType.RightParen, "Expected ')' after if condition.");

            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = Statement();
            }

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after value.");
            return new PrintStmt(value);
        }

        private Stmt BreakStatement()
        {
            var keyword = Previous();
            Consume(TokenType.Semicolon, "Expected ';' after break.");
            return new BreakStmt(keyword);
        }

        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            Expr value = null;
            if (!Check(TokenType.Semicolon))
            {
                value = Expression();
            }
            Consume(TokenType.Semicolon, "Expected ',' after return value.");
            return new ReturnStmt(keyword, value);
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LeftParen, "Expected '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RightParen, "Expected ')' after while condition.");
            // add 1 to loop depth
            var body = Statement();
            // remove 1 from loop depth

            // error if break/continue exist if loop depth == 0

            return new WhileStmt(condition, body);
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            if (!Check(TokenType.Semicolon))
                return new ExpressionStmt(expr);
            Consume(TokenType.Semicolon, "Expected ';' after expression.");
            return new ExpressionStmt(expr);
        }

        private List<Stmt> Block()
        {
            var statements = new List<Stmt>();

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RightBrace, "Expected '}' after block.");
            return statements;
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(TokenType.Equal))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is VariableExpr)
                {
                    var name = (expr as VariableExpr).Name;
                    return new AssignExpr(name, value);
                }
                else if (expr is GetExpr)
                {
                    var get = expr as GetExpr;
                    return new SetExpr(get.Object, get.Name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = And();
                expr = new LogicalExpr(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new LogicalExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                var op = Previous();
                var right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Addition();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var op = Previous();
                var right = Addition();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            var expr = Multiplication();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                var op = Previous();
                var right = Multiplication();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            var expr = Unary();

            while (Match(TokenType.Slash, TokenType.Star))
            {
                var op = Previous();
                var right = Unary();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                var op = Previous();
                var right = Unary();
                return new UnaryExpr(op, right);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParen))
                    expr = FinishCall(expr);
                else if (Match(TokenType.Dot))
                {
                    var name = Consume(TokenType.Identifier, "Expected property name after '.'.");
                    expr = new GetExpr(expr, name);
                }
                else
                    break;
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (arguments.Count >= 8)
                    {
                        Error(Peek(), "Cannot have more than 8 arguments.");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.Comma));
            }

            var paren = Consume(TokenType.RightParen, "Expected ')' after arguments.");

            return new CallExpr(callee, paren, arguments);
        }

        private Expr Primary()
        {
            if (Match(TokenType.False)) return new LiteralExpr(false);
            if (Match(TokenType.True)) return new LiteralExpr(true);
            if (Match(TokenType.Nil)) return new LiteralExpr(null);

            if (Match(TokenType.Number, TokenType.String))
            {
                return new LiteralExpr(Previous().Literal);
            }

            if (Match(TokenType.Super))
            {
                var keyword = Previous();
                Consume(TokenType.Dot, "Expected '.' after 'super'.");
                var method = Consume(TokenType.Identifier, "Expected superclass method name.");
                return new SuperExpr(keyword, method);
            }

            if (Match(TokenType.This))
            {
                return new ThisExpr(Previous());
            }

            if (Match(TokenType.Identifier))
            {
                return new VariableExpr(Previous());
            }

            if (Match(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression.");
                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expected expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }
            throw Error(Peek(), message);
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon)
                    return;

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private bool Match(params TokenType[] types)
        {
            foreach (TokenType token in types)
            {
                if (Check(token))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }
            return Previous();
        }

        public bool IsAtEnd()
        {
            return Peek().Type == TokenType.Eof;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }
    }
}
