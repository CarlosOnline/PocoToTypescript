﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Pocoyo
{
    public class Options
    {
        private string _errorMessage;

        /// <summary>
        /// Files or Folders to parse
        /// </summary>
        [ValueList(typeof(List<string>))]
        public IList<string> Files { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file or folder, if folder file name matches input file")]
        public string OutputFile { get; set; }
        public string OutputFolder { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints all messages")]
        public bool Verbose { get; set; }

        [Option('s', "Silent", DefaultValue = false, HelpText = "Turns off all console messages")]
        public bool Silent { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        private void LogError(string errorMessage)
        {
            _errorMessage += errorMessage + "\r\n";
        }

        [HelpOption]
        public string GetUsage()
        {
            var parseErrors = new HelpText().RenderParsingErrorsText(this, 3);
            var errorMessage = string.IsNullOrEmpty(parseErrors) && string.IsNullOrEmpty(_errorMessage) ? "" : $@"

========================================================
========================================================
========================================================
        PARSE ERRRORS

{parseErrors}{_errorMessage}
========================================================
========================================================
";
            return $@"{Utility.AssemblyName} [InputFileOrFolder] ...  [options below]:

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
    {Utility.AssemblyName} Sample.cs -o Sample.d.ts
    {Utility.AssemblyName} Sample.cs SampleFolder --output=Combined.d.ts --verbose
{errorMessage}
";
        }

        public List<string> InputFiles { get; } = new List<string>();

        private bool ProcesArgs()
        {
            if (Files.Count == 0)
            {
                LogError($"Missing input files / folders");
                return false;
            }

            foreach (var filePath in Files)
            {
                var inputFile = Path.GetFullPath(filePath);
                if (Directory.Exists(inputFile))
                {
                    foreach (var csharpFilePath in Directory.EnumerateFiles(inputFile, "*.cs", SearchOption.AllDirectories))
                    {
                        var found = InputFiles.FirstOrDefault(item => string.Equals(item, csharpFilePath));
                        if (found == null)
                            InputFiles.Add(csharpFilePath);
                    }
                }
                else if (File.Exists(inputFile))
                {
                    var found = InputFiles.FirstOrDefault(item => string.Equals(item, inputFile));
                    if (found == null)
                        InputFiles.Add(inputFile);
                }
                else
                {
                    LogError($"Invalid file: {inputFile}");
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(OutputFile))
            {
                var outputFilePath = Path.GetFullPath(OutputFile);
                var outputFolder = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputFolder);

                if (Directory.Exists(outputFilePath))
                {
                    OutputFolder = outputFilePath;
                    OutputFile = null;
                }
                else
                {
                    if (File.Exists(outputFilePath))
                    {
                        File.Delete(outputFilePath);
                    }
                    OutputFile = outputFilePath;
                }
            }

            if (InputFiles.Count == 0)
            {
                LogError($"Missing input files / folders");
                return false;
            }

            return InputFiles.Count > 0;
        }

        public static Options ParseArgs(string[] args)
        {
            try
            {
                var options = new Options();
                if (Parser.Default.ParseArguments(args, options))
                {
                    if (options.ProcesArgs())
                        return options;
                    Log.Error(options.GetUsage());
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Parse error {ex.Message}");
            }
            return null;
        }
    }
}