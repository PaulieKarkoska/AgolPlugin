using AgolPlugin.Models.Common;
using System.Collections;

namespace AgolPlugin.Helpers
{
    public class FieldAttributeMappingComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var a = x as FieldAttributeMapping;
            var b = y as FieldAttributeMapping;

            int result = a.IsGlobalId.CompareTo(b.IsGlobalId);
            if (result == 0)
                result = a.IsObjectId.CompareTo(b.IsObjectId);
            if (result == 0)
                result = a.Field.Alias.CompareTo(b.Field.Alias);
            return result;
        }
    }
}