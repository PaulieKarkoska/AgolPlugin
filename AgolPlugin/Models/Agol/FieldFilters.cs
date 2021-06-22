namespace AgolPlugin.Models.Agol
{
    public static class FieldFilters
    {
        public static FieldFilter[] ObjectIdFilter(string objectIdField, int objectId)
        {
            return new[] { new FieldFilter { FieldName = objectIdField, Operator = FilterOperators.EqualTo, Value1 = objectId, FieldType = "int" } };
        }

        public static FieldFilter[] ParentObjectIdFilter(string parentGlobalId)
        {
            return new[] { new FieldFilter { FieldName = FeatureRelationship.PARENT_GLOBALID_FIELD, Operator = FilterOperators.EqualTo, Value1 = parentGlobalId, FieldType = "guid" } };
        }
    }
}