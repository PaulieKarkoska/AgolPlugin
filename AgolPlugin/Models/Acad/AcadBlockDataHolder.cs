using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AgolPlugin.Models.Acad
{
    public class AcadBlockDataHolder
    {
        public AcadBlockDataHolder(ObjectId objectId, string blockName, IList<AcadAttribDataHolder> attributes, string featureUrl)
        {
            AcadObjectId = objectId;
            BlockName = blockName;
            Attributes = attributes;
            FeatureUrl = featureUrl;
        }

        public ObjectId AcadObjectId { get; private set; }
        public string BlockName { get; private set; }
        public IList<AcadAttribDataHolder> Attributes { get; set; }
        public string FeatureUrl { get; private set; }
    }
}