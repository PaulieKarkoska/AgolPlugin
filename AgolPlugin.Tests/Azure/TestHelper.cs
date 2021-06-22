using AgolPlugin.Services.MapTiles;
using System;

namespace AgolPlugin.Tests.Azure
{
    internal static class TestHelpers
    {
        public static AzureMapTileProvider GetMapTileProvider()
        {
            string key = Environment.GetEnvironmentVariable("AZURE_MAPS_SUB_KEY", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = Environment.GetEnvironmentVariable("AZURE_MAPS_SUB_KEY", EnvironmentVariableTarget.Process);
            }
            return new AzureMapTileProvider(key);
        }
    }
}
