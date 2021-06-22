using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using static AgolPlugin.Helpers.AzureTileMath;

namespace AgolPlugin.Services.MapTiles
{
    public class AzureMapTileProvider : TileProviderBase
    {
        private string _subscriptionKey;
        private string _tilesetId;
        public int TileSize { get; private set; }

        public AzureMapTileProvider(string subscriptionKey, string tilesetId = "microsoft.imagery", bool hdTiles = false)
        {
            _subscriptionKey = subscriptionKey;
            _tilesetId = tilesetId;
            TileSize = hdTiles ? 512 : 256;
        }

        public List<byte[]> GetTiles(IEnumerable<int[]> tilesXY, int zoom)
        {
            var list = new List<byte[]>();
            foreach (var xy in tilesXY)
            {
                list.Add(GetTile(xy, zoom));
            }

            return list;
        }

        /// <summary>
        /// Queries the Azure Maps REST endpoint to retrieve the tile of the given tileXY at the provided zoom level. Also writes the image data to a file if given a save path
        /// </summary>
        /// <param name="tileXY">The tileXY value to query</param>
        /// <param name="zoom">The selected zoom level</param>
        /// <param name="savePath">The optional save path (file is not saved locally if left null)</param>
        /// <returns></returns>
        public byte[] GetTile(int[] tileXY, int zoom, string savePath = null)
        {
            var client = new RestClient($"https://atlas.microsoft.com/map/tile");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("subscription-key", _subscriptionKey);
            request.AddParameter("tilesetId", _tilesetId);
            request.AddParameter("tileSize", TileSize);
            request.AddParameter("api-version", "2.0");
            request.AddParameter("zoom", zoom);
            request.AddParameter("x", tileXY[0]);
            request.AddParameter("y", tileXY[1]);
            request.AddParameter("tileSize", 256);

            var responseBytes = client.DownloadData(request);

            if (savePath != null)
            {
                File.WriteAllBytes(savePath, responseBytes);
            }

            return responseBytes;
        }

        public Dictionary<string, (int, int)> GetImageInfos(IEnumerable<string> quads, string saveDir, out HashSet<int> xVals, out HashSet<int> yVals, out int minX, out int maxX, out int minY, out int maxY, CancellationTokenSource cts = null, Action onIncrement = null)
        {
            var imageInfos = new Dictionary<string, (int, int)>();

            xVals = new HashSet<int>();
            yVals = new HashSet<int>();

            minX = int.MaxValue;
            minY = int.MaxValue;

            maxX = int.MinValue;
            maxY = int.MinValue;

            bool isFirstQuad = true;
            foreach (var quad in quads)
            {
                if (cts != null && cts.IsCancellationRequested) break;

                QuadKeyToTileXY(quad, out int tileX, out int tileY, out int zoom);
                yVals.Add(tileY);
                xVals.Add(tileX);

                if (isFirstQuad)
                {
                    minY = tileY;
                    minX = tileX;
                    isFirstQuad = false;
                }

                maxX = tileX > maxX ? tileX : maxX;
                maxY = tileY > maxY ? tileY : maxY;

                var path = Path.Combine(saveDir, $"{tileX},{tileY}.jpg");
                imageInfos.Add(path, (tileX - minX, tileY - minY));
                GetTile(new[] { tileX, tileY }, zoom, path);
                onIncrement?.Invoke();
            }
            return imageInfos;
        }

        public bool MergeTileImages(Dictionary<string, (int, int)> imageInfos, int tileSize, int colCount, int rowCount, string finalImagePath)
        {
            var images = new List<Bitmap>();
            Bitmap finalImage = null;

            int w = tileSize * colCount;
            int h = tileSize * rowCount;

            try
            {
                foreach (var info in imageInfos)
                {
                    var bmp = new Bitmap(info.Key);
                    images.Add(bmp);
                }
                finalImage = new Bitmap(w, h);

                using (Graphics gr = Graphics.FromImage(finalImage))
                {
                    Console.WriteLine($"There are {imageInfos.Count} images in the imageInfos list");
                    gr.Clear(Color.Black);
                    for (int i = 0; i < imageInfos.Count; i++)
                    {
                        Console.WriteLine($"Using image at index {i}");
                        var img = images[i];
                        var info = imageInfos.Values.ElementAt(i);

                        gr.DrawImage(img, new Rectangle(info.Item1 * tileSize, info.Item2 * tileSize, img.Width, img.Height));
                        Console.WriteLine($"Image at index {i} was added to final image");
                    }
                }
                finalImage.Save(finalImagePath);
            }
            catch
            {
                if (finalImage != null)
                    finalImage.Dispose();
                return false;
            }
            finally
            {
                var delNum = 0;
                foreach (var info in imageInfos)
                {
                    images[delNum++].Dispose();
                    File.Delete(info.Key);
                }
            }
            return true;
        }
    }
}
