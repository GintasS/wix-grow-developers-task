using Newtonsoft.Json.Linq;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public static class JsonObjectHelper
    {
        public static string GetValueOfAnyType(JObject jObject)
        {
            var obj = jObject.DeepClone() as JObject;

            if (obj.ContainsKey("text"))
            {
                return obj["text"].ToString();
            }
            else if (obj.ContainsKey("number"))
            {
                return obj["number"].ToString();
            }
            else if (obj.ContainsKey("boolean"))
            {
                return obj["boolean"].ToString();
            }
            return "";
        }

        public static string GetValueOfAnyType(JProperty jProperty)
        {
            var property = jProperty.DeepClone() as JProperty;

            if (property.Value["reference"] != null)
            {
                return property.Value["reference"].ToString();
            }
            else if (property.Value["text"] != null)
            {
                return property.Value["text"].ToString();
            }
            else if (property.Value["number"] != null)
            {
                return property.Value["number"].ToString();
            }
            else if (property.Value["boolean"] != null)
            {
                return property.Value["boolean"].ToString();
            }

            return "";
        }
    }
}