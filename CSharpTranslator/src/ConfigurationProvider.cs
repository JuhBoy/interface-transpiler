using System;
using System.IO;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;

namespace CSharpTranslator.src
{
    public static class ConfigurationProvider
    {
        public static IGeneratorConfiguration GetConfiguration(string filePath)
        {
            var configuration = new GeneratorConfiguration(filePath);
            return configuration;
        }
    }
}
