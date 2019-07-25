using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal struct TypeWrapper
    {
        internal string RawKind { get; set; }
        internal SyntaxKind Kind { get; set; }
        internal string RawUnderlyingKind { get; set;}
        internal SyntaxKind UnderlyingKind { get; set; }
    }
}
