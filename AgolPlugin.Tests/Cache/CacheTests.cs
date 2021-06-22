using AgolPlugin.Services.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AgolPlugin.Tests.Cache
{
    [TestClass]
    public class CacheTests
    {
        public CacheService _svc;

        [TestInitialize]
        public void DbInit()
        {
            _svc = CacheService.CustomizeInstance(Path.GetTempPath());
        }

        [TestMethod]
        [Priority(0)]
        public void DbValid_Negative()
        {
            File.Delete(_svc.DbPath);
            Assert.IsFalse(_svc.DbIsValid());
        }

        [TestMethod]
        [Priority(1)]
        public void DbValid_Positive()
        {
            Assert.IsTrue(_svc.DbIsValid());
        }

        [TestMethod]
        [Priority(2)]
        public void CheckCreateTable()
        {
            _svc.CreateNewTable("new_table");
            Assert.IsTrue(_svc.CheckTable("new_table"));
        }

        [TestMethod]
        [Priority(3)]
        public void CheckTableExists_Positive()
        {
            _svc.CreateNewTable("test_table");
            Assert.IsTrue(_svc.CheckTable("test_table"));
        }

        [TestMethod]
        [Priority(4)]
        public void CheckTableExists_Negative()
        {
            Assert.IsFalse(_svc.CheckTable("fake_table"));
        }

        [TestCleanup]
        [Priority(99)]
        public void DbCleanup()
        {
            File.Delete(_svc.DbPath);
        }
    }
}