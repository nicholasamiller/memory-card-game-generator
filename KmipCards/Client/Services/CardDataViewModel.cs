using KmipCards.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KmipCards.Client.Interfaces;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using KmipCards.Client.Model;
using Microsoft.Extensions.Logging;

namespace KmipCards.Client.Services
{

    // user: when opens with no cards loaded, a new set, untitled
    // that way they can use the app without caring about folder management
    // create untitled in Card repo init
    
    public class CardDataViewModel : Interfaces.ICardSetViewModel
    {
        private CardSet _currentlyLoadedSet;
        private ICardRepository _cardRepo;
        private readonly ILogger _logger;

        public event EventHandler<CardViewModelChanged> CardSetChanged;

        public CardDataViewModel(ICardRepository cardRepository, ILoggerProvider loggerProvider)
        {
            _cardRepo = cardRepository;
            _logger = loggerProvider.CreateLogger(this.GetType().Name);
        }

        public virtual void OnViewModelChanged(CardViewModelChanged args)
        {
            CardSetChanged?.Invoke(this, args);
        }

        public string CurrentlyLoadedListName { get { return _currentlyLoadedSet.name; } set { _currentlyLoadedSet = _currentlyLoadedSet with { name = value }; } }

        public async Task LoadSetFromLocalStorage(string name)
        {
            var cardSet = await _cardRepo.GetCardSetAsync(name);
            if (cardSet == null)
            {
                _logger.Log(LogLevel.Error, $"Attempted to load card set that is not in repository: {name}.");
            }
            else
            {
                _currentlyLoadedSet = cardSet;
                OnViewModelChanged(null);
            }
        }
        
        private async Task SaveSet()
        {
            await _cardRepo.SaveCardSetAsync(_currentlyLoadedSet);
            await _cardRepo.SetDefaultCardSetName(_currentlyLoadedSet.name);
        }

        
        public async Task AddCard(CardRecord cardRecord)
        {
            var existing = _currentlyLoadedSet.cards.FirstOrDefault(r => r == cardRecord);
            if (existing == null)
            {
                _currentlyLoadedSet.cards.Add(cardRecord);
            }
            else
            {
                existing = cardRecord;
            }
            await SaveSet();
            OnViewModelChanged(null);
        }

        public async Task<List<CardRecord>> GetAllCards()
        {
            if (_currentlyLoadedSet == null)
            {
                _currentlyLoadedSet = await _cardRepo.GetDefaultCardSetAsync();
            }
            return _currentlyLoadedSet.cards;
        }

        public Task RemoveCard(CardRecord cardRecord)
        {
            _currentlyLoadedSet.cards.Remove(cardRecord);
            OnViewModelChanged(null);
            return Task.CompletedTask;
        }
        
        public static CardRecord ParseFromLine(string line)
        {
            return CardRecord.ParseFromLine(line);
        }
        
        public static List<CardRecord> ParseFromTextLines(string lines)
        {
            return
                lines.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => CardRecord.ParseFromLine(l))
                .Where(i => i != null)
                .ToList(); 
        }

        public static string RenderToLines(List<CardRecord> cards)
        {
            StringBuilder sb = new StringBuilder();
            foreach (CardRecord card in cards)
            {
                sb.AppendLine($"{card.CardDataDto.Chinese} ({card.CardDataDto.Pinyin}) {card.CardDataDto.English} {String.Join(" ", card.Tags)}");
            }

            return sb.ToString();
        }

        public Task RemoveAllCards()
        {
            _currentlyLoadedSet = new CardSet(_currentlyLoadedSet.name, new List<CardRecord>());
            OnViewModelChanged(null);
            return Task.CompletedTask; 
        }

    }
}
