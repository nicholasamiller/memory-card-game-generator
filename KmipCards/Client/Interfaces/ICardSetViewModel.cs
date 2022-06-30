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
        Task<IEnumerable<CardRecord>> GetCards();
        
        Task RemoveAllCards();
        void OnViewModelChanged(CardViewModelChanged args);
        Task RenameList(string text);
        Task SetCardSet(string cardSetName);

        string CurrentlyLoadedListName { get;  }

        event EventHandler<CardViewModelChanged> CardSetChanged;
    }


    
}
