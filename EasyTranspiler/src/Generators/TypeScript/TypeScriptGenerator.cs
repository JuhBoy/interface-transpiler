using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.SyntaxHelpers;

namespace CSharpTranslator.src.Generators.TypeScript
{
    internal class TypeScriptGenerator : IGenerator
    {
        private int _size;

        internal TypeScriptGenerator()
        {
            _size = 0;
        }

        public ISyntaxTree GetSyntaxTree(CSharpNode head)
        {
            _size = 0;
            GenericNode node = BuildGenericTree(head);
            TypeScriptSyntaxTree tree = new TypeScriptSyntaxTree(node, _size);
            return tree;
        }

        private GenericNode BuildGenericTree(CSharpNode head)
        {
            GenericNode node = CreateGenericNode(head);
            _size++;

            foreach (var cSharpNode in head.Children)
            {
                GenericNode child = BuildGenericTree(cSharpNode);
                GenericNodeTreeHelper.AddChild(ref node, ref child);
                _size++;
            }
            return node;
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