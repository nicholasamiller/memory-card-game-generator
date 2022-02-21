using KmipCards.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ICardSetViewModel
    {
        Task AddCard(CardRecord cardRecord);
        Task RemoveCard(CardRecord cardRecord);
        Task ReplaceCard(CardRecord oldCard, CardRecord newCard);
        Task<List<CardRecord>> LoadInitialCards();
        Task RemoveAllCards();
        void OnViewModelChanged(CardViewModelChanged args);
        Task RenameList(string text);

        string CurrentlyLoadedListName { get;  }

        event EventHandler<CardViewModelChanged> CardSetChanged;
    }


    
}
