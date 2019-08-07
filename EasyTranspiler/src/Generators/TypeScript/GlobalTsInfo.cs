using System;

namespace CSharpTranslator.src.Generators.TypeScript
{
    internal static class GlobalTsInfo
    {
        public static readonly string CloseExpressionToken = ";";
        public static readonly string ToTypeToken = ":";
        public static readonly string Void = "";
        public static readonly string LineJump = Environment.NewLine;
        public static readonly string SpaceToken = " ";
        public static readonly string VisibilityPublicToken = "export";
        public static readonly string ImportToken = "import";
        public static readonly string OpenContentToken = "{";
        public static readonly string CloseContentToken = "}";
        public static readonly string NullableToken = "_"; // TODO: ? is for null-check property, which is not exactly nullable c# field.
        public static readonly string ArraySpecifier = "[]";
    }
}
