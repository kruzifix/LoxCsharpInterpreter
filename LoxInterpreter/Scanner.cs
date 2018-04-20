using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    class Scanner
    {
        private readonly string source;
        private List<Token> tokens;
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
            tokens = new List<Token>();
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.Eof, "", null, line));
            return tokens;
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                default:
                    Lox.Error(line, string.Format("Unexpected character '{0}'.", c));
                    break;
            }
        }

        private char Advance()
        {
            current++;
            return source[current];
        }

        private void AddToken(TokenType type)
        {

        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start + 1);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
