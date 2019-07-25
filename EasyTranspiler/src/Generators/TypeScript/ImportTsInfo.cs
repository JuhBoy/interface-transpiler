using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.Generators.TypeScript;
using CSharpTranslator.src.SyntaxHelpers;

namespace EasyTranspiler.src.Generators.TypeScript
{
    /*
     *                
                         {Text = "import", Trivia = new GenericTrivia() {Left = "", Right = ""}});
                
                        {Text = identifier, Trivia = new GenericTrivia() {Left = "{", Right = "}"}});
                
                         {Text = "from", Trivia = new GenericTrivia() {Left = string.Empty, Right = string.Empty}});
                
                            {Text = "the_path_todo", Trivia = new GenericTrivia() {Left = "'", Right = "'"}});
                
                        {Text = ";", Trivia = new GenericTrivia() {Left = string.Empty, Right = string.Empty}});
     */
    internal class ImportTsInfo
    {
        public static readonly int TokenSize = 5;
        public static readonly string ImportTokenIdentifier = "import";
        public static readonly string ImportFromTokenIdentifier = "from";

        public static string TokenContent(int tokenIndex, string identifier)
        {
            switch (tokenIndex)
            {
                case 0:
                    return ImportTokenIdentifier;
                case 1:
                    return identifier;
                case 2:
                    return ImportFromTokenIdentifier;
                case 3:
                    return $"./{identifier}.model.ts";
                case 4:
                    return GlobalTsInfo.CloseExpressionToken;
                default:
                    return "";
            }
        }

        public static GenericTrivia GetTokenTrivia(int tokenIndex)
        {
            switch (tokenIndex)
            {
                case 0:
                    return new GenericTrivia() {Left = "", Right = " "};
                case 1:
                    return new GenericTrivia() { Left = $"{GlobalTsInfo.OpenContentToken} ", Right = $" {GlobalTsInfo.CloseContentToken}" };
                case 2:
                    return new GenericTrivia() {Left = " ", Right = " "};
                case 3:
                    return new GenericTrivia() {Left = "'", Right = "'"};
                case 4:
                    return new GenericTrivia() { Left = string.Empty, Right = GlobalTsInfo.LineJump };
                default:
                    return GenericTrivia.Empty;
            }
        }
    }
}
