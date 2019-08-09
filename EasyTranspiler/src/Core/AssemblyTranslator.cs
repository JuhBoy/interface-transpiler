using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSharpTranslator.src.Accessors;
using CSharpTranslator.src.Core;
using CSharpTranslator.src.Generators;
using CSharpTranslator.src.SyntaxHelpers;
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
            Assembly assembly = Assembly.LoadFrom(Configuration.InputPath);
            
            var markerAttribute = GetAssemblyAttribute(assembly);

            foreach (var type in markerAttribute.Types ?? Enumerable.Empty<Type>())
            {
                var head = new CSharpNode(type.Name, TypeConversion.FromCSharpReflection(type));
                if (head.CSNodeType.Equals(CSharpNodeType.None)) continue;
                head.Visibility = Visibility.Public;

                Heads.Add(head);

                foreach (var property in type.GetProperties())
                    InsertPropertyNodeInTree(property);

                Trees.Add(Generator.GetSyntaxTree(head));
            }

            Trees.ForEach(tree => Generator.LinkingResolver.Resolve(tree));
        }

        private void InsertPropertyNodeInTree(PropertyInfo propertyInfo)
        {
            if (!string.IsNullOrEmpty(Configuration.AttributeNameConstraint))
            {
                var shouldBeAvoided = propertyInfo.GetCustomAttributes().Any(e => e.GetType().Name.Equals(Configuration.AttributeNameConstraint));
                if (shouldBeAvoided) return;
            }

            var node = new CSharpNode(propertyInfo.Name, CSharpNodeType.Property);
            node.Type = new TypeWrapper()
            {
                Kind = TypeConversion.KindFromReflectionType(propertyInfo.PropertyType),
                RawKind = TypeConversion.RawKindFromReflectionType(propertyInfo.PropertyType),
                UnderlyingKind = TypeConversion.UnderlyingKindFromReflectionType(propertyInfo.PropertyType)
            };
            Heads.Last().Children.Add(node);
        }

        public TranspileMarkerAssemblyAttribute GetAssemblyAttribute(Assembly assembly)
        {
            return assembly.GetCustomAttribute<TranspileMarkerAssemblyAttribute>();
        }
    }
}
