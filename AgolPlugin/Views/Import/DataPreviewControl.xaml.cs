using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using System.Windows.Controls;

namespace AgolPlugin.Views.Import
{
    public partial class DataPreviewControl : UserControl, IContextIsViewModel
    {
        public DataPreviewViewModel ViewModel { get; private set; }

        public DataPreviewControl(LayerConfiguratorViewModel data)
        {
            ViewModel = new DataPreviewViewModel(data);
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