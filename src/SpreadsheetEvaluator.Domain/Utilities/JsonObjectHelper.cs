using Newtonsoft.Json.Linq;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public static class JsonObjectHelper
    {
        public static string GetValueOfAnyType(JObject jObject)
        {
            if (jObject.ContainsKey("text"))
            {
                return jObject["text"].ToString();
            }
            else if (jObject.ContainsKey("number"))
            {
                return jObject["number"].ToString();
            }
            else if (jObject.ContainsKey("boolean"))
            {
                return jObject["boolean"].ToString();
            }
            return "";
        }

        public static string GetValueOfAnyType(JProperty jProperty)
        {
            if (jProperty.Value["reference"] != null)
            {
                return jProperty.Value["reference"].ToString();
            }
            else if (jProperty.Value["text"] != null)
            {
                return jProperty.Value["text"].ToString();
            }
            else if (jProperty.Value["number"] != null)
            {
                return jProperty.Value["number"].ToString();
            }
            else if (jProperty.Value["boolean"] != null)
            {
                return jProperty.Value["boolean"].ToString();
            }

            return "";
        }
    }
}