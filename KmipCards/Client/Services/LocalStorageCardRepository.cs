using Blazored.LocalStorage;
using KmipCards.Client.Interfaces;
using KmipCards.Client.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KmipCards.Shared;

namespace KmipCards.Client.Services
{
    public class LocalStorageCardRepository : ICardRepository
    {
        public LocalStorageCardRepository(ILocalStorageService localStorageService, ILoggerProvider loggerProvider)
        {
            this.localStorageService = localStorageService;
            this.logger = loggerProvider.CreateLogger("Local storage repository");
        }

        private const string LOCAL_STORAGE_KEY = "KMIP_CARDS_REPOSITORY";
        private const string INITIAL_CARD_SET_NAME = "Untitled";
        private readonly ILocalStorageService localStorageService;
        private readonly ILogger logger;
        private AppData _appData;


        public async Task Init()
        {
            var existing = await localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_KEY);
            if (existing != null)
            {
                try
                {
                    var cardSetCollection = JsonConvert.DeserializeObject<AppData>(existing);
                    _appData = cardSetCollection;
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, e, "Could not deserialise existing cards from local storage.");
                    throw;
                }
            }
            else
            {
                _appData = new AppData(INITIAL_CARD_SET_NAME, new List<CardSet>() { new CardSet(INITIAL_CARD_SET_NAME, new List<KmipCards.Shared.CardRecord>()) });
                await SerializeToLocalStorageAsync();
            }
        }

        private async Task SerializeToLocalStorageAsync()
        {
            await localStorageService.SetItemAsStringAsync(LOCAL_STORAGE_KEY, JsonConvert.SerializeObject(_appData));
        }    
              
        public async Task<AppData> GetAppDataAsync()
        {
            if (_appData == null)
                await Init();
            return _appData;
        }

        public async Task SetAppDataAsync(AppData appData)
        {
            _appData = appData;
            await SerializeToLocalStorageAsync();
        }
    }
}
