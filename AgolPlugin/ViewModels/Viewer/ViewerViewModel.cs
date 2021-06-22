using AgolPlugin.Models.Acad;
using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Acad;
using AgolPlugin.Services.Agol;
using AgolPlugin.Services.Background;
using AgolPlugin.Services.Caching;
using AgolPlugin.Services.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AgolPlugin.ViewModels.Viewer
{
    public class ViewerViewModel : ViewModelBase
    {
        public BasicCommand FocusOnPosition_Command { get; private set; }
        public BasicCommand OpenInDefaultViewer_Command { get; private set; }
        public BasicCommand SaveSelectedImage_Command { get; private set; }

        public ViewerViewModel()
        {
            FocusOnPosition_Command = new BasicCommand(FocusOnPosition_Execute, FocusOnPosition_CanExecute);
            OpenInDefaultViewer_Command = new BasicCommand(OpenInDefaultViewer_Execute, OpenInDefaultViewer_CanExecute);
            SaveSelectedImage_Command = new BasicCommand(SaveSelectedImage_Execute, SaveSelectedImage_CanExecute);

            DocumentEventSubscriber.Instance.ImpliedSelectionChanged += Instance_ImpliedSelectionChanged;
        }

        private void Instance_ImpliedSelectionChanged(object sender, EventArgs e)
        {
            if (Cts != null) Cts.Cancel();
            Cts = new CancellationTokenSource();

            using (var acad = new AcadService())
            {
                var reader = new AcadReader(acad);

                if (reader.CheckAgolSelection(Cts, out AcadBlockDataHolder obj))
                {
                    LoadAgolData(obj, Cts);
                    DocumentManager.MdiActiveDocument.Editor.WriteMessage($"{obj.BlockName} AGOL block selected");
                }
                else
                {
                    ClearAgolData(obj);
                    DocumentManager.MdiActiveDocument.Editor.WriteMessage("No valid AGOL block selected");
                }
            }
        }

        private void LoadAgolData(AcadBlockDataHolder obj, CancellationTokenSource cts)
        {
            SelectedRecord = null;
            SelectedRecordLabel = obj.Attributes.FirstOrDefault(a => !a.Invisible)?.TextString;

            var currentCts = _cts;
            Worker.DoWork((s, e) =>
            {
                var client = new AgolClient(Plugin.CredContainer.Credential, Plugin.CredContainer.UrlKey);
                var feature = client.GetFeatureLayer(obj.FeatureUrl).Result;

                if (cts != null && cts.IsCancellationRequested) return;

                var objId = obj.Attributes.FirstOrDefault(a => a.Tag == feature.ObjectIdField);

                if (objId != null && int.TryParse(objId.TextString, out int objIdValue))
                {
                    int? srid = null;
                    using (var acad = new AcadService())
                    {
                        acad.Database.TryGetValue(DwgPropsExtension.OUTPUT_SRID_KEY, out string sridString);
                        srid = int.TryParse(sridString, out int sridValue) ? sridValue : (int?)null;
                    }

                    client.GetSingleRecord(this, feature, objIdValue, srid, cts).Wait();
                }
            },
            (s, e) =>
            {
                if (!currentCts.IsCancellationRequested)
                    IsBusy = false;
            });
        }

        private void ClearAgolData(AcadBlockDataHolder obj)
        {
            SelectedRecord = null;
            SelectedRecordLabel = null;
        }

        #region Properties
        private CancellationTokenSource _cts;
        public CancellationTokenSource Cts
        {
            get { return _cts; }
            set { _cts = value; OnPropertyChanged(); }
        }

        private AgolRecord _selectedRecord;
        public AgolRecord SelectedRecord
        {
            get { return _selectedRecord; }
            set { _selectedRecord = value; OnPropertyChanged(); }
        }

        private string _selectedRecordLabel;
        public string SelectedRecordLabel
        {
            get { return _selectedRecordLabel; }
            set { _selectedRecordLabel = value; OnPropertyChanged(); }
        }

        private bool _convertElevationMetersToFeet = true;
        public bool ConvertElevationMetersToFeet
        {
            get { return _convertElevationMetersToFeet; }
            set { _convertElevationMetersToFeet = value; OnPropertyChanged(); }
        }

        private MediaAttachment _selectedMediaAttachment;
        public MediaAttachment SelectedMediaAttachment
        {
            get { return _selectedMediaAttachment; }
            set { _selectedMediaAttachment = value; OnPropertyChanged(); }
        }
        #endregion

        #region Navigation
        #endregion

        #region Interaction
        private void FocusOnPosition_Execute(object param)
        {
            using (var acad = new AcadService())
            {
                acad.SetEditorView(SelectedRecord.Geometry.Value.X, SelectedRecord.Geometry.Value.Y);
            }
        }
        private bool FocusOnPosition_CanExecute(object param)
        {
            return SelectedRecord != null && SelectedRecord?.Geometry != null;
        }

        private void OpenInDefaultViewer_Execute(object param)
        {
            var temp = Path.GetTempFileName().Replace(".tmp", ".jpg");
            CacheService.Instance.SaveImageToFile(SelectedMediaAttachment.ImageSource, temp, true);
        }
        private bool OpenInDefaultViewer_CanExecute(object param)
        {
            return SelectedMediaAttachment != null
                && SelectedMediaAttachment.IsSupportedFileType;
        }

        private void SaveSelectedImage_Execute(object param)
        {
            var dlg = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                RestoreDirectory = true,
                Title = "Save image",
                FileName = SelectedMediaAttachment.Name,
                Filter = "JPG Image|*.jpg",
                DefaultExt = "jpg",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                CacheService.Instance.SaveImageToFile(SelectedMediaAttachment.ImageSource, dlg.FileName, true);
            }
        }
        private bool SaveSelectedImage_CanExecute(object param)
        {
            return SelectedMediaAttachment != null
                && SelectedMediaAttachment.IsSupportedFileType;
        }
        #endregion
    }
}