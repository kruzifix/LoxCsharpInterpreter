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
    f.write(f"\tabstract class {baseClass}\n\t{{\n")
    f.write("\t}\n")

    for className, members in classes:
        f.write(f"\tclass {className}{baseClass} : {baseClass}\n\t{{\n")
        
        for varType, varName in members:
            f.write(f"\t\tpublic {varType} {varName} {{ get; }}\n")

        params = ', '.join([t + ' ' + n for t, n in members])
        f.write(f"\t\tpublic {className}{baseClass}({params})\n\t\t{{\n")

        for varType, varName in members:
            f.write(f"\t\t\tthis.{varName} = {varName};\n")

        f.write("\t\t}\n")

        f.write("\t}\n")
    f.write("\n}")
