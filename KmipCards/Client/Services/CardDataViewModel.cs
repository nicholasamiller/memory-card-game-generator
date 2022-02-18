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
    
    public class CardDataViewModel : Interfaces.ICardSetViewModel
    {
        private CardSet _currentlyLoadedSet;
        private ICardRepository _cardRepo;
        private AppData _appData;
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


        public string CurrentlyLoadedListName => _currentlyLoadedSet.Name;
        
        public async Task RenameList(string text)
        {
                         
        }
        private async Task SaveSet()
        {
            await _cardRepo.SetDefaultCardSetName(_currentlyLoadedSet.Name);
            await _cardRepo.SaveCardSetAsync(_currentlyLoadedSet);
        }

        
        public async Task AddCard(CardRecord cardRecord)
        {
            var existing = _currentlyLoadedSet.Cards.FirstOrDefault(r => r == cardRecord);
            if (existing == null)
            {
                _currentlyLoadedSet.Cards.Add(cardRecord);
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
            if (_appData == null)
            {
                _appData = await _cardRepo.GetAppDataAsync();
            }
            _currentlyLoadedSet = 
            if (_currentlyLoadedSet == null)
            {
                _currentlyLoadedSet = await _cardRepo.GetAppDataAsync();
            }
            return _currentlyLoadedSet.Cards;
        }

        public async Task RemoveCard(CardRecord cardRecord)
        {
            _currentlyLoadedSet.Cards.Remove(cardRecord);
            await SaveSet();
            OnViewModelChanged(null);
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

        public async Task RemoveAllCards()
        {
            _currentlyLoadedSet = new CardSet(_currentlyLoadedSet.Name, new List<CardRecord>());
            await SaveSet();
            OnViewModelChanged(null);
        }

    }
}
