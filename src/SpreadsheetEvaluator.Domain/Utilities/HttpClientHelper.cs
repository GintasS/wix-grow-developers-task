using System;
using System.Net.Http;
using System.Text;
using SpreadsheetEvaluator.Domain.Configuration;

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
            try
            {
                var result = _httpClient.GetAsync(url);
                return result.Result.Content.ReadAsStringAsync().Result;
            } 
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void PostStringContentToUrl(string url, string payload)
        {
            try
            {
                var content = new StringContent(payload, Encoding.UTF8, Constants.HubApi.PostMediaType);
                var result = _httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
