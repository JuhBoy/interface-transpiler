using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.SyntaxHelpers
{
    internal static class TypeConversion
    {
        public static CSharpNodeType FromRoslynKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                    return CSharpNodeType.Method;
                case SyntaxKind.ClassDeclaration:
                    return CSharpNodeType.Class;
                case SyntaxKind.InterfaceDeclaration:
                    return CSharpNodeType.Interface;
                case SyntaxKind.PropertyDeclaration:
                    return CSharpNodeType.Property;
                case SyntaxKind.FieldDeclaration:
                    return CSharpNodeType.Field;
                default:
                    return CSharpNodeType.None;
            }
        }

        public static CSharpNodeType FromCSharpReflection(Type type)
        {
            if (type.IsClass || IsStruct(type))
                return CSharpNodeType.Class;
            if (type.IsInterface)
                return CSharpNodeType.Interface;
            if (type.IsEnum)
                return CSharpNodeType.Enum;

            return CSharpNodeType.None;
        }

        public static SyntaxKind KindFromReflectionType(Type info)
        {
            if (info.IsEnum)
                return SyntaxKind.EnumKeyword;

            if (info.IsPrimitive)
                return ExtractPrimitiveKind(info.Name);

            #region Special Case
            if (info == typeof(string))
                return SyntaxKind.StringKeyword;

            if (info == typeof(byte[]))
                return SyntaxKind.ByteKeyword;
            #endregion

            if (info.IsGenericType)
                return KindFromReflectionType(info.GenericTypeArguments[0]);
            
            if (IsClassOrInterface(info) || IsStruct(info))
                return SyntaxKind.IdentifierName;

            return SyntaxKind.ObjectKeyword;
        }

        private static readonly string[] DecimalProperties =
        {
            "decimal", "double", "float", "int", "Int32", "Int64", "uint", "long", "ulong", "short", "ushort"
        };

        public static SyntaxKind UnderlyingKindFromReflectionType(Type type)
        {
            // Under the hood nullable are generics of type Nullable<T> where T : struct.
            if (Nullable.GetUnderlyingType(type) != null)
                return SyntaxKind.NullableType;

            if (type.IsGenericType)
                return SyntaxKind.GenericName;

            if (type.IsArray)
                return SyntaxKind.ArrayType;

            return SyntaxKind.None;
        }

        private static SyntaxKind ExtractPrimitiveKind(string name)
        {
            if (name == "bool" || name == "Boolean")
                return SyntaxKind.BoolKeyword;
            if (name == "object")
                return SyntaxKind.ObjectKeyword;

            if (DecimalProperties.Contains(name))
                return SyntaxKind.IntKeyword;

            return SyntaxKind.ObjectKeyword;
        }

        private static bool IsStruct(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        private static bool IsClassOrInterface(Type info)
        {
            return info.IsClass || info.IsInterface;
        }

        public static string RawKindFromReflectionType(Type type)
        {
            string name = type.IsGenericType
                ? type.GenericTypeArguments[0].Name
                : type.Name;
            return name.Replace("[]", "");
        }
    }
}
