using Blazor.DownloadFileFast.Interfaces;
using KmipCards.Client.Interfaces;
using KmipCards.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        private async void OnValidSubmit()
        {
            var requestDto = new CardsGenerationRequestDto()
            {
                Cards = CardRepository.GetAllCards().ToArray(),
                CardsPerPage = _printRequestModel.CardsPerPage.Value,
                Name = CardRepository.CurrentlyLoadedListName
            };

        }
    }
}
