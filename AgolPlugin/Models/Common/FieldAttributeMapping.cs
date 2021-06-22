using AgolPlugin.Models.Agol;

namespace AgolPlugin.Models.Common
{
    public class FieldAttributeMapping : ModelBase
    {
        public FieldAttributeMapping(AgolFeature feature, FeatureField field)
        {
            Feature = feature;
            Field = field;
            Field.Mapping = this;
            IsGlobalId = Field.Name == Feature.GlobalIdField;
            IsObjectId = Field.Name == Feature.ObjectIdField;
            CanDisable = !IsGlobalId && !IsObjectId;
        }

        private AgolFeature _feature;
        public AgolFeature Feature
        {
            get { return _feature; }
            set { _feature = value; OnPropertyChanged(); }
        }

        private FeatureField _field;
        public FeatureField Field
        {
            get { return _field; }
            set { _field = value; OnPropertyChanged(); }
        }

        private bool _include = true;
        public bool Include
        {
            get { return _include; }
            set { _include = value; OnPropertyChanged(); }
        }

        private bool _isGlobalId;
        public bool IsGlobalId
        {
            get { return _isGlobalId; }
            set { _isGlobalId = value; OnPropertyChanged(); }
        }

        private bool _isObjectId;
        public bool IsObjectId
        {
            get { return _isObjectId; }
            set { _isObjectId = value; OnPropertyChanged(); }
        }

        private bool _canDisable;
        public bool CanDisable
        {
            get { return _canDisable; }
            set { _canDisable = value; OnPropertyChanged(); }
        }

        private bool _useAsBlockLabel;
        public bool UseAsBlockLabel
        {
            get { return _useAsBlockLabel; }
            set { _useAsBlockLabel = value; OnPropertyChanged(); }
        }

        //private ObjectId _acadObjectId;
        //public ObjectId AcadObjectId
        //{
        //    get { return _acadObjectId; }
        //    set { _acadObjectId = value; OnPropertyChanged(); }
        //}
    }
}