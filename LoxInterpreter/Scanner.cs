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

                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;

                case '/':
                    if (Match('/'))
                    {
                        // a comment goes until the end of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    line++;
                    break;

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
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start + 1);
            tokens.Add(new Token(type, text, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd() || source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : source[current];
        }
    }
}
