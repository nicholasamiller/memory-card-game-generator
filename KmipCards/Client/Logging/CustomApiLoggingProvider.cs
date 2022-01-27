using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace KmipCards.Client.Logging
{
    public class HostApiCustomLoggingProvider : ILoggerProvider
    {
        public HostApiCustomLoggingProvider(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(HttpClient);
        }

        public void Dispose()
        {
            return;
        }
    }
}