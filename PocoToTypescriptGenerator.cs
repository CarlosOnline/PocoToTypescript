using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Pocoyo
{
    public static class PocoToTypescriptGenerator
    {
        private static string TempExtension => $".temp.{Process.GetCurrentProcess().Id}";

        public static Options Options { get; private set; }

        public static int Main(string[] args)
        {
            try
            {
                Console.WriteLine(nameof(PocoToTypescriptGenerator) + ".exe " + string.Join(" ", args));

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
                    var realOutputFilePath = outputFilePath.Replace(TempExtension, "");
                    if (!outputFiles.Contains(outputFilePath))
                    {
                        outputFiles.Add(outputFilePath);
                        Utility.TryDeleteFile(outputFilePath);
                    }

                    Pocoyo.Process(inputFile, outputFilePath, !Options.PreProcess);

                    if (!string.Equals(Options.OutputFilePath, realOutputFilePath))
                        Log.Info($"Generated: {realOutputFilePath}");
                }

                if (Options.TypescriptFiles != null)
                {
                    foreach (var inputFile in Options.TypescriptFiles)
                    {
                        if (File.Exists(inputFile))
                        {
                            var outputFilePath = GetTempOutputFilePath(inputFile);
                            if (!outputFiles.Contains(outputFilePath))
                            {
                                outputFiles.Add(outputFilePath);
                                Utility.TryDeleteFile(outputFilePath);
                            }

                            var contents = SharedFile.ReadAllText(inputFile);
                            SharedFile.AppendAllText(outputFilePath, contents);

                            Execute(outputFilePath, () =>
                            {
                                SharedFile.AppendAllText(outputFilePath, contents);
                            });
                        }
                    }
                }

                foreach (var outputFile in outputFiles)
                {
                    var realOutputFilePath = outputFile.Replace(TempExtension, "");

                    Execute(realOutputFilePath, () =>
                    {
                        Utility.TryMoveFile(outputFile, realOutputFilePath);
                        Utility.TryDeleteFile(outputFile);
                    });
                }

                // Log combined output file if any
                if (!string.IsNullOrEmpty(Options.OutputFilePath))
                {
                    Log.Info($"Generated {Options.OutputFilePath}");

                    var pocoyoDataFilePath = Path.ChangeExtension(Options.OutputFilePath, ".Pocoyo.json");

                    Execute(pocoyoDataFilePath, () =>
                    {
                        var data = new PocoyoData();
                        data.SaveToData(pocoyoDataFilePath);
                    });
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Main)} Exception: {ex}");
                return -2;
            }
        }

        /// <summary>
        /// Lock output operations for multi target case
        /// </summary>
        private static void Execute(string name, Action action)
        {
            try
            {
                var semaphore = new Semaphore(1, 1, name.Replace(":", ".").Replace("\\", "."));
                using (new SemaphoreLock(semaphore))
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"{name} {ex.Message}");
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

                return outputFilePath + TempExtension;
            }

            return Options.OutputFilePath + TempExtension;
        }


    }
}
