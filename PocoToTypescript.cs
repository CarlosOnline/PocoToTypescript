using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Pocoyo
    {
        public bool Silent { get; set; }
        public bool PreprocessMode { get; set; }
        public bool DiscoverTypes { get; set; }
        public string OutputFile { get; set; }
        public static string DefaultNamespace { get; set; }
        public static List<string> Excluded { get; set; } = new List<string>();
        public static List<string> ExcludedAttributes { get; set; } = new List<string>();
        public static List<string> KnownTypes { get; set; } = new List<string>();

        private int _indent = 0;
        private string Indent => _indent > 0 ? " ".PadRight(_indent) : "";

        private const string OpenBrace = "{";
        private const string CloseBrase = "}";

        private static List<string> Namespaces { get; } = new List<string>();
        private static string Namespace => string.Join(".", Namespaces);

        private static Dictionary<string, string> DiscoveredTypes { get; } = new Dictionary<string, string>();
        private static Dictionary<string, string> ExcludedTypes { get; } = new Dictionary<string, string>();

        internal static bool IsKnownType(string fullType)
        {
            return DiscoveredTypes.ContainsKey(fullType) || KnownTypes.Contains(fullType);
        }

        internal static bool IsSpecifiedKnownType(string fullType)
        {
            return KnownTypes.Contains(fullType);
        }

        internal static string MapKnownType(string fullType)
        {
            return DiscoveredTypes.ContainsKey(fullType) ? DiscoveredTypes[fullType] : null;
        }

        internal static bool IsExcludedType(string fullType)
        {
            return Excluded.Contains(fullType) || ExcludedTypes.ContainsKey(fullType);
        }

        internal static string MapExcludedType(string fullType)
        {
            if (Excluded.Contains(fullType))
                return fullType;

            return ExcludedTypes.ContainsKey(fullType) ? ExcludedTypes[fullType] : null;
        }

        /// <summary>
        /// Pre-process compulation unit to output file
        /// </summary>
        public static bool PreProcess(string inputFile)
        {
            var textCode = SharedFile.ReadAllText(inputFile);
            if (string.IsNullOrEmpty(textCode))
            {
                Log.Error($"empty c# input file: {inputFile}");
                return false;
            }

            var tree = CSharpSyntaxTree.ParseText(textCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var info = new Pocoyo
            {
                PreprocessMode = true,
                DiscoverTypes = true,
                Silent = !Debugger.IsAttached,
            };
            info.Process(root.Members);
            return true;
        }

        /// <summary>
        /// Process compulation unit to output file
        /// </summary>
        public static bool Process(string inputFile, string outputFile, bool discoverTypes)
        {
            var textCode = SharedFile.ReadAllText(inputFile);
            if (string.IsNullOrEmpty(textCode))
            {
                Log.Error($"empty c# input file: {inputFile}");
                return false;
            }

            var tree = CSharpSyntaxTree.ParseText(textCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var info = new Pocoyo
            {
                DiscoverTypes = discoverTypes,
                OutputFile = outputFile,
            };

            if (Log.VerbosMode)
            {
                info.AddLine($@"/*
==========================================
{inputFile}
==========================================
*/
");
            }

            info.Process(root.Members);

            if (Log.VerbosMode)
            {
                info.AddLine($@"/*
==========================================
==========================================
*/
");
            }
            return true;
        }

        public static void Process(CompilationUnitSyntax compilationUnit)
        {
            var info = new Pocoyo();
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
                case DelegateDeclarationSyntax syntaxItem:
                    Process(syntaxItem);
                    return;
            }

            Log.Warn($"Unhandled type: {memberItem.GetType().Name} {memberItem.Kind()}          => {memberItem}");
        }

        public void Process(DelegateDeclarationSyntax members)
        {
            // Nothing to do for delegates
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

            if (syntaxItem.IsExluded())
            {
                AddExcludedType(syntaxItem);
                return;
            }

            AddDiscoveredType(syntaxItem);

            if (PreprocessMode)
                return;

            AddLevel($@"export {syntaxItem.EnumKeyword} {syntaxItem.Identifier}");

            for (var idx = 0; idx < syntaxItem.Members.Count - 1; idx++)
            {
                var memberItem = syntaxItem.Members[idx];
                var value = memberItem.EqualsValue?.Value != null ? " = " + memberItem.EqualsValue.Value : "";
                AddLine($"{memberItem.Identifier}{value},");
            }

            // Add final item no trailing comma
            {
                var idx = syntaxItem.Members.Count - 1;
                var memberItem = syntaxItem.Members[idx];
                var value = memberItem.EqualsValue?.Value != null ? " = " + memberItem.EqualsValue.Value : "";
                AddLine($"{memberItem.Identifier}{value}");
            }

            CloseLevel();
        }

        private string ToExtends(BaseListSyntax syntaxItem)
        {
            if (syntaxItem == null)
                return "";

            var baseTypes = new List<string>();
            foreach (var syntaxItemType in syntaxItem.Types)
            {
                switch (syntaxItemType.Type)
                {
                    case IdentifierNameSyntax typeItem:
                        if (typeItem.IsKnownType())
                            baseTypes.Add(syntaxItemType.Type.ToTypescript());
                        else if (!typeItem.IsExcluded())
                            Log.Warn($"Uknown base type: {syntaxItemType.Type} for {syntaxItemType}");
                        break;
                    case GenericNameSyntax typeItem:
                        {
                            var baseType = typeItem.ToTypescript();
                            if (baseType != "any")
                                baseTypes.Add(syntaxItemType.Type.ToTypescript());
                            else if (!typeItem.IsExcluded())
                                Log.Warn($"Uknown base type: {syntaxItemType.Type}");
                        }
                        break;

                    default:
                        Log.Warn($"Uknown base type: {syntaxItemType.Type} for {syntaxItemType}");
                        break;
                }
            }

            // Dont add extends any
            if (baseTypes.Count > 0)
            {
                var extension = string.Join(",", baseTypes);
                if (extension != "any")
                    return " extends " + string.Join(",", baseTypes);
            }

            return "";
        }

        private void Process(TypeDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null || syntaxItem.Members.Count == 0)
                return;

            if (syntaxItem.IsExluded())
            {
                AddExcludedType(syntaxItem);
                return;
            }

            if (syntaxItem.PropertyCount() == 0)
                return;

            AddDiscoveredType(syntaxItem);

            if (!PreprocessMode)
                AddLevel($@"export interface {syntaxItem.Identifier}{syntaxItem.TypeParameterList}{ToExtends(syntaxItem.BaseList)}");

            foreach (var memberItem in syntaxItem.Members)
            {
                switch (memberItem)
                {
                    case PropertyDeclarationSyntax declItem:
                        Process(declItem);
                        break;
                    case ClassDeclarationSyntax declItem:
                        Process(declItem);
                        break;
                    case FieldDeclarationSyntax fieldItem:
                    case MethodDeclarationSyntax methodItem:
                    case ConstructorDeclarationSyntax constructorItem:
                        // skip methods etc
                        break;
                    default:
                        Log.Warn($"unhandled type {memberItem.Kind()} {memberItem}");
                        break;
                }
            }

            CloseLevel();
        }

        private void Process(PropertyDeclarationSyntax syntaxItem)
        {
            if (PreprocessMode)
                return;

            if (syntaxItem == null || syntaxItem.IsPrivate() || syntaxItem.IsInternal() || syntaxItem.IsExluded())
                return;

            var publicOnly = syntaxItem.Parent.Kind() == SyntaxKind.ClassDeclaration;
            if (!publicOnly || syntaxItem.IsPublic())
            {
                var nullable = syntaxItem.Type.IsNullable() ? "?" : "";
                AddLine($"{syntaxItem.Identifier.ToTypescript()}{nullable} : {syntaxItem.Type.ToTypescript()};");
            }
        }

        private bool AddDiscoveredType(TypeDeclarationSyntax syntaxItem)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(Namespace))
                return false;

            var fullType = $"{Namespace}.{syntaxItem.Identifier.Text}{syntaxItem.TypeParameterList}";
            return AddDiscoveredType(syntaxItem, fullType);
        }

        private bool AddDiscoveredType(BaseTypeDeclarationSyntax syntaxItem)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(Namespace))
                return false;

            var fullType = $"{Namespace}.{syntaxItem.Identifier.Text}{syntaxItem.Identifier}";
            return AddDiscoveredType(syntaxItem, fullType);
        }

        private bool AddDiscoveredType(BaseTypeDeclarationSyntax syntaxItem, string fullType)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(fullType) || string.IsNullOrEmpty(Namespace))
                return false;

            //if (fullType.Contains("BaseDataObject"))
            //    Log.Info("found");

            // Add Identifier
            var identifierFullType = $"{Namespace}.{syntaxItem.Identifier.Text}";
            if (fullType != identifierFullType)
            {
                if (!DiscoveredTypes.ContainsKey(identifierFullType))
                {
                    DiscoveredTypes[identifierFullType] = identifierFullType;
                }
            }

            // Add Unqualified Identifier
            if (fullType != syntaxItem.Identifier.Text)
            {
                if (!DiscoveredTypes.ContainsKey(syntaxItem.Identifier.Text))
                {
                    DiscoveredTypes[syntaxItem.Identifier.Text] = syntaxItem.Identifier.Text;
                }
            }

            if (DiscoveredTypes.ContainsKey(fullType))
            {
                Log.Warn($"Discovered Type already found: {fullType} {syntaxItem}");
                return false;
            }

            // Use default namespace if specified
            if (!string.IsNullOrEmpty(DefaultNamespace))
                DiscoveredTypes[fullType] = fullType.Replace(Namespace, DefaultNamespace);
            else
                DiscoveredTypes[fullType] = fullType;

            return true;
        }

        private bool AddExcludedType(TypeDeclarationSyntax syntaxItem)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(Namespace))
                return false;

            var fullType = $"{Namespace}.{syntaxItem.Identifier.Text}{syntaxItem.TypeParameterList}";
            return AddExcludedType(syntaxItem, fullType);
        }

        private bool AddExcludedType(BaseTypeDeclarationSyntax syntaxItem)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(Namespace))
                return false;

            var fullType = $"{Namespace}.{syntaxItem.Identifier.Text}{syntaxItem.Identifier}";
            return AddExcludedType(syntaxItem, fullType);
        }

        private bool AddExcludedType(BaseTypeDeclarationSyntax syntaxItem, string fullType)
        {
            if (!DiscoverTypes || string.IsNullOrEmpty(fullType) || string.IsNullOrEmpty(Namespace))
                return false;

            //if (fullType.Contains("BaseDataObject"))
            //    Log.Info("found");

            // Add Identifier
            var identifierFullType = $"{Namespace}.{syntaxItem.Identifier.Text}";
            if (fullType != identifierFullType)
            {
                if (!ExcludedTypes.ContainsKey(identifierFullType))
                {
                    ExcludedTypes[identifierFullType] = identifierFullType;
                }
            }

            // Add Unqualified Identifier
            if (fullType != syntaxItem.Identifier.Text)
            {
                if (!ExcludedTypes.ContainsKey(syntaxItem.Identifier.Text))
                {
                    ExcludedTypes[syntaxItem.Identifier.Text] = syntaxItem.Identifier.Text;
                }
            }

            if (ExcludedTypes.ContainsKey(fullType))
            {
                Log.Warn($"Excluded Type already found: {fullType} {syntaxItem}");
                return false;
            }

            // Use default namespace if specified
            if (!string.IsNullOrEmpty(DefaultNamespace))
                ExcludedTypes[fullType] = fullType.Replace(Namespace, DefaultNamespace);
            else
                ExcludedTypes[fullType] = fullType;

            return true;
        }

        private void IncreaseLevel()
        {
            if (Silent || PreprocessMode)
                return;

            _indent += 3;
            if (_indent <= 0)
                throw new Exception($"Invalid indent {_indent}");
        }

        private void DecreaseLevel()
        {
            if (Silent || PreprocessMode)
                return;

            _indent -= 3;
            if (_indent < 0)
                throw new Exception($"Invalid indent {_indent}");
        }

        private void AddLevel(string line)
        {
            if (Silent || PreprocessMode)
                return;

            var output = $"{Indent}{line} {OpenBrace}";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                SharedFile.AppendLine(OutputFile, output);
            IncreaseLevel();
        }

        private void AddLine(string line)
        {
            if (Silent || PreprocessMode)
                return;

            var output = $"{Indent}{line}";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                SharedFile.AppendLine(OutputFile, output);
        }

        private void CloseLevel(string line = "")
        {
            if (Silent || PreprocessMode)
                return;

            DecreaseLevel();
            var output = $@"{Indent}{line}{CloseBrase}
";
            Log.Verbose(output);
            if (!string.IsNullOrEmpty(OutputFile))
                SharedFile.AppendLine(OutputFile, output);
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
        public static string ToTypescript(this PredefinedTypeSyntax syntaxItem)
        {
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
                    Log.Warn($"Unknown type: {syntaxItem}");
                    return "any";
            }
        }

        public static string ToTypescript(this IdentifierNameSyntax syntaxItem)
        {
            if (syntaxItem.Identifier.Text == "dynamic")
                return "any";

            if (string.Equals(syntaxItem.Identifier.Text, "datetime", StringComparison.OrdinalIgnoreCase))
                return "string";

            if (string.Equals(syntaxItem.Identifier.Text, "timespan", StringComparison.OrdinalIgnoreCase))
                return "string";

            if (syntaxItem.IsSpecifiedKnownType())
                return "any";

            if (syntaxItem.IsKnownType())
                return syntaxItem.Identifier.Text;

            // Don't warn for common generic <T> argument
            if (syntaxItem.Identifier.Text == "T")
                return "T";

            Log.Warn($"Uknown identifier {syntaxItem}");
            return "any";
        }

        public static string ToTypescript(this QualifiedNameSyntax syntaxItem)
        {
            // check for Generic List
            // TODO: Other known types?
            if (syntaxItem.ToFullType().StartsWith("System.Collections.Generic.List<"))
                return syntaxItem.Right.ToTypescript();

            var fullType = Pocoyo.MapKnownType(syntaxItem.ToFullType());
            if (!string.IsNullOrEmpty(fullType))
                return fullType;

            // Can't get full type so return any
            return "any";
        }

        public static string ToTypescript(this TypeSyntax type)
        {
            switch (type)
            {
                case PredefinedTypeSyntax syntaxItem:
                    return syntaxItem.ToTypescript();

                case NullableTypeSyntax syntaxItem:
                    return syntaxItem.ElementType.ToTypescript();

                case GenericNameSyntax syntaxItem:
                    return syntaxItem.ToTypescript();

                case IdentifierNameSyntax syntaxItem:
                    return syntaxItem.ToTypescript();

                case ArrayTypeSyntax syntaxItem:
                    if (syntaxItem.RankSpecifiers.IsMultiRankArray())
                    {
                        Log.Info($"WARNING: To many ranks: {syntaxItem} returning any");
                        return "any";
                    }
                    return syntaxItem.ElementType.ToTypescript() + syntaxItem.RankSpecifiers.ToTypescript();

                case QualifiedNameSyntax syntaxItem:
                    return syntaxItem.ToTypescript();
            }

            Log.Warn($"unknown type: {type}");
            return type.ToString();
        }

        /// <summary>
        /// TODO: Figure out how to map multip rank c# to typescript.
        /// For example:
        ///     c# array[,] or array[][]        typescript: ???
        /// </summary>
        public static string ToTypescript(this SyntaxList<ArrayRankSpecifierSyntax> syntaxList)
        {
            if (syntaxList.Count != 1 || syntaxList[0].Rank != 1)
            {
                Log.Info($"WARNING: To many ranks: {syntaxList} returning any");
                return "any";
            }

            return "[]";
        }

        public static string ToTypescript(this TypeArgumentListSyntax syntaxItem, bool dictionary = false)
        {
            if (dictionary && syntaxItem.Arguments.Count == 2)
            {
                var key = syntaxItem.Arguments[0].ToTypescript();
                var value = syntaxItem.Arguments[1].ToTypescript();

                if (string.Equals(key, "string", StringComparison.OrdinalIgnoreCase))
                {
                    return $"Map<string, {value}>";

                }
            }

            if (syntaxItem.Arguments.Count != 1)
            {
                Log.Warn($"to many generic type args: {string.Join(",", syntaxItem.Arguments)} from {syntaxItem}");
                return "any";
            }

            var arg = syntaxItem.Arguments.First();
            return arg.ToTypescript();
        }

        public static string ToTypescript(this GenericNameSyntax syntaxItem)
        {
            if (syntaxItem.Identifier.Text == "Dictionary")
            {
                return syntaxItem.TypeArgumentList.ToTypescript(true);
            }

            if (syntaxItem.Identifier.Text == "List")
            {
                return syntaxItem.TypeArgumentList.ToTypescript() + "[]";
            }

            if (syntaxItem.Identifier.Text == "Expression")
            {
                return "any";
            }

            if (syntaxItem.IsSpecifiedKnownType())
            {
                return "any";
            }

            if (syntaxItem.IsKnownType())
            {
                return $"{syntaxItem.Identifier.Text}<{syntaxItem.TypeArgumentList.ToTypescript()}>";
            }

            if (syntaxItem.IsExcluded())
            {
                return "any";
            }

            Log.Warn($"unknown Generic: {syntaxItem} mapped to any");
            return "any";
        }

        public static string ToTypescript(this SyntaxToken syntaxItem)
        {
            return syntaxItem.ToString().ToCamelCaseString();
        }

        public static string ToCamelCaseString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return char.ToLower(value[0]) + value.Substring(1);
        }

        public static bool IsNullable(this TypeSyntax syntaxItem)
        {
            return syntaxItem is NullableTypeSyntax;
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
                        if (!propItem.IsPrivate() && !propItem.IsInternal() && !propItem.IsExluded())
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

        public static bool IsKnownType(this BaseTypeDeclarationSyntax syntaxItem)
        {
            return Pocoyo.IsKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsKnownType(this IdentifierNameSyntax syntaxItem)
        {
            return Pocoyo.IsKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsKnownType(this GenericNameSyntax syntaxItem)
        {
            return Pocoyo.IsKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsSpecifiedKnownType(this BaseTypeDeclarationSyntax syntaxItem)
        {
            return Pocoyo.IsSpecifiedKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsSpecifiedKnownType(this IdentifierNameSyntax syntaxItem)
        {
            return Pocoyo.IsSpecifiedKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsSpecifiedKnownType(this GenericNameSyntax syntaxItem)
        {
            return Pocoyo.IsSpecifiedKnownType(syntaxItem.Identifier.Text);
        }

        public static bool IsExcluded(this BaseTypeDeclarationSyntax syntaxItem)
        {
            return Pocoyo.IsExcludedType(syntaxItem.Identifier.Text);
        }

        public static bool IsExcluded(this IdentifierNameSyntax syntaxItem)
        {
            return Pocoyo.IsExcludedType(syntaxItem.Identifier.Text);
        }

        public static bool IsExcluded(this GenericNameSyntax syntaxItem)
        {
            return Pocoyo.IsExcludedType(syntaxItem.Identifier.Text);
        }

        public static bool IsExluded(this BaseTypeDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null)
                return true;

            if (Pocoyo.IsExcludedType(syntaxItem.Identifier.Text))
                return true;

            return syntaxItem.AttributeLists.IsExluded();
        }

        public static bool IsExluded(this PropertyDeclarationSyntax syntaxItem)
        {
            if (syntaxItem == null)
                return true;

            return syntaxItem.AttributeLists.IsExluded();
        }

        public static bool IsExluded(this SyntaxList<AttributeListSyntax> attributeListSyntax)
        {
            if (Pocoyo.ExcludedAttributes.Count == 0)
                return false;

            foreach (var attributeList in attributeListSyntax)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case IdentifierNameSyntax attribItem:
                            {
                                var found = Pocoyo.ExcludedAttributes.FirstOrDefault(item => string.Equals(item, attribItem.Identifier.Text));
                                if (found != null)
                                    return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }

        public static bool IsMultiRankArray(this SyntaxList<ArrayRankSpecifierSyntax> syntaxList)
        {
            return syntaxList.Count != 1 || syntaxList[0].Rank != 1;
        }
    }

}
