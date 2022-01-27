using System;
using System.Collections.Generic;
using System.Text;

namespace KmipCards.Api.Exceptions
{
    internal class ConfigurationException : Exception
    {
        public ConfigurationException(string msg) : base(msg)
        {

        }
    }
}
