using System.Collections.Generic;
using CSharpTranslator.src.Accessors;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal class CSharpNode
    {
        public string Identifier { get; }
        public CSharpNodeType CSNodeType { get; }

        public CSharpNode(string identifier, CSharpNodeType csNodeType)
        {
            Identifier = identifier;
            CSNodeType = csNodeType;
            Children = new List<CSharpNode>(10);
        }

        public TypeWrapper Type { get; set; }
        public bool IsReadOnly { get; set; }
        public Visibility Visibility { get; set; }
        public List<CSharpNode> Children { get; set; }

        public override string ToString()
        {
            return $"{CSNodeType} {Type.UnderlyingKind} {Type.Kind} {Identifier}";
        }
    }
}
