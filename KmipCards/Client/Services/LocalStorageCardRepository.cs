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
            var set = _cards.cardSets.FirstOrDefault(s => s.name == name);
            return Task.FromResult(set);
        }

        public Task SaveCardSetAsync(CardSet cardSet)
        {
            var existing = _cards.cardSets.FirstOrDefault(s => s.name == cardSet.name);
            if (existing != null)
            {
                var indexOfExisting = _cards.cardSets.IndexOf(existing);
                _cards.cardSets[indexOfExisting] = cardSet;
                return Task.CompletedTask;
            }
            else
            {
                _cards.cardSets.Add(cardSet);
                return Task.CompletedTask;
            }
        }


    }
}
