using AgolPlugin.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace AgolPlugin.Models.Agol
{
    public class AgolSearchResultCollection : ModelBase
    {
        private string _query;
        [JsonProperty(PropertyName = "query")]
        public string Query
        {
            get { return _query; }
            set { _query = value; OnPropertyChanged(); }
        }

        private int _total;
        [JsonProperty(PropertyName = "total")]
        public int Total
        {
            get { return _total; }
            set { _total = value; OnPropertyChanged(); }
        }

        private int _start;
        [JsonProperty(PropertyName = "start")]
        public int Start
        {
            get { return _start; }
            set { _start = value; OnPropertyChanged(); }
        }

        private int _resultDisplayCount;
        [JsonProperty(PropertyName = "num")]
        public int ResultDisplayCount
        {
            get { return _resultDisplayCount; }
            set { _resultDisplayCount = value; OnPropertyChanged(); }
        }

        private int _nextStart;
        [JsonProperty(PropertyName = "nextStart")]
        public int NextStart
        {
            get { return _nextStart; }
            set { _nextStart = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public int CurrentPage
        {
            get
            {
                return (Start / ResultDisplayCount) + 1;
            }
        }

        [JsonIgnore]
        public int TotalPages
        {
            get
            {
                return Convert.ToInt32(
                    Math.Ceiling(
                        Convert.ToDouble(Total) / Convert.ToDouble(ResultDisplayCount)));
            }
        }

        private ObservableCollection<AgolSearchResult> _searchResults;
        [JsonProperty(PropertyName = "results")]
        public ObservableCollection<AgolSearchResult> SearchResults
        {
            get { return _searchResults; }
            set { _searchResults = value; OnPropertyChanged(); }
        }


    }
}
