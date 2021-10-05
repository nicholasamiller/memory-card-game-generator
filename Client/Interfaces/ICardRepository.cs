using MemoryCardGenerator.Shared;
using System.Collections.Generic;

namespace Client
{
    public interface ICardRepository
    {
        void AddCard(CardDataDto cardDataDto);
        void RemoveCard(CardDataDto cardDataDto);
        IList<CardDataDto> GetAllCards();

    }
}
