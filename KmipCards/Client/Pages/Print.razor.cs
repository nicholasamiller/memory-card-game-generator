using Blazor.DownloadFileFast.Interfaces;
using KmipCards.Client.Interfaces;
using KmipCards.Shared;
using MemoryCardGameGenerator.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KmipCards.Client.Pages
{
    public partial class Print
    {
        [Inject]
        private ICardRepository CardRepository { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }


        private PrintRequestModel _printRequestModel;
        EditContext _editContext;
        protected override void OnInitialized()
        {
            _printRequestModel = new PrintRequestModel();
            _editContext = new EditContext(_printRequestModel);
        }

        private class PrintRequestModel
        {
            [Required(ErrorMessage = "How many cards per page?")]
            public KmipCards.Shared.CardsPerPage? CardsPerPage { get; set; }
        }

        private async Task OnValidSubmit()
        {
            var requestDto = new CardsGenerationRequestDto()
            {
                Cards = CardRepository.GetAllCards().ToArray(),
                CardsPerPage = _printRequestModel.CardsPerPage.Value,
                Name = CardRepository.CurrentlyLoadedListName
            };

            var specs = requestDto.Cards.Select(c => new CardPairSpec(new ChineseCardSpec(c.CardDataDto.Chinese, c.CardDataDto.Pinyin), new EnglishCardSpec(c.CardDataDto.English))).ToList();

            var name = CardRepository.CurrentlyLoadedListName + ".pdf";
            using (var ms = new MemoryStream())
            {
                MemoryCardGameGenerator.Drawing.Generate.WritePdf(ms, specs, ConvertNumberOfCardsToCardsPerRow(requestDto.CardsPerPage));
                await BlazorDownloadFileService.DownloadFileAsync(name, ms.ToArray());      
                
            }
            

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
