using AgolPlugin;
using AgolPlugin.Models.Agol;
using AgolPlugin.ViewModels.Login;
using AgolPlugin.Views.AerialBase;
using AgolPlugin.Views.Import;
using AgolPlugin.Views.Login;
using AgolPlugin.Views.Viewer;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Esri.ArcGISRuntime.Security;
using System;
using System.Drawing;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(Plugin))]
namespace AgolPlugin
{
    public class Plugin : IExtensionApplication
    {
        #region Palettes
        public static PaletteSet ImportPalette { get; private set; } = null;
        public const string IMPORT_GUID = "559c5f3c-624c-479c-8975-9573a413815f";
        public static ImportPage ImportPage { get; private set; } = null;

        public static PaletteSet ViewerPalette { get; private set; } = null;
        public const string VIEWER_GUID = "C387A675-B4B2-4244-B22B-22F67524C09E";
        public static ViewerPage ViewerPage { get; private set; } = null;
        #endregion

        public static CredentialContainer CredContainer { get; private set; }

        #region Commands
        [CommandMethod("BASE_IMAGERY", CommandFlags.Modal | CommandFlags.NoBlockEditor | CommandFlags.NoPaperSpace)]
        public async void ShowBaseImageryImporter()
        {
            var win = new AerialBaseWindow();
            win.ShowDialog();
        }

        [CommandMethod("AGOL_IMPORT")]
        public async void ShowAgolImporter()
        {
            if (Application.DocumentManager.MdiActiveDocument == null
                || !await Authenticate()) return;

            SetupImportPalette();

            ImportPalette.KeepFocus = true;
            ImportPalette.Visible = true;
        }

        [CommandMethod("AGOL_VIEWER")]
        public async void ShowAgolViewer()
        {
            if (Application.DocumentManager.MdiActiveDocument == null
                || !await Authenticate()) return;

            SetupViewerPalette();

            ViewerPalette.KeepFocus = true;
            ViewerPalette.Visible = true;
        }

        [CommandMethod("AGOL_SIGNOUT")]
        public void SignOut()
        {
            ToggleImportPanelVisibility(false);
            ToggleViewerPanelVisibility(false);

            ImportPage = null;
            ViewerPage = null;

            CredContainer = null;

            new LoginViewModel().CancelCreds();
        }
        #endregion

        #region Visibility
        public static void ToggleImportPanelVisibility(bool? show = null)
        {
            if (ImportPalette != null)
                ImportPalette.Visible = show ?? !ImportPalette.Visible;
        }
        public static void ToggleViewerPanelVisibility(bool? show = null)
        {
            if (ViewerPalette != null)
                ViewerPalette.Visible = show ?? !ViewerPalette.Visible;
        }
        #endregion

        #region Setups
        public void SetupImportPalette()
        {
            if (ImportPalette == null)
            {
                ImportPalette = new PaletteSet("ArGIS Online Import", new Guid(IMPORT_GUID))
                {
                    Size = new Size(500, 800),
                    MinimumSize = new Size(350, 500),
                    KeepFocus = true,
                    Style = (PaletteSetStyles)((int)PaletteSetStyles.Snappable + (int)PaletteSetStyles.ShowAutoHideButton + (int)PaletteSetStyles.ShowCloseButton),
                    DockEnabled = DockSides.Left | DockSides.Right | DockSides.Top | DockSides.Bottom,
                };

                ImportPage = new ImportPage();
                ImportPalette.AddVisual("Import", ImportPage);
            }
        }
        public void SetupViewerPalette()
        {
            if (ViewerPalette == null)
            {
                ViewerPalette = new PaletteSet("ArGIS Online Viewer", new Guid(VIEWER_GUID))
                {
                    Size = new Size(500, 800),
                    MinimumSize = new Size(350, 500),
                    KeepFocus = true,
                    Style = (PaletteSetStyles)((int)PaletteSetStyles.Snappable + (int)PaletteSetStyles.ShowAutoHideButton + (int)PaletteSetStyles.ShowCloseButton),
                    DockEnabled = DockSides.Left | DockSides.Right | DockSides.Top | DockSides.Bottom,
                };
                ViewerPage = new ViewerPage();
                ViewerPalette.AddVisual("View", ViewerPage);
            }
        }
        public async Task<bool> Authenticate()
        {
            var win = new AgolLoginWindow();

            if (CredContainer?.Credential == null)
            {
                CredContainer = new CredentialContainer { Credential = await win.GetViewModel().CheckForUserCredential<ArcGISTokenCredential>(), UrlKey = win.GetViewModel().UrlKey };
            }

            if (CredContainer?.Credential == null)
            {
                win.ShowDialog();
                CredContainer = new CredentialContainer { Credential = win.GetCredential(), UrlKey = win.GetViewModel().UrlKey };
                if (CredContainer?.Credential == null)
                    return false;
            }

            return CredContainer?.Credential != null;
        }
        #endregion

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
}