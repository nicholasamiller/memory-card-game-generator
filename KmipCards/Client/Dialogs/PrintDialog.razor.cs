using FluentValidation;
using KmipCards.Client.Interfaces;
using KmipCards.Client.Shared;
using KmipCards.Shared;
using MemoryCardGameGenerator.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KmipCards.Client.Dialogs
{
    public partial class PrintDialog : ComponentBase
    {
        private bool _processing = false;
        private async Task StartGeneratePdf()
        {
            _processing = true;
            await GeneratePdf();
            _processing = false;
        }

        [Inject]
        private ICardDataViewModel CardRepository { get; set; }


        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        
        private enum CardsPerPage
        {
            One,
            Four,
            Twelve,
            Twenty
        }

        private CardsPerPage _cardsPerPage = CardsPerPage.Twenty;

        void Cancel() => MudDialog.Cancel();

        private async Task GeneratePdf()
        {

            var currentCards = await CardRepository.GetAllCards();

            var specs = currentCards.Select(c => new CardPairSpec(new ChineseCardSpec(c.CardDataDto.Chinese, c.CardDataDto.Pinyin), new EnglishCardSpec(c.CardDataDto.English))).ToList();

            var name = CardRepository.CurrentlyLoadedListName + ".pdf";
            using (var outputStream = new MemoryStream())
            {
                await MemoryCardGameGenerator.Drawing.Generate.WritePdfAsync(outputStream, specs, ConvertNumberOfCardsToCardsPerRow(_cardsPerPage));

                await JSRuntime.InvokeVoidAsync("downloadFromByteArray",
                new
                {
                    ByteArray = outputStream.ToArray(),
                    FileName = name,
                    ContentType = "application/pdf"
                });
            }

            MudDialog.Close();
        }
        
        private static int ConvertNumberOfCardsToCardsPerRow(CardsPerPage cardsPerPage)
        {
            switch (cardsPerPage)
            {
                case CardsPerPage.One: return 1;
                case CardsPerPage.Four: return 2;
                case CardsPerPage.Twelve: return 3;
                case CardsPerPage.Twenty: return 4;
                default: return 4;
            }
        }
        

        
    }
}
