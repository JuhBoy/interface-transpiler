using System;

namespace CSharpTranslator.src.Generators
{
    public interface ISyntaxTree
    {
        string Name { get; }
        string Extension { get; }
        GenericNode Head { get; }
        int Size { get; }
        void TraverseTree(Action<GenericNode, bool> action);
        void Prepend(GenericNode node, Level level);
        void Append(GenericNode node);
    }
}