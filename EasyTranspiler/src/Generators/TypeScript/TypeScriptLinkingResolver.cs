﻿using System;
using System.Collections.Generic;
using System.Text;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.Generators;

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
            if (!LinkingCache.ContainsKey(identifier)) return;
            LinkingCache[identifier].Add(reference);
        }

        public void Clear()
        {
            LinkingCache.Clear();
        }
    }
}