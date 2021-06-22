using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Acad;
using AgolPlugin.Services.Background;
using AgolPlugin.Services.Extensions;
using Autodesk.AutoCAD.ApplicationServices;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AgolPlugin.ViewModels.Import
{
    public class ImportProgressViewModel : ViewModelBase
    {
        private readonly LayerSelectorViewModel _selectorViewModel;
        private object _lock = new object();

        public BasicCommand CloseWindow_Command { get; private set; }

#if DEBUG
        public ImportProgressViewModel() { }
#endif
        public ImportProgressViewModel(LayerSelectorViewModel selectorViewModel)
        {
            _selectorViewModel = selectorViewModel;

            var featureLayerCount = _selectorViewModel.LayerConfigurators.Count(c => c.Feature.FeatureType == AgolFeature.TYPE_LAYER);
            TotalLayerCount = featureLayerCount;
            TotalBlockDefCount = featureLayerCount;

            CloseWindow_Command = new BasicCommand(CloseWindow_Execute);
        }

        public void Run()
        {
            Worker.DoWork(WorkerDoImport, WorkerFinishImport);
        }

        #region Properties
        private ImportStatus _layerCreationStatus = ImportStatus.Pending;
        public ImportStatus LayerCreationStatus
        {
            get
            {
                lock (_lock)
                {
                    return _layerCreationStatus;
                }
            }
            set
            {
                lock (_lock)
                {
                    _layerCreationStatus = value; OnPropertyChanged();
                }
            }
        }
        private int _currentLayerCount;
        public int CurrentLayerCount
        {
            get
            {
                lock (_lock)
                {
                    return _currentLayerCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentLayerCount = value; OnPropertyChanged();
                }
            }
        }
        private int _totalLayerCount;
        public int TotalLayerCount
        {
            get
            {
                lock (_lock)
                {
                    return _totalLayerCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _totalLayerCount = value; OnPropertyChanged();
                }
            }
        }

        private ImportStatus _blockDefCreationStatus = ImportStatus.Pending;
        public ImportStatus BlockDefCreationStatus
        {
            get
            {
                lock (_lock)
                {
                    return _blockDefCreationStatus;
                }
            }
            set
            {
                lock (_lock)
                {
                    _blockDefCreationStatus = value; OnPropertyChanged();
                }
            }
        }
        private int _currentBlockDefCount;
        public int CurrentBlockDefCount
        {
            get
            {
                lock (_lock)
                {
                    return _currentBlockDefCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentBlockDefCount = value; OnPropertyChanged();
                }
            }
        }
        private int _totalBlockDefCount;
        public int TotalBlockDefCount
        {
            get
            {
                lock (_lock)
                {
                    return _totalBlockDefCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _totalBlockDefCount = value; OnPropertyChanged();
                }
            }
        }

        private ImportStatus _recordExtractStatus = ImportStatus.Pending;
        public ImportStatus RecordExtractStatus
        {
            get
            {
                lock (_lock)
                {
                    return _recordExtractStatus;
                }
            }
            set
            {
                lock (_lock)
                {
                    _recordExtractStatus = value; OnPropertyChanged();
                }
            }
        }
        private int _currentRecordExtractCount;
        public int CurrentRecordExtractCount
        {
            get
            {
                lock (_lock)
                {
                    return _currentRecordExtractCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentRecordExtractCount = value; OnPropertyChanged();
                }
            }
        }
        //private int _totalRecordExtractCount;
        //public int TotalRecordExtractCount
        //{
        //    get
        //    {
        //        lock (_lock)
        //        {
        //            return _totalRecordExtractCount;
        //        }
        //    }
        //    set
        //    {
        //        lock (_lock)
        //        {
        //            _totalRecordExtractCount = value; OnPropertyChanged();
        //        }
        //    }
        //}

        private ImportStatus _blockRefCreationStatus = ImportStatus.Pending;
        public ImportStatus BlockRefCreationStatus
        {
            get
            {
                lock (_lock)
                {
                    return _blockRefCreationStatus;
                }
            }
            set
            {
                lock (_lock)
                {
                    _blockRefCreationStatus = value; OnPropertyChanged();
                }
            }
        }
        private int _currentBlockRefCount;
        public int CurrentBlockRefCount
        {
            get
            {
                lock (_lock)
                {
                    return _currentBlockRefCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentBlockRefCount = value; OnPropertyChanged();
                }
            }
        }
        private int _totalBlockRefCount;
        public int TotalBlockRefCount
        {
            get
            {
                lock (_lock)
                {
                    return _totalBlockRefCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    _totalBlockRefCount = value; OnPropertyChanged();
                }
            }
        }

        private ImportStatus _overallStatus = ImportStatus.Working;
        public ImportStatus OverallStatus
        {
            get
            {
                lock (_lock)
                {
                    return _overallStatus;
                }
            }
            set
            {
                lock (_lock)
                {
                    _overallStatus = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanClose));
                }
            }
        }

        public bool CanClose
        {
            get
            {
                lock (_lock)
                {
                    return OverallStatus == ImportStatus.Complete;
                }
            }
        }
        #endregion

        #region Interaction
        private void CloseWindow_Execute(object param)
        {
            if (param is string response && response == "Yes")
                Application.DocumentManager.MdiActiveDocument.SendStringToExecute("_.ZOOM _E ", false, false, false);

            OnCloseWindowCalled();
        }
        #endregion

        #region Worker
        private void WorkerDoImport(object s, DoWorkEventArgs e)
        {
            var configs = _selectorViewModel.LayerConfigurators.Where(c => c.Feature.FeatureType == AgolFeature.TYPE_LAYER).ToList();

            using (var acad = new AcadService())
            {
                var writer = new AcadWriter(acad);
                Plugin.ImportPage.Invoke(() => LayerCreationStatus = ImportStatus.Working);
                writer.LayersFromConfigs(configs, () =>
                {
                    Plugin.ImportPage.Invoke(() => CurrentLayerCount++);
                });
                Plugin.ImportPage.Invoke(() => LayerCreationStatus = ImportStatus.Complete);

                Plugin.ImportPage.Invoke(() => BlockDefCreationStatus = ImportStatus.Working);
                writer.BlockDefsFromConfigs(configs, () =>
                {
                    Plugin.ImportPage.Invoke(() => CurrentBlockDefCount++);
                });
                Plugin.ImportPage.Invoke(() => BlockDefCreationStatus = ImportStatus.Complete);

                Plugin.ImportPage.Invoke(() => RecordExtractStatus = ImportStatus.Working);
                void onRecordExtract()
                {
                    Plugin.ImportPage.Invoke(() => CurrentRecordExtractCount++);
                };
                foreach (var config in configs)
                {
                    var parentFeature = config.Feature.ParentLayerId == null ? null : configs.FirstOrDefault(c => c.Feature.Id == config.Feature.ParentLayerId)?.Feature;
                    _selectorViewModel.Client.GetRecords(config.Feature, new FeatureQueryParams
                    {
                        OutputSpatialReference = _selectorViewModel.OutputSrid.Key,
                        Filters = _selectorViewModel.FieldFilters,
                        ConvertElevationMetersToFeet = _selectorViewModel.ConvertElevationMetersToFeet
                    },
                    parentFeature,
                    onRecordExtract).Wait();
                }
                Plugin.ImportPage.Invoke(() => RecordExtractStatus = ImportStatus.Complete);

                Plugin.ImportPage.Invoke(() => BlockRefCreationStatus = ImportStatus.Working);
                Plugin.ImportPage.Invoke(() => TotalBlockRefCount = configs.Sum(c => c.Feature.Records.Count));
                writer.AddBlockReferences(configs, () =>
                {
                    Plugin.ImportPage.Invoke(() => CurrentBlockRefCount++);
                });

                writer.SetPdmodeAndSize();
                Plugin.ImportPage.Invoke(() => BlockRefCreationStatus = ImportStatus.Complete);

                var links = new Dictionary<string, string>();

                acad.Database.SetValues(configs.ToDictionary(c => c.NewAcadBlockName, c => $"{c.Feature.ServiceItem.ServiceUrl}/{c.Feature.Id}"));
                acad.Database.SetValue(DwgPropsExtension.OUTPUT_SRID_KEY, _selectorViewModel.OutputSrid.Key.ToString());
            }

            using (var acad = new AcadService())
                foreach (var config in configs)
                {
                    acad.Document.SendStringToExecute($"._ATTSYNC _NAME {config.NewAcadBlockName}\n", false, false, false);
                }
        }
        private void WorkerFinishImport(object s, RunWorkerCompletedEventArgs e)
        {
            OverallStatus = ImportStatus.Complete;
        }
        #endregion

        #region Events
        public event EventHandler CloseWindowCalled;
        private void OnCloseWindowCalled()
        {
            CloseWindowCalled?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}