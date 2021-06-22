using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AgolPlugin.Services.Acad.Extensions
{
    public static class LayerExtension
    {
        public static bool LayerExists(this Database db, string layerName)
        {
            using (var trans = db.TransactionManager.StartTransaction())
            using (var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable)
            {
                return layerTable.Has(layerName);
            }
        }

        public static IEnumerable<string> GetLayerNames(this Database db)
        {
            using (var trans = db.TransactionManager.StartTransaction())
            using (var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable)
                foreach (ObjectId layerId in layerTable)
                {
                    yield return (trans.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord).Name;
                }
        }
    }
}