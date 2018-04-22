using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    class ClockFunction : ICallable
    {
        private long start;

        public ClockFunction()
        {
            start = DateTime.Now.Ticks;
        }

        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)((DateTime.Now.Ticks - start) / 1000000.0);
        }
    }
}
