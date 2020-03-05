using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace EasyTranspiler.src.Core.Utils
{
    internal sealed class AssemblyResolver : IDisposable
    {
        private readonly DependencyContext DepsContext;
        private readonly AssemblyLoadContext AssemblyContext;
        private readonly ICompilationAssemblyResolver Resolver;

        public AssemblyResolver(string path)
        {
            AssemblyPath = path;
            Assembly = Assembly.LoadFile(path);
            DepsContext = DependencyContext.Load(Assembly);

            Resolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[] {
                new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
                new ReferenceAssemblyPathResolver(),
                new PackageCompilationAssemblyResolver()
            });

            AssemblyContext = AssemblyLoadContext.GetLoadContext(Assembly);
            AssemblyContext.Resolving += OnResolving;
        }

        public Assembly Assembly { get; set; }
        public string AssemblyPath { get; }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            CompilationLibrary compLib = TryGetFromCompilationLibs(assemblyName);

            if (compLib == null)
            {
                compLib = TryGetFromRuntimeLibs(assemblyName);
            }

            if (compLib != null)
                return LoadLibrary(compLib);
            else
                return LoadFromSharedLibrary(assemblyName);
        }

        private Assembly LoadFromSharedLibrary(AssemblyName assemblyName)
        {
            string dllPath = Path.Combine(Path.GetDirectoryName(AssemblyPath), $"{assemblyName.Name.Split(',')[0]}.dll");

            try
            {
                return AssemblyContext.LoadFromAssemblyPath(dllPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private CompilationLibrary TryGetFromCompilationLibs(AssemblyName assemblyName)
        {
            return DepsContext.CompileLibraries
                .FirstOrDefault(e => e.Name.Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase));
        }

        private CompilationLibrary TryGetFromRuntimeLibs(AssemblyName assemblyName)
        {
            RuntimeLibrary runLib = DepsContext.RuntimeLibraries
                .FirstOrDefault(e => e.Name.Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase));

            if (runLib != null)
            {
                var compLib = new CompilationLibrary(
                    runLib.Type,
                    runLib.Name,
                    runLib.Version,
                    runLib.Hash,
                    runLib.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    runLib.Dependencies,
                    runLib.Serviceable);

                return compLib;
            }

            return null;
        }

        private Assembly LoadLibrary(CompilationLibrary compLib)
        {
            try
            {
                List<string> assemblies = new List<string>();
                Resolver.TryResolveAssemblyPaths(compLib, assemblies);
                return AssemblyContext.LoadFromAssemblyPath(assemblies[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void Dispose()
        {
            AssemblyContext.Resolving -= OnResolving;
        }
    }
}
