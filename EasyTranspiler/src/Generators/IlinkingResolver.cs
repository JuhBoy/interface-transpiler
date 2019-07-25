using System;
using System.Collections.Generic;
using System.Text;
using CSharpTranslator.src.Generators;

namespace EasyTranspiler.src.Generators.TypeScript
{
    public interface ILinkingResolver
    {
        void Resolve(ISyntaxTree tree);
        void Add(string identifier, IList<string> references);
        void Push(string identifier, string reference);
        void Clear();
    }
}
