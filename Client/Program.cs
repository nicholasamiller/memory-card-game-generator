using KmipCards.Client.Interfaces;
using KmipCards.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KmipCards.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });

            if (builder.HostEnvironment.IsDevelopment())
            {
                builder.Services.AddSingleton<ICardRepository>(_ => new MockDataRepository());
            }
            else
            {
                builder.Services.AddSingleton<ICardRepository>(_ => new CardDataRepository());
            }

            await builder.Build().RunAsync();
        }
    }
}
