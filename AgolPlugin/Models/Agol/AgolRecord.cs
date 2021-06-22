using AgolPlugin.Models.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace AgolPlugin.Models.Agol
{
    public class AgolRecord : ModelBase
    {
        private AgolFeature _parentFeature;
        public AgolFeature ParentFeature
        {
            get { return _parentFeature; }
            set { _parentFeature = value; OnPropertyChanged(); }
        }

        private AgolGeometry? _geometry;
        public AgolGeometry? Geometry
        {
            get { return _geometry; }
            set { _geometry = value; OnPropertyChanged(); }
        }

        private ObservableCollection<RecordFieldValue> _fieldValues = new ObservableCollection<RecordFieldValue>();
        public ObservableCollection<RecordFieldValue> FieldValues
        {
            get { return _fieldValues; }
            set { _fieldValues = value; OnPropertyChanged(); }
        }

        private AgolRecordGroupCollection _relatedRecords = new AgolRecordGroupCollection();
        public AgolRecordGroupCollection RelatedRecords
        {
            get { return _relatedRecords; }
            set { _relatedRecords = value; OnPropertyChanged(); }
        }

        private ReadOnlyDictionary<string, RecordFieldValue> _valueDict;
        public RecordFieldValue this[string fieldName]
        {
            get
            {
                if (_valueDict == null)
                    _valueDict = new ReadOnlyDictionary<string, RecordFieldValue>(FieldValues.ToDictionary(f => f.Field.Name, f => f));
                return _valueDict.TryGetValue(fieldName, out RecordFieldValue val) ? val : null;
            }
        }

        private ObservableCollection<MediaAttachment> _attachments;
        public ObservableCollection<MediaAttachment> Attachments
        {
            get { return _attachments; }
            set { _attachments = value; OnPropertyChanged(); }
        }
    }
}