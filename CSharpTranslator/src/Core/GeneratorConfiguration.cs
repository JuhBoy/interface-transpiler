using System;
using System.IO;
using CSharpTranslator.src.Accessors;

namespace CSharpTranslator.src.Core
{
    internal class GeneratorConfiguration : IGeneratorConfiguration
    {
        private string _inputPath;

        public GeneratorConfiguration(string inputPath)
        {
            ValidInputPath(inputPath);
            InputPath = inputPath;
        }

        public Visibility Visibility { get; set; } = Visibility.Private;
        public InclusionStrategy Strategy { get; set; } = InclusionStrategy.PropertiesAndFields;
        public bool OverrideExistingFile { get; set; }
        public string OutputPath { get; set; }
        public string InputPath
        {
            get => _inputPath;
            set { ValidInputPath(value); _inputPath = value; }
        }

        #region VALIDATOR
        /// <summary>
        /// Validate the Input CSharp File.
        /// </summary>
        /// <param name="inputPath">A complete file path to the source C# file</param>
        private void ValidInputPath(string inputPath)
        {
            if (!File.Exists(inputPath))
                throw new ArgumentException("Input File doesn't exist");
        }
        #endregion
    }
}
