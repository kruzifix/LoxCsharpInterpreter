using System.Collections.Generic;

namespace LoxInterpreter
{
    class LoxClass : ICallable
    {
        public string Name { get; private set; }
        private Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            this.methods = methods;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return new LoxInstance(this);
        }

        public LoxFunction FindMethod(LoxInstance instance, string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name].Bind(instance);
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("<class {0}>", Name);
        }
    }
}
