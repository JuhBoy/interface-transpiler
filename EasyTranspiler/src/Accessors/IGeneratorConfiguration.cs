namespace CSharpTranslator.src.Accessors
{
    public interface IGeneratorConfiguration
    {
        /// <summary>
        /// Can be a folder or a file.
        /// Call CompileRecursively if it's a folder.
        /// </summary>
        string InputPath { get; set; }

        /// <summary>
        /// Directory Output path
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// If not null or empty, will check for the attributes or reject the class.
        /// </summary>
        string AttributeNameConstraint { get; set; }

        bool OverrideExistingFile { get; set; }

        Visibility Visibility { get; set; }
        InclusionStrategy Strategy { get; set; }
    }
}
