using CSharpTranslator.src.SyntaxHelpers;

namespace CSharpTranslator.src.Generators
{
    internal interface IGenerator
    {
        ISyntaxTree GetSyntaxTree(CSharpNode head);
    }
}
