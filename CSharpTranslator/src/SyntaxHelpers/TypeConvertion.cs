using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal static class TypeConversion
    {
        public static CSharpNodeType FromRoslynKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                    return CSharpNodeType.Method;
                case SyntaxKind.ClassDeclaration:
                    return CSharpNodeType.Class;
                case SyntaxKind.InterfaceDeclaration:
                    return CSharpNodeType.Interface;
                case SyntaxKind.PropertyDeclaration:
                    return CSharpNodeType.Property;
                case SyntaxKind.FieldDeclaration:
                    return CSharpNodeType.Field;
                default:
                    return CSharpNodeType.None;
            }

        }
    }
}
