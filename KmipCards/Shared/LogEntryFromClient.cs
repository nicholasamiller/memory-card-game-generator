
using Microsoft.Extensions.Logging;
using System;

namespace KmipCards.Shared
{
    public class LogEntryFromClient
    {
        public LogLevel LogLevel { get; set; }
        public Exception Exception { get; set; }
        public String Message { get; set; }
    }
}
