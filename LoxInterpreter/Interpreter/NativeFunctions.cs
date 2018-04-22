using System;
using System.Collections.Generic;
using System.Linq;
namespace LoxInterpreter
{
    class ClockFunction : ICallable
    {
        private int start;

        public ClockFunction()
        {
            start = System.Environment.TickCount;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)((System.Environment.TickCount - start) / 1000.0);
        }
    }
}
