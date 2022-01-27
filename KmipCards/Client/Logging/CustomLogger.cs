using KmipCards.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace KmipCards.Client.Logging
{
    public class CustomLogger : ILogger
    {
        private readonly HttpClient httpClient;

        public CustomLogger(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var dto = new LogEntryFromClient() { LogLevel = logLevel, Exception = exception, Message = state.ToString() };
            httpClient.PostAsJsonAsync("/api/log", dto);
        }

      
    }
}
