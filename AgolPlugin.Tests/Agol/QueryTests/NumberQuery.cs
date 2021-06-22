using AgolPlugin.Models.Agol;
using Esri.ArcGISRuntime.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static AgolPlugin.Tests.Agol.TestHelpers;

namespace AgolPlugin.Tests.Agol.QueryTests
{
    [TestClass]
    public class NumberQuery
    {
        private ArcGISTokenCredential _cred;
        private string _urlRealm = "empact";

        [TestInitialize]
        public void Init()
        {
            _cred = GetCredential();
        }

        [TestMethod]
        public void TestNumberGreaterThan()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "pole_length",
                        FieldType = "dec",
                        Operator = FilterOperators.GreaterThan,
                        Value1 = 50
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestNumberGreaterThanEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "pole_length",
                        FieldType = "dec",
                        Operator = FilterOperators.GreaterThanEqualTo,
                        Value1 = 50
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Console.WriteLine($"Returned records: {results.Count}");
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestNumberLessThan()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "pole_length",
                        FieldType = "dec",
                        Operator = FilterOperators.LessThan,
                        Value1 = 50
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Console.WriteLine($"Returned records: {results.Count}");
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestNumberLessThanEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = "pole_length",
                        FieldType = "dec",
                        Operator = FilterOperators.LessThanEqualTo,
                        Value1 = 50
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Console.WriteLine($"Returned records: {results.Count}");
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestNumberNotEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.ObjectIdField,
                        FieldType = "int",
                        Operator = FilterOperators.NotEqualTo,
                        Value1 = 555
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestNumberEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.ObjectIdField,
                        FieldType = "int",
                        Operator = FilterOperators.EqualTo,
                        Value1 = 555
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count == 1);
        }

        [TestMethod]
        public void TestNumberBetween()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);

            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.ObjectIdField,
                        FieldType = "int",
                        Operator = FilterOperators.Between,
                        Value1 = 555,
                        Value2 = 557
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count == 1);
        }
    }
}