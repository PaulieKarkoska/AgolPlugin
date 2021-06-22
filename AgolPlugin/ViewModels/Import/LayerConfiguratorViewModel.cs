using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Background;
using AgolPlugin.Validation.Acad;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Data;
using Forms = System.Windows.Forms;

namespace AgolPlugin.ViewModels.Import
{
    public class LayerConfiguratorViewModel : ViewModelBase
    {
        private AgolClient _agol;
        private readonly AgolServiceItem _selectedService;
        private readonly AgolFeature _initialFeature;
        private Action OnLoaded;

        public BasicCommand SelectMappings_Command { get; private set; }
        public BasicCommand DeselectAllLabels_Command { get; private set; }
        public BasicCommand SelectAcadLayerColor_Command { get; private set; }
        public BasicCommand SelectAcadBlockColor_Command { get; private set; }
        public BasicCommand GoToPreview_Command { get; private set; }


#if DEBUG
        public LayerConfiguratorViewModel() { }
#endif
        public LayerConfiguratorViewModel(AgolFeature feature, AgolServiceItem serviceItem, Action onLoaded = null)
        {
            OnLoaded = onLoaded;

            _initialFeature = feature;
            _selectedService = serviceItem;
            _agol = new AgolClient(Plugin.CredContainer.Credential, Plugin.CredContainer.UrlKey);

            SelectMappings_Command = new BasicCommand(SelectMappings_Execute, SelectMappings_CanExecute);
            DeselectAllLabels_Command = new BasicCommand(DeselectAllLabels_Execute, DeselectAllLabels_CanExecute);
            SelectAcadLayerColor_Command = new BasicCommand(SelectAcadLayerColor_Execute, SelectAcadLayerColor_CanExecute);
            SelectAcadBlockColor_Command = new BasicCommand(SelectAcadBlockColor_Execute, SelectAcadBlockColor_CanExecute);
            GoToPreview_Command = new BasicCommand(GoToPreview_Execute, GoToPreview_CanExecute);

            AcadLayerName = LayerNameValidator.RemoveInvalidLayerNameChars($"{_initialFeature.Name}${serviceItem.ServiceItemId}".ToUpper());
            NewAcadBlockName = $"{_initialFeature.Name}${serviceItem.ServiceItemId}";

            AcadLayerColor = new Color();
            NewAcadBlockColor = new Color();

            Worker.DoWork(WorkerDoPopulate, WorkerFinishPopulate);
        }

        #region Worker
        private void WorkerDoPopulate(object s, DoWorkEventArgs e)
        {
            IsBusy = true;

            var feature = _agol.GetFeatureLayer($"{_selectedService.ServiceUrl}/{_initialFeature.Id}", _selectedService).Result;
            var mappings = new ObservableCollection<FieldAttributeMapping>(
                feature.Fields.Select(f => new FieldAttributeMapping(feature, f))
                .OrderByDescending(m => m.IsGlobalId)
                .ThenByDescending(m => m.IsObjectId));

            e.Result = (feature, mappings);
        }
        private void WorkerFinishPopulate(object s, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ValueTuple<AgolFeature, ObservableCollection<FieldAttributeMapping>> data)
            {
                Feature = data.Item1;
                Mappings = data.Item2;
                _mappingsView = CollectionViewSource.GetDefaultView(Mappings);
                _mappingsView.Filter = MappingsFilter;
                _refreshTimer = new Timer(_timerDelay)
                {
                    AutoReset = false,
                };
                _refreshTimer.Elapsed += _refreshTimerElapsed;

            }
            IsBusy = false;
            OnLoaded?.Invoke();
        }
        #endregion

        #region Properties
        private string _mappingsFilterText;
        private string _mappingsFilterTextLower;
        public string MappingsFilterText
        {
            get { return _mappingsFilterText; }
            set
            {
                _mappingsFilterText = value;
                OnPropertyChanged();
                _mappingsFilterTextLower = value;
                //if (string.IsNullOrWhiteSpace(value))
                //    ClearMappingsFilter();
                //else
                FilterRefreshCountdown();
            }
        }
        private ObservableCollection<FieldAttributeMapping> _mappings;
        public ObservableCollection<FieldAttributeMapping> Mappings
        {
            get { return _mappings; }
            set { _mappings = value; OnPropertyChanged(); }
        }
        private ICollectionView _mappingsView;

        private PropertySortType _propertySortType = PropertySortType.None;
        public PropertySortType PropertySortType
        {
            get { return _propertySortType; }
            set { _propertySortType = value; OnPropertyChanged(); }
        }

        private AgolFeature _feature;
        public AgolFeature Feature
        {
            get { return _feature; }
            set { _feature = value; OnPropertyChanged(); }
        }

        private string _acadLayerName;
        public string AcadLayerName
        {
            get { return _acadLayerName; }
            set { _acadLayerName = value; OnPropertyChanged(); }
        }

        private Color _acadLayerColor;
        public Color AcadLayerColor
        {
            get { return _acadLayerColor; }
            set { _acadLayerColor = value; OnPropertyChanged(); }
        }

        private string _newAcadBlockName;
        public string NewAcadBlockName
        {
            get { return _newAcadBlockName; }
            set { _newAcadBlockName = value; OnPropertyChanged(); }
        }

        private Color _newAcadBlockColor;
        public Color NewAcadBlockColor
        {
            get { return _newAcadBlockColor; }
            set { _newAcadBlockColor = value; OnPropertyChanged(); }
        }

        private ObjectId _blockDefObjectId;
        public ObjectId BlockDefObjectId
        {
            get { return _blockDefObjectId; }
            set { _blockDefObjectId = value; OnPropertyChanged(); }
        }
        #endregion

        #region Interaction
        private void SelectMappings_Execute(object param)
        {
            if (param is string toggle)
            {
                foreach (var mapping in Mappings)
                {
                    if (mapping.CanDisable)
                        mapping.Include = toggle == "On" ? true : false;
                }
            }
        }
        private bool SelectMappings_CanExecute(object param)
        {
            return !IsBusy
                   && param is string;
        }

        private void DeselectAllLabels_Execute(object param)
        {
            foreach (var mapping in Mappings)
                mapping.UseAsBlockLabel = false;
        }
        private bool DeselectAllLabels_CanExecute(object param)
        {
            return !IsBusy;
        }

        private void SelectAcadLayerColor_Execute(object param)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == Forms.DialogResult.OK)
            {
                AcadLayerColor = dlg.Color;
            }
        }
        private bool SelectAcadLayerColor_CanExecute(object param)
        {
            return !IsBusy;
        }

        private void SelectAcadBlockColor_Execute(object param)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == Forms.DialogResult.OK)
            {
                NewAcadBlockColor = dlg.Color;
            }
        }
        private bool SelectAcadBlockColor_CanExecute(object param)
        {
            return !IsBusy;
        }

        private void GoToPreview_Execute(object param)
        {
            var errors = new List<string>();
            if (!new LayerNameValidator().Validate(AcadLayerName, out string layerError))
                errors.Add(layerError);

            if (!new BlockNameValidator().Validate(NewAcadBlockName, out string blockError))
                errors.Add(blockError);

            if (errors.Count > 0)
            {
                System.Windows.MessageBox.Show(string.Join("\n", errors), "Cannot Continue", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                return;
            }

            Plugin.ImportPage.ViewModel.GoToNextControl(ViewType.DataPreviewer, this);

        }
        private bool GoToPreview_CanExecute(object param)
        {
            return !IsBusy
                   && _selectedService != null
                   && _initialFeature != null;
        }
        #endregion

        #region Filtering / Refresh
        private Timer _refreshTimer;
        private int _timerDelay = 500;
        private void ClearMappingsFilter()
        {
            _refreshTimer.Stop();
            _mappingsView?.Refresh();
        }

        private void FilterRefreshCountdown()
        {
            _refreshTimer.Interval = _timerDelay;
            _refreshTimer.Start();
        }
        private void _refreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Plugin.ImportPage.Invoke(new Action(() => _mappingsView.Refresh()));
        }
        private bool MappingsFilter(object item)
        {
            var mapping = item as FieldAttributeMapping;

            if (string.IsNullOrWhiteSpace(_mappingsFilterTextLower) || !mapping.CanDisable)
                return true;
            return mapping.Field.Alias.ToLower().Contains(_mappingsFilterTextLower)
                || mapping.Field.Name.ToLower().Contains(_mappingsFilterTextLower);

            //return valid;
            //return CultureInfo.CurrentCulture.CompareInfo.IndexOf(mapping.Field.Alias, MappingsFilterText, CompareOptions.IgnoreCase) >= 0;
        }
        #endregion

        #region Events
        #endregion
    }
}