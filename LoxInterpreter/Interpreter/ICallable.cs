using System.Collections.Generic;

namespace LoxInterpreter
{
    interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
