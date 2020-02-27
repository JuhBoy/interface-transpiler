using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace EasyTranspiler.src.Core.Impl
{
    internal class EnumStrategy : INodeStrategy
    {

        public EnumStrategy(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
        public string AttributeNameConstraint { get; set; }
        public InclusionStrategy InclusionStrategy { get; set; }

        public CSharpNode ProduceNode()
        {
            if (!Type.IsEnum) throw new InvalidOperationException($"Type: {Type.Name} is not an Enum");

            CSharpNode node = new CSharpNode(Type.Name, CSharpNodeType.Enum);
            if (node.CSNodeType.Equals(CSharpNodeType.None)) return null;
            node.Visibility = Visibility.Public;

            SetChildren(node);

            return node;
        }

        private void SetChildren(CSharpNode node)
        {
            Array values = Type.GetEnumValues();

            foreach (var value in values)
            {
                CSharpNode child = new CSharpNode(Type.GetEnumName(value), CSharpNodeType.EnumValue);
                child.Type = new TypeWrapper()
                {
                    Kind = SyntaxKind.IntKeyword,
                    UnderlyingKind = SyntaxKind.None
                };
                node.Children.Add(child);
            }
        }
    }
}
