using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AgolPlugin.Services.Agol
{
    public static class EpsgCodes
    {
        static EpsgCodes()
        {
            Nad83 = new ReadOnlyDictionary<int, string>(_nad83);
        }

        public static ReadOnlyDictionary<int, string> Nad83 { get; private set; }

        private static Dictionary<int, string> _nad83 = new Dictionary<int, string>
        {
            { 2275, "NAD83 / Texas North (ftUS)" },
            { 2276, "NAD83 / Texas North Central (ftUS)" },
            { 2277, "NAD83 / Texas Central (ftUS)" },
            { 2278, "NAD83 / Texas South Central (ftUS)" },
            { 2279, "NAD83 / Texas South (ftUS)" },
            { 2225, "NAD83 / California zone 1 (ftUS)" },
            { 2226, "NAD83 / California zone 2 (ftUS)" },
            { 2227, "NAD83 / California zone 3 (ftUS)" },
            { 2228, "NAD83 / California zone 4 (ftUS)" },
            { 2229, "NAD83 / California zone 5 (ftUS)" },
            { 2230, "NAD83 / California zone 6 (ftUS)" },
            { 2231, "NAD83 / Colorado North (ftUS)" },
            { 2232, "NAD83 / Colorado Central (ftUS)" },
            { 2233, "NAD83 / Colorado South (ftUS)" },
            { 2234, "NAD83 / Connecticut (ftUS)" },
            { 2235, "NAD83 / Delaware (ftUS)" },
            { 2236, "NAD83 / Florida East (ftUS)" },
            { 2237, "NAD83 / Florida West (ftUS)" },
            { 2238, "NAD83 / Florida North (ftUS)" },
            { 2239, "NAD83 / Georgia East (ftUS)" },
            { 2240, "NAD83 / Georgia West (ftUS)" },
            { 2241, "NAD83 / Idaho East (ftUS)" },
            { 2242, "NAD83 / Idaho Central (ftUS)" },
            { 2243, "NAD83 / Idaho West (ftUS)" },
            { 2244, "NAD83 / Indiana East (ftUS)" },
            { 2245, "NAD83 / Indiana West (ftUS)" },
            { 2246, "NAD83 / Kentucky North (ftUS)" },
            { 2247, "NAD83 / Kentucky South (ftUS)" },
            { 2248, "NAD83 / Maryland (ftUS)" },
            { 2249, "NAD83 / Massachusetts Mainland (ftUS)" },
            { 2250, "NAD83 / Massachusetts Island (ftUS)" },
            { 2254, "NAD83 / Mississippi East (ftUS)" },
            { 2255, "NAD83 / Mississippi West (ftUS)" },
            { 2257, "NAD83 / New Mexico East (ftUS)" },
            { 2258, "NAD83 / New Mexico Central (ftUS)" },
            { 2259, "NAD83 / New Mexico West (ftUS)" },
            { 2260, "NAD83 / New York East (ftUS)" },
            { 2261, "NAD83 / New York Central (ftUS)" },
            { 2262, "NAD83 / New York West (ftUS)" },
            { 2263, "NAD83 / New York Long Island (ftUS)" },
            { 2264, "NAD83 / North Carolina (ftUS)" },
            { 2267, "NAD83 / Oklahoma North (ftUS)" },
            { 2268, "NAD83 / Oklahoma South (ftUS)" },
            { 2271, "NAD83 / Pennsylvania North (ftUS)" },
            { 2272, "NAD83 / Pennsylvania South (ftUS)" },
            { 2274, "NAD83 / Tennessee (ftUS)" },
            { 2283, "NAD83 / Virginia North (ftUS)" },
            { 2284, "NAD83 / Virginia South (ftUS)" },
            { 2285, "NAD83 / Washington North (ftUS)" },
            { 2286, "NAD83 / Washington South (ftUS)" },
            { 2287, "NAD83 / Wisconsin North (ftUS)" },
            { 2288, "NAD83 / Wisconsin Central (ftUS)" },
            { 2289, "NAD83 / Wisconsin South (ftUS)" },
            { 2965, "NAD83 / Indiana East (ftUS)" },
            { 2966, "NAD83 / Indiana West (ftUS)" },
            { 3089, "NAD83 / Kentucky Single Zone (ftUS)" },
            { 3433, "NAD83 / Arkansas North (ftUS)" },
            { 3434, "NAD83 / Arkansas South (ftUS)" },
            { 3435, "NAD83 / Illinois East (ftUS)" },
            { 3436, "NAD83 / Illinois West (ftUS)" },
            { 3437, "NAD83 / New Hampshire (ftUS)" },
            { 3438, "NAD83 / Rhode Island (ftUS)" },
            { 3451, "NAD83 / Louisiana North (ftUS)" },
            { 3452, "NAD83 / Louisiana South (ftUS)" },
            { 3453, "NAD83 / Louisiana Offshore (ftUS)" },
            { 3454, "NAD83 / South Dakota North (ftUS)" },
            { 3455, "NAD83 / South Dakota South (ftUS)" },
            { 3560, "NAD83 / Utah North (ftUS)" },
            { 3566, "NAD83 / Utah Central (ftUS)" },
            { 3567, "NAD83 / Utah South (ftUS)" },
            { 3734, "NAD83 / Ohio North (ftUS)" },
            { 3735, "NAD83 / Ohio South (ftUS)" },
            { 3736, "NAD83 / Wyoming East (ftUS)" },
            { 3737, "NAD83 / Wyoming East Central (ftUS)" },
            { 3738, "NAD83 / Wyoming West Central (ftUS)" },
            { 3739, "NAD83 / Wyoming West (ftUS)" },
            { 3759, "NAD83 / Hawaii zone 3 (ftUS)" },
        };
    }
}