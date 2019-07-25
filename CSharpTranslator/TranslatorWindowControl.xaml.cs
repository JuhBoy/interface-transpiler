using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using CSharpTranslator.Annotations;
using CSharpTranslator.src;
using CSharpTranslator.src.Accessors;
using EasyTranspiler.src;
using MessageBox = System.Windows.MessageBox;
using Visibility = CSharpTranslator.src.Accessors.Visibility;

namespace CSharpTranslator
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TranslatorWindowControl.
    /// </summary>
    public partial class TranslatorWindowControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly string[] InclusionStrategies = {"Properties And Fields", "All", "Methods"};
        public readonly string[] VisibilityScopes = {"Public", "Private"};

        private string[] _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorWindowControl"/> class.
        /// </summary>
        public TranslatorWindowControl()
        {
            InitializeComponent();
            
            DataContext = this;

            VisibilityCombo.ItemsSource = VisibilityScopes;
            StrategyCombo.ItemsSource = InclusionStrategies;

            VisibilityCombo.Items.MoveCurrentToPosition(0);
            StrategyCombo.Items.MoveCurrentToPosition(0);
        }

        public string FileSelected { get; set; } = "";
        public string OutputPath { get; set; }
        

        private void OpenFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.ShowDialog();

                if (fileDialog.FileNames == null ||
                    fileDialog.FileNames.Length < 1) return;
                if (Path.GetExtension(fileDialog.FileNames[0]) != ".cs") return;

                FileSelected = fileDialog.FileNames[0];
                OnPropertyChanged(nameof(FileSelected));
            }
        }

        private void Compile_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileSelected))
            {
                MessageBox.Show("No C# File Selected");
                return;
            }

            string visibility = VisibilityCombo.SelectedValue as string;
            string strategy = StrategyCombo.SelectedValue as string;

            IGeneratorConfiguration configuration = ConfigurationProvider.GetConfiguration(FileSelected);
            configuration.Visibility = GetVisibility(visibility);
            configuration.Strategy = GetStrategy(strategy);
            configuration.OverrideExistingFile = GetOverride();
            configuration.OutputPath = GetOutputPath();

            try
            {
                var translator = TranslateProvider.GetTranslator(AccessType.SourceCode, GeneratorType.TypeScript, configuration);
                translator.Compile();
                translator.Flush();
                MessageBox.Show("File has been generated [" + configuration.OutputPath + "]");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File Generation Failed: {ex.Message}");
            }



            //////////// TEST

            var assemblyConf = ConfigurationProvider.GetConfiguration("dll");
            assemblyConf.OutputPath = configuration.OutputPath;
            assemblyConf.OverrideExistingFile = true;
            assemblyConf.Strategy = InclusionStrategy.PropertiesAndFields;
            
            var roslynTranslator = TranslateProvider.GetTranslator(AccessType.SourceCode, GeneratorType.TypeScript, configuration);
            var assemblyTranslator = TranslateProvider.GetTranslator(AccessType.Assembly, GeneratorType.TypeScript, assemblyConf);

            var solutionConfiguration = new SolutionConfiguration() { LinkStrategy = LinkStrategy.Link };
            var solution = TranslateProvider.GetSolutionTranslator(solutionConfiguration, roslynTranslator, assemblyTranslator);
            
            solution.Compile();
            solution.Flush();
        }

        private string GetOutputPath()
        {
            if (OutputPath == null)
                OutputPath = "C:/";

            string name = Path.GetFileName(FileSelected);
            if (name != null)
            {
                int end = name.IndexOf(".cs");
                name = name.Substring(0, end);
            }
            else
                name = "generated_file";

            return Path.Combine(OutputPath, name);
        }

        private bool GetOverride()
        {
            if (OverrideFile.IsChecked.HasValue)
                return (bool)OverrideFile.IsChecked;
            return false;
        }

        private static InclusionStrategy GetStrategy(string strategy)
        {
            InclusionStrategy result;

            switch (strategy)
            {
                case "All":
                    result = InclusionStrategy.All;
                    break;
                case "Properties And Fields":
                    result = InclusionStrategy.PropertiesAndFields;
                    break;
                case "Methods":
                    result = InclusionStrategy.Methods;
                    break;
                default:
                    result = InclusionStrategy.PropertiesAndFields;
                    break;
            }

            //TODO: Handle Methods, When needed

            return InclusionStrategy.PropertiesAndFields;
        }

        private static Visibility GetVisibility(string visibility)
        {
            return (visibility == "Public") ? src.Accessors.Visibility.Public : 
                                              src.Accessors.Visibility.Private;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    OutputPath = dialog.SelectedPath;
                    OnPropertyChanged(nameof(OutputPath));
                }
            }
        }
    }
}