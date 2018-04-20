targetDir = '../LoxInterpreter/AST/'

baseClass = 'Expr'
classes = [
    ('Binary', 
    [ # members
        ('Expr', 'Left'),
        ('Token', 'Operator'),
        ('Expr', 'Right'),
    ]),
    ('Grouping',
    [
        ('Expr', 'Expression')
    ]),
    ('Literal',
    [
        ('object', 'Value')
    ]),
    ('Unary',
    [
        ('Token', 'Operator'),
        ('Expr', 'Right')
    ])
]

with open(f"{targetDir}{baseClass}.cs", 'w') as f:
    f.write("namespace LoxInterpreter\n{\n")
    
    f.write("    interface IVisitor<T>\n    {\n")
    for className, members in classes:
        f.write(f"        T Visit{className}{baseClass}({className}{baseClass} {baseClass.lower()});\n")
    f.write("    }\n")

    f.write(f"    abstract class {baseClass}\n    {{\n")
    f.write("        public abstract T Accept<T>(IVisitor<T> visitor);\n")
    f.write("    }\n")

    for className, members in classes:
        f.write(f"    class {className}{baseClass} : {baseClass}\n    {{\n")
        # members
        for varType, varName in members:
            f.write(f"        public {varType} {varName} {{ get; }}\n")
        # constructor
        params = ', '.join([t + ' ' + n for t, n in members])
        f.write(f"        public {className}{baseClass}({params})\n        {{\n")

        for varType, varName in members:
            f.write(f"            this.{varName} = {varName};\n")

        f.write("        }\n")

        # accept method
        f.write("        public override T Accept<T>(IVisitor<T> visitor)\n        {\n")
        f.write(f"            return visitor.Visit{className}{baseClass}(this);\n")
        f.write("        }\n")

        f.write("    }\n")
    f.write("}")
