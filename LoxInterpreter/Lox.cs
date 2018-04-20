using System;
using System.IO;

namespace LoxInterpreter
{
    class Lox
    {
        static bool hadError = false;

        static void Main(string[] args)
        {
            var e = new BinaryExpr(
                new UnaryExpr(
                    new Token(TokenType.Minus, "-", null, 1),
                    new LiteralExpr(123)),
                new Token(TokenType.Star, "*", null, 1),
                new GroupingExpr(new LiteralExpr(45.67)));

            Console.WriteLine(new AstPrinter().Print(e));

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            string source = File.ReadAllText(path);
            Run(source);

            if (hadError)
            {
                Environment.Exit(65);
            }
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                Run(input);
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var t in tokens)
            {
                Console.WriteLine(t);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.Eof)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, "at '" + token.Lexeme + "'", message);
            }
        }

        public static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[Line {0}] Error {1}: {2}", line, where, message);
            hadError = true;
        }
    }
}
