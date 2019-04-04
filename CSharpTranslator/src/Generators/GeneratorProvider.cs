﻿using System;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Generators.TypeScript;

namespace CSharpTranslator.src.Generators
{
    internal static class GeneratorProvider
    {
        public static IGenerator Get(GeneratorType type, Visibility visibility = Visibility.Public)
        {
            switch (type)
            {
                case GeneratorType.TypeScript:
                    return new TypeScriptGenerator();
                default:
                    throw new ArgumentException("No Generator defined for this type");
            }
        }
    }
}
