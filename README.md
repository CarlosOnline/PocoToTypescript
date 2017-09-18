# PocoToTypescript


PocoToTypescriptGenerator.exe [c# file(s) / folder(s)] ...  [options]:

Generates typescript definition files from files or folder containing c# files.

## Options:
```
  [c# file(s) / folder(s)] (Required) c# source files or folder containing c# files
                           Specifies c# file(s) and/or c# folder(s) to convert to typescript definition files.
                           Seperate files/folders by spaces.

  o, output                (Required) - Output file or folder.  If folder, then
                           uses input file name for output file name within
                           output folder.

  n, namespace             Alternate namespace to use in typescript
                           definitions. Defaults c# file's namespace.

  e, Excluded              List of types that should be exclude (comma
                           seperated). For example MyClass,MyEnum.

  f, ExcludedAttributes    List of Attributes that should be exclude (comma
                           seperated). For example JsonIgnore,NotMapped.

  k, Known                 List of known types which are not found in c# files
                           (comma seperated). For example T as in MyClass<T>.

  c, commands              Read command line args in from specified file or
                           json file.

  p, SkipPreprocess        (Default: False) Skips pre-processing files for
                           types.

  v, verbose               (Default: False) Prints all messages.

  s, Silent                (Default: False) Turns off all console messages.

  help                     Display this help screen.
```

Produced by Carlos Gomes (cgomes@iinet.com)

_Examples:_
```
    PocoToTypescriptGenerator Sample.cs -o Sample.d.ts
    PocoToTypescriptGenerator Sample.cs SampleFolder --output=Combined.d.ts --verbose  --excluded=MyClass,MyEnum --excludedAttributes=JsonIgnore,NotMapped
    PocoToTypescriptGenerator Sample.cs SampleFolder --output=Combined.d.ts --Known=T
    PocoToTypescriptGenerator --commands=command_line_args_file.txt
    PocoToTypescriptGenerator --commands=options.json
```
