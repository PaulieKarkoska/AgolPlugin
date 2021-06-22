using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DotSpatial.Projections;

namespace AgolPlugin.Services.Geo
{
    public static class Reprojector
    {
        public const int WGS_84_EPSG = 4326;

        public static Extents3d ReprojectToWgs84(Extents3d source, int sourceSrid)
        {
            var wgs84Proj = ProjectionInfo.FromEpsgCode(WGS_84_EPSG);
            var sourceProj = ProjectionInfo.FromEpsgCode(sourceSrid);

            var minXY = new[] { source.MinPoint.X, source.MinPoint.Y };
            var minZ = new[] { source.MinPoint.Z };

            var maxXY = new[] { source.MaxPoint.X, source.MaxPoint.Y };
            var maxZ = new[] { source.MaxPoint.Z };

            Reproject.ReprojectPoints(minXY, minZ, sourceProj, wgs84Proj, 0, 1);
            Reproject.ReprojectPoints(maxXY, maxZ, sourceProj, wgs84Proj, 0, 1);

            return new Extents3d(new Point3d(minXY[0], minXY[1], minZ[0]), new Point3d(maxXY[0], maxXY[1], maxZ[0]));
        }
    }
}