using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using System;

namespace EasyTranspiler.src.Core.Impl
{
    internal class InterfaceStrategy : DefaultStrategy, INodeStrategy
    {

        public InterfaceStrategy(Type type) : base(type)
        { }

        public override CSharpNode ProduceNode()
        {
            return base.ProduceNode();
        }
    }
}
