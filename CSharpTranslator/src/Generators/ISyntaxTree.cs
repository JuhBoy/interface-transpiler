using System;

namespace CSharpTranslator.src.Generators
{
    public interface ISyntaxTree
    {
        string Extension { get; }
        GenericNode Head { get; }
        int Size { get; }
        void TraverseTree(Action<GenericNode, bool> action);
    }
}