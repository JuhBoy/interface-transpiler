using CSharpTranslator.src.SyntaxHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyTranspiler.src.Core.Impl.Extensions
{
    internal static class TypeExtensions
    {
        public static TypeWrapper ToTypeWrapper(this Type type)
        {
            return new TypeWrapper()
            {
                Kind = TypeConversion.KindFromReflectionType(type),
                RawKind = TypeConversion.RawKindFromReflectionType(type),
                UnderlyingKind = TypeConversion.UnderlyingKindFromReflectionType(type)
            };
        }
    }
}
