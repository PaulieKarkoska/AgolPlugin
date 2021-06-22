using AgolPlugin.Models.Agol;
using Esri.ArcGISRuntime.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static AgolPlugin.Tests.Agol.TestHelpers;

namespace AgolPlugin.Tests.Agol.QueryTests
{
    [TestClass]
    public class DateQuery
    {
        private ArcGISTokenCredential _cred;
        private string _urlRealm = "empact";

        [TestInitialize]
        public void Init()
        {
            _cred = GetCredential();
        }

        [TestMethod]
        public void TestDateBetweenDate()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.FieldsInfo.CreationDateField,
                        FieldType = "date",
                        Operator = FilterOperators.Between,
                        Value1 = DateTime.Parse("10/1/2020"),
                        Value2 = DateTime.Parse("10/5/2020"),
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestDateGreaterThan()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.FieldsInfo.CreationDateField,
                        FieldType = "date",
                        Operator = FilterOperators.GreaterThan,
                        Value1 = DateTime.Parse("10/1/2020"),
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestDateGreaterThanEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.FieldsInfo.CreationDateField,
                        FieldType = "date",
                        Operator = FilterOperators.GreaterThanEqualTo,
                        Value1 = DateTime.Parse("10/1/2020"),
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestDateLessThan()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.FieldsInfo.CreationDateField,
                        FieldType = "date",
                        Operator = FilterOperators.LessThan,
                        Value1 = DateTime.Parse("10/1/2020"),
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public void TestDateLessThanEqualTo()
        {
            var (feature, client) = GetFeatureAndClient(_cred, _urlRealm);
            var query = new FeatureQueryParams()
            {
                OutputSpatialReference = 2276,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FieldName = feature.FieldsInfo.CreationDateField,
                        FieldType = "date",
                        Operator = FilterOperators.LessThanEqualTo,
                        Value1 = DateTime.Parse("10/1/2020"),
                    }
                }
            };

            var results = client.GetRecords(feature, query).Result;
            Assert.IsTrue(results.Count > 0);
        }

    }
}