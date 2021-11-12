using KmipCards.Client.Dialogs;
using KmipCards.Client.Interfaces;
using KmipCards.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KmipCards.Client.Toolbar
{
    public partial class KmipCardsToolbar
    {
        DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };



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
                if (cardData.Tags == null)
                    cardData.Tags = new List<string>();
                await CardRepository.AddCard(cardData);
            }
        }

        private void OpenPrintDialog()
        {
            DialogService.Show<PrintDialog>("Make Printable Cards", maxWidth);
        }

    }
}
