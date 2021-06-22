using AgolPlugin.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgolPlugin.Models.Agol
{
    public class AgolServiceItem : ModelBase
    {
        private string _serviceItemId;
        [JsonProperty(PropertyName = "serviceItemId")]
        public string ServiceItemId
        {
            get { return _serviceItemId; }
            set { _serviceItemId = value; OnPropertyChanged(); }
        }

        private string _serviceDescription;
        [JsonProperty(PropertyName = "serviceDescription")]
        public string ServiceDescription
        {
            get { return _serviceDescription; }
            set { _serviceDescription = value; OnPropertyChanged(); }
        }

        private int _maxRecordCount;
        [JsonProperty(PropertyName = "maxRecordCount")]
        public int MaxRecordCount
        {
            get { return _maxRecordCount; }
            set { _maxRecordCount = value; OnPropertyChanged(); }
        }

        private string _description;
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AgolFeature> _layers;
        [JsonProperty(PropertyName = "layers")]
        public ObservableCollection<AgolFeature> Layers
        {
            get { return _layers; }
            set { _layers = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AgolFeature> _tables;
        [JsonProperty(PropertyName = "tables")]
        public ObservableCollection<AgolFeature> Tables
        {
            get { return _tables; }
            set { _tables = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AgolFeature> _layersAndTables;
        public ObservableCollection<AgolFeature> LayersAndTables
        {
            get
            {
                if (_layersAndTables == null)
                    _layersAndTables = new ObservableCollection<AgolFeature>(Layers.Concat(Tables));

                return _layersAndTables;
            }
            set { _layersAndTables = value; OnPropertyChanged(); }
        }
    }
}
