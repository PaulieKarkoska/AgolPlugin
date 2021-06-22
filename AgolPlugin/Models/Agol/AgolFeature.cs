using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AgolPlugin.Models.Agol
{
    public class AgolFeature : ModelBase
    {
        public const string TYPE_LAYER = "Feature Layer";
        public const string TYPE_TABLE = "Table";

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

        private string _featureType;
        [JsonProperty(PropertyName = "type")]
        public string FeatureType
        {
            get { return _featureType; }
            set { _featureType = value; OnPropertyChanged(); }
        }

        private string _serviceItemId;
        [JsonProperty(PropertyName = "serviceItemId")]
        public string ServiceItemId
        {
            get { return _serviceItemId; }
            set { _serviceItemId = value; OnPropertyChanged(); }
        }

        private string _displayField;
        [JsonProperty(PropertyName = "displayField")]
        public string DisplayField
        {
            get { return _displayField; }
            set { _displayField = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FeatureRelationship> _relationships;
        [JsonProperty(PropertyName = "relationships")]
        public ObservableCollection<FeatureRelationship> Relationships
        {
            get { return _relationships; }
            set { _relationships = value; OnPropertyChanged(); }
        }

        private FieldsInfo _fieldsInfo;
        [JsonProperty(PropertyName = "editFieldsInfo")]
        public FieldsInfo FieldsInfo
        {
            get { return _fieldsInfo; }
            set { _fieldsInfo = value; OnPropertyChanged(); }
        }

        private int? _parentLayerId;
        [JsonProperty(PropertyName = "parentLayerId")]
        public int? ParentLayerId
        {
            get { return _parentLayerId; }
            set { _parentLayerId = value; OnPropertyChanged(); }
        }

        private string _objectIdField;
        [JsonProperty(PropertyName = "objectIdField")]
        public string ObjectIdField
        {
            get { return _objectIdField; }
            set { _objectIdField = value; OnPropertyChanged(); }
        }

        private string _globalIdField;
        [JsonProperty(PropertyName = "globalIdField")]
        public string GlobalIdField
        {
            get { return _globalIdField; }
            set { _globalIdField = value; OnPropertyChanged(); }
        }

        private string _typeIdField;
        [JsonProperty(PropertyName = "typeIdField")]
        public string TypeIdField
        {
            get { return _typeIdField; }
            set { _typeIdField = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FeatureField> _fields;
        [JsonProperty(PropertyName = "fields")]
        public ObservableCollection<FeatureField> Fields
        {
            get { return _fields; }
            set { _fields = value; OnPropertyChanged(); }
        }

        private bool _hasAttachments;
        [JsonProperty(PropertyName = "hasAttachments")]
        public bool HasAttachments
        {
            get { return _hasAttachments; }
            set { _hasAttachments = value; OnPropertyChanged(); }
        }

        private bool _recordsAreLoaded;
        [JsonIgnore]
        public bool RecordsAreLoaded
        {
            get { return _recordsAreLoaded; }
            set { _recordsAreLoaded = value; OnPropertyChanged(); }
        }

        private string _featureUrl;
        public string FeatureUrl
        {
            get { return _featureUrl; }
            set { _featureUrl = value; OnPropertyChanged(); }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; OnPropertyChanged(); }
        }

        private ReadOnlyDictionary<string, FeatureField> _fieldDict;
        /// <param name="fieldName">The name of the field</param>
        /// <returns>The <seealso cref="FeatureField"/> of the given name, or null if one cannot be found</returns>
        public FeatureField this[string fieldName]
        {
            get
            {
                if (_fieldDict == null)
                    _fieldDict = new ReadOnlyDictionary<string, FeatureField>(Fields.ToDictionary(f => f.Name, f => f));

                return _fieldDict[fieldName];
            }
        }

        public LayerSelectorViewModel ParentViewModel { get; set; }

        public AgolServiceItem ServiceItem { get; set; }

        public List<AgolRecord> Records { get; set; }
    }
}