using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.IO;

namespace AgolPlugin.Services.Acad
{
    public class AcadService : IDisposable
    {
        public Document Document { get; private set; }
        public Database Database { get; private set; }
        public Editor Editor { get; private set; }
        public Transaction Transaction { get; private set; }
        public DocumentLock DocLock { get; private set; }

        private bool _lockDocument;
        private bool _autoCommit;
        private bool _autoTransaction;

        public AcadService(bool lockDoc = true, bool autoTransaction = true, bool autoCommit = true)
        {
            _lockDocument = lockDoc;
            _autoCommit = autoCommit;
            _autoTransaction = autoTransaction;

            Document = Application.DocumentManager.MdiActiveDocument;
            Database = Document.Database;
            Editor = Document.Editor;

            if (_autoTransaction)
                Transaction = Document.TransactionManager.StartTransaction();

            if (_lockDocument)
                DocLock = Document.LockDocument();
        }

        public string GetActiveDrawingFilePath()
        {
            try
            {
                var hs = HostApplicationServices.Current;
                return hs.FindFile(Document.Name, Database, FindFileHint.Default);
            }
            catch
            {
                return Path.Combine(Path.GetTempPath(), "Imagery", "Aerial.png");
            }
        }

        public string GetActiveDrawingDirectory()
        {
            return Path.GetDirectoryName(GetActiveDrawingFilePath());
        }

        public void SetEditorView(double x, double y, double xOffset = 100, double yOffset = 100)
        {
            var maxX = x + xOffset;
            var minX = x - xOffset;
            var maxY = y + yOffset;
            var minY = y - yOffset;

            var vt = new ViewTableRecord
            {
                CenterPoint = new Point2d(x, y),
                Height = maxY - minY,
                Width = maxX - minX,
            };
            if (vt.Height < 110) vt.Height = 110;
            if (vt.Width < 110) vt.Width = 110;

            Editor.SetCurrentView(vt);
        }

        public void Dispose()
        {
            if (_lockDocument)
                DocLock.Dispose();
            if (_autoCommit && Transaction != null)
                Transaction.Commit();
            if (_autoTransaction)
                Transaction.Dispose();
        }
    }
}