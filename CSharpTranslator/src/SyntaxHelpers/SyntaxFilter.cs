using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal static class SyntaxFilter
    {
        internal static bool PropertiesAndFields(SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.FieldDeclaration ||
                   node.Kind() == SyntaxKind.PropertyDeclaration;
        }

        internal static bool Methods(SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.MethodDeclaration;
        }

        internal static bool All(SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.FieldDeclaration ||
                   node.Kind() == SyntaxKind.PropertyDeclaration ||
                   node.Kind() == SyntaxKind.MethodDeclaration;
        }
    }
}
