using KmipCards.Shared;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace KmipCards.Api
{
    public class LoggingApiFunction
    {

        [FunctionName("log")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] LogEntryFromClient logEntryFromClient, 
            ILogger log)
        {
            log.Log(logEntryFromClient.LogLevel, logEntryFromClient.Exception, logEntryFromClient.Message);
            return new OkResult();
        }
    }
}
