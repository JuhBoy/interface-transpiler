using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyTranspiler.src.Core.Impl.Extensions
{
    internal static class PropertiesExtensionsg
    {
        internal static CSharpNode ComputeChildrenProperty(this INodeStrategy strategy, PropertyInfo propertyInfo)
        {
            if (!string.IsNullOrEmpty(strategy.AttributeNameConstraint))
            {
                var shouldBeAvoided = propertyInfo.GetCustomAttributes().Any(e => e.GetType().Name.Equals(strategy.AttributeNameConstraint));
                if (shouldBeAvoided) return null;
            }

            var node = new CSharpNode(propertyInfo.Name, CSharpNodeType.Property);
            node.Type = propertyInfo.PropertyType.ToTypeWrapper();

            return node;
        }
    }
}
