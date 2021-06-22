using AgolPlugin.Helpers;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Acad;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Extensions;
using AgolPlugin.Services.Geo;
using AgolPlugin.Services.MapTiles;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AgolPlugin.Helpers.AzureTileMath;
using acad = Autodesk.AutoCAD.ApplicationServices;

namespace AgolPlugin.ViewModels.AerialBase
{
    public class AerialBaseViewModel : ViewModelBase
    {
        private const string _azureMapsEnvKey = "AZURE_MAPS_SUB_KEY";
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public BasicCommand SelectExtents_Command { get; private set; }
        public BasicCommand BrowseForSavePath_Command { get; private set; }
        public BasicCommand BeginImport_Command { get; private set; }
        public BasicCommand CancelImport_Command { get; private set; }

        public AerialBaseViewModel()
        {
            BrowseForSavePath_Command = new BasicCommand(BrowseForSavePath_Execute, BrowseForSavePath_CanExecute);
            SelectExtents_Command = new BasicCommand(SelectExtents_Execute, SelectExtents_CanExecute);
            BeginImport_Command = new BasicCommand(BeginImport_Execute, BeginImport_CanExecute);
            CancelImport_Command = new BasicCommand(CancelImport_Execute, CancelImport_CanExecute);

            if (Properties.Settings.Default.RememberAzureMapsSubKey)
            {
                if (TryGetSubKey(out string key))
                    AzureMapsSubKey = key;
            }

            using (var acad = new AcadService())
            {
                TileImageSavePath = Path.Combine(acad.GetActiveDrawingDirectory(), "Imagery", "Aerial.png");

                if (acad.Database.TryGetValue("EPSG Code", out string epsgCode)
                    && int.TryParse(epsgCode, out int epsgCodeInt)
                    && EpsgCodes.Nad83.ContainsKey(epsgCodeInt))
                {
                    SelectedSrid = EpsgCodes.Nad83.FirstOrDefault(n => n.Key == epsgCodeInt);
                }
                else
                {
                    SelectedSrid = EpsgCodes.Nad83.First();
                }
            }
            TileParamsUpdated += ForceRecalcTiles;
        }

        #region Properties
        public bool IsCancelling
        {
            get { return _cts.IsCancellationRequested; }
        }

        private int _progressCount;
        public int ProgressCount
        {
            get { return _progressCount; }
            set { _progressCount = value; OnPropertyChanged(); }
        }

        private KeyValuePair<int, string> _sourceSrid;
        public KeyValuePair<int, string> SourceSrid
        {
            get { return _sourceSrid; }
            set { _sourceSrid = value; OnPropertyChanged(); }
        }

        private int _zoomLevel = 13;
        public int ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                _zoomLevel = value;
                OnPropertyChanged();
                OnTileParamsUpdated();
            }
        }

        private ObservableCollection<string> _quads = new ObservableCollection<string>();
        public ObservableCollection<string> Quads
        {
            get { return _quads; }
            set { _quads = value; OnPropertyChanged(); }
        }

        private string _azureMapsSubKey;
        public string AzureMapsSubKey
        {
            get { return _azureMapsSubKey; }
            set { _azureMapsSubKey = value; OnPropertyChanged(); }
        }

        public bool RememberAzureMapsSubKey
        {
            get { return Properties.Settings.Default.RememberAzureMapsSubKey; }
            set
            {
                Properties.Settings.Default.RememberAzureMapsSubKey = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public ReadOnlyDictionary<int, string> SridList
        {
            get { return EpsgCodes.Nad83; }
        }

        private KeyValuePair<int, string>? _selectedSrid;
        public KeyValuePair<int, string>? SelectedSrid
        {
            get { return _selectedSrid; }
            set
            {
                _selectedSrid = value;
                OnPropertyChanged();
                SelectedEpsgCode = value != null ? value.Value.Key : (int?)null;
            }
        }

        private string _tileImageSavePath;
        public string TileImageSavePath
        {
            get { return _tileImageSavePath; }
            set { _tileImageSavePath = value; OnPropertyChanged(); }
        }

        private int? _selectedEpsgCode;
        public int? SelectedEpsgCode
        {
            get { return _selectedEpsgCode; }
            private set
            {
                _selectedEpsgCode = value;
                SetExtentsWgs84();
                OnPropertyChanged();
                OnTileParamsUpdated();
            }
        }

        private Extents3d? _selectionExtents = null;
        public Extents3d? SelectionExtents
        {
            get { return _selectionExtents; }
            set
            {
                _selectionExtents = value;
                OnPropertyChanged();
                SetExtentsWgs84();
            }
        }

        private Extents3d? _extentsWgs84;
        public Extents3d? ExtentsWgs84
        {
            get { return _extentsWgs84; }
            set
            {
                _extentsWgs84 = value;
                OnPropertyChanged();
                OnTileParamsUpdated();
                BeginImport_Command.Refresh();
            }
        }

        private bool _useHdTiles = false;
        public bool UseHdTiles
        {
            get { return _useHdTiles; }
            set { _useHdTiles = value; OnPropertyChanged(); }
        }
        #endregion

        #region Interaction
        private void CancelImport_Execute(object param)
        {
            _cts.Cancel();
        }
        private bool CancelImport_CanExecute(object param)
        {
            return IsBusy
                && !_cts.IsCancellationRequested;
        }

        private void BrowseForSavePath_Execute(object param)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Select merged tile image",
                Filter = "PNG Image|*.png",
                DefaultExt = "png",
                RestoreDirectory = true,
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TileImageSavePath = dlg.FileName;
            }
        }
        private bool BrowseForSavePath_CanExecute(object param)
        {
            return !IsBusy;
        }

        private void SelectExtents_Execute(object param)
        {
            using (var acad = new AcadService())
            {
                var pso = new PromptSelectionOptions
                {
                    MessageForAdding = "\nSelect region for imagery",
                    AllowDuplicates = false,
                    AllowSubSelections = false,
                    RejectObjectsFromNonCurrentSpace = true,
                    RejectObjectsOnLockedLayers = true,
                };

                var selResult = acad.Editor.GetSelection(pso);
                if (selResult.Status != PromptStatus.OK || selResult.Value.Count != 1)
                {
                    SelectionExtents = null;
                    ExtentsWgs84 = null;
                    return;
                }

                var ent = acad.Transaction.GetObject(selResult.Value[0].ObjectId, OpenMode.ForRead);
                var name = RXClass.GetClass(ent.GetType()).Name;
                if (name != "AcDbPolyline")
                {
                    System.Windows.MessageBox.Show("The selected entity was not the correct type");
                    return;
                }
                SelectionExtents = ((Polyline)ent).GeometricExtents;
            }
        }
        private bool SelectExtents_CanExecute(object param)
        {
            return true;
        }

        private async void BeginImport_Execute(object param)
        {
            IsBusy = true;
            SaveSubKey();
            bool plur = Quads.Count != 1;
            var dlg = await new DialogCoordinator().ShowProgressAsync(this, $"Importing {Quads.Count} tile{(plur ? "s" : "")}", $"Please wait while tile{(plur ? "s are" : " is")} downloaded");
            dlg.Maximum = Quads.Count;
            dlg.Minimum = 0;
            dlg.SetCancelable(true);
            dlg.Canceled += Progress_Cancelled;

            await Task.Run(async () =>
            {
                ForceRecalcTiles();
                var svc = new AzureMapTileProvider(AzureMapsSubKey);
                var dir = Path.GetDirectoryName(TileImageSavePath);
                Directory.CreateDirectory(dir);
                var imageInfos = svc.GetImageInfos(Quads, dir, out HashSet<int> xVals, out HashSet<int> yVals, out int minX, out int maxX, out int minY, out int maxY, _cts, () => { dlg.SetProgress(++ProgressCount); });

                bool didTilesMerge = svc.MergeTileImages(imageInfos, svc.TileSize, xVals.Count, yVals.Count, TileImageSavePath);

                //Orient and scale image
                var bl = TileXYToBoundingBox(minX, maxY, ZoomLevel, svc.TileSize);
                var tr = TileXYToBoundingBox(maxX, minY, ZoomLevel, svc.TileSize);

                bl = GeoConverter.ReprojectFromWgs84(new[] { bl[0], bl[1] }, (int)SelectedEpsgCode);
                tr = GeoConverter.ReprojectFromWgs84(new[] { tr[2], tr[3] }, (int)SelectedEpsgCode);

                var left = bl[0];
                var bot = bl[1];
                var right = tr[0];
                var top = tr[1];

                var mapH = top - bot;
                var mapW = right - left;

                var uCorner = new Vector3d(mapW, 0, 0);
                var vOnPlane = new Vector3d(0, mapH, 0);
                var orientation = new CoordinateSystem3d(new Point3d(bl[0], bl[1], 0), uCorner, vOnPlane);

                BindImage(orientation);

                await dlg.CloseAsync();
                IsBusy = false;
            });

            if (File.Exists(TileImageSavePath))
                OnProcessSuccess();
            else
                OnProcessFailed();
        }
        private bool BeginImport_CanExecute(object param)
        {
            //TODO: Fix Bug - Why is this not being hit during subsequent runs, resulting in failure to refresh command and not allowing user to start a subsequent import?
            return ExtentsWgs84 != null
                && Quads.Count > 0
                && SelectedEpsgCode != null
                && !string.IsNullOrWhiteSpace(AzureMapsSubKey)
                && !IsBusy;
        }

        private void Progress_Cancelled(object sender, EventArgs e)
        {
            _cts.Cancel();
        }
        #endregion

        #region Misc
        private void ForceRecalcTiles(object sender = null, EventArgs e = null)
        {
            if (ExtentsWgs84 != null)
            {
                var ex = (Extents3d)ExtentsWgs84;
                var bBox = new[] { ex.MinPoint.X, ex.MinPoint.Y, ex.MaxPoint.X, ex.MaxPoint.Y };
                Quads = new ObservableCollection<string>(GetQuadkeysInBoundingBox(bBox, ZoomLevel, UseHdTiles ? 512 : 256));
            }
            else
            {
                Quads = new ObservableCollection<string>();
            }
        }
        private bool TryGetSubKey(out string key)
        {
            key = Environment.GetEnvironmentVariable(_azureMapsEnvKey, EnvironmentVariableTarget.Machine);
            return !string.IsNullOrEmpty(key);
        }
        private void SaveSubKey()
        {
            if (RememberAzureMapsSubKey)
            {
                Environment.SetEnvironmentVariable(_azureMapsEnvKey, AzureMapsSubKey, EnvironmentVariableTarget.Machine);
            }
            else
            {
                Environment.SetEnvironmentVariable(_azureMapsEnvKey, string.Empty, EnvironmentVariableTarget.Machine);
            }
        }
        private void SetExtentsWgs84()
        {
            if (SelectionExtents != null && SelectedEpsgCode != null)
            {
                ExtentsWgs84 = Reprojector.ReprojectToWgs84((Extents3d)SelectionExtents, (int)SelectedEpsgCode);
            }
        }

        private void BindImage(CoordinateSystem3d orientation)
        {
            if (File.Exists(TileImageSavePath))
            {
                var doc = acad.Application.DocumentManager.MdiActiveDocument;
                var db = doc.Database;

                using (var trans = db.TransactionManager.StartTransaction())
                {
                    using (var docLock = doc.LockDocument())
                    {
                        try
                        {
                            RasterImageDef imgDef;
                            ObjectId imgDefId;

                            var dictName = Path.GetFileNameWithoutExtension(TileImageSavePath);
                            var imgDictId = RasterImageDef.GetImageDictionary(db);
                            if (imgDictId.IsNull)
                                imgDictId = RasterImageDef.CreateImageDictionary(db);

                            var imgDict = (DBDictionary)trans.GetObject(imgDictId, OpenMode.ForRead);

                            if (imgDict.Contains(dictName))
                            {
                                imgDefId = imgDict.GetAt(dictName);
                                imgDef = (RasterImageDef)trans.GetObject(imgDefId, OpenMode.ForWrite);
                            }
                            else
                            {
                                imgDef = new RasterImageDef();
                                imgDef.SourceFileName = TileImageSavePath;

                                imgDef.Load();
                                imgDict.UpgradeOpen();
                                imgDefId = imgDict.SetAt(dictName, imgDef);

                                trans.AddNewlyCreatedDBObject(imgDef, true);
                            }

                            var img = new RasterImage();
                            img.ImageDefId = imgDefId;
                            img.Orientation = orientation;

                            img.ImageTransparency = true;
                            img.ShowImage = true;

                            var bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                            var ms = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                            ms.AppendEntity(img);
                            trans.AddNewlyCreatedDBObject(img, true);

                            RasterImage.EnableReactors(true);
                            img.AssociateRasterDef(imgDef);

                            trans.Commit();
                        }
                        catch (System.Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Failed to bind image to drawing\n\n{ex.Message}");
                        }
                    }
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler ProcessSucceeded;
        private void OnProcessSuccess()
        {
            ProcessSucceeded?.Invoke(this, new EventArgs());
        }
        public event EventHandler ProcessFailed;
        private void OnProcessFailed()
        {
            ProcessFailed?.Invoke(this, new EventArgs());
        }
        public event EventHandler TileParamsUpdated;
        private void OnTileParamsUpdated()
        {
            TileParamsUpdated?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}