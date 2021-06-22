using AgolPlugin.ViewModels.Import;
using MahApps.Metro.Controls;
using System.Windows.Controls;

namespace AgolPlugin.Views.Import
{
    public partial class ImportPage : Page
    {
        public ImportViewModel ViewModel { get; private set; }

        public ImportPage()
        {
            ViewModel = new ImportViewModel();
            DataContext = ViewModel;
            InitializeComponent();

            ViewModel.GoingBack += (s,e) => ControlContent.Transition = TransitionType.Right;
            ViewModel.GoingForward += (s,e) => ControlContent.Transition = TransitionType.Left;
        }
    }
}