using System;
using System.Collections.Generic;

namespace CSharpTranslator.src.Generators
{
    internal class TypeScriptSyntaxTree : ISyntaxTree
    {
        public string Extension { get; }
        public GenericNode Head { get; }
        public int Size { get; }

        internal TypeScriptSyntaxTree(GenericNode head, int size)
        {
            Extension = ".ts";
            Head = head;
            Size = size;
        }

        public void TraverseTree(Action<GenericNode, bool> action)
        {
            Loop(action, Head);
        }

        private static void Loop(Action<GenericNode, bool> action, GenericNode node)
        {
            while (node != null)
            {
                action(node, node.Next == null && node.Child == null);
                if (node.Child != null)
                    Loop(action, node.Child);
                node = node.Next;
            }
        }
    }

    public class GenericNode
    {
        internal Kind Kind { get; set; }
        internal string Identifier { get; set; }
        internal List<GenericToken> Tokens { get; } = new List<GenericToken>();
        //internal List<GenericNode> Children { get; } = new List<GenericNode>();

        internal GenericNode Child { get; set; }
        internal GenericNode Next { get; set; }
    }

    internal class GenericToken
    {
        internal string Text { get; set; }
        internal GenericTrivia Trivia { get; set; }

        public override string ToString()
        {
            return $"{Trivia.Left}{Text}{Trivia.Right}";
        }
    }

    internal struct GenericTrivia
    {
        internal string Left { get; set; }
        internal string Right { get; set; }

        public static GenericTrivia Empty => new GenericTrivia() {Left = "", Right = ""};
    }

    internal enum Kind
    {
        None,
        Property,
        Field,
        Method,
        Interface,
        Class
    }
}