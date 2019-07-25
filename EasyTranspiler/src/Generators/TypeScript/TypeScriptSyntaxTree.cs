using System;
using System.Collections.Generic;
using System.Net.Security;
using CSharpTranslator.src.Core;

namespace CSharpTranslator.src.Generators
{
    public enum Level { Sibling, Child }

    internal class TypeScriptSyntaxTree : ISyntaxTree
    {
        public string Name { get; }
        public string Extension { get; }
        public GenericNode Head { get; private set; }
        public int Size { get; private set; }

        internal TypeScriptSyntaxTree(GenericNode head, int size, string name = "default_name")
        {
            Extension = ".ts";
            Name = name;
            Head = head;
            Size = size;
        }

        public void TraverseTree(Action<GenericNode, bool> action)
        {
            Loop(action, Head);
        }

        public void Prepend(GenericNode node, Level level)
        {
            var head = Head;
            if (level == Level.Sibling)
            {
                node.Next = head;
                Head = node;
            }
            else
            {
                GenericNodeTree.PrependChild(ref head, ref node);
            }
            
            Size++;
        }

        public void Append(GenericNode node)
        {
            var head = Head;
            GenericNodeTree.AddChild(ref head, ref node);
            Size++;
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
        Class,
        Import
    }
}