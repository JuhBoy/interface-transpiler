using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;

namespace CSharpTranslator.src
{
    public static class TranslateProvider
    {

        public static ITranslator GetTranslator(GeneratorType type, IGeneratorConfiguration configuration)
        {
            var translator = new Translator(configuration, type);
            return translator;
        }
    }
}
