using Kitana.Core.Logger;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Services.ExternalServices
{
    public class RaidenService
    {
        private readonly CloudWatchLogger _cloudWatchLogger;
        public RaidenService(CloudWatchLogger cloudWatchLogger)
        {
            _cloudWatchLogger = cloudWatchLogger;
        }
        public async Task<string> GetAppToken()
        {
            try
            {
                var raiden_url = Environment.GetEnvironmentVariable("Raiden_Url");
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, raiden_url);
                var clientId = Environment.GetEnvironmentVariable("Client_Id");
                var clientSecret = Environment.GetEnvironmentVariable("Client_Secret");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent("{\r\n  \"clientId\": \"" + clientId + "\",\r\n  \"clientSecret\": \"" + clientSecret + "\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var fetchedResponse = await response.Content.ReadAsStringAsync();
                var JsonToken = JObject.Parse(fetchedResponse);
                var token = JsonToken["token"].ToString();
                return token;
            }
            catch (Exception ex)
            {
                await _cloudWatchLogger.log($"Exception occurred: {ex.Message}", null);
                throw;
            }
        }
    }
}
