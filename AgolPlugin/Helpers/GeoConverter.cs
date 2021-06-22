using AgolPlugin.Models.Agol;
using DotSpatial.Projections;
using System.Collections.Generic;

namespace AgolPlugin.Helpers
{
    public static class GeoConverter
    {
        public static void ReprojectFromWgs84(IEnumerable<AgolRecord> records, int outputSrid, bool elevationMetersToFeet = false)
        {
            var xy = new double[2];
            var z = new double[1];

            foreach (var record in records)
            {
                xy[0] = record.Geometry.Value.X;
                xy[1] = record.Geometry.Value.Y;
                z[0] = record.Geometry.Value.Z;
                var zVal = elevationMetersToFeet ? MetersToFeet(z[0]) : z[0];

                Reproject.ReprojectPoints(xy, z,
                    ProjectionInfo.FromEpsgCode(4326),
                    ProjectionInfo.FromEpsgCode(outputSrid),
                    0, 1);

                record.Geometry = new AgolGeometry(xy[0], xy[1], zVal);
            }
        }

        /// <summary>
        /// Reprojects an XYZ position to WGS84 (longitude/latitude/elevation)
        /// </summary>
        /// <param name="positions">A double array, where index 0 is X, index 1 is Y, and index 2 is Z (elevation)</param>
        /// <param name="sourceSrid">The source EPSG code</param>
        /// <param name="elevationMetersToFeet"></param>
        /// <returns>A double array, where index 0 is longitude (X), index 1 is latitude (Y), and index 2 is elevation (Z)</returns>
        public static double[] ReprojectToWgs84(double[] position, int sourceSrid, bool elevationMetersToFeet = false)
        {
            var xy = new[] { position[0], position[1] };
            var z = new[] { position.Length == 3 ? (elevationMetersToFeet ? MetersToFeet(position[2]) : position[2]) : 0 };

            Reproject.ReprojectPoints(xy, z,
                ProjectionInfo.FromEpsgCode(sourceSrid),
                ProjectionInfo.FromEpsgCode(4326),
                0, 1);

            return position.Length == 3 ?
                new[] { xy[0], xy[1], z[0] } :
                new[] { xy[0], xy[1] };
        }

        public static double[] ReprojectFromWgs84(double[] position, int outputSrid, bool elevationMetersToFeet = false)
        {
            var xy = new[] { position[0], position[1] };
            var z = new[] { position.Length == 3 ? (elevationMetersToFeet ? MetersToFeet(position[2]) : position[2]) : 0 };

            Reproject.ReprojectPoints(xy, z,
                ProjectionInfo.FromEpsgCode(4326),
                ProjectionInfo.FromEpsgCode(outputSrid),
                0, 1);

            return position.Length == 3 ?
                new[] { xy[0], xy[1], z[0] } :
                new[] { xy[0], xy[1] };
        }


        public static double MetersToFeet(double meters)
        {
            return meters * 3.280839895D;
        }
    }
}