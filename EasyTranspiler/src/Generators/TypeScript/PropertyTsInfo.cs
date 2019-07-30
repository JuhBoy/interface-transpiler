using System;
using CSharpTranslator.src.SyntaxHelpers;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpTranslator.src.Generators.TypeScript
{
    internal class PropertyTsInfo
    {
        public static readonly int TokenSize = 3;
        public static readonly string ReadonlyToken = "readonly";

        public static string TokenContent(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                case 0:
                    return (node.IsReadOnly) ? ReadonlyToken : "";
                case 1:
                    var text = $"\t{char.ToLowerInvariant(node.Identifier[0]) + node.Identifier.Substring(1)}";
                    if (node.Type.UnderlyingKind == SyntaxKind.NullableType)
                        text += GlobalTsInfo.NullableToken;
                    return text;
                default:
                    return GetTsType(node.Type);
            }
        }

        private static string GetTsType(TypeWrapper type)
        {
            string text = "";

            switch (type.Kind)
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.IdentifierName:
                    text = GetKnownIdentifier(type.RawKind);
                    break;
                case SyntaxKind.IntKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.DoubleKeyword:
                    text = "number";
                    break;
                case SyntaxKind.StringKeyword:
                    text = "string";
                    break;
                case SyntaxKind.BoolKeyword:
                    text = "boolean";
                    break;
                case SyntaxKind.ObjectKeyword:
                    text = "object";
                    break;
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.ByteKeyword:
                    text = "number";
                    break;
                default:
                    text = "any";
                    break;
            }

            if (IsArray(type.UnderlyingKind))
                text += GlobalTsInfo.ArraySpecifier;

            return text;
        }

        private static bool IsArray(SyntaxKind kind)
        {
            return kind == SyntaxKind.ArrayRankSpecifier || kind == SyntaxKind.ArrayType || kind == SyntaxKind.GenericName;
        }

        public static GenericTrivia GetTokenTrivia(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                case 0:
                    if (node.IsReadOnly)
                        return new GenericTrivia() {Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.Void};
                    return GenericTrivia.Empty;
                case 1:
                    return new GenericTrivia() {Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.ToTypeToken};
                default:
                    return new GenericTrivia()
                    {
                        Left = GlobalTsInfo.SpaceToken,
                        Right = GlobalTsInfo.CloseExpressionToken + GlobalTsInfo.LineJump
                    };
            }
        }

        private static string GetKnownIdentifier(string rawKind)
        {
            switch (rawKind)
            {
                case "DateTime":
                    return "Date";
                case "Guid":
                    return "string";
                default:
                    return rawKind;
            }
        }
    }
}