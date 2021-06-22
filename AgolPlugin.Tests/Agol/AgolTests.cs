using AgolPlugin.Services.Agol;
using Esri.ArcGISRuntime.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgolPlugin.Tests.Agol.QueryTests
{
    [TestClass]
    public class AgolTests
    {
        private ArcGISTokenCredential _cred;
        private string _urlRealm = "empact";

        [TestInitialize]
        public void Init()
        {
            _cred = TestHelpers.GetCredential();
        }

        [TestMethod]
        public void TestAuth()
        {
            _cred = TestHelpers.GetCredential();
            Assert.IsTrue(_cred.Token != null);
        }

        [TestMethod]
        public void TestSearch()
        {
            var client = new AgolClient(_cred, _urlRealm);

            var results = client.Search("", "empact_gis", useExactowner: true, useExactType: true).Result;

            Assert.IsTrue(results.SearchResults.Count > 0);
        }
    }
}