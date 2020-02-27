using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using CSharpTranslator.src.Accessors;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

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
                    string type = ex.GetType().FullName;
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
