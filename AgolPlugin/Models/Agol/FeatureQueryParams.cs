using System.Collections.Generic;
using System.Linq;

namespace AgolPlugin.Models.Agol
{
    public class FeatureQueryParams
    {
        public int? OutputSpatialReference = null;
        public bool CanRemove { get; set; } = true;
        public IEnumerable<FieldFilter> Filters { get; set; }
        public bool ConvertElevationMetersToFeet { get; set; }
        public string ToWhereClause(AgolFeature feature)
        {
            var validFields = new List<string>
            {
                feature.GlobalIdField,
                feature.ObjectIdField,
                feature.FieldsInfo.CreationDateField,
                feature.FieldsInfo.CreatorField,
                feature.FieldsInfo.EditDateField,
                feature.FieldsInfo.EditorField
            };

            validFields.AddRange(feature.Fields.Select(f => f.Name));
            var filterFields = Filters.Where(f => validFields.Contains(f.FieldName)).ToList();

            return filterFields.Count > 0 ? string.Join(" AND ", filterFields) : "1=1";
        }
    }
}