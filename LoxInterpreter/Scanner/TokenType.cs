﻿namespace LoxInterpreter
{
    enum TokenType
    {
        // single-character tokens
        LeftParen, RightParen, LeftBrace, RightBrace,
        Comma, Dot, Minus, Plus, Semicolon, Slash, Star,

        // one or two character tokens
        Bang, BangEqual,
        Equal, EqualEqual,
        Greater, GreaterEqual,
        Less, LessEqual,

        // literals
        Identifier, String, Number,

        // keywords
        And, Break, Class, Else, False, Fun, For, If, Nil, Or,
        Print, Return, Super, This, True, Var, While,

        Eof
    }
}