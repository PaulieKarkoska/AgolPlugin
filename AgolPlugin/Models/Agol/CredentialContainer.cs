using Esri.ArcGISRuntime.Security;

namespace AgolPlugin.Models.Agol
{
    public class CredentialContainer
    {
        public ArcGISTokenCredential Credential { get; set; }
        public string UrlKey { get; set; }
    }
}