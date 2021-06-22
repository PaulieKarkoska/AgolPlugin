using Autodesk.AutoCAD.Geometry;

namespace AgolPlugin.Models.Agol
{
    public struct AgolGeometry
    {
        public AgolGeometry(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point3d ToPoint3d()
        {
            return new Point3d(X, Y, Z);
        }
    }
}