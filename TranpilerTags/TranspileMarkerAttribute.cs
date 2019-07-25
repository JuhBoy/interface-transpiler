using System;

namespace Transpile
{
    public class TranspileMarkerAttribute : Attribute
    {
        public TranspileMarkerAttribute(string outputDirectory)
        {
            OutputDirectory = outputDirectory;
        }

        public string OutputDirectory { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class TranspileMarkerAssemblyAttribute : Attribute
    {
        public TranspileMarkerAssemblyAttribute(Type[] types, string outputDirectory)
        {
            Types = types;
            OutputDirectory = outputDirectory;
        }

        public Type[] Types { get; }
        public string OutputDirectory { get; }
    }
}
