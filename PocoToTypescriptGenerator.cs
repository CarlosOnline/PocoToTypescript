using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                Log.VerbosMode = Options.Verbose  || Debugger.IsAttached;
                Pocoyo.DefaultNamespace = Options.Namespace;
                if (Options.Excluded != null)
                    Pocoyo.Excluded = Options.Excluded.ToList();
                if (Options.ExcludedAttributes != null)
                    Pocoyo.ExcludedAttributes = Options.ExcludedAttributes.ToList();
                if (Options.KnownTypes != null)
                    Pocoyo.KnownTypes = Options.KnownTypes.ToList();

                Log.Verbose($"{Utility.AssemblyName} {string.Join(" ", args)}");

                // Preprocess all files
                if (Options.PreProcess)
                {
                    foreach (var inputFile in Options.InputFiles)
                    {
                        Pocoyo.PreProcess(inputFile);
                    }
                }

                // Generate typescript definition files

                foreach (var inputFile in Options.InputFiles)
                {
                    var outputFilePath = GetOutputFilePath(inputFile);

                    Pocoyo.Process(inputFile, outputFilePath, !Options.PreProcess);

                    if (!string.Equals(Options.OutputFile, outputFilePath))
                        Log.Info($"Generated: {outputFilePath}");
                }

                // Log combined output file if any
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

        /// <summary>
        /// Returns OutputFile if specified or output file from OutputFolder based on input file
        /// </summary>
        private static string GetOutputFilePath(string inputFile)
        {
            if (!string.IsNullOrEmpty(Options.OutputFile))
                return Options.OutputFile;

            var outputFilePath = Path.Combine(Options.OutputFolder, Path.GetFileNameWithoutExtension(inputFile) + ".d.ts");
            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            return outputFilePath;
        }

    }
}
