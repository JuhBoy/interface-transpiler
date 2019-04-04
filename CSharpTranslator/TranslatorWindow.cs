using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace CSharpTranslator
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("d046803a-f839-4112-b52d-387fab237419")]
    public class TranslatorWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorWindow"/> class.
        /// </summary>
        public TranslatorWindow() : base(null)
        {
            this.Caption = "C# Transcompiler Tool";
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
            this.Content = new TranslatorWindowControl();
        }
    }
}
