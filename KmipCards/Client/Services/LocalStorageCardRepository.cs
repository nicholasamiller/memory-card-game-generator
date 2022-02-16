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
        private const string INITIAL_CARD_SET_NAME = "Untitled";
        private readonly ILocalStorageService localStorageService;
        private readonly ILogger logger;
        private CardSetCollection _cardSets;


        public async Task Init()
        {
            var existing = await localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_KEY);
            if (existing != null)
            {
                try
                {
                    var cardSetCollection = JsonConvert.DeserializeObject<CardSetCollection>(existing);
                    _cardSets = cardSetCollection;
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Error, e, "Could not deserialise existing cards from local storage.");
                    throw;
                }
            }
            else
            {
                _cardSets = new CardSetCollection(INITIAL_CARD_SET_NAME, new List<CardSet>() { new CardSet(INITIAL_CARD_SET_NAME, new List<KmipCards.Shared.CardRecord>()) });
                await SerializeToLocalStorageAsync();
            }
        }

        private async Task SerializeToLocalStorageAsync()
        {
            await localStorageService.SetItemAsStringAsync(LOCAL_STORAGE_KEY, JsonConvert.SerializeObject(_cardSets));
        }    

        public async Task<CardSet> GetCardSetAsync(string name)
        {
            if (_cardSets == null)
            {
                await Init();
            }
            return _cardSets.cardSets.FirstOrDefault(s => s.name == name);
        }

        public async Task SaveCardSetAsync(CardSet cardSet)
        {
            if (_cardSets == null)
            {
                await Init();
            }
            var existing = _cardSets.cardSets.FirstOrDefault(s => s.name == cardSet.name);
            if (existing != null)
            {
                var indexOfExisting = _cardSets.cardSets.IndexOf(existing);
                _cardSets.cardSets[indexOfExisting] = cardSet;
            }
            else
            {
                _cardSets.cardSets.Add(cardSet);
            }
            await SerializeToLocalStorageAsync();
        }
                
        public async Task<CardSet> GetDefaultCardSetAsync()
        {
            if (_cardSets == null)
            {
                await Init();
                return await GetCardSetAsync(INITIAL_CARD_SET_NAME);
            }
            else
            {
                return await GetCardSetAsync(_cardSets.defaultCardSetName);
            }
        }

        public async Task SetDefaultCardSetName(string name)
        {
            if (_cardSets == null)
            {
                await Init();
            }
            _cardSets = _cardSets with { defaultCardSetName = name };
        }
    }
}
