using System;
using System.Net.Http;
using System.Text;

namespace SpreadsheetEvaluator.Domain.Utilities
{
    public class HttpClientHelper
    {
        private HttpClient _httpClient;
        public HttpClientHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string GetStringContentFromUrl(string url) 
        {
            String content = null;
            try
            {
                var client = _httpClient;

                var result = client.GetAsync(url);
                content = result.Result.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                // TODO: catch exceptions and HTTP response codes
            }

            return content;
        }

        public void PostStringContentToUrl(string url, string payload)
        {
            try
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var result = _httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync();
                Console.WriteLine("TEST");
            }
            catch(Exception ex)
            {
                // TODO: catch exceptions and HTTP response codes
            }
        }


    }
}
