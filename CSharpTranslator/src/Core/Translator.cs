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

namespace CSharpTranslator.src.Core
{
    internal class Translator : ITranslator
    {
        internal IGeneratorConfiguration Configuration { get; }
        internal IGenerator Generator { get; }

        private SyntaxTree Tree { get; set; }
        private SyntaxNode Root { get; set; }
        private CSharpNode Head { get; set; }
        private FileBuilder Builder { get; set; }

        public Translator(IGeneratorConfiguration configuration, GeneratorType generatorType)
        {
            Configuration = configuration ??
                            throw new NullReferenceException("Configuration Cannot be null");
            Generator     = GeneratorProvider.Get(generatorType);
        }

        public void Dispose()
        { }

        /// <inheritdoc cref="ITranslator.Compile"/>
        public void Compile()
        {
            string text = File.ReadAllText(Configuration.InputPath, Encoding.UTF8);

            Tree = CSharpSyntaxTree.ParseText(text, CSharpParseOptions.Default);
            Root = Tree.GetRoot();

            ThrowIfNotCSharpLang();

            SyntaxNode namespaceNode         = GetNamespaceNode();
            SyntaxNode classOrInterfaceNode  = GetDeclarationNode(namespaceNode);
            IEnumerable<SyntaxNode> children = GetFilteredNodes(classOrInterfaceNode);

            CreateCSharpHeadNode(classOrInterfaceNode);

            foreach (var child in children) InsertNodeInTree(child);

            ISyntaxTree head = Generator.GetSyntaxTree(Head);
            Builder = new FileBuilder(Configuration.OutputPath, Configuration.OverrideExistingFile);
            Builder.Build(head);
        }

        private void InsertNodeInTree(SyntaxNode node)
        {
            SyntaxKind  topLevelKind = node.Kind();
            bool        isReadonly   = false;
            string      name         = "";
            TypeWrapper type         = new TypeWrapper();

            SyntaxNode[] children = node.ChildNodes().ToArray();
            if (children.Length < 1) return;

            SyntaxNode firstChild = children.First();

            switch (topLevelKind)
            {
                case SyntaxKind.PropertyDeclaration:
                    type       = Extractors.ExtractTypesFromProperty(firstChild);
                    name       = Extractors.ExtractName(node);
                    isReadonly = Extractors.IsReadOnly(node);
                    break;
                case SyntaxKind.FieldDeclaration:
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

        private static SyntaxNode GetDeclarationNode(SyntaxNode namespaceNode)
        {
            SyntaxNode classOrInterface = namespaceNode.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.ClassDeclaration);
            classOrInterface = classOrInterface ?? namespaceNode.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.InterfaceDeclaration);

            if (classOrInterface == null)
                throw new FileFormatException("Implementation not found");

            return classOrInterface;
        }

        private void ThrowIfNotCSharpLang()
        {
            if (Root.Language != "C#")
                throw new FileFormatException("Input File Is not a c# Node tree");
        }

        private SyntaxNode GetNamespaceNode()
        {
            return Root.ChildNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.NamespaceDeclaration);
        }

        public bool Flush()
        {
            bool ok = Builder.Flush();
            return ok;
        }
    }
}
