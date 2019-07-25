using System;
using CSharpTranslator.src.Accessors;

namespace EasyTranspiler.src.Core
{
    internal class TranspilationSolution : ITranslator
    {
        private ITranslator[] Translators { get; }
        private SolutionConfiguration Configuration { get; }

        internal TranspilationSolution(SolutionConfiguration configuration, params ITranslator[] translators)
        {
            Translators = translators;
            Configuration = configuration;
        }

        public void Dispose()
        {
            foreach (var translator in Translators)
            {
                translator.Dispose();
            }
        }

        public void Compile()
        {
            foreach (var translator in Translators)
            {
                try
                {
                    translator.Compile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public bool Flush()
        {
            bool valid = true;
            foreach (var translator in Translators)
            {
                valid &= translator.Flush();
            }

            return valid;
        }
    }
}
