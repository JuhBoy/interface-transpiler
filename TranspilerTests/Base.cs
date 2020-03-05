using System;
using System.Reflection;
using CSharpTranslator.src;
using CSharpTranslator.src.Accessors;
using EasyTranspiler.src;
using Xunit;

// C:\Lab\P2.Marlin\P2.Marlin\bin\Debug\netcoreapp2.2\P2.Marlin.dll
//C:\Lab\P2.Nemo\Nemo\bin\Release\netstandard2.0\Nemo.dll
// C:\Lab\P2.Nemo.DataModels\P2.Nemo.DataModels.SharedModels\bin\Debug\netcoreapp2.2\P2.Nemo.DataModels.SharedModels.dll
// C:\Lab\P2.Marlin\P2.Marlin\bin\Release\netcoreapp2.2\win-x86\P2.Marlin.exe

namespace TranspilerTests
{
    public class Base
    {
        [Fact]
        public void ShouldDoItsJob_WhenAsked()
        {
            //string link = @"C:\Lab\P2.Nemo.DataModels\P2.Nemo.DataModels.SharedModels\bin\Debug\netcoreapp2.2\P2.Nemo.DataModels.SharedModels.dll";
            string link = @"C:\Lab\P2.Marlin\P2.Marlin\bin\Debug\netcoreapp3.1\P2.Marlin.dll";
            //string link = @"C:\Lab\P2.Marlin\P2.PostBuildTypescriptTranspiler\bin\Debug\netcoreapp3.1\dotnet-transpil.dll";

            var assemblyConf = ConfigurationProvider.GetConfiguration(link);
            assemblyConf.OutputPath = @"C:\Users\JuH\Desktop\ReflectionModel";
            assemblyConf.OverrideExistingFile = true;
            assemblyConf.Strategy = InclusionStrategy.PropertiesAndFields;

            var assemblyTranslator = TranslateProvider.GetTranslator(AccessType.Assembly, GeneratorType.TypeScript, assemblyConf);

            var solutionConf = new SolutionConfiguration() { LinkStrategy = LinkStrategy.Link };
            var solution = TranslateProvider.GetSolutionTranslator(solutionConf, assemblyTranslator);

            solution.Compile();
            solution.Flush();
        }
    }
}
