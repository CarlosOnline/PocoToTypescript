PocoToTypescript

Generates typescript definition files from files or folder containing c# files.

InputFileOrFolder:
    Either an input c# file or folder containing c# files to generate typescript definition files.
    Multiple files can be specified by adding to command line, for example Sample.cs Foo.cs Bar.cs

options:

  -o, --output     Output file or folder.  If folder, then uses input file name for output file name within output folder.

  -v, --verbose    (Default: False) Prints all messages

  -s, --Silent     (Default: False) Turns off all console messages

  --help           Display this help screen.

Produced by Carlos Gomes (cgomes@iinet.com)

Examples:
    PocoToTypescript.exe Sample.cs -o Sample.d.ts
    PocoToTypescript.exe Sample.cs SampleFolder --output=Combined.d.ts --verbose
