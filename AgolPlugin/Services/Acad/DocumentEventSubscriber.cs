using Autodesk.AutoCAD.ApplicationServices;
using System;
using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AgolPlugin.Services.Acad
{
    public class DocumentEventSubscriber
    {
        private static DocumentEventSubscriber _instance;
        public static DocumentEventSubscriber Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DocumentEventSubscriber();

                return _instance;
            }
        }

        private DocumentEventSubscriber()
        {
            DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
            DocumentManager.DocumentToBeActivated += DocumentManager_DocumentToBeActivated;

            if (DocumentManager.MdiActiveDocument != null)
                SelectionChanged_Subscribe(DocumentManager.MdiActiveDocument);
        }

        private void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e) => SelectionChanged_Unsubscribe(e.Document);

        private void DocumentManager_DocumentToBeActivated(object sender, DocumentCollectionEventArgs e) => SelectionChanged_Subscribe(e.Document);

        #region Acad Events
        private void SelectionChanged_Subscribe(Document doc)
        {
            if (doc != null)
                doc.ImpliedSelectionChanged += MdiActiveDocument_ImpliedSelectionChanged;
        }
        private void SelectionChanged_Unsubscribe(Document doc)
        {
            if (doc != null)
                doc.ImpliedSelectionChanged -= MdiActiveDocument_ImpliedSelectionChanged;
        }

        public event EventHandler ImpliedSelectionChanged;
        private void OnImpliedSelectionChanged()
        {
            ImpliedSelectionChanged?.Invoke(this, new EventArgs());
        }

        private void MdiActiveDocument_ImpliedSelectionChanged(object sender, EventArgs e)
        {
            OnImpliedSelectionChanged();
        }
        #endregion

    }
}