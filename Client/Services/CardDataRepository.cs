using MemoryCardGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Services
{
    public class CardDataRepository : ICardRepository
    {
        private List<CardRecord> _cards;

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

        public static List<CardRecord> ParseFromTextLines(string lines)
        {
            return
                lines.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => CardRecord.ParseFromLine(l))
                .Where(i => i != null)
                .ToList(); 

        }
    }
}
