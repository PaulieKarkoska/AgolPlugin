using AgolPlugin.ViewModels.AerialBase;
using MahApps.Metro.Controls;

namespace AgolPlugin.Views.AerialBase
{
    public partial class AerialBaseWindow : MetroWindow
    {
        private AerialBaseViewModel _vm;
        public AerialBaseWindow()
        {
            _vm = new AerialBaseViewModel();
            DataContext = _vm;

            _vm.ProcessSucceeded += (s, e) =>
            {
                Close();
            };
            _vm.ProcessFailed += (s, e) =>
            {
                Close();
            };

            InitializeComponent();
        }
    }
}