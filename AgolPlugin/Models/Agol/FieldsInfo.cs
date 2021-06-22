using Newtonsoft.Json;

namespace AgolPlugin.Models.Agol
{
    public class FieldsInfo
    {
        [JsonProperty(PropertyName = "creationDateField")]
        public string CreationDateField { get; set; }

        [JsonProperty(PropertyName = "creatorField")]
        public string CreatorField { get; set; }

        [JsonProperty(PropertyName = "editDateField")]
        public string EditDateField { get; set; }

        [JsonProperty(PropertyName = "editorField")]
        public string EditorField { get; set; }
    }
}