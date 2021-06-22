using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Background;
using AgolPlugin.Views.Import;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AgolPlugin.ViewModels.Import
{
    public class LayerSelectorViewModel : ViewModelBase
    {
        public AgolClient Client { get; }

        private readonly AgolSearchResult _selectedResult;

        public static ReadOnlyDictionary<string, string> FieldTypeDict { get; private set; }
        static LayerSelectorViewModel()
        {
            FieldTypeDict = new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>
                {
                    { "esriFieldTypeBlob", null },
                    { "esriFieldTypeDate", "date" },
                    { "esriFieldTypeDouble", "dec" },
                    { "esriFieldTypeGeometry", null },
                    { "esriFieldTypeGlobalID", "guid" },
                    { "esriFieldTypeGUID", "guid" },
                    { "esriFieldTypeInteger", "int" },
                    { "esriFieldTypeOID", "int" },
                    { "esriFieldTypeRaster", null },
                    { "esriFieldTypeSingle", "dec" },
                    { "esriFieldTypeSmallInteger", "int" },
                    { "esriFieldTypeString", "string" },
                    { "esriFieldTypeXML", null },
                });
        }

        public BasicCommand BeginImport_Command { get; private set; }
        public BasicCommand NewFieldFilter_Command { get; private set; }
        public BasicCommand DeleteFieldFilter_Command { get; private set; }

#if DEBUG
        public LayerSelectorViewModel() { }
#endif
        public LayerSelectorViewModel(AgolSearchResult selectedResult)
        {
            _selectedResult = selectedResult;
            Client = new AgolClient(Plugin.CredContainer.Credential, Plugin.CredContainer.UrlKey);

            BeginImport_Command = new BasicCommand(BeginImport_Execute, BeginImport_CanExecute);
            NewFieldFilter_Command = new BasicCommand(NewFieldFilter_Execute, NewFieldFilter_CanExecute);
            DeleteFieldFilter_Command = new BasicCommand(DeleteFieldFilter_Execute);

            OutputSrid = OutputSrids.FirstOrDefault();
            Title = "Configure";

            Worker.DoWork(WorkerDoPopulate, WorkerFinishPopulate);

            SetupOperators();
        }

        private void SetupOperators()
        {
            ValueFilterOperators = new ObservableCollection<FilterOperator>(new[] { FilterOperators.EqualTo, FilterOperators.NotEqualTo, FilterOperators.LessThanEqualTo, FilterOperators.GreaterThanEqualTo, FilterOperators.LessThan, FilterOperators.GreaterThan, FilterOperators.Between });
            StringFilterOperators = new ObservableCollection<FilterOperator>(new[] { FilterOperators.EqualTo, FilterOperators.NotEqualTo, FilterOperators.Like });
        }

        #region Worker
        private void WorkerDoPopulate(object s, DoWorkEventArgs e)
        {
            IsBusy = true;
            var item = Client.GetServiceItem(_selectedResult.Url).Result;
            e.Result = item;
        }
        private void WorkerFinishPopulate(object s, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is AgolServiceItem item)
                ServiceItem = item;

            LayerConfigurators = new ObservableCollection<LayerConfiguratorViewModel>(ServiceItem.LayersAndTables.Select(l => new LayerConfiguratorViewModel(l, ServiceItem, () =>
            {
                BeginImport_Command.Refresh();
                NewFieldFilter_Command.Refresh();
            })));

            IsBusy = false;
        }
        #endregion

        #region Properties
        private AgolServiceItem _serviceItem;
        public AgolServiceItem ServiceItem
        {
            get { return _serviceItem; }
            set { _serviceItem = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LayerConfiguratorViewModel> _layerConfigurators;
        public ObservableCollection<LayerConfiguratorViewModel> LayerConfigurators
        {
            get { return _layerConfigurators; }
            set { _layerConfigurators = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FieldFilter> _fieldFilters = new ObservableCollection<FieldFilter>();
        public ObservableCollection<FieldFilter> FieldFilters
        {
            get { return _fieldFilters; }
            set { _fieldFilters = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Tuple<string, string>> _fieldFilterNames;
        public ObservableCollection<Tuple<string, string>> FieldFilterNames
        {
            get { return _fieldFilterNames; }
            set { _fieldFilterNames = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FilterOperator> _valueFilterOperators;
        public ObservableCollection<FilterOperator> ValueFilterOperators
        {
            get { return _valueFilterOperators; }
            set { _valueFilterOperators = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FilterOperator> _stringFilterOperators;
        public ObservableCollection<FilterOperator> StringFilterOperators
        {
            get { return _stringFilterOperators; }
            set { _stringFilterOperators = value; OnPropertyChanged(); }
        }

        private bool _convertElevationMetersToFeet = true;
        public bool ConvertElevationMetersToFeet
        {
            get { return _convertElevationMetersToFeet; }
            set { _convertElevationMetersToFeet = value; OnPropertyChanged(); }
        }

        private KeyValuePair<int, string> _outputSrid;
        public KeyValuePair<int, string> OutputSrid
        {
            get { return _outputSrid; }
            set { _outputSrid = value; OnPropertyChanged(); }
        }

        private LayerConfiguratorViewModel _selectedLayerConfig;
        public LayerConfiguratorViewModel SelectedLayerConfig
        {
            get { return _selectedLayerConfig; }
            set { _selectedLayerConfig = value; OnPropertyChanged(); }
        }

        private ReadOnlyDictionary<int, string> _outputSrids = EpsgCodes.Nad83;
        public ReadOnlyDictionary<int, string> OutputSrids
        {
            get { return _outputSrids; }
            set { _outputSrids = value; OnPropertyChanged(); }
        }

        private bool _allFeaturesLoaded;
        public bool AllFeaturesLoaded
        {
            get { return _allFeaturesLoaded; }
            set { _allFeaturesLoaded = value; OnPropertyChanged(); }
        }
        #endregion

        #region Interaction
        private void BeginImport_Execute(object param)
        {
            var win = new ImportProgressWindow(this);
            Plugin.ImportPage.ViewModel.GoToNextControl(ViewType.RunImport, null);
            Plugin.ToggleImportPanelVisibility(false);
            win.ShowDialog();
        }
        private bool BeginImport_CanExecute(object param)
        {
            var canExecute = LayerConfigurators != null
                          && LayerConfigurators.Count > 0
                          && !LayerConfigurators.Any(c => c.IsBusy)
                          && !FieldFilters.Any(f => !f.GetIsValid());

            if (canExecute && !AllFeaturesLoaded)
                OnAllFeaturesLoaded();

            return canExecute;
        }

        private void NewFieldFilter_Execute(object param)
        {
            FieldFilters.Add(new FieldFilter());
        }
        private bool NewFieldFilter_CanExecute(object param)
        {
            return AllFeaturesLoaded;
        }

        private void DeleteFieldFilter_Execute(object param)
        {
            if (param is FieldFilter filter)
            {
                FieldFilters.Remove(filter);
            }
        }
        #endregion

        #region Events
        private void OnAllFeaturesLoaded()
        {
            //var fieldListCollection = LayerConfigurators.Select(l => l.Feature.Fields.Select(f => new Tuple<string, EsriFieldType>(f.Name, (EsriFieldType)Enum.Parse(typeof(EsriFieldType), f.Type)))).ToList();
            //var intersected = fieldListCollection
            //    .Skip(1)
            //    .Aggregate(new HashSet<Tuple<string, EsriFieldType>>(fieldListCollection.First()),
            //    (h, e) => { h.IntersectWith(e); return h; }).ToList();

            FieldFilterNames = new ObservableCollection<Tuple<string, string>>(LayerConfigurators.First().Feature.Fields.Select(f => new Tuple<string, string>(f.Name, FieldTypeDict[f.Type])));
            AllFeaturesLoaded = true;
        }
        #endregion
    }
}