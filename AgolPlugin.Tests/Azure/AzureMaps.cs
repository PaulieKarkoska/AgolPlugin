using AgolPlugin.Services.MapTiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static AgolPlugin.Helpers.AzureTileMath;

namespace AgolPlugin.Tests.Azure
{
    [TestClass]
    public class AzureMaps
    {
        private double[] _minPt = new[] { 126.9780, 37.5665 };
        private double[] _maxPt = new[] { 128.000, 37.600 };
        private int[] _xy;
        private int _zoom = 13;
        private int _tileSize = 256;
        private string _subKey;

        [TestInitialize]
        public void Init()
        {
            PositionToTileXY(_minPt, _zoom, _tileSize, out int tileX, out int tileY);
            _xy = new[] { tileX, tileY };
            _subKey = Environment.GetEnvironmentVariable("AZURE_MAPS_SUB_KEY", EnvironmentVariableTarget.Machine);
        }

        [TestMethod]
        public void TestGetTileData()
        {
            var svc = new AzureMapTileProvider(_subKey);
            var zoom = 12;
            var tileInfos = new[] { _xy };
            var tiles = svc.GetTiles(tileInfos, zoom);

            Assert.IsTrue(tiles.Count > 0);
        }
    }
}