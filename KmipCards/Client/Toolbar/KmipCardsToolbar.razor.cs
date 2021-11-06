using KmipCards.Client.Dialogs;
using KmipCards.Client.Interfaces;
using KmipCards.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace KmipCards.Client.Toolbar
{
    public partial class KmipCardsToolbar
    {
        DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Large, FullWidth = true };



        // open add dialog
        [Inject]
        IDialogService DialogService { get; set; }

        [Inject]
        ICardRepository CardRepository { get; set; }

        private async Task OpenAddDialog()
        {
            var addCardDialog = DialogService.Show<AddCharacterDialog>("Add Card",maxWidth);
            var result = await addCardDialog.Result;
            var cardData = result.Data as CardRecord;
            if (cardData != null)
            {
                CardRepository.AddCard(cardData);
            }
            CardRepository.OnRepositoryChanged(null);
            
        }

    }
}
