namespace LoxInterpreter
{
    class LoxClass
    {
        public string Name { get; private set; }

        public LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("<class {0}>", Name);
        }
    }
}
