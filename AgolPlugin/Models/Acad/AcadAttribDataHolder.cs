using Autodesk.AutoCAD.DatabaseServices;

namespace AgolPlugin.Models.Acad
{
    public class AcadAttribDataHolder
    {
        public string Tag { get; set; }
        public string TextString { get; set; }
        public bool Invisible { get; set; }
        public ObjectId ObjectId { get; set; }
    }
}
