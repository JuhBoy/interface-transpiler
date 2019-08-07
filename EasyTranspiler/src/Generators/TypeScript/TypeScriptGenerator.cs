using System.Collections.Generic;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Generators.TypeScript;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.Generators.TypeScript
{
    internal class TypeScriptGenerator : IGenerator
    {
        private int _size;
        public ILinkingResolver LinkingResolver { get; private set; }

        private string CurrentHeadIdentifier { get; set; }
        
        internal TypeScriptGenerator(ILinkingResolver linkingLinkingResolver)
        {
            _size = 0;
            LinkingResolver = linkingLinkingResolver;
        }

        public ISyntaxTree GetSyntaxTree(CSharpNode head)
        {
            _size = 0;
            CurrentHeadIdentifier = head.Identifier;
            GenericNode node = BuildGenericTree(head);
            return new TypeScriptSyntaxTree(node, _size, head.Identifier);
        }

        private GenericNode BuildGenericTree(CSharpNode head)
        {
            GenericNode node = CreateGenericNode(head);
            _size++;

            ProcessLinking(head);
            
            foreach (var cSharpNode in head.Children)
            {
                GenericNode child = BuildGenericTree(cSharpNode);
                GenericNodeTree.AddChild(ref node, ref child);
                _size++;
            }
            return node;
        }

        private void ProcessLinking(CSharpNode node)
        {
            if (node.CSNodeType == CSharpNodeType.Class ||
                 node.CSNodeType == CSharpNodeType.Interface)
            {
                LinkingResolver.Add(CurrentHeadIdentifier, new List<string>());
                return;
            }

            if (node.Type.Kind == SyntaxKind.IdentifierName && !PropertyTsInfo.HasKnowIdentifier(node.Identifier))
                LinkingResolver.Push(CurrentHeadIdentifier, node.Type.RawKind);
        }

        private GenericNode CreateGenericNode(CSharpNode node)
        {
            Kind tsKind = (Kind) node.CSNodeType;

            GenericNode gNode = new GenericNode()
            {
                Identifier = node.Identifier,
                Kind = tsKind
            };

            int size = GetTokenSizeFor(tsKind);
            for (int i = 0; i < size; i++)
            {
                var token = new GenericToken();
                token.Text = GetTokenContent(node, i);
                token.Trivia = GetTokenTrivia(node, i);
                gNode.Tokens.Add(token);
            }

            return gNode;
        }

        private GenericTrivia GetTokenTrivia(CSharpNode node, int tokenIndex)
        {
            switch ((Kind)node.CSNodeType)
            {
                case Kind.Class:
                case Kind.Interface:
                    return InterfaceInfo.GetTokenTrivia(tokenIndex, node);
                case Kind.Field:
                case Kind.Property:
                    return PropertyTsInfo.GetTokenTrivia(tokenIndex, node);
                default:
                    return GenericTrivia.Empty;
            }
        }

        private string GetTokenContent(CSharpNode node, int tokenIndex)
        {
            switch ((Kind)node.CSNodeType)
            {
                case Kind.Class:
                case Kind.Interface:
                    return InterfaceInfo.TokenContent(tokenIndex, node);
                case Kind.Field:
                case Kind.Property:
                    return PropertyTsInfo.TokenContent(tokenIndex, node);
                default:
                    return "";
            }
        }

        private int GetTokenSizeFor(Kind tsKind)
        {
            switch (tsKind)
            {
                case Kind.Interface:
                case Kind.Class:
                    return InterfaceInfo.TokenSize;
                case Kind.Field:
                case Kind.Property:
                    return PropertyTsInfo.TokenSize;
                default:
                    return 0;
            }
        }
    }
}