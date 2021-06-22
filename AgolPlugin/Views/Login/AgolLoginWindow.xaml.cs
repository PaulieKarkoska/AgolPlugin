using AgolPlugin.ViewModels.Login;
using Esri.ArcGISRuntime.Security;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace AgolPlugin.Views.Login
{
    public partial class AgolLoginWindow : MetroWindow
    {
        private LoginViewModel _vm;
        public AgolLoginWindow()
        {
            _vm = new LoginViewModel();
            DataContext = _vm;

            InitializeComponent();

            Closing += (s, e) =>
            {
                if (_vm.EsriCredential == null)
                    _vm.ThrowCancelled();
            };
            _vm.SubmissionSuccessful += (s, e) =>
            {
                Close();
            };
            _vm.SubmissionFailed += async (s, e) =>
            {
                UserPasswordBox.Clear();
                if (this.Visibility == Visibility.Visible)
                    await new DialogCoordinator().ShowMessageAsync(_vm, "Login failed", "Could not authenticate with the provided credentials. Please try again.", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = "Ok" });
            };
        }

        public ArcGISTokenCredential GetCredential()
        {
            return _vm.EsriCredential as ArcGISTokenCredential;
        }

        public LoginViewModel GetViewModel()
        {
            return _vm;
        }

        private void UserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.SecurePassword = UserPasswordBox.SecurePassword;
        }
    }
}