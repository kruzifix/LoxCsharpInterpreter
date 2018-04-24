namespace LoxInterpreter
{
    class LoxInstance
    {
        private LoxClass klass;

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        public override string ToString()
        {
            return string.Format("<instance {0}>", klass.Name);
        }
    }
}
