using MemoryCardGenerator.Shared;
using System.Collections.Generic;

namespace Client
{
    public interface ICardRepository
    {
        void AddCard(CardRecord cardRecord);
        void RemoveCard(CardRecord cardRecord);
        IList<CardRecord> GetAllCards();

    }
}
