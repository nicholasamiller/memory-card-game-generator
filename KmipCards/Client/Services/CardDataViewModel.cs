using KmipCards.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KmipCards.Client.Interfaces;
using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace KmipCards.Client.Services
{
   
    
    public class CardDataViewModel : Interfaces.ICardDataViewModel
    {
        private const string LOCAL_STORAGE_KEY = "KMIP_CARDS_REPOSITORY";
        
        protected List<CardRecord> _cards;
        private ILocalStorageService _localStorageService;
        private string _currentlyLoadedListName;

        public event EventHandler<CardRepositoryChangedEventArgs> RepositoryChanged;

        public virtual void OnRepositoryChanged(CardRepositoryChangedEventArgs args)
        {
            RepositoryChanged?.Invoke(this, args);
        }
        
        public string CurrentlyLoadedListName {  get {  return _currentlyLoadedListName; } set {  _currentlyLoadedListName = value;} }
        
        
        public async Task LoadSetFromLocalStorage(string name)
        {
            var setString = await _localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_KEY);
            if (setString != null)
            {
                var cardRecords = ParseFromTextLines(setString);
                _cards = cardRecords;
                OnRepositoryChanged(null);
            }
        }
        
        private async Task SaveSetToLocalStorage()
        {
            var setString = RenderToLines(_cards);
            await _localStorageService.SetItemAsStringAsync(LOCAL_STORAGE_KEY, setString);
        }

        public CardDataViewModel(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        
        public async Task AddCard(CardRecord cardRecord)
        {
            if (!_cards.Contains(cardRecord))
                _cards.Add(cardRecord);
            await SaveSetToLocalStorage();
            OnRepositoryChanged(null);
        }

        public Task<List<CardRecord>> GetAllCards()
        {
            return Task.FromResult(_cards);
        }

        public Task RemoveCard(CardRecord cardRecord)
        {
            _cards.Remove(cardRecord);
            OnRepositoryChanged(null);
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
            _cards = new List<CardRecord>();
            OnRepositoryChanged(null);
            return Task.CompletedTask; 
        }

      
        public virtual async Task InitAsync()
        {
            await LoadSetFromLocalStorage();
            if (_cards == null)
            {
                _cards = new List<CardRecord>();
            }
            OnRepositoryChanged(null);
        }
    }
}
