using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using System.Windows;
using System.Windows.Controls;

namespace AgolPlugin.Views.Import
{
    public partial class SearchControl : UserControl, IContextIsViewModel
    {
        public SearchViewModel ViewModel { get; private set; }

        public SearchControl()
        {
            ViewModel = new SearchViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public ViewModelBase VM
        {
            get
            {
                return ViewModel;
            }
        }
        public bool IsViewModelBusy
        {
            get
            {
                return ViewModel.IsBusy;
            }
        }
    }
}