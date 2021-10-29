using KmipCards.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KmipCards.Client.Interfaces;

namespace KmipCards.Client.Services
{
    public class CardDataRepository : ICardRepository
    {
        private List<CardRecord> _cards;

        private string _currentlyLoadedListName;
        public string CurrentlyLoadedListName {  get {  return _currentlyLoadedListName; } set {  _currentlyLoadedListName = value;} }

        public CardDataRepository()
        {
            _cards = new List<CardRecord>();
        }
        
        public void AddCard(CardRecord cardRecord)
        {
            _cards.Add(cardRecord);
        }

        public IList<CardRecord> GetAllCards()
        {
            return _cards;
        }

        public void RemoveCard(CardRecord cardRecord)
        {
            _cards.Remove(cardRecord);
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

        public void RemoveAllCards()
        {
            _cards = new List<CardRecord>();
        }

    }
}
