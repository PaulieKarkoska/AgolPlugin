using AgolPlugin.Models.Common;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace AgolPlugin.Models.Agol
{
    public class MediaAttachment : ModelBase
    {
        public static readonly string[] ValidExtensions = new[] { ".jpg", ".jpeg" };

        private string _url;
        [JsonProperty("url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; OnPropertyChanged(); }
        }
        private string _globalId;
        [JsonProperty("globalId")]
        public string GlobalId
        {
            get { return _globalId; }
            set { _globalId = value; OnPropertyChanged(); }
        }
        private int id;
        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(); }
        }
        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private long _size;
        [JsonProperty("size")]
        public long Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged(); }
        }
        private string _contentType;
        [JsonProperty("contentType")]
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; OnPropertyChanged(); }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private bool _isSupportedFileType;
        public bool IsSupportedFileType
        {
            get { return _isSupportedFileType; }
            set { _isSupportedFileType = value; OnPropertyChanged(); }
        }

        private AgolRecord _parentRecord;
        public AgolRecord ParentRecord
        {
            get { return _parentRecord; }
            set { _parentRecord = value; OnPropertyChanged(); }
        }
    }
}