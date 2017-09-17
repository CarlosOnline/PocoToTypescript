using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Pocoyo
{
    /// <summary>
    /// Generates Typescript generations from CodeAnalysis compulation
    /// </summary>
    public class PocoToTypescriptSpitter
    {
        public bool Silent { get; set; }
        public bool DiscoverTypes { get; set; }
        public string OutputFile { get; set; }
        public static string DefaultNamespace { get; set; }

        private int _indent = 0;
        private string Indent => " ".PadRight(_indent);

        private const string OpenBrace = "{";
        private const string CloseBrase = "}";

        private static List<string> Namespaces { get; } = new List<string>();
        private static string Namespace => string.Join(".", Namespaces);

        private static Dictionary<string, string> DiscoveredTypes { get; } = new Dictionary<string, string>();

        internal static string ContainsType(string fullType)
        {
            return DiscoveredTypes.ContainsKey(fullType) ? DiscoveredTypes[fullType] : null;
        }

        /// <summary>
        /// Pre-process compulation unit to output file
        /// </summary>
        public static bool PreProcess(string inputFile)
        {
            var textCode = Utility.ReadAllText(inputFile);
            if (string.IsNullOrEmpty(textCode))
            {
                Log.Error($"empty c# input file: {inputFile}");
                return false;
            }

            var tree = CSharpSyntaxTree.ParseText(textCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var info = new PocoToTypescriptSpitter
            {
                DiscoverTypes = true,
                Silent = true,
            };
            info.Process(root.Members);
            return true;
        }

        /// <summary>
        /// Process compulation unit to output file
        /// </summary>
        public static bool Process(string inputFile, string outputFile, bool discoverTypes)
        {
            var textCode = Utility.ReadAllText(inputFile);
            if (string.IsNullOrEmpty(textCode))
            {
                Log.Error($"empty c# input file: {inputFile}");
                return false;
            }

            var tree = CSharpSyntaxTree.ParseText(textCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var info = new PocoToTypescriptSpitter
            {
                DiscoverTypes = discoverTypes,
                OutputFile = outputFile,
            };
            info.Process(root.Members);
            return true;
        }

        public static void Process(CompilationUnitSyntax compilationUnit)
        {
            var info = new PocoToTypescriptSpitter();
            info.Process(compilationUnit.Members);
        }

        public void Process(MemberDeclarationSyntax memberItem)
        {
            switch (memberItem)
            {
                case NamespaceDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
                case EnumDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
                case ClassDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
                case InterfaceDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
                case StructDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
            }

            Log.Error($"Unhandled type: {memberItem.GetType().Name} {memberItem.Kind()}");
        }

        public void Process(IEnumerable<MemberDeclarationSyntax> members)
        {
            foreach (var memberItem in members)
            {
                Process(memberItem);
            }
        }

        private void Process(NamespaceDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.Members.Count == 0)
                return;

            Namespaces.Add(syntaxItem.Name.ToString());

            if (!string.IsNullOrEmpty(DefaultNamespace))
                AddLevel($@"declare module {DefaultNamespace}");
            else
                AddLevel($@"declare module {syntaxItem.Name}");

            Process(syntaxItem.Members);

            CloseLevel();
            Namespaces.RemoveAt(Namespaces.Count - 1);
        }

        private void Process(EnumDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.Members.Count == 0)
                return;

            AddDiscoveredType(syntaxItem);

            AddLevel($@"export {syntaxItem.EnumKeyword} {syntaxItem.Identifier}");

            for (var idx = 0; idx < syntaxItem.Members.Count - 1; idx++)
            {
                var memberItem = syntaxItem.Members[idx];
                AddLine($"{memberItem.Identifier} = {idx},");
            }

            // Add final item no trailing comma
            {
                var idx = syntaxItem.Members.Count - 1;
                var memberItem = syntaxItem.Members[idx];
                AddLine($"{memberItem.Identifier} = {idx}");
            }

            CloseLevel();
        }

        private string GetClassBase(BaseListSyntax syntaxItem)
        {
            if (syntaxItem == null)
                return "";

            var baseTypes = new List<string>();
            foreach (var syntaxItemType in syntaxItem.Types)
            {
                baseTypes.Add(syntaxItemType.Type.ToTypescript());
            }
            return " extends " + string.Join(",", baseTypes);
        }

        private void Process(TypeDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.Members.Count == 0 || syntaxItem.PropertyCount() == 0)
                return;

            AddDiscoveredType(syntaxItem);

            AddLevel($@"export interface {syntaxItem.Identifier}{syntaxItem.TypeParameterList}{GetClassBase(syntaxItem.BaseList)}");

            foreach (var memberItem in syntaxItem.Members)
            {
                switch (memberItem)
                {
                    case PropertyDeclarationSyntax propItem:
                        Process(propItem);
                        break;
                    case MethodDeclarationSyntax methodItem:
                    case ConstructorDeclarationSyntax constructorItem:
                        // skip methods etc
                        break;
                    default:
                        Log.Error($"unhandled type {memberItem.Kind()} {memberItem}");
                        break;
                }
            }

            CloseLevel();
        }

        private void Process(PropertyDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.IsPrivate() || syntaxItem.IsInternal())
                return;

            var publicOnly = syntaxItem.Parent.Kind() == SyntaxKind.ClassDeclaration;
            if (!publicOnly || syntaxItem.IsPublic())
                AddLine($"{syntaxItem.Identifier.ToTypescript()} : {syntaxItem.Type.ToTypescript()};");
        }

        private bool AddDiscoveredType(BaseTypeDeclarationSyntax syntaxItem)
        {
            if (!DiscoverTypes)
                return false;

            if (!string.IsNullOrEmpty(Namespace))
            {
                var fullType = $"{Namespace}.{syntaxItem.Identifier.Text}";
                if (DiscoveredTypes.ContainsKey(fullType))
                {
                    Log.Error($"Discovered Type already found: {fullType}");
                    return false;
                }

                // Use default namespace if specified
                if (!string.IsNullOrEmpty(DefaultNamespace))
                    DiscoveredTypes[fullType] = fullType.Replace(Namespace, DefaultNamespace);
                else
                    DiscoveredTypes[fullType] = fullType;

                return true;
            }
            return false;
        }

        private void IncreaseLevel()
        {
            _indent += 3;
            if (_indent <= 0)
                throw new Exception($"Invalid indent {_indent}");
        }

        private void DecreaseLevel()
        {
            _indent -= 3;
            if (_indent < 0)
                throw new Exception($"Invalid indent {_indent}");
        }

        private void AddLevel(string line)
        {
            if (Silent)
                return;

            var output = $"{Indent}{line} {OpenBrace}";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                Utility.AppendLine(OutputFile, output);
            IncreaseLevel();
        }

        private void AddLine(string line)
        {
            if (Silent)
                return;

            var output = $"{Indent}{line}";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                Utility.AppendLine(OutputFile, output);
        }

        private void CloseLevel(string line = "")
        {
            if (Silent)
                return;

            DecreaseLevel();
            var output = $@"{Indent}{line}{CloseBrase}
";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                Utility.AppendLine(OutputFile, output);
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Extension methods for CodeAnaylsis SyntaxItems
    /// </summary>
    public static class CodeAnalysisExtensions
    {
        public static string ToTypescript(this TypeSyntax type)
        {
            switch (type)
            {
                case PredefinedTypeSyntax syntaxItem:
                    switch (syntaxItem.Keyword.Kind())
                    {
                        case SyntaxKind.StringKeyword:
                        case SyntaxKind.CharKeyword:
                            return "string";

                        case SyntaxKind.BoolKeyword:
                            return "boolean";

                        case SyntaxKind.ByteKeyword:
                        case SyntaxKind.SByteKeyword:
                        case SyntaxKind.ShortKeyword:
                        case SyntaxKind.UShortKeyword:
                        case SyntaxKind.IntKeyword:
                        case SyntaxKind.UIntKeyword:
                        case SyntaxKind.LongKeyword:
                        case SyntaxKind.ULongKeyword:
                        case SyntaxKind.DoubleKeyword:
                        case SyntaxKind.FloatKeyword:
                        case SyntaxKind.DecimalKeyword:
                            return "number";

                        case SyntaxKind.ObjectKeyword:
                            return "any";

                        default:
                            Log.Error($"Unknown type: {syntaxItem}");
                            return "any";
                    }

                case NullableTypeSyntax syntaxItem:
                    return syntaxItem.ElementType.ToTypescript() + "?";

                case GenericNameSyntax syntaxItem:
                    return syntaxItem.ToTypescript();

                case IdentifierNameSyntax syntaxItem:
                    {
                        if (syntaxItem.Identifier.Text == "dynamic")
                            return "any";
                        
                        return syntaxItem.Identifier.Text;
                    }

                case ArrayTypeSyntax syntaxItem:
                    // TODO: All rank specifiers?
                    return syntaxItem.ElementType.ToTypescript() + syntaxItem.RankSpecifiers.ToString();

                case QualifiedNameSyntax syntaxItem:
                {
                    // check for Generic List
                    // TODO: Other known types?
                    if (syntaxItem.ToFullType().StartsWith("System.Collections.Generic.List<"))
                        return syntaxItem.Right.ToTypescript();

                    var fullType = PocoToTypescriptSpitter.ContainsType(syntaxItem.ToFullType());
                    if (!string.IsNullOrEmpty(fullType))
                        return fullType;

                    // Can't get full type so return any
                    return "any";
                }
            }

            Log.Error($"unknown type: {type}");
            return type.ToString();
        }

        public static string ToTypescript(this GenericNameSyntax syntax)
        {
            if (syntax.Identifier.Text == "List")
            {
                if (syntax.TypeArgumentList.Arguments.Count != 1)
                {
                    Log.Error($"to many generic type args: {string.Join(",", syntax.TypeArgumentList.Arguments)}");
                    return "any[]";
                }

                return syntax.TypeArgumentList.Arguments.First().ToTypescript() + "[]";
            }

            Log.Error($"unknown Generic: {syntax}");
            return "any";
        }

        public static string ToTypescript(this SyntaxToken token)
        {
            return token.ToString().ToCamelCaseString();
        }

        public static string ToCamelCaseString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return char.ToLower(value[0]) + value.Substring(1);
        }

        public static bool IsPublic(this PropertyDeclarationSyntax syntaxItem)
        {
            return syntaxItem.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword);
        }

        public static bool IsPrivate(this PropertyDeclarationSyntax syntaxItem)
        {
            return syntaxItem.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PrivateKeyword);
        }

        public static bool IsInternal(this PropertyDeclarationSyntax syntaxItem)
        {
            return syntaxItem.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.InternalKeyword);
        }

        public static int PropertyCount(this TypeDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.Members.Count == 0)
                return 0;

            var publicOnly = syntaxItem.Keyword.Kind() == SyntaxKind.ClassKeyword;

            var count = 0;
            foreach (var memberItem in syntaxItem.Members)
            {
                switch (memberItem)
                {
                    case PropertyDeclarationSyntax propItem:
                        if (!propItem.IsPrivate() && !propItem.IsInternal())
                        {
                            if (!publicOnly || propItem.IsPublic())
                                count++;
                        }
                        break;
                }
            }
            return count;
        }

        public static string ToFullType(this QualifiedNameSyntax syntaxItem)
        {
            return $"{syntaxItem.Left}.{syntaxItem.Right}";
        }
    }

}
