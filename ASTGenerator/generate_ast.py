import sys

targetDir = ''
if len(sys.argv) >= 1:
    targetDir = sys.argv[1]
    print(f"targetDir: {targetDir}")

def defineTypedAst(baseClass, classes):
    with open(f"{targetDir}{baseClass}.cs", 'w') as f:
        f.write("namespace LoxInterpreter\n{\n")
        
        f.write(f"    interface I{baseClass}Visitor<T>\n    {{\n")
        for className, members in classes:
            f.write(f"        T Visit{className}{baseClass}({className}{baseClass} {baseClass.lower()});\n")
        f.write("    }\n")

        f.write(f"    abstract class {baseClass}\n    {{\n")
        f.write(f"        public abstract T Accept<T>(I{baseClass}Visitor<T> visitor);\n")
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
            f.write(f"        public override T Accept<T>(I{baseClass}Visitor<T> visitor)\n        {{\n")
            f.write(f"            return visitor.Visit{className}{baseClass}(this);\n")
            f.write("        }\n")

            f.write("    }\n")
        f.write("}")

def defineAst(baseClass, classes, namespaces = []):
    with open(f"{targetDir}{baseClass}.cs", 'w') as f:
        for n in namespaces:
            f.write(f"using {n};\n")
        f.write("namespace LoxInterpreter\n{\n")
        
        f.write(f"    interface I{baseClass}Visitor\n    {{\n")
        for className, members in classes:
            f.write(f"        void Visit{className}{baseClass}({className}{baseClass} {baseClass.lower()});\n")
        f.write("    }\n")

        f.write(f"    abstract class {baseClass}\n    {{\n")
        f.write(f"        public abstract void Accept(I{baseClass}Visitor visitor);\n")
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
            f.write(f"        public override void Accept(I{baseClass}Visitor visitor)\n        {{\n")
            f.write(f"            visitor.Visit{className}{baseClass}(this);\n")
            f.write("        }\n")

            f.write("    }\n")
        f.write("}")

defineTypedAst('Expr', [
    ('Assign',
    [
        ('Token', 'Name'),
        ('Expr', 'Value')
    ]),
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
    ('Logical',
    [
        ('Expr', 'Left'),
        ('Token', 'Operator'),
        ('Expr', 'Right')
    ]),
    ('Unary',
    [
        ('Token', 'Operator'),
        ('Expr', 'Right')
    ]),
    ('Variable',
    [
        ('Token', 'Name')
    ])
])

defineAst('Stmt', [
    ('Block', [('List<Stmt>', 'Statements')]),
    ('Expression', [('Expr', 'Expression')]),
    ('If', [('Expr', 'Condition'), ('Stmt', 'ThenBranch'), ('Stmt', 'ElseBranch')]),
    ('Print', [('Expr', 'Expression')]),
    ('Var', [('Token', 'Name'), ('Expr', 'Initializer')])
], ['System.Collections.Generic'])

print("generated code successfully.")