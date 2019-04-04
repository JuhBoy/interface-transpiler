namespace CSharpTranslator.src.Accessors
{
    public interface IGeneratorConfiguration
    {
        string InputPath { get; set; }
        string OutputPath { get; set; }
        bool OverrideExistingFile { get; set; }

        Visibility Visibility { get; set; }
        InclusionStrategy Strategy { get; set; }
    }
}
