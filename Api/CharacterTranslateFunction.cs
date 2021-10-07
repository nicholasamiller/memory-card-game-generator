using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MemoryCardGenerator.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorApp.Api
{
    public class CharacterTranslateFunction
    {

        public CharacterTranslateFunction(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // todo: put this in key vault
        private static readonly string subscriptionKey = "8d14ac84d4b84101be7cef9d7996bdbf";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";

        // Add your location, also known as region. The default is global.
        // This is required if using a Cognitive Services resource.
        private static readonly string location = "australiaeast";
        private readonly HttpClient httpClient;

        [FunctionName("Translate")]
        public async Task<IActionResult> Run(
               [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] CardDataDto cardDataDto,
               ILogger log)
        {
            // take a chinese character, get english and pinyin

            // Input and output languages are defined as parameters.
            string sourceLang = "zh-Hans";
            string targetLang = "en";
            string transliterateTo = "Latn";
            string route = $"/translate?api-version=3.0&from={sourceLang}&to={targetLang}&toScript={transliterateTo}";
            string textToTranslate = cardDataDto.Chinese;
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                return new OkObjectResult(result);
;            }
        }
    }
}
