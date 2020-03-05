using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.Generators.TypeScript;
using CSharpTranslator.src.SyntaxHelpers;

namespace EasyTranspiler.src.Generators.TypeScript
{
    internal static class EnumTsInfo
    {
        internal static readonly int TokenSize = 5;
        internal static readonly int ValueSize = 2;
        internal static readonly string TypeName = "enum";


        internal static GenericTrivia GetTokenTrivia(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                case 0:
                    return GenericTrivia.Empty;
                case 1:
                    return (node.Visibility == Visibility.Public) ?
                        new GenericTrivia() { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.Void } :
                        GenericTrivia.Empty;
                case 2:
                    return new GenericTrivia() { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.Void };
                case 3:
                    return new GenericTrivia { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.LineJump };
                case 4:
                    return new GenericTrivia { Left = GlobalTsInfo.Void, Right = GlobalTsInfo.LineJump };
                default:
                    return GenericTrivia.Empty;
            }
        }

        internal static string GetTokenContent(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                case 0:
                    return (node.Visibility == Visibility.Public) ? GlobalTsInfo.VisibilityPublicToken : GlobalTsInfo.Void;
                case 1:
                    return TypeName;
                case 2:
                    return node.Identifier;
                case 3:
                    return GlobalTsInfo.OpenContentToken;
                case 4:
                    return GlobalTsInfo.CloseContentToken;
                default:
                    return string.Empty;
            }
        }

        internal static GenericTrivia GetTokenValueTrivia(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                default:
                    return GenericTrivia.Empty;
            }
        }

        internal static string GetTokenValueContent(int tokenIndex, CSharpNode node)
        {
            switch (tokenIndex)
            {
                case 0:
                    return node.Identifier;
                case 1:
                    return GlobalTsInfo.Comma;
                default:
                    return GlobalTsInfo.LineJump;
            }
        }
    }
}
