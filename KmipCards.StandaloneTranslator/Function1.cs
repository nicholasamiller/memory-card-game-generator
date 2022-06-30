using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KmipCards.Api;
using KmipCards.Shared;
using System.Net.Http;

namespace KmipCards.StandaloneTranslator
{
    public class Function
    {
        private HttpClient _httpClient;

        public Function(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("translate")]
        public async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] CardDataDto cardDataDto,
             ILogger log)
        {
              var kmipCardsFunction = new KmipCards.Api.CharacterTranslateFunction()
        }
    }
}
