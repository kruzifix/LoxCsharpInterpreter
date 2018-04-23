﻿using System.Collections.Generic;

namespace LoxInterpreter
{
    class Environment
    {
        private Environment enclosing;
        private Dictionary<string, object> values;

        public Environment()
            : this(null)
        { }

        public Environment(Environment environment)
        {
            enclosing = environment;
            values = new Dictionary<string, object>();
        }

        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.Lexeme] = value;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (enclosing != null)
            {
                return enclosing.Get(name);
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (int i = 0; i < distance; i++)
                environment = environment.enclosing;
            return environment;
        }
    }
}
