using AgolPlugin.Converters.Json;
using AgolPlugin.Helpers;
using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.Services.Caching;
using AgolPlugin.ViewModels.Viewer;
using Esri.ArcGISRuntime.Security;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgolPlugin.Services.Agol
{
    public class AgolClient
    {
        public string UrlKey { get; private set; }

        public ArcGISTokenCredential Credential { get; private set; }

        public AgolClient(ArcGISTokenCredential cred, string realmKey)
        {
            Credential = cred;
            UrlKey = realmKey;
        }

        public async Task<bool> ValidateUrlKey()
        {
            var client = new RestClient($"https://{UrlKey}.maps.arcgis.com/sharing/rest/portals/self");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);

            try
            {
                var response = await client.ExecuteGetAsync(request);

                var info = JObject.Parse(response.Content);
                if (info.TryGetValue("urlKey", out JToken val))
                    return UrlKey.Equals(val.Value<string>(), System.StringComparison.InvariantCultureIgnoreCase);
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AgolSearchResultCollection> Search(string query = "", string owner = null, string type = "Feature Service", bool useExactowner = true, bool useExactType = true, bool includeThumbnailUrl = true, int? resultCount = 20)
        {
            if (owner != null && useExactowner)
                owner = $"\"{owner}\"";

            if (type != null && useExactType)
                type = $"\"{type}\"";

            var client = new RestClient($"https://{UrlKey}.maps.arcgis.com/sharing/rest/search");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);
            request.AddParameter("q", $"{(string.IsNullOrWhiteSpace(query) ? string.Empty : query + " ")}{(owner != null ? $"owner:{owner}" : string.Empty)} {(owner != null && type != null ? "AND" : string.Empty)} {(type != null ? $"type:{type}" : string.Empty)}");
            if (resultCount != null)
                request.AddParameter("num", resultCount);

            var response = await client.ExecuteGetAsync(request);

            var results = JsonConvert.DeserializeObject<AgolSearchResultCollection>(response.Content);

            if (includeThumbnailUrl)
                foreach (var result in results.SearchResults)
                    result.ThumbnailUrl = $"{result.Url}/info/thumbnail?token={Credential.Token}";

            return results;
        }

        public async Task<AgolSearchResultCollection> SearchByPage(string unmodifiedQuery, int num, int start, bool includeThumbnailUrl = true)
        {
            var client = new RestClient($"https://{UrlKey}.maps.arcgis.com/sharing/rest/search");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);
            request.AddParameter("q", unmodifiedQuery);
            request.AddParameter("num", num);
            request.AddParameter("start", start);

            var response = await client.ExecuteGetAsync(request);
            var results = JsonConvert.DeserializeObject<AgolSearchResultCollection>(response.Content);

            if (includeThumbnailUrl)
                foreach (var result in results.SearchResults)
                    result.ThumbnailUrl = $"{result.Url}/info/thumbnail?token={Credential.Token}";

            return results;
        }

        public async Task<AgolServiceItem> GetServiceItem(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);

            var response = await client.ExecuteGetAsync(request);
            var item = JsonConvert.DeserializeObject<AgolServiceItem>(response.Content);
            item.ServiceUrl = serviceUrl;

            return item;
        }

        public async Task<AgolFeature> GetFeatureLayer(string featureUrl, AgolServiceItem serviceItem = null)
        {
            var client = new RestClient(featureUrl);
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);

            var response = await client.ExecuteGetAsync(request);
            var feature = JsonConvert.DeserializeObject<AgolFeature>(response.Content);
            feature.ServiceItem = serviceItem;
            feature.FeatureUrl = featureUrl;
            feature.ServiceUrl = featureUrl.Remove(feature.FeatureUrl.Length - $"/{feature.Id}".Length);
            feature.ParentLayerId = feature.Relationships.FirstOrDefault(r => r.KeyField == FeatureRelationship.PARENT_GLOBALID_FIELD)?.RelatedTableId;
            return feature;
        }

        public async Task GetSingleRecord(ViewerViewModel vm, AgolFeature feature, int objectId, int? outSrid = null, CancellationTokenSource cts = null)
        {
            vm.IsBusy = true;
            await GetRecords(feature, new FeatureQueryParams
            {
                OutputSpatialReference = outSrid,
                Filters = FieldFilters.ObjectIdFilter(feature.ObjectIdField, objectId),
                ConvertElevationMetersToFeet = vm.ConvertElevationMetersToFeet,
            });

            var record = feature.Records.FirstOrDefault();
            Plugin.ViewerPage.Invoke(() =>
            {
                vm.SelectedRecord = record;
                vm.FocusOnPosition_Command.Refresh();
            });

            record.Attachments = new ObservableCollection<MediaAttachment>();
            TryGetAttachments(record, record);
            await AssignRelatedRecords(record, record, outSrid);
            vm.IsBusy = false;
        }

        private async Task AssignRelatedRecords(AgolRecord record, AgolRecord topRecord, int? outputSrid = null, CancellationTokenSource cts = null)
        {
            if (cts != null && cts.IsCancellationRequested) return;

            foreach (var rel in record.ParentFeature.Relationships.Where(r => r.KeyField == record.ParentFeature.GlobalIdField))
            {
                if (cts != null && cts.IsCancellationRequested) return;

                var relFeature = await GetFeatureLayer($"{record.ParentFeature.ServiceUrl}/{rel.RelatedTableId}");
                var relRecords = await GetRecords(relFeature, new FeatureQueryParams
                {
                    OutputSpatialReference = outputSrid,
                    Filters = FieldFilters.ParentObjectIdFilter(record[record.ParentFeature.GlobalIdField].Value)
                });

                if (relRecords.Count >= 1)
                {
                    Plugin.ViewerPage.Invoke(() => record.RelatedRecords.Add(new AgolRecordGroup(relFeature.Name, relRecords)));
                }

                foreach (var relRecord in relRecords)
                {
                    TryGetAttachments(relRecord, topRecord);
                    if (cts != null && cts.IsCancellationRequested) return;
                    await AssignRelatedRecords(relRecord, topRecord, outputSrid);
                }
            }
        }

        private void TryGetAttachments(AgolRecord relRecord, AgolRecord topRecord)
        {
            if (relRecord.ParentFeature.HasAttachments)
            {
                Task.Run(() =>
                {
                    var atts = GetRecordAttachments(relRecord, topRecord).Result;
                    Plugin.ViewerPage.Invoke(() => atts.ForEach(a =>
                    {
                        topRecord.Attachments.Add(a);
                        CacheService.Instance.LoadImage(a);
                    }));
                });
            }
        }

        public async Task<List<MediaAttachment>> GetRecordAttachments(AgolRecord record, AgolRecord topRecord)
        {
            var client = new RestClient($"{record.ParentFeature.FeatureUrl}/queryAttachments");
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            request.AddParameter("f", "json");
            request.AddParameter("token", Credential.Token);
            request.AddParameter("returnUrl", "true");
            request.AddParameter("objectIds", record[record.ParentFeature.ObjectIdField].Value);

            var response = await client.ExecuteGetAsync(request);
            var data = JObject.Parse(response.Content);

            var attList = new List<MediaAttachment>();

            if (data["attachmentGroups"] is JArray attGroups && attGroups.Count > 0
                && attGroups[0]["attachmentInfos"] is JArray attInfos && attInfos.Count > 0)
            {
                MediaAttachment thisMedia;
                foreach (JObject att in attInfos)
                {
                    thisMedia = att.ToObject<MediaAttachment>();
                    thisMedia.ParentRecord = record;
                    attList.Add(thisMedia);
                }
            }

            return attList;
        }

        public async Task<List<AgolRecord>> GetRecords(AgolFeature feature, FeatureQueryParams queryParams, AgolFeature parentFeature = null, Action onEachExtract = null)
        {
            feature.Records = new List<AgolRecord>();
            feature.RecordsAreLoaded = false;

            var client = new RestClient($"{feature.FeatureUrl}/query");
            client.Timeout = -1;
            var parentGlobalIds = parentFeature != null ? parentFeature.Records.Select(r => r[r.ParentFeature.GlobalIdField].Value).ToList() : null;

            List<List<string>> idLists = parentGlobalIds != null ? parentGlobalIds.SplitList(20) : new List<List<string>>
            {
                new List<string>
                {
                    null
                }
            };

            foreach (var parentIds in idLists)
            {
                var request = new RestRequest(Method.GET);
                request.AddParameter("where", parentGlobalIds == null ? queryParams.ToWhereClause(feature) : $"{FeatureRelationship.PARENT_GLOBALID_FIELD} in ({string.Join(",", parentIds.Select(id => $"'{id}'"))})");
                request.AddParameter("f", "json");
                request.AddParameter("token", Credential.Token);
                request.AddParameter("outFields", "*");
                request.AddParameter("returnHiddenFields", "true");
                request.AddParameter("units", "esriSRUnit_Foot");
                request.AddParameter("returnGeometry", "true");

                var response = await client.ExecuteGetAsync(request);
                var data = JObject.Parse(response.Content);

                var dtConverter = new UnixTimestampToDateTimeConverter();

                foreach (JObject j in (JArray)data["features"])
                {
                    var record = new AgolRecord();
                    double elevation = 0;

                    foreach (JProperty jProp in j["attributes"].Children())
                    {
                        var stringValue = jProp.Value.ToString();
                        if (feature[jProp.Name].Type == EsriFieldType.esriFieldTypeDate.ToString() && dtConverter.TryConvert(jProp.Value.ToString(), out DateTime dt))
                            stringValue = dt.ToString("MM-dd-yyyy HH:mm:ss");

                        record.FieldValues.Add(new RecordFieldValue(feature[jProp.Name], (JValue)jProp.Value, stringValue));
                        if (jProp.Name.Contains("elevation") && double.TryParse(jProp.Value.ToString(), out double elev))
                            elevation = elev;
                        record.ParentFeature = feature;
                    }

                    if (j["geometry"] is JObject geom)
                    {
                        record.Geometry = new AgolGeometry(geom["x"].Value<double>(), geom["y"].Value<double>(), geom["z"]?.Value<double>() ?? elevation);
                    }
                    onEachExtract?.Invoke();
                    feature.Records.Add(record);
                }
            }

            if (queryParams.OutputSpatialReference != null)
                GeoConverter.ReprojectFromWgs84(feature.Records.Where(r => r.Geometry != null), (int)queryParams.OutputSpatialReference, queryParams.ConvertElevationMetersToFeet);
            feature.RecordsAreLoaded = true;
            return feature.Records;
        }
    }
}