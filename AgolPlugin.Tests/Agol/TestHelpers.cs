using AgolPlugin.Models.Agol;
using AgolPlugin.Services.Agol;
using AgolPlugin.ViewModels.Login;
using Esri.ArcGISRuntime.Security;
using System;
using System.Linq;

namespace AgolPlugin.Tests.Agol
{
    internal static class TestHelpers
    {
        public static (AgolFeature, AgolClient) GetFeatureAndClient(ArcGISTokenCredential cred, string urlKey)
        {
            var client = new AgolClient(cred, urlKey);
            var search = client.Search("Pole v3", "empact_gis", useExactowner: true, useExactType: true).Result;
            var item = client.GetServiceItem(search.SearchResults.First().Url).Result;
            var feature = client.GetFeatureLayer($"{item.ServiceUrl}/{item.Layers.First().Id}", item).Result;
            return (feature, client);
        }

        public static ArcGISTokenCredential GetCredential()
        {
            string password = Environment.GetEnvironmentVariable("AGOL_TEST_PASS", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrWhiteSpace(password))
            {
                password = Environment.GetEnvironmentVariable("AGOL_TEST_PASS", EnvironmentVariableTarget.Process);
            }
            return (ArcGISTokenCredential)new LoginViewModel().GenerateCredential(LoginViewModel.AuthHostAddress, "EMPACT_GIS", password).Result;
        }
    }
}