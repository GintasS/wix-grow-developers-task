using System.IO;
using System.Reflection;

namespace SpreadsheetEvaluator.UnitTests
{
    public static class ResourceFileReader
    {
        public static string GetString(string path)
        {
            var jsonRaw = "";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            using (var streamReader = new StreamReader(stream))
            {
                jsonRaw = streamReader.ReadToEnd();
            }

            return jsonRaw;
        }
    }
}
