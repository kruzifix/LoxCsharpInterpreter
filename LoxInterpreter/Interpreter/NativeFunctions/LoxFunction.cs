using System.Collections.Generic;

namespace LoxInterpreter
{
    class LoxFunction : ICallable
    {
        private FunctionStmt declaration;

        public LoxFunction(FunctionStmt declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(interpreter.Globals);

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
