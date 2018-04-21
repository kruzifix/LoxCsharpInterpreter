using System.Collections.Generic;

namespace LoxInterpreter
{
    class Environment
    {
        private Dictionary<string, object> values;

        public Environment()
        {
            values = new Dictionary<string, object>();
        }

        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }
    }
}
