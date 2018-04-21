using System;
using System.IO;

namespace LoxInterpreter
{
    class Lox
    {
        static bool hadError = false;
        static bool hadRuntimeError = false;

        static Interpreter interpreter = new Interpreter();

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
                System.Environment.Exit(65);
            }
            if (hadRuntimeError)
            {
                System.Environment.Exit(70);
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

            var statements = parser.Parse();

            if (hadError)
                return;

            interpreter.Interpret(statements);
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

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine("[line {0}] RuntimeError: {1}", error.Token.Line, error.Message);
            hadRuntimeError = true;
        }
    }
}
