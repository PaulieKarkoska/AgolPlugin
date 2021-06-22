using AgolPlugin.Converters.Json;
using AgolPlugin.Models.Common;
using Newtonsoft.Json;
using System;

namespace AgolPlugin.Models.Agol
{
    public class AgolSearchResult : ModelBase
    {
        private string _id;
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _owner;
        [JsonProperty(PropertyName = "owner")]
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; OnPropertyChanged(); }
        }

        private DateTime? _created;
        [JsonProperty(PropertyName = "created")]
        [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
        public DateTime? Created
        {
            get { return _created; }
            set { _created = value; OnPropertyChanged(); }
        }

        private DateTime? _modified;
        [JsonProperty(PropertyName = "modified")]
        [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
        public DateTime? Modified
        {
            get { return _modified; }
            set
            {
                _modified = value;
                OnPropertyChanged();
            }
        }

        private bool _isOrgItem;
        [JsonProperty(PropertyName = "isOrgItem")]
        public bool IsOrgItem
        {
            get { return _isOrgItem; }
            set { _isOrgItem = value; OnPropertyChanged(); }
        }

        private Guid? _guid;
        [JsonProperty(PropertyName = "guid")]
        public Guid? Guid
        {
            get { return _guid; }
            set { _guid = value; OnPropertyChanged(); }
        }

        private string _name;
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _title;
        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        private string _itemType;
        [JsonProperty(PropertyName = "type")]
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; OnPropertyChanged(); }
        }

        private string[] _typeKeywords;
        [JsonProperty(PropertyName = "typeKeywords")]
        public string[] TypeKeywords
        {
            get { return _typeKeywords; }
            set { _typeKeywords = value; OnPropertyChanged(); }
        }

        private string _description;
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        private string[] _tags;
        [JsonProperty(PropertyName = "tags")]
        public string[] Tags
        {
            get { return _tags; }
            set { _tags = value; OnPropertyChanged(); }
        }

        private string _snippet;
        [JsonProperty(PropertyName = "snippet")]
        public string Snippet
        {
            get { return _snippet; }
            set { _snippet = value; OnPropertyChanged(); }
        }

        private string _url;
        [JsonProperty(PropertyName = "url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; OnPropertyChanged(); }
        }

        private string _thumbnailurl;
        public string ThumbnailUrl
        {
            get { return _thumbnailurl; }
            set { _thumbnailurl = value; OnPropertyChanged(); }
        }
    }
}