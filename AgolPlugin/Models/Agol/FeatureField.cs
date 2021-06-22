using AgolPlugin.Models.Common;
using Newtonsoft.Json;

namespace AgolPlugin.Models.Agol
{
    public class FeatureField : ModelBase
    {
        private string _name;
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _type;
        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        private string _alias;
        [JsonProperty(PropertyName = "alias")]
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; OnPropertyChanged(); }
        }

        private string _sqlType;
        [JsonProperty(PropertyName = "sqlType")]
        public string SqlType
        {
            get { return _sqlType; }
            set { _sqlType = value; OnPropertyChanged(); }
        }

        private int _length;
        [JsonProperty(PropertyName = "length")]
        public int Length
        {
            get { return _length; }
            set { _length = value; OnPropertyChanged(); }
        }

        private bool _nullable;
        [JsonProperty(PropertyName = "nullable")]
        public bool Nullable
        {
            get { return _nullable; }
            set { _nullable = value; OnPropertyChanged(); }
        }

        private bool _editable;
        [JsonProperty(PropertyName = "editable")]
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; OnPropertyChanged(); }
        }

        private object _defaultValue;
        [JsonProperty(PropertyName = "defaultValue")]
        public object DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; OnPropertyChanged(); }
        }

        public FieldAttributeMapping Mapping { get; set; }
    }
}