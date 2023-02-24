using FluentValidation;
using KmipCards.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KmipCards.Client.Dialogs
{
    public partial class CharacterDialog : ComponentBase
    {
        [Inject]
        HttpClient _httpClient { get; set; }

        [Inject]
        ILoggerFactory _loggerFactory { get; set; }

        ILogger _logger;

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public CardRecord CardRecord { get; set; } // mud dialog sets this from objects passed into Show method

        MudForm form;
        NewCardValidator newCardValidator;


        protected override void OnInitialized()
        {
            _logger = _loggerFactory.CreateLogger<CharacterDialog>();
            newCardValidator = new NewCardValidator();
        }

        protected override void OnInitialized()
        {
            newCardValidator = new NewCardValidator();
            _logger = _loggerFactory.CreateLogger<CharacterDialog>();
            
            base.OnInitialized();
        }

        void Cancel() => MudDialog.Cancel();

        private enum TranslationSource
        {
            Chinese,
            English
        }

        private async Task OnTranslationRequestedChinese()
        {
            await OnTranslationRequested(TranslationSource.Chinese);
        }
        private async Task OnTranslationRequestedEnglish()
        {
            await OnTranslationRequested(TranslationSource.English);
        }

        private async Task OnTranslationRequested(TranslationSource translationSource)
        {
            try
            {
                if (translationSource == TranslationSource.Chinese)
                {
                    // clear pinyin and english
                    CardRecord.CardDataDto.English = null;
                    CardRecord.CardDataDto.Pinyin = null;
                }
                else if (translationSource == TranslationSource.English)
                {
                    CardRecord.CardDataDto.Chinese = null;
                    CardRecord.CardDataDto.Pinyin = null;
                }
                var requestDto = new TranslationRequestDto() { English = CardRecord.CardDataDto.English, Chinese = CardRecord.CardDataDto.Chinese, Pinyin = CardRecord.CardDataDto.Pinyin };

                var translationResponse = await _httpClient.PostAsJsonAsync("/api/translate", requestDto);
                translationResponse.EnsureSuccessStatusCode();
                var dtoReturned = await translationResponse.Content.ReadFromJsonAsync<CardDataDto>();
                CardRecord = new CardRecord() { CardDataDto = dtoReturned };
                StateHasChanged();
                await form.Validate();

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Error getting translation.");
            }
        }

        public class NewCardValidator : AbstractValidator<CardRecord>
        {
            public NewCardValidator()
            {
                RuleFor(x => x.CardDataDto.Chinese)
                .NotNull()
                .WithMessage("Required")
                .NotEmpty()
                .WithMessage("Required");

                RuleFor(x => x.CardDataDto.Chinese)
                .MaximumLength(6)
                .WithMessage("Six characters max");

                RuleFor(x => x.CardDataDto.Pinyin)
                .NotNull()
                .WithMessage("Required")
                .NotEmpty()
                .WithMessage("Required");

                RuleFor(x => x.CardDataDto.English)
                .NotNull()
                .WithMessage("Required")
                .NotEmpty()
                .WithMessage("Required");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<CardRecord>.CreateWithOptions((CardRecord)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };

        }

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                var cardToReturn = CardRecord;
                MudDialog.Close(DialogResult.Ok<CardRecord>(cardToReturn));
            }
        }
    }
}
