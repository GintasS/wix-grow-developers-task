using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                var client = _httpClient;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var result = client.PostAsync(url, content).Result;
                Console.WriteLine("TEST");
            }
            catch(Exception ex)
            {
                // TODO: catch exceptions and HTTP response codes
            }
        }


    }
}
