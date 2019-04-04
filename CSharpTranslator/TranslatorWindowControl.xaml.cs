using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CSharpTranslator.Annotations;
using CSharpTranslator.src;
using CSharpTranslator.src.Accessors;
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

            CSharpFileListing();
        }

        public string FileSelected { get; set; }
        public string OutputPath { get; set; }
        public string SolutionPath { get; set; }

        /// <summary>
        /// Fetch All C# Files in project
        /// </summary>
        /// <returns>All C# Files Except Program.cs and Startup.cs</returns>
        private void CSharpFileListing()
        {
            SolutionPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            string[] allFiles = Directory.GetFiles(SolutionPath, "*.cs", SearchOption.AllDirectories);
            _files = allFiles.Where(file => file.IndexOf(@"\obj") == -1 && file.IndexOf(@"\bin") == -1).ToArray();

            string[] fileNames = _files.Select(Path.GetFileName).ToArray();
            FileCombo.ItemsSource = fileNames;
        }

        private void InputFileCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string list = e.AddedItems[0] as string;
            if (list is null) return;

            int i = FileCombo.SelectedIndex;
            FileSelected = _files[i];
            OnPropertyChanged(nameof(FileSelected));
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            string combinedPath = Path.Combine(SolutionPath, ((TextBox)e.Source).Text);
            OutputPath = combinedPath;
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
                var translator = TranslateProvider.GetTranslator(GeneratorType.TypeScript, configuration);
                translator.Compile();
                translator.Flush();
                MessageBox.Show("File has been generated [" + configuration.OutputPath + "]");
            }
            catch (Exception)
            {
                MessageBox.Show("File Generation Failed, ensure input file contains a class/interface declaration inside a valid namespace");
            }
        }

        private string GetOutputPath()
        {
            if (OutputPath == null)
            {
                string name = Path.GetFileName(FileSelected);
                if (name != null)
                {
                    int end = name.IndexOf(".cs");
                    name = name.Substring(0, end);
                }
                else
                    name = "generated_file";

                OutputPath = Path.Combine(SolutionPath, name);
            }
                
            return OutputPath;
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
    }
}