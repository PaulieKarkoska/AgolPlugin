using AgolPlugin.Models.Common;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Security;
using Esri.ArcGISRuntime.Security;
using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Creds = CredentialManagement;

namespace AgolPlugin.ViewModels.Login
{
    public class LoginViewModel : ModelBase
    {
        private DirectoryInfo _configDirectory = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AgolPlugin"));
        private string _winCredName = "AgolPlugin";
        public BasicCommand Submit_Command { get; private set; }
        public LoginViewModel()
        {
            Submit_Command = new BasicCommand(Submit_Execute, Submit_CanExecute);

            SubmissionSuccessful += (s, e) =>
            {
                IsBusy = false;
            };
            SubmissionFailed += (s, e) =>
            {
                IsBusy = false;
                EsriCredential = null;
                SecurePassword = null;
            };
        }

        #region Properties
        public const string AuthHostAddress = "https://www.arcgis.com/sharing/rest";
        public Credential EsriCredential { get; private set; }

        private bool _rememberMe = true;
        public bool RememberMe
        {
            get { return _rememberMe; }
            set { _rememberMe = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); }
        }


        private string _urlKey;
        public string UrlKey
        {
            get { return _urlKey; }
            set { _urlKey = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        private SecureString _securePassword;
        public SecureString SecurePassword
        {
            get { return _securePassword; }
            set { _securePassword = value; OnPropertyChanged(); }
        }
        #endregion

        #region Interaction
        public async Task<T> CheckForUserCredential<T>() where T : Credential
        {
            string urlKey = Properties.Settings.Default.ArcGisUrlKey;
            RecallCreds(out string user, out string pass);

            if (string.IsNullOrEmpty(urlKey) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                return null;

            UrlKey = urlKey;

            return (T)await GenerateCredential(AuthHostAddress, user, pass);
        }

        public async Task<Credential> GenerateCredential(string host, string username, string pass)
        {
            IsBusy = true;

            try
            {
                EsriCredential = await AuthenticationManager.Current.GenerateCredentialAsync(
                    new Uri(host), username, pass, new GenerateTokenOptions
                    {
                        TokenAuthenticationType = TokenAuthenticationType.ArcGISToken,
                    });

                bool urlKeyIsValid = false;

                try
                {
                    urlKeyIsValid = await new AgolClient(EsriCredential as ArcGISTokenCredential, UrlKey).ValidateUrlKey();
                }
                catch { }

                if (EsriCredential != null && RememberMe && urlKeyIsValid)
                {
                    Properties.Settings.Default.ArcGisUrlKey = UrlKey;
                    Properties.Settings.Default.Save();

                    using (var cred = new Creds.Credential())
                    {
                        cred.Username = username;
                        cred.Password = pass;
                        cred.Target = _winCredName;
                        cred.Type = Creds.CredentialType.Generic;
                        cred.PersistanceType = Creds.PersistanceType.LocalComputer;
                        cred.Save();
                    }

                    OnCredentialsSaved();
                }
                else
                {
                    Properties.Settings.Default.ArcGisUrlKey = string.Empty;
                    Properties.Settings.Default.Save();

                    CancelCreds();
                }

                if (EsriCredential is ArcGISTokenCredential tokenCred)
                {
                    tokenCred.UserName = username;
                    tokenCred.Password = pass;
                }

                OnSubmissionSuccessful();
            }
            catch
            {
                OnSubmissionFailed();
            }
            IsBusy = false;
            return EsriCredential;
        }

        public void RecallCreds(out string user, out string pass)
        {
            RememberMe = false;
            using (var cred = new Creds.Credential())
            {
                cred.Target = _winCredName;
                cred.Load();
                user = cred.Username;
                pass = cred.Password;
            }
        }

        public void CancelCreds()
        {
            try
            {
                using (var cred = new Creds.Credential())
                {
                    cred.Target = _winCredName;
                    cred.Load();
                    cred.Delete();
                }
            }
            catch { }
        }

        private async void Submit_Execute(object param)
        {
            IsBusy = true;
            var options = new GenerateTokenOptions
            {
                TokenAuthenticationType = TokenAuthenticationType.ArcGISToken,
            };

            try
            {
                var cred = await GenerateCredential(AuthHostAddress, Username, StringProtector.SecureStringToString(SecurePassword));

                EsriCredential = cred;
            }
            catch
            {
                OnSubmissionFailed();
            }
            IsBusy = false;

        }
        private bool Submit_CanExecute(object param)
        {
            return SecurePassword?.Length > 0
                && !string.IsNullOrEmpty(Username);
        }
        #endregion

        #region Events
        public event EventHandler VariablesSaved;
        private void OnCredentialsSaved()
        {
            VariablesSaved?.Invoke(this, new EventArgs());
        }

        public event EventHandler SubmissionSuccessful;
        private void OnSubmissionSuccessful()
        {
            SubmissionSuccessful?.Invoke(this, new EventArgs());
        }

        public event EventHandler SubmissionCancelled;
        public void ThrowCancelled()
        {
            OnSubmissionCancelled();
        }
        private void OnSubmissionCancelled()
        {
            SubmissionCancelled?.Invoke(this, new EventArgs());
        }

        public event EventHandler SubmissionFailed;
        private void OnSubmissionFailed()
        {
            SubmissionFailed?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}