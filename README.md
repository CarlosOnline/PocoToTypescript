# PocoToTypescript

Generates typescript definition files from c# files or a folder containing c# files.

**InputFileOrFolder**:
    Either an input c# file or folder containing c# files to generate typescript definition files.
    Multiple files can be specified by adding to command line, for example Sample.cs Foo.cs Bar.cs

## Options:

  -o, --output     Output file or folder.  If folder, then uses input file name for output file name within output folder.

  -v, --verbose    (Default: False) Prints all messages

  -s, --Silent     (Default: False) Turns off all console messages

  --help           Display this help screen.

Produced by Carlos Gomes (cgomes@iinet.com)

_Examples:_
```
    PocoToTypescript.exe Sample.cs -o Sample.d.ts
    PocoToTypescript.exe Sample.cs SampleFolder --output=Combined.d.ts --verbose
```

# Exceptions:

### Qualified Names converted to any
Qualified types that are not found in the c# files with be given the **any** typescript tye.

##### For example: 

| c#            | Typescript    |
| ------------- |:-------------:|
| `UnknownNameSpace.FirsClass_ myProp {get; set;}`      | `myProp: any;` |
| `KnownNameSpace.FirsClass_ myProp {get; set;}`      | `myProp: KnownNameSpace.FirstClass` |
| `System.Collections.Generic.List<bool> listProp { get; set; }` | `listProp: bool[]`      |
| `List<bool> boolProp { get; set; }` | `listProp: bool[]`      |

### Nested classes / structs / enums not handled
Nested classes, enums, structs are not handled.  Feel free to add and share the code please.


