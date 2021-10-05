using MemoryCardGenerator.Shared;
using System.Collections.Generic;

namespace Client.Services
{
    public class CardDataRepository : ICardRepository
    {
        private List<CardDataDto> _cards;

        public CardDataRepository()
        {
            _cards = new List<CardDataDto>();
        }
        
        public void AddCard(CardDataDto cardDataDto)
        {
            _cards.Add(cardDataDto);
        }

        public IList<CardDataDto> GetAllCards()
        {
            return _cards;
        }

        public void RemoveCard(CardDataDto cardDataDto)
        {
            _cards.Remove(cardDataDto);
        }
    }
}
