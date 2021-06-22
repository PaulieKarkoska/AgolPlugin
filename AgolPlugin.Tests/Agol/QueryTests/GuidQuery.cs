using AgolPlugin.Models.Agol;
using Esri.ArcGISRuntime.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static AgolPlugin.Tests.Agol.TestHelpers;

namespace AgolPlugin.Tests.Agol.QueryTests
{
    [TestClass]
    public class GuidQuery
    {
        private ArcGISTokenCredential _cred;
        private string _urlRealm = "empact";

        [TestInitialize]
        public void Init()
        {
            _cred = GetCredential();
        }

        [TestMethod]
        public void TestGuidEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.GlobalIdField,
                        FieldType = "guid",
                        Operator = FilterOperators.EqualTo,
                        Value1 = "8baf2ec4-da11-4f86-a819-833c7b994cdf"
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count == 1);
        }

        [TestMethod]
        public void TestGuidNotEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.GlobalIdField,
                        FieldType = "guid",
                        Operator = FilterOperators.NotEqualTo,
                        Value1 = "8baf2ec4-da11-4f86-a819-833c7b994cdf"
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }
    }
}