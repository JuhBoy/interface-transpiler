using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using System;

namespace EasyTranspiler.src.Core.Impl
{
    internal static class StrategyFactory
    {
        public static INodeStrategy Get(Type mType, 
            string attributeNameConstraint = null,
            InclusionStrategy inclusionStrategy = InclusionStrategy.PropertiesAndFields)
        {
            CSharpNodeType type = TypeConversion.FromCSharpReflection(mType);
            INodeStrategy strategy;

            switch (type)
            {
                case CSharpNodeType.Class:
                    strategy = new ClassStrategy(mType);
                    break;
                case CSharpNodeType.Interface:
                    strategy = new InterfaceStrategy(mType);
                    break;
                case CSharpNodeType.Enum:
                    strategy = new EnumStrategy(mType);
                    break;
                case CSharpNodeType.None:
                default:
                    return null;
            }

            strategy.AttributeNameConstraint = attributeNameConstraint;
            strategy.InclusionStrategy = inclusionStrategy;

            return strategy;
        }
    }
}
