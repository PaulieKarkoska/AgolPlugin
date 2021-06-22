using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace AgolPlugin.Services.Extensions
{
    public static class DwgPropsExtension
    {
        public const string OUTPUT_SRID_KEY = "EPSG Code";

        public static bool TryGetValue(this Database db, string key, out string val)
        {
            var summary = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            var props = summary.CustomPropertyTable;

            if (props.Contains(key))
            {
                val = props[key].ToString();
                return true;
            }

            val = null;
            return false;
        }

        public static bool TryGetValueWithFallbacks(this Database db, IEnumerable<string> keysToTry, out string val)
        {
            var summary = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            var props = summary.CustomPropertyTable;

            foreach (var key in keysToTry)
            {
                if (props.Contains(key))
                {
                    val = props[key].ToString();
                    return true;
                }
            }

            val = null;
            return false;
        }

        public static string GetValue(this Database db, string key)
        {
            var summary = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            var props = summary.CustomPropertyTable;
            return props.Contains(key) ? props[key].ToString() : null;
        }

        public static void SetValue(this Database db, string key, string value, bool overwrite = true)
        {
            var summary = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            var props = summary.CustomPropertyTable;

            if (props.Contains(key) && overwrite)
                props[key] = value;
            else
                props.Add(key, value);

            db.SummaryInfo = summary.ToDatabaseSummaryInfo();
        }

        public static void SetValues(this Database db, Dictionary<string, string> dict, bool overwrite = true)
        {
            var summary = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
            var props = summary.CustomPropertyTable;

            foreach (var pair in dict)
            {
                if (!props.Contains(pair.Key) || overwrite)
                    props[pair.Key] = pair.Value;
            }
            db.SummaryInfo = summary.ToDatabaseSummaryInfo();
        }
    }
}