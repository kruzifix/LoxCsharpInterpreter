using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    class Scanner
    {
        #region Keywords

        private static Dictionary<string, TokenType> keywords;

        static Scanner()
        {
            keywords = new Dictionary<string, TokenType>();
            keywords.Add("and", TokenType.And);
            keywords.Add("class", TokenType.Class);
            keywords.Add("else", TokenType.Else);
            keywords.Add("false", TokenType.False);
            keywords.Add("for", TokenType.For);
            keywords.Add("fun", TokenType.Fun);
            keywords.Add("if", TokenType.If);
            keywords.Add("nil", TokenType.Nil);
            keywords.Add("or", TokenType.Or);
            keywords.Add("print", TokenType.Print);
            keywords.Add("return", TokenType.Return);
            keywords.Add("super", TokenType.Super);
            keywords.Add("this", TokenType.This);
            keywords.Add("true", TokenType.True);
            keywords.Add("var", TokenType.Var);
            keywords.Add("while", TokenType.While);
        }

        #endregion

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

        #region Tokens

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

                case '"': ParseString(); break;

                default:
                    if (IsDigit(c))
                    {
                        ParseNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ParseIdentifier();
                    }
                    else
                    {
                        Lox.Error(line, string.Format("Unexpected character '{0}'.", c));
                    }
                    break;
            }
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        #endregion

        #region Identifier

        private void ParseIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            string text = source.Substring(start, current - start);

            TokenType type = TokenType.Identifier;
            if (keywords.ContainsKey(text))
            {
                type = keywords[text];
            }
            AddToken(type);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        #endregion

        private void ParseString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // closing '
            Advance();

            // trim quotes
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.String, value);
        }

        #region Number

        private void ParseNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // consume the .
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.Number, double.Parse(source.Substring(start, current - start)));
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        #endregion

        #region Control Flow

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private char Advance()
        {
            current++;
            return source[current- 1];
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

        private char PeekNext()
        {
            return (current + 1 >= source.Length) ? '\0' : source[current + 1];
        }

        #endregion
    }
}
