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
            if (methods.ContainsKey("init"))
            {
                return methods["init"].Arity();
            }
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = FindMethod(instance, "init");
            if (initializer != null)
            {
                initializer.Call(interpreter, arguments);
            }
            return instance;
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
