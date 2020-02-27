using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using System;

namespace EasyTranspiler.src.Core.Impl
{
    internal class ClassStrategy : DefaultStrategy, INodeStrategy
    {
        internal ClassStrategy(Type type) : base(type)
        { }

        public override CSharpNode ProduceNode()
        {
            return base.ProduceNode();
        }
    }
}
