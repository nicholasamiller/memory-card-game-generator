using KmipCards.Shared;
using System.Collections.Generic;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        void AddCard(CardRecord cardRecord);
        void RemoveCard(CardRecord cardRecord);
        IList<CardRecord> GetAllCards();
        void RemoveAllCards();
        string CurrentlyLoadedListName { get; set; }

    }
}
