using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.SyntaxHelpers;
using EasyTranspiler.src.Accessors;
using EasyTranspiler.src.Core.Impl;
using EasyTranspiler.src.Core.Utils;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Transpile;

namespace EasyTranspiler.src.Core
{
    public class AssemblyTranslator : ITranslator
    {
        private IGenerator Generator { get; }
        private IGeneratorConfiguration Configuration { get; }

        private List<CSharpNode> Heads { get; set; }
        private List<ISyntaxTree> Trees { get; set; }

        public AssemblyTranslator(IGeneratorConfiguration configuration, GeneratorType generatorType)
        {
            Configuration = configuration ??
                            throw new ArgumentNullException(nameof(configuration));
            Generator = GeneratorProvider.Get(generatorType);
            Heads = new List<CSharpNode>();
            Trees = new List<ISyntaxTree>();
        }

        public void Dispose()
        { }

        public bool Flush()
        {
            return Trees.All(tree =>
            {
                FileBuilder builder = new FileBuilder(Path.Combine(Configuration.OutputPath, $"{tree.Name}.model"), Configuration.OverrideExistingFile);
                builder.Build(tree);
                return builder.Flush();
            });
        }

        public void Compile()
        {
            var resolver = new AssemblyResolver(Configuration.InputPath);
            Assembly assembly = resolver.Assembly;

            try
            {
                var markerAttribute = GetAssemblyAttribute(assembly);

                foreach (var type in markerAttribute.Types ?? Enumerable.Empty<Type>())
                {
                    INodeStrategy strategy = StrategyFactory.Get(type, Configuration.AttributeNameConstraint, Configuration.Strategy);
                    CSharpNode root = strategy.ProduceNode();
                    if (root == null) continue;

                    ISyntaxTree syntaxTree = Generator.GetSyntaxTree(root);
                    Trees.Add(syntaxTree);
                }

                Trees.ForEach(tree => Generator.LinkingResolver.Resolve(tree));
            }
            finally
            {
                resolver.Dispose();
            }
        }

        public TranspileMarkerAssemblyAttribute GetAssemblyAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<TranspileMarkerAssemblyAttribute>();
        }
    }
}
