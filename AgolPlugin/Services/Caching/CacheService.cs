using AgolPlugin.Models.Agol;
using AgolPlugin.Services.Background;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace AgolPlugin.Services.Caching
{
    public class CacheService
    {
        private static CacheService _instance;
        public static CacheService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CacheService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AgolPlugin"));
                return _instance;
            }
        }
        public string DbPath { get; private set; }
        private string _connectionString;

        private CacheService(string dbDirectory, string dbName = "media_cache.db")
        {
            DbPath = Path.Combine(dbDirectory, "media_cache.db");
            Directory.CreateDirectory(Path.GetDirectoryName(DbPath));
            _connectionString = $"Data Source={DbPath}";

            if (!DbIsValid())
                InitDb();
        }

        public static CacheService CustomizeInstance(string dbDirectory, string dbName = "media_cache.db")
        {
            if (_instance == null)
            {
                _instance = new CacheService(dbDirectory, dbName);
            }
            else
            {
                _instance.DbPath = Path.Combine(dbDirectory, dbName);
                Directory.CreateDirectory(Path.GetDirectoryName(_instance.DbPath));
                _instance._connectionString = $"Data Source={_instance.DbPath}";

                if (!_instance.DbIsValid())
                    _instance.InitDb();
            }
            return Instance;
        }

        private void InitDb()
        {
            if (File.Exists(DbPath))
                File.Delete(DbPath);

            SQLiteConnection.CreateFile(DbPath);
        }
        public bool DbIsValid()
        {
            if (!File.Exists(DbPath))
                return false;

            try
            {
                using (var db = new SQLiteConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        transaction.Rollback();
                    }
                    db.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void LoadImage(MediaAttachment attachment)
        {
            Worker.DoWork(async (s, e) =>
            {
                if (MediaAttachment.ValidExtensions.Any(x => attachment.Name.ToLower().EndsWith(x)))
                {
                    attachment.IsSupportedFileType = true;
                    CheckTable(attachment.ParentRecord.ParentFeature.ServiceItemId, true);
                    LoadImageTask(attachment);
                }
                else
                    attachment.IsSupportedFileType = false;
            });
        }

        public bool CheckTable(string tableName, bool createIfNotExists = false)
        {
            bool exists = false;
            using (var db = new SQLiteConnection(_connectionString))
            {
                db.Open();
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name = @tableName";
                    cmd.Parameters.AddWithValue("tableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exists = reader.GetInt32(0) == 1;
                        }
                    }
                }
                db.Close();
            }

            if (!exists && createIfNotExists)
                CreateNewTable(tableName);

            return exists;
        }

        public void CreateNewTable(string tableName)
        {
            using (var db = new SQLiteConnection(_connectionString))
            {
                db.Open();
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = $"CREATE TABLE IF NOT EXISTS '{tableName}' ('id' INTEGER NOT NULL UNIQUE, 'img' BLOB NOT NULL, PRIMARY KEY('id'))";
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        public bool TryRecallImageData(string featureServiceItemId, int attachmentId, out BitmapImage img)
        {
            byte[] bytes;
            using (var db = new SQLiteConnection(_connectionString))
            {
                db.Open();
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = $"SELECT img FROM '{featureServiceItemId}' WHERE id = @attachmentId";
                    cmd.Parameters.AddWithValue("attachmentId", attachmentId);
                    bytes = cmd.ExecuteScalar() as byte[];
                }
                db.Close();
            }

            if (bytes != null)
            {
                img = BytesToImage(bytes);
                return true;
            }
            else
            {
                img = null;
                return false;
            }
        }

        public BitmapImage AddAndReturnImage(string featureServiceItemId, int attachmentId, byte[] bytes)
        {
            var bmp = new BitmapImage();

            using (var db = new SQLiteConnection(_connectionString))
            {
                db.Open();
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = $"INSERT INTO '{featureServiceItemId}' VALUES (@imgId, @imgData)";
                    cmd.Parameters.AddWithValue("imgId", attachmentId);
                    cmd.Parameters.AddWithValue("imgData", bytes);
                    var result = cmd.ExecuteNonQuery();
                    if (result == 0)
                        throw new Exception("Paul says that if this error is thrown, he thinks that the image was not written to the database for some reason...");
                }
                db.Close();
            }
            var img = BytesToImage(bytes);
            return img;
        }

        private void LoadImageTask(MediaAttachment attachment)
        {
            attachment.IsLoading = true;

            BitmapImage img;

            if (!TryRecallImageData(attachment.ParentRecord.ParentFeature.ServiceItemId, attachment.Id, out img))
            {

                var client = new RestClient(attachment.Url);
                client.Timeout = -1;

                var request = new RestRequest(Method.GET);
                request.AddParameter("f", "json");
                request.AddParameter("token", Plugin.CredContainer.Credential.Token);

                var response = client.ExecuteGetAsync(request).Result;
                var json = JObject.Parse(response.Content);
                var bytes = Convert.FromBase64String(json["Attachment"].Value<string>());
                img = AddAndReturnImage(attachment.ParentRecord.ParentFeature.ServiceItemId, attachment.Id, bytes);
            }

            Plugin.ViewerPage.BeginInvoke(() => attachment.ImageSource = img);

            attachment.IsLoading = false;
        }

        public void SaveImageToFile(BitmapImage imageSource, string savePath, bool openInDefaultViewer = false)
        {
            try
            {
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imageSource));
                    encoder.Save(stream);
                }
                if (openInDefaultViewer)
                    Process.Start(savePath);
            }
            catch { }
        }

        private BitmapImage BytesToImage(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}