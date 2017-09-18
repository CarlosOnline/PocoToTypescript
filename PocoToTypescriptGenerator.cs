using System;
using System.Collections.Generic;
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
                Options = Options.ParseArgs(args);
                if (Options == null)
                    return -1;

                Log.SilentMode = Options.Silent;
                Log.VerbosMode = Options.Verbose; //  || Debugger.IsAttached;
                if (Log.VerbosMode)
                    Log.Info($"Options: {Options}");

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
                var outputFiles = new List<string>();

                foreach (var inputFile in Options.InputFiles)
                {
                    var outputFilePath = GetTempOutputFilePath(inputFile);
                    var realOutputFilePath = outputFilePath.Replace(".temp", "");
                    if (!outputFiles.Contains(outputFilePath))
                        outputFiles.Add(outputFilePath);

                    Pocoyo.Process(inputFile, outputFilePath, !Options.PreProcess);

                    if (!string.Equals(Options.OutputFilePath, realOutputFilePath))
                        Log.Info($"Generated: {realOutputFilePath}");
                }

                foreach (var outputFile in outputFiles)
                {
                    var realOutputFilePath = outputFile.Replace(".temp", "");
                    File.Move(outputFile, realOutputFilePath);
                }

                // Log combined output file if any
                if (!string.IsNullOrEmpty(Options.OutputFilePath))
                    Log.Info($"Generated {Options.OutputFilePath}");

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
        private static string GetTempOutputFilePath(string inputFile)
        {
            if (!string.IsNullOrEmpty(Options.OutputFolder))
            {
                var outputFilePath = Path.Combine(Options.OutputFolder, Path.GetFileNameWithoutExtension(inputFile) + ".d.ts");
                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);

                return outputFilePath + ".temp";
            }

            return Options.OutputFilePath + ".temp";
        }

    }
}
