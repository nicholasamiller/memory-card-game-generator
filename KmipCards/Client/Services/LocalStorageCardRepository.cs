using Blazored.LocalStorage;
using KmipCards.Client.Interfaces;
using KmipCards.Client.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace KmipCards.Client.Services
{
    public class LocalStorageCardRepository : ICardRepository
    {
        
        public LocalStorageCardRepository(ILocalStorageService localStorageService, ILogger logger)
        {
            this.localStorageService = localStorageService;
            this.logger = logger;
        }

        private const string LOCAL_STORAGE_KEY = "KMIP_CARDS_REPOSITORY";
        private readonly ILocalStorageService localStorageService;
        private readonly ILogger logger;
        private CardSetCollection _cards;

        public async Task InitAsync()
        {
            var existing = await localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_KEY);
            if (existing != null)
            {
                try
                {
                    var cardSetCollection = JsonConvert.DeserializeObject<CardSetCollection>(existing);
                    _cards = cardSetCollection;
                    return;
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, e, "Could not deserialise existing cards from local storage.");
                    throw;
                }

            }
        }
        
        public Task<CardSet> GetCardSetAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveCardSetAsync(CardSet cardSet)
        {
            throw new System.NotImplementedException();
        }
    }
}
