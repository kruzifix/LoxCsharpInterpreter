using System.Collections.Generic;

namespace LoxInterpreter
{
    class LoxFunction : ICallable
    {
        private FunctionStmt declaration;
        private Environment closure;

        public LoxFunction(FunctionStmt declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var environment = new Environment(closure);
            environment.Define("this", instance);
            return new LoxFunction(declaration, environment);
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(closure);

            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("<fn {0}>", declaration.Name.Lexeme);
        }
    }
}
