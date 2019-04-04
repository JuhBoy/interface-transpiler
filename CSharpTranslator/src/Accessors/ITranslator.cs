using System;
using System.IO;

namespace CSharpTranslator.src.Accessors
{
    public interface ITranslator : IDisposable
    {
        /// <summary>
        /// Parse the C# File, Translate into an in memory Tree for the generator type.
        /// </summary>
        /// <exception cref="FileFormatException">Thrown if the file isn't a c# or doesn't contains Class/interface declaration</exception>
        /// <exception cref="ArgumentException">In case of Unknown strategy</exception>
        void Compile();

        /// <summary>
        /// Save the file, free all resources.
        /// </summary>
        bool Flush();
    }
}
