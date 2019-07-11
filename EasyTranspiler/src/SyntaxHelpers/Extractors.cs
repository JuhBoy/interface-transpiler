using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal static class Extractors
    {
        internal static TypeWrapper ExtractTypesFromProperty(SyntaxNode propertyNode)
        {
            string rawUnderlyingKind = "";
            string rawKind = "";
            SyntaxKind underlyingKind = propertyNode.Kind();
            SyntaxKind kind = propertyNode.GetFirstToken().Kind();

            if (propertyNode.Kind() == SyntaxKind.GenericName)
            {
                kind = ExtractKindFromGenericNode(propertyNode);
                rawKind = ExtractGenericNodeTypeName(propertyNode);
                rawUnderlyingKind = propertyNode.GetFirstToken().Text;
            }
            else
            {
                var typeNodes = propertyNode.ChildNodes().ToArray();
                if (typeNodes.Length > 1)
                    underlyingKind = typeNodes[1].Kind();
                rawKind = propertyNode.GetFirstToken().Text;
            }

            return new TypeWrapper()
            {
                Kind = kind,
                RawKind = rawKind,
                UnderlyingKind = underlyingKind,
                RawUnderlyingKind = rawUnderlyingKind
            };
        }

        private static string ExtractGenericNodeTypeName(SyntaxNode node)
        {
            if (node.Kind() != SyntaxKind.GenericName) return "";
            SyntaxToken[] descendants = node.DescendantTokens().ToArray();
            return descendants[2].Text;
        }

        internal static TypeWrapper ExtractTypesFromField(SyntaxNode fieldNode)
        {
            string rawKind = "";
            string rawUnderlyingKind = "";
            SyntaxKind kind = fieldNode.GetFirstToken().Kind();
            SyntaxKind underlyingKind = SyntaxKind.None;

            var children = fieldNode.ChildNodes().ToArray();
            underlyingKind = children[0].Kind();

            if (underlyingKind == SyntaxKind.GenericName)
            {
                kind = Extractors.ExtractKindFromGenericNode(children[0]);
                rawKind = ExtractGenericNodeTypeName(children[0]);
                rawUnderlyingKind = children[0].GetFirstToken().Text;
            }
            else
            {
                rawKind = children[0].GetFirstToken().Text;
            }

            return new TypeWrapper()
            {
                RawKind = rawKind,
                Kind = kind,
                RawUnderlyingKind = rawUnderlyingKind,
                UnderlyingKind = underlyingKind
            };
        }

        internal static SyntaxKind ExtractKindFromGenericNode(SyntaxNode node)
        {
            if (node.Kind() != SyntaxKind.GenericName) return SyntaxKind.None;
            return node.DescendantTokens().ToArray()[2].Kind();
        }

        internal static string ExtractName(SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.VariableDeclaration)
                return node.GetLastToken().Text;

            var tokens = node.ChildTokens().ToArray();

            // If there is no visibility keyword (For a property)
            if (tokens.Length == 1) return tokens[0].Text;

            // If It's 5 there is a visibility keyword (for a class or interface)
            string name = tokens.Length == 5 ? tokens[2].Text : tokens[1].Text;

            return name;
        }

        public static bool IsReadOnly(SyntaxNode node)
        {
            if (node.Kind() != SyntaxKind.PropertyDeclaration) return false;

            foreach (var child in node.ChildNodes().ToArray())
            {
                if (!child.IsKind(SyntaxKind.AccessorList)) continue;
                var children = child.ChildNodes().ToArray();
                if (children.Any(innerChildNode => innerChildNode.IsKind(SyntaxKind.SetAccessorDeclaration)))
                    return false;
            }

            return true;
        }
    }
}
