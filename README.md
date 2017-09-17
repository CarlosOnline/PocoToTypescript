# PocoToTypescript


PocoToTypescriptGenerator.exe [c# file(s) / folder(s)] ...  [options]:

Generates typescript definition files from files or folder containing c# files.

## Options:

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
  k, Known                 List of known types which are not found in c# files
                           (comma seperated). For example T as in MyClass<T>.
  f, ExcludedAttributes    List of Attributes that should be exclude (comma
                           seperated). For example JsonIgnore,NotMapped.
  c, commands              Read command line args in from specified file.
  p, SkipPreprocess        (Default: False) Skips pre-processing files for
                           types.
  v, verbose               (Default: False) Prints all messages.
  s, Silent                (Default: False) Turns off all console messages.
  help                     Display this help screen.


Produced by Carlos Gomes (cgomes@iinet.com)

_Examples:_
```
    PocoToTypescript.exe Sample.cs -o Sample.d.ts
    PocoToTypescript.exe Sample.cs SampleFolder --output=Combined.d.ts --verbose  --excluded=MyClass,MyEnum --excludedAttributes=JsonIgnore,NotMapped
```

# Notes

* All input files are pre-processed in order to discover all types.  This allows for tyescript files to reference the discovered types.  Uknown types are emitted as any.  This can be turned off with --skipPreprocess.

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
Workaround: use --exclude command line option to exclude these types.

### All declarations of must have identical type parameters
c# classes with similar bases can lead to dupliate typescript definitions.
Workaround: use --exclude command line option to exclude these types.

_Typescript example_
```
   export interface IData {
      primaryKey : number;
   }

   export interface IData<T> extends IData {
      primaryKeyPredicate : any;
   }
````

### Multiple dimensional arrays mapped to any
c# multi-dimensional arrays are not handled. Instead they are mapped to any.

```
array[,];
array[][];
```



