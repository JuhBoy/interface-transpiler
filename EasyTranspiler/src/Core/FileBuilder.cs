using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpTranslator.src.Generators;

namespace CSharpTranslator.src.Core
{
    internal class FileBuilder
    {
        private string _path;
        private string _extension;
        private readonly bool _overrideFile;

        private readonly StringBuilder _content;
        private readonly Stack<GenericToken> _closeStack;

        internal FileBuilder(string path, bool overrideFile)
        {
            _path = path;
            _overrideFile = overrideFile;
            _closeStack = new Stack<GenericToken>();
            _content = new StringBuilder();
        }

        public void Build(ISyntaxTree tree)
        {
            _extension = tree.Extension;
            tree.TraverseTree(OnNextNode);
        }

        private void OnNextNode(GenericNode node, bool lastCurrentLevelChild)
        {
            int last = node.Tokens.Count;
            int modifier = 0;

            if (node.Kind == Kind.Interface || node.Kind == Kind.Class)
            {
                _closeStack.Push(node.Tokens[last-1]);
                modifier = -1;
            }

            for (int i = 0; i < (last + modifier); i++)
            {
                _content.Append(node.Tokens[i].Trivia.Left);
                _content.Append(node.Tokens[i].Text);
                _content.Append(node.Tokens[i].Trivia.Right);
            }

            if (lastCurrentLevelChild)
            {
                _content.Append(_closeStack.Pop());
                _content.Append(Environment.NewLine);
            }
        }

        public bool Flush()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? "");
                _path += _extension;
                if (File.Exists(_path) && _overrideFile)
                    File.Delete(_path);
                File.AppendAllText(_path, _content.ToString());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}
