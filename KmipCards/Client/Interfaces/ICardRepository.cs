using KmipCards.Shared;
using System;
using System.Collections.Generic;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        void AddCard(CardRecord cardRecord);
        void RemoveCard(CardRecord cardRecord);
        IList<CardRecord> GetAllCards();
        void RemoveAllCards();
        void OnRepositoryChanged(CardRepositoryChangedEventArgs args);

        string CurrentlyLoadedListName { get; set; }

        event EventHandler<CardRepositoryChangedEventArgs> RepositoryChanged;

    }
}
