using AgolPlugin.Models.Agol;
using Esri.ArcGISRuntime.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static AgolPlugin.Tests.Agol.TestHelpers;

namespace AgolPlugin.Tests.Agol.QueryTests
{
    [TestClass]
    public class StringQuery
    {
        private ArcGISTokenCredential _cred;
        private string _urlRealm = "empact";

        [TestInitialize]
        public void Init()
        {
            _cred = GetCredential();
        }

        [TestMethod]
        public void TestStringNotEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "project_code",
                        FieldType = "string",
                        Operator = FilterOperators.NotEqualTo,
                        Value1 = "UR-2-5.A"
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestStringEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "project_code",
                        FieldType = "string",
                        Operator = FilterOperators.EqualTo,
                        Value1 = "UR-2-5.A"
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestStringLike()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "project_code",
                        FieldType = "string",
                        Operator = FilterOperators.Like,
                        Value1 = "UR-2%"
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Console.WriteLine($"Returned records: {results.Count}");
            Assert.IsTrue(results.Count > 0);
        }
    }
}