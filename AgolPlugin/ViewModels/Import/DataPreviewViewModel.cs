using AgolPlugin.Models.Common;

namespace AgolPlugin.ViewModels.Import
{
    public class DataPreviewViewModel : ViewModelBase
    {




#if DEBUG
        public DataPreviewViewModel() { }
#endif
        public DataPreviewViewModel(LayerConfiguratorViewModel data)
        {
            Title = "Preview Data";
        }
    }
}