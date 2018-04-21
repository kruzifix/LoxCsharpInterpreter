using System;
using System.IO;

namespace LoxInterpreter
{
    class Lox
    {
        static bool hadError = false;

        static void Main(string[] args)
        {
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
            var parser = new Parser(tokens);

            var expr = parser.Parse();

            if (hadError)
                return;

            Console.WriteLine(new AstPrinter().Print(expr));
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
