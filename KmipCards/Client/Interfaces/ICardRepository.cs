using KmipCards.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        Task AddCard(CardRecord cardRecord);
        Task RemoveCard(CardRecord cardRecord);
        Task<List<CardRecord>> GetAllCards();
        Task RemoveAllCards();
        void OnRepositoryChanged(CardRepositoryChangedEventArgs args);

        string CurrentlyLoadedListName { get; set; }

        event EventHandler<CardRepositoryChangedEventArgs> RepositoryChanged;

    }
}
