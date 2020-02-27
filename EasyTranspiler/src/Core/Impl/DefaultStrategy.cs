using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using EasyTranspiler.src.Core.Impl.Extensions;
using System;
using System.Reflection;

namespace EasyTranspiler.src.Core.Impl
{
    internal class DefaultStrategy : INodeStrategy
    {
        public DefaultStrategy(Type type)
        {
            Type = type;
        }

        #region Properties
        public Type Type { get; }
        public string AttributeNameConstraint { get; set; }
        public InclusionStrategy InclusionStrategy { get; set; }
        #endregion

        public virtual CSharpNode ProduceNode()
        {
            var headNode = new CSharpNode(Type.Name, TypeConversion.FromCSharpReflection(Type));
            if (headNode.CSNodeType.Equals(CSharpNodeType.None)) return null;
            headNode.Visibility = Visibility.Public;

            if (InclusionStrategy == InclusionStrategy.PropertiesAndFields)
            {
                SetProperties(headNode);
                SetField(headNode);
            }

            return headNode;
        }

        protected virtual void SetProperties(CSharpNode headNode)
        {
            foreach (var property in Type.GetProperties())
            {
                CSharpNode child = ((INodeStrategy)this).ComputeChildrenProperty(property);
                if (child == null) continue;
                headNode.Children.Add(child);
            }
        }

        protected virtual void SetField(CSharpNode headNode)
        {
            foreach (FieldInfo field in Type.GetFields())
            {
                CSharpNode child = new CSharpNode(field.Name, TypeConversion.FromCSharpReflection(field.FieldType));
                child.Type = field.FieldType.ToTypeWrapper();
                headNode.Children.Add(child);
            }
        }
    }
}
