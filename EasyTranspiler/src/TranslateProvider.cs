using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;
using EasyTranspiler.src;
using EasyTranspiler.src.Core;

namespace CSharpTranslator.src
{
    public static class TranslateProvider
    {

        public static ITranslator GetTranslator(AccessType accessType, GeneratorType generatorType, IGeneratorConfiguration configuration)
        {
            switch (accessType)
            {
                case AccessType.SourceCode:
                    return new RoslynTranslator(configuration, generatorType);
                case AccessType.Assembly:
                    return new AssemblyTranslator(configuration, generatorType);
                default:
                    return null;
            }
        }

        public static ITranslator GetSolutionTranslator(SolutionConfiguration configuration, params ITranslator[] translators)
        {
            return new TranspilationSolution(configuration, translators);
        }
    }
}
