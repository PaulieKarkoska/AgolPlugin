using AgolPlugin.Models.Acad;
using AgolPlugin.Services.Extensions;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Threading;

namespace AgolPlugin.Services.Acad
{
    public class AcadReader
    {
        public const string BLOCK_REFERENCE_OBJ_NAME = "AcDbBlockReference";
        public const string BLOCK_REFERENCE_DXF_NAME = "INSERT";

        private AcadService _acad;

        public AcadReader(AcadService acad)
        {
            _acad = acad;
        }

        public bool CheckAgolSelection(CancellationTokenSource cts, out AcadBlockDataHolder data)
        {
            data = null;

            var selection = _acad.Editor.SelectImplied().Value;
            if (selection == null) return false;

            var objectIds = selection.GetObjectIds();
            if (objectIds.Length > 1) return false;

            var id = objectIds[0];
            if (id.ObjectClass.DxfName != BLOCK_REFERENCE_DXF_NAME && id.ObjectClass.Name != BLOCK_REFERENCE_OBJ_NAME) return false;

            if (cts != null && cts.IsCancellationRequested) return false;

            var blockRef = _acad.Transaction.GetObject(id, OpenMode.ForRead) as BlockReference;
            if (blockRef.OwnerId == SymbolUtilityServices.GetBlockModelSpaceId(_acad.Database))
            {
                if (_acad.Database.TryGetValue(blockRef.Name, out string url))
                {
                    if (cts != null && cts.IsCancellationRequested) return false;
                    var atts = new List<AcadAttribDataHolder>();
                    var attIds = blockRef.AttributeCollection;

                    foreach (ObjectId attId in attIds)
                    {
                        if (cts != null && cts.IsCancellationRequested) return false;
                        if (_acad.Transaction.GetObject(attId, OpenMode.ForRead) is AttributeReference attrib)
                        {
                            atts.Add(new AcadAttribDataHolder
                            {
                                Tag = attrib.Tag,
                                TextString = attrib.TextString,
                                Invisible = attrib.Invisible,
                                ObjectId = attrib.ObjectId,
                            });
                        }
                    }
                    data = new AcadBlockDataHolder(blockRef.ObjectId, blockRef.Name, atts, url);
                    return true;
                }
            }
            return false;
        }
    }
}