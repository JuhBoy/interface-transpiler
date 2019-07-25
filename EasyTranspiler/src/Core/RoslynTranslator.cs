using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.SyntaxHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpTranslator.src.Core
{
    internal class RoslynTranslator : ITranslator
    {
        internal IGeneratorConfiguration Configuration { get; }
        internal IGenerator Generator { get; }

        private SyntaxTree Tree { get; set; }
        private SyntaxNode Root { get; set; }
        private CSharpNode Head { get; set; }
        private List<FileBuilder> Builder { get; set; }

        private string CurrentFilePath { get; set; }
        private string CurrentOutputPath { get; set; }

        public RoslynTranslator(IGeneratorConfiguration configuration, GeneratorType generatorType)
        {
            Configuration   = configuration ?? 
                              throw new NullReferenceException("Configuration Cannot be null");
            Generator       = GeneratorProvider.Get(generatorType);
            CurrentFilePath = Configuration.InputPath;
            Builder         = new List<FileBuilder>();
        }

        public void Dispose()
        { }

        public void Compile()
        {
            if ((File.GetAttributes(Configuration.InputPath) & FileAttributes.Directory) == 0)
            {
                InternalCompile();
                return;
            }

            string[] files = Directory.GetFiles(Configuration.InputPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                CurrentFilePath = file;
                try
                {
                    InternalCompile();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (FileLoadException ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }

        /// <inheritdoc cref="ITranslator.Compile"/>
        private void InternalCompile()
        {
            string text = File.ReadAllText(CurrentFilePath, Encoding.UTF8);

            Tree = CSharpSyntaxTree.ParseText(text, CSharpParseOptions.Default);
            Root = Tree.GetRoot();

            ThrowIfNotCSharpLang();

            SyntaxNode namespaceNode         = GetNamespaceNode();
            SyntaxNode classOrInterfaceNode  = GetDeclarationNode(namespaceNode);
            IEnumerable<SyntaxNode> children = GetFilteredNodes(classOrInterfaceNode);

            CreateCSharpHeadNode(classOrInterfaceNode);

            foreach (var child in children) InsertNodeInTree(child);

            ISyntaxTree head = Generator.GetSyntaxTree(Head);
            Builder.Add(new FileBuilder(CurrentOutputPath, Configuration.OverrideExistingFile));
            Builder.Last().Build(head);
        }

        private void InsertNodeInTree(SyntaxNode node)
        {
            SyntaxKind  topLevelKind = node.Kind();
            bool        isReadonly   = false;
            string      name         = "";
            TypeWrapper type         = new TypeWrapper();

            SyntaxNode[] children = node.ChildNodes().ToArray();
            if (children.Length < 1) return;

            switch (topLevelKind)
            {
                case SyntaxKind.PropertyDeclaration:
                    type       = Extractors.ExtractTypesFromProperty(GetPropertyTypeNode(node));
                    name       = Extractors.ExtractName(node);
                    isReadonly = Extractors.IsReadOnly(node);
                    break;
                case SyntaxKind.FieldDeclaration:
                    SyntaxNode firstChild = children.First();
                    type = Extractors.ExtractTypesFromField(firstChild);
                    name = Extractors.ExtractName(firstChild);
                    break;
                case SyntaxKind.MethodDeclaration:
                    /*TODO: Method, can have multiple return/parameters type. Handle that when needed*/
                    break;
            }

            var csharpNode = new CSharpNode(name, TypeConversion.FromRoslynKind(topLevelKind))
            {
                Type = type,
                IsReadOnly = isReadonly
            };
            Head.Children.Add(csharpNode);
        }

        private IEnumerable<SyntaxNode> GetFilteredNodes(SyntaxNode classOrInterfaceNode)
        {
            Func<SyntaxNode, bool> filter;

            switch (Configuration.Strategy)
            {
                case InclusionStrategy.All:
                     filter = SyntaxFilter.All;
                    break;
                case InclusionStrategy.Methods:
                    filter = SyntaxFilter.Methods;
                    break;
                case InclusionStrategy.PropertiesAndFields:
                    filter = SyntaxFilter.PropertiesAndFields;
                    break;
                default:
                    throw new ArgumentException("Unknown inclusion strategy");
            }
            
            return classOrInterfaceNode.ChildNodes().Where(filter).ToArray();
        }

        private void CreateCSharpHeadNode(SyntaxNode root)
        {
            var nodeType = TypeConversion.FromRoslynKind(root.Kind());
            string name  = Extractors.ExtractName(root);
            Head         = new CSharpNode(name, nodeType) {Visibility = Configuration.Visibility};
        }

        private SyntaxNode GetDeclarationNode(SyntaxNode namespaceNode)
        {
            SyntaxNode classOrInterface = namespaceNode.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.ClassDeclaration);

            if (classOrInterface == null)
                throw new FileLoadException("Implementation not found");

            bool isAuthorized = GetConstraintAuthorization(classOrInterface);
            if (!isAuthorized)
                throw new UnauthorizedAccessException(
                    $"Class {Extractors.ExtractName(classOrInterface)} do not respect the Attribute constraint {Configuration.AttributeNameConstraint}");

            classOrInterface = classOrInterface ?? namespaceNode.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.InterfaceDeclaration);

            return classOrInterface;
        }

        private bool GetConstraintAuthorization(SyntaxNode classOrInterface)
        {
            if (!string.IsNullOrEmpty(Configuration.AttributeNameConstraint))
            {
                var attributeList = classOrInterface.ChildNodes().Where(child => child.IsKind(SyntaxKind.AttributeList)).ToList();
                bool ok = attributeList.Any(attr =>
                {
                    var attributes = attr.ChildNodes().ToList();

                    foreach (var attribute in attributes)
                    {
                        var attributeSyntax = attribute as AttributeSyntax;

                        bool isConstraint = attributeSyntax?.Name.ToString().Equals(Configuration.AttributeNameConstraint) ?? false;
                        string myname = attributeSyntax.Name.ToString();

                        if (isConstraint)
                        {
                            var relativeOutputPath = attributeSyntax.ArgumentList.Arguments[0].Expression.ToString()
                                .Replace("\"", "");
                            string name = Path.GetFileName(CurrentFilePath) ?? "default.cs";
                            CurrentOutputPath = Path.Combine(Configuration.OutputPath, relativeOutputPath, name);
                            return true;
                        }
                    }

                    return false;
                });
                if (!ok) return false;
            }

            return true;
        }

        private void ThrowIfNotCSharpLang()
        {
            if (Root.Language != "C#")
                throw new FileLoadException("Input File Is not a c# Node tree");
        }

        private SyntaxNode GetNamespaceNode()
        {
            return Root.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.NamespaceDeclaration);
        }

        private SyntaxNode GetPropertyTypeNode(SyntaxNode node)
        {
            return node.ChildNodes().FirstOrDefault(n => n.IsKind(SyntaxKind.PredefinedType) ||
                                                         n.IsKind(SyntaxKind.IdentifierName) ||
                                                         n.IsKind(SyntaxKind.GenericName));
        }

        public bool Flush()
        {
            bool ok = true;
            foreach (var fileBuilder in Builder)
                ok &= fileBuilder.Flush();
            return ok;
        }
    }
}
