using KmipCards.Client.Dialogs;
using KmipCards.Client.Interfaces;
using KmipCards.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
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
        ICardSetViewModel CardSetViewModel { get; set; }

        [Inject]
        ICardRepository CardRepository { get; set; }

        private bool _printDialogDisabled = false;

        protected override void OnInitialized()
        {
            // add event handler to repository changed to switch disabled state for print dialog
            CardSetViewModel.CardSetChanged += CardRepository_RepositoryChanged;
        }

        private void CardRepository_RepositoryChanged(object sender, CardViewModelChanged e)
        {
            
        }

        private async Task OpenAddDialog()
        {
            var dialogParams = new DialogParameters();
            dialogParams.Add("CardRecord", CardRecord.GetEmpty());
            var addCardDialog = DialogService.Show<CharacterDialog>("Add Card",dialogParams, maxWidth);

            var result = await addCardDialog.Result;
            var cardData = result.Data as CardRecord;
            if (cardData != null)
            {
                if (cardData.Tags == null)
                    cardData.Tags = new List<string>();
                await CardSetViewModel.AddCard(cardData);
            }
        }

        private void OpenPrintDialog()
        {
            DialogService.Show<PrintDialog>("Make Printable Cards", maxWidth);
        }

        private void OpenUploadDialog()
        {
            DialogService.Show<UploadFileDialog>("Upload Saved Set", maxWidth);
        }

        private async Task OpenSetDialog()
        {
            var setNames = (await CardRepository.GetAppDataAsync()).Cardsets.Select(cs => cs.Name).ToList();
            var dialogParams = new DialogParameters();
            dialogParams.Add("Sets", setNames);
            var selectedSet = DialogService.Show<OpenSetDialog>("Open Set",dialogParams,maxWidth);
        }
        
        private async Task NewSet()
        {
            var appData = await CardRepository.GetAppDataAsync();
            var existingNames = appData.Cardsets.Select(cs => cs.Name).ToList();
            var newSetName = GetNewSetName(existingNames);
            appData.Cardsets.Add(new CardSet(newSetName, new List<CardRecord>()));
            appData.DefaultCardSetName = newSetName;
            await CardRepository.SetAppDataAsync(appData);
            await CardSetViewModel.SetCardSet(newSetName);
            StateHasChanged();
        }

        string GetNewSetName(List<string> existingNames)
        {
            string newSetName = "Untitled";
         
            int count = 1;
            while (existingNames.Contains(newSetName))
            {
                count++;
                newSetName = $"{newSetName} {count}";
            }
            return newSetName;

        }



        
    }
}
