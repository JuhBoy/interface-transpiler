using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Generators.TypeScript;

namespace CSharpTranslator.src.Generators
{
    internal interface IGenerator
    {
        ILinkingResolver LinkingResolver { get; }
        ISyntaxTree GetSyntaxTree(CSharpNode head);
    }
}
