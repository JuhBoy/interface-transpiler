using System;
using System.Collections.Generic;
using System.Text;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.Generators.TypeScript;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyTranspiler.src.Generators.TypeScript
{
    public class TypeScriptLinkingResolver : ILinkingResolver
    {
        public static IDictionary<string, IList<string>> LinkingCache { get; private set; }

        static TypeScriptLinkingResolver()
        {
            LinkingCache = new Dictionary<string, IList<string>>();
        }

        public void Resolve(ISyntaxTree tree)
        {
            IList<string> identifiers = LinkingCache[tree.Head.Identifier];

            foreach (var identifier in identifiers)
            {
                if (!LinkingCache.ContainsKey(identifier))
                {
                    Console.WriteLine($"Linking Failed for type <{identifier}>");
                    tree.TraverseTree((node, lon) =>
                    {
                        if (PropertyTsInfo.IsAPropertyOrField(ref node) &&
                            PropertyTsInfo.RawTypeIs(ref node, identifier))
                        {
                            PropertyTsInfo.SetAsUnknownType(ref node);
                        }
                    });
                    continue;
                }

                var refNode = new GenericNode { Kind = Kind.Import, Identifier = identifier };

                for (int i = 0; i < ImportTsInfo.TokenSize; i++)
                {
                    var token = new GenericToken();
                    token.Text = ImportTsInfo.TokenContent(i, identifier);
                    token.Trivia = ImportTsInfo.GetTokenTrivia(i);
                    refNode.Tokens.Add(token);
                }

                tree.Prepend(refNode, Level.Sibling);
            }
        }

        public void Add(string identifier, IList<string> references)
        {
            LinkingCache.Add(identifier, references);
        }

        public void Push(string identifier, string reference)
        {
            if (!LinkingCache.ContainsKey(identifier) ||
                LinkingCache[identifier].Contains(reference)) return;
            LinkingCache[identifier].Add(reference);
        }

        public void Clear()
        {
            LinkingCache.Clear();
        }
    }
}
