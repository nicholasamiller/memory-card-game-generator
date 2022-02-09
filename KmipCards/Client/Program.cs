using Blazored.LocalStorage;
using KmipCards.Client.Interfaces;
using KmipCards.Client.Logging;
using KmipCards.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
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

            builder.Services.AddBlazoredLocalStorage();
            
            var localStorageService = builder.Services.BuildServiceProvider().GetService<ILocalStorageService>();
            {
                builder.Services.AddSingleton((Func<IServiceProvider, Interfaces.ICardDataViewModel>)(_ => new Services.CardDataViewModel(localStorageService)));
            }

            var httpClient = builder.Services.BuildServiceProvider().GetService<HttpClient>();
           builder.Logging.AddProvider(new HostApiCustomLoggingProvider(httpClient));
            

            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }

    }
}
