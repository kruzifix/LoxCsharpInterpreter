using System.Collections.Generic;

namespace LoxInterpreter
{
    class LoxClass : ICallable
    {
        public string Name { get; private set; }

        public LoxClass(string name)
        {
            Name = name;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return new LoxInstance(this);
        }

        public override string ToString()
        {
            return string.Format("<class {0}>", Name);
        }
    }
}
