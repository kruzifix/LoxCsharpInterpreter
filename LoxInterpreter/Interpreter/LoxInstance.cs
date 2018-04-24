using System.Collections.Generic;

namespace LoxInterpreter
{
    class LoxInstance
    {
        private LoxClass klass;
        private Dictionary<string, object> fields;

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
            fields = new Dictionary<string, object>();
        }

        public object Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }

            throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
        }

        public override string ToString()
        {
            return string.Format("<instance {0}>", klass.Name);
        }
    }
}
