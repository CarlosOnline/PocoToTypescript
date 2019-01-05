using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;

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

        /// <summary>
        /// Additional Typescript Files to include
        /// </summary>
        [Option('t', "AdditionalFiles", Required = false, HelpText = "Additional typescript files to include")]
        public string AdditionalFiles { get; set; }
        public string[] TypescriptFiles => AdditionalFiles?.Split(',').Select(item => Path.GetFullPath(item.Trim())).ToArray();

        [Option('o', "output", Required = false, HelpText = "(Required) - Output file or folder.  If folder, then uses input file name for output file name within output folder.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Namespace to use in typescript definitions
        /// </summary>
        [Option('n', "namespace", Required = false, HelpText = "Alternate namespace to use in typescript definitions. Defaults c# file's namespace.")]
        public string Namespace { get; set; }

        [OptionList('e', "Excluded", Separator = ',', HelpText = "List of types that should be exclude (comma seperated). For example MyClass,MyEnum.")]
        public IList<string> Excluded { get; set; }

        [OptionList('f', "ExcludedAttributes", Separator = ',', HelpText = "List of Attributes that should be exclude (comma seperated). For example JsonIgnore,NotMapped.")]
        public IList<string> ExcludedAttributes { get; set; }

        [OptionList('k', "Known", Separator = ',', HelpText = "List of known types which are not found in c# files (comma seperated). For example T as in MyClass<T>.")]
        public IList<string> KnownTypes { get; set; }

        [Option('c', "commands", Required = false, HelpText = "Read command line args in from specified file.")]
        public string CommandFile { get; set; }

        [Option('d', "PocoyoDataFilePath", Required = false, HelpText = "Pocoyo Data file path")]
        public string PocoyoDataFilePath { get; set; }

        [Option('p', "SkipPreprocess", DefaultValue = false, HelpText = "Skips pre-processing files for types.")]
        public bool SkipPreprocess { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints all messages.")]
        public bool Verbose { get; set; }

        [Option('s', "Silent", DefaultValue = false, HelpText = "Turns off all console messages.")]
        public bool Silent { get; set; }

        [JsonIgnore]
        public List<string> InputFiles { get; set; } = new List<string>();

        [JsonIgnore]
        public string OutputFolder { get; set; }

        [JsonIgnore]
        public string OutputFilePath => string.IsNullOrEmpty(OutputFolder) ? OutputFile : null;

        [JsonIgnore]
        public bool PreProcess => !SkipPreprocess;

        [ParserState]
        [JsonIgnore]
        public IParserState LastParserState { get; set; }

        private void LogError(string errorMessage)
        {
            _errorMessage += errorMessage + "\r\n";
        }

        [HelpOption]
        public string GetUsage()
        {
            var helpText = new HelpText
            {
                Heading = $@"{Utility.AssemblyName} [c# file(s) / folder(s)] ...  [options]:

Generates typescript definition files from files or folder containing c# files.

Options:

  [c# file(s) / folder(s)] (Required) c# source files or folder containing c# files
                           Specifies c# file(s) and/or c# folder(s) to convert to typescript definition files.
                           Separate files/folders by spaces.",
            };
            helpText.AddOptions(this);

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

            return $@"
{helpText}

Produced by Carlos Gomes (cgomes@iinet.com)

Examples:
    {Utility.AssemblyName} Sample.cs -o Sample.d.ts
    {Utility.AssemblyName} Sample.cs SampleFolder --output=Combined.d.ts --verbose --excluded=MyClass,MyEnum --excludedAttributes=JsonIgnore,NotMapped
    {Utility.AssemblyName} Sample.cs SampleFolder --output=Combined.d.ts --Known=T
    {Utility.AssemblyName} --commands=command_line_args_file.txt
{errorMessage}
";
        }

        private bool ProcessArgs(bool ignoreCommandFile = false)
        {
            if (!string.IsNullOrEmpty(PocoyoDataFilePath))
            {
                if (!File.Exists(PocoyoDataFilePath))
                {
                    LogError($"Missing file: {PocoyoDataFilePath}");
                    return false;
                }

                PocoyoData.LoadFromData(PocoyoDataFilePath);
            }

            if (!ignoreCommandFile)
            {
                if (!string.IsNullOrEmpty(CommandFile))
                {
                    if (!File.Exists(CommandFile))
                    {
                        LogError($"Missing --commands file: {CommandFile}");
                        return false;
                    }

                    return false;
                }
            }

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

            if (string.IsNullOrEmpty(OutputFile))
            {
                LogError($"Missing --outputFile");
                return false;
            }

            var outputFilePath = Path.GetFullPath(OutputFile);
            var outputFolder = Path.GetDirectoryName(outputFilePath);
            Directory.CreateDirectory(outputFolder);

            if (Directory.Exists(outputFilePath))
            {
                OutputFolder = outputFilePath;
            }
            else
            {
                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }
                OutputFile = outputFilePath;
            }

            if (InputFiles.Count == 0)
            {
                LogError($"Missing input files / folders");
                return false;
            }

            if (string.IsNullOrEmpty(CommandFile) || !CommandFile.ToLower().EndsWith(".json"))
                SerializeToJson();

            return true;
        }

        private void SerializeToJson()
        {
            try
            {
                var options = (Options)this.MemberwiseClone();
                var outputFolder = !string.IsNullOrEmpty(OutputFolder) ? OutputFolder : Path.GetDirectoryName(OutputFilePath);
                var outputFileName = !string.IsNullOrEmpty(OutputFolder) ? "pocoyo.json" : Path.GetFileNameWithoutExtension(OutputFilePath) + ".json";

#if !SkipMakeRelative
                var relativePath = Environment.CurrentDirectory;
                options.OutputFile = Utility.MakeRelativePath(options.OutputFile, relativePath);
                options.CommandFile = Utility.MakeRelativePath(options.CommandFile, relativePath);

                var paths = new List<string>();
                foreach (var filePath in options.Files)
                {
                    paths.Add(Utility.MakeRelativePath(filePath, relativePath));
                }
                options.Files = paths;
#endif

                var optionsJsonPath = Path.Combine(outputFolder, outputFileName);
                if (!string.Equals(CommandFile, optionsJsonPath, StringComparison.OrdinalIgnoreCase))
                {
                    Json.SerializeToFile(options, optionsJsonPath);
                    if (!options.Silent)
                        Log.Info($"Generated --commands file: {optionsJsonPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Exception($"{nameof(SerializeToJson)}", ex);
            }
        }

        /// <summary>
        /// Gets options from command line file - text or json
        /// </summary>
        public Options GetOptionsFromCommandFile()
        {
            if (string.IsNullOrEmpty(CommandFile))
                return null;

            CommandFile = Path.GetFullPath(CommandFile);
            if (!File.Exists(CommandFile))
            {
                LogError($"Missing --commands file: {CommandFile}");
                return null;
            }

            Options options = null;

            if (Path.GetExtension(CommandFile) == ".json")
            {
                // Get from json
                options = Json.TryDeserializeFromFile<Options>(CommandFile);
            }
            else
            {
                // Re-parse with command file
                var commandArgs = GetCommandFileArgs(CommandFile);

                options = new Options();
                if (!Parser.Default.ParseArguments(commandArgs, options))
                    options = null;
            }

            if (options != null)
            {
                if (string.IsNullOrEmpty(options.CommandFile))
                    options.CommandFile = CommandFile;

                if (Verbose)
                    options.Verbose = true;

                if (Silent)
                    options.Silent = true;

                if (options.ProcessArgs(true))
                    return options;
            }

            return null;
        }

        public static Options ParseArgs(string[] args)
        {
            try
            {
                var options = new Options();
                if (Parser.Default.ParseArguments(args, options))
                {
                    if (options.ProcessArgs())
                        return options;

                    // Try options from command line file
                    var commandLineOptions = options.GetOptionsFromCommandFile();
                    if (commandLineOptions != null)
                        return commandLineOptions;

                    Log.Error(options.GetUsage());
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Parse error {ex.Message}");
            }
            return null;
        }

        private static string[] GetCommandFileArgs(string commandFile)
        {
            var commandArgs = File.ReadAllLines(commandFile);
            var trimmed = new List<string>();
            foreach (var arg in commandArgs)
            {
                trimmed.Add(arg.Trim());
            }
            return trimmed.ToArray();
        }

        public override string ToString()
        {
            return Json.Serialize(this);
        }
    }
}
