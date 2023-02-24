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
    public class LocalStorageCardRepository : IAppDataRepository
    {
        public LocalStorageCardRepository(ILocalStorageService localStorageService, ILoggerProvider loggerProvider)
        {
            this.localStorageService = localStorageService;
            this.logger = loggerProvider.CreateLogger("Local storage repository");
        }

        private const string LOCAL_STORAGE_KEY = "KMIP_CARDS_APPDATA";
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
              
        public Task SetDefaultCardSetName(string cardsetName)
        {
            _appData.DefaultCardSetName = cardsetName;
            return SerializeToLocalStorageAsync();
        }

        public Task<CardSet> GetCardSetAsync(string cardsetName)
        {
            return Task.FromResult(_appData.CardSets.FirstOrDefault(s => s.Name == cardsetName));
        }

        public Task SaveCardSetAsync(CardSet cardSet)
        {
            var existing = _appData.CardSets.FirstOrDefault(s => s.Name == cardSet.Name);
            if (existing == null)
            {
                _appData.CardSets.Add(cardSet);
            }
            else
            {
                existing = cardSet;
            }
            return SerializeToLocalStorageAsync();
        }

        public Task<IEnumerable<string>> GetCardSetNames()
        {
            return Task.FromResult(_appData.CardSets.Select(c => c.Name));
        }
    }
}
