using Newtonsoft.Json.Linq;

namespace AgolPlugin.Models.Agol
{
    public class RecordFieldValue
    {
        public RecordFieldValue() { }
        public RecordFieldValue(FeatureField field, JValue jsonValue, string value)
        {
            Field = field;
            JsonValue = jsonValue;
            Value = value;
        }
        public FeatureField Field { get; set; }
        public JValue JsonValue { get; set; }
        public string Value { get; set; }
    }
}