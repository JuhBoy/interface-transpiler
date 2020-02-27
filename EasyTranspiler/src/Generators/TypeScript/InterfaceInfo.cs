using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.SyntaxHelpers;

namespace CSharpTranslator.src.Generators.TypeScript
{
    internal static class InterfaceInfo
    {
        public static readonly string InterfaceNameType = "interface";
        public static readonly int TokenSize = 5;

        public static string TokenContent(int tokenIndex, CSharpNode csNode)
        {
            switch (tokenIndex)
            {
                case 0:
                    string text = (csNode.Visibility == Visibility.Public) ? GlobalTsInfo.VisibilityPublicToken : GlobalTsInfo.Void;
                    return text;
                case 1:
                    return InterfaceNameType;
                case 2:
                    return csNode.Identifier;
                case 3:
                    return GlobalTsInfo.OpenContentToken;
                case 4:
                    return GlobalTsInfo.CloseContentToken;
                default:
                    return string.Empty;
            }
        }

        public static GenericTrivia GetTokenTrivia(int tokenIndex, CSharpNode csNode)
        {
            switch (tokenIndex)
            {
                case 0:
                    return GenericTrivia.Empty;
                case 1:
                    return (csNode.Visibility == Visibility.Public)
                        ? new GenericTrivia { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.Void }
                        : GenericTrivia.Empty;
                case 2:
                    return new GenericTrivia { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.Void };
                case 3:
                    return new GenericTrivia { Left = GlobalTsInfo.SpaceToken, Right = GlobalTsInfo.LineJump };
                case 4:
                    return new GenericTrivia { Left = GlobalTsInfo.Void, Right = GlobalTsInfo.LineJump };
                default:
                    return GenericTrivia.Empty;
            }
        }
    }
}
