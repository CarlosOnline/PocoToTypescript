using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pocoyo
{
    public static class PocoToTypescriptGenerator
    {
        public static Options Options { get; private set; }

        public static int Main(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(Utility.AssemblyLocation);

                Options = Options.ParseArgs(args);
                if (Options == null)
                    return -1;

                Log.SilentMode = Options.Silent;
                Log.VerbosMode = Options.Verbose;
                Log.Verbose($"{Utility.AssemblyName} {string.Join(" ", args)}");

                foreach (var inputFile in Options.InputFiles)
                {
                    var textCode = Utility.ReadAllText(inputFile);
                    if (string.IsNullOrEmpty(textCode))
                    {
                        Log.Error($"empty c# input file: {inputFile}");
                        return -1;
                    }

                    string outputFilePath;
                    if (!string.IsNullOrEmpty(Options.OutputFile))
                    {
                        outputFilePath = Options.OutputFile;
                    }
                    else
                    {
                        outputFilePath = Path.Combine(Options.OutputFolder, Path.GetFileNameWithoutExtension(inputFile) + ".d.ts");
                        if (File.Exists(outputFilePath))
                            File.Delete(outputFilePath);
                    }

                    var tree = CSharpSyntaxTree.ParseText(textCode);
                    var root = (CompilationUnitSyntax)tree.GetRoot();
                    PocoToTypescriptSpitter.Process(root, outputFilePath);

                    if (!string.IsNullOrEmpty(Options.OutputFolder))
                        Log.Info($"Generated: {outputFilePath}");
                }

                if (!string.IsNullOrEmpty(Options.OutputFile))
                    Log.Info($"Generated {Options.OutputFile}");

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error($"Exception: {ex}");
                return -2;
            }
        }
    }
}
