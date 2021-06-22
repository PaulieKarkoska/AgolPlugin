using AgolPlugin.Models.Common;
using Newtonsoft.Json;

namespace AgolPlugin.Models.Agol
{
    public class FeatureRelationship : ModelBase
    {
        public const string PARENT_GLOBALID_FIELD = "parentglobalid"; 

        private int _id;
        [JsonProperty(PropertyName = "id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _name;
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private int _relatedTableId;
        [JsonProperty(PropertyName = "relatedTableId")]
        public int RelatedTableId
        {
            get { return _relatedTableId; }
            set { _relatedTableId = value; OnPropertyChanged(); }
        }

        private string _keyField;
        [JsonProperty(PropertyName = "keyField")]
        public string KeyField
        {
            get { return _keyField; }
            set { _keyField = value; OnPropertyChanged(); }
        }
    }
}