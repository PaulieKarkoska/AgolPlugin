using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using MahApps.Metro.Controls;

namespace AgolPlugin.Views.Import
{
    public partial class ImportProgressWindow : MetroWindow, IContextIsViewModel
    {
        public ImportProgressViewModel ViewModel { get; private set; }
        public ImportProgressWindow(LayerSelectorViewModel vm)
        {
            ViewModel = new ImportProgressViewModel(vm);
            DataContext = ViewModel;
            InitializeComponent();

            ViewModel.CloseWindowCalled += (s, e) => Close();
            ViewModel.Run();
        }

        public ViewModelBase VM => ViewModel;
        public bool IsViewModelBusy => ViewModel.IsBusy;
    }
}