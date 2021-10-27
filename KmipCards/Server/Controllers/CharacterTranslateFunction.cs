using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KmipCards.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KmipCards.Server
{
    [ApiController]
    public class CharacterTranslateFunction
    {
        public class TranslationResult
        {
            public DetectedLanguage DetectedLanguage { get; set; }
            public TextResult SourceText { get; set; }
            public Translation[] Translations { get; set; }
        }

        public class DetectedLanguage
        {
            public string Language { get; set; }
            public float Score { get; set; }
        }

        public class TextResult
        {
            public string Text { get; set; }
            public string Script { get; set; }
        }

        public class Translation
        {
            public string Text { get; set; }
            public TextResult Transliteration { get; set; }
            public string To { get; set; }
            public Alignment Alignment { get; set; }
            public SentenceLength SentLen { get; set; }
        }

        public class Alignment
        {
            public string Proj { get; set; }
        }

        public class SentenceLength
        {
            public int[] SrcSentLen { get; set; }
            public int[] TransSentLen { get; set; }
        }

        public class TransliterationResult
        {
            public string Text { get; set; }
            public string Script { get; set; }
        }


        public CharacterTranslateFunction(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // todo: put this in key vault
        private static readonly string subscriptionKey = "8d14ac84d4b84101be7cef9d7996bdbf";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";

        // Add your location, also known as region. The default is global.
        // This is required if using a Cognitive Services resource.
        private static readonly string location = "australiaeast";
        private readonly HttpClient httpClient;

        private class TranslationRequestInfo
        {
            public TranslationSourceLanguage TranslationSource { get; set; }
            public string TextToTranslate { get; set; }
            public string SourceLanguage {  get; set; }
            public string[] TargetLanguages {  get; set; }
            public string FromScript {  get; set; }
            public string[] ToScripts {  get; set; }
        }
        
        public enum TranslationSourceLanguage
        {
            Chinese,
            Engilsh
        };

        TranslationRequestInfo DetermineTranslationParams(CardDataDto cardDataDto)
        {
            if (cardDataDto.Chinese != null)
            {
                return new TranslationRequestInfo()
                {
                    TranslationSource = TranslationSourceLanguage.Chinese,
                    TextToTranslate = cardDataDto.Chinese,
                    SourceLanguage = "zh-Hans",
                    TargetLanguages = new[] { "zh-Hans", "en" },
                    
                    FromScript = "Hans",
                    ToScripts =  new[] {  "Latn","Latn" }
                };
            }
            else if (cardDataDto.English != null) // only English present
            {
                return new TranslationRequestInfo()
                {
                    TranslationSource = TranslationSourceLanguage.Engilsh,
                    TextToTranslate = cardDataDto.English,
                    SourceLanguage = "en",
                    TargetLanguages = new[] { "zh-Hans" },
                    FromScript = "Latn",
                    ToScripts = new[] { "Latn" }
                };
            }
            throw new NotImplementedException();
        }
        
        CardDataDto PopulateCardData(CardDataDto inputCard, TranslationRequestInfo translationRequestInfo, TranslationResult[] translationResults)
        {

            var translations = translationResults.SelectMany(r => r.Translations);

            
            if (translationRequestInfo.TranslationSource == TranslationSourceLanguage.Chinese)
            {
                var englishTranslationResult = translations.FirstOrDefault(t => t.To == "en");
                if (englishTranslationResult != null)
                {
                    inputCard.English = englishTranslationResult.Text;
                }
                var chineseToChineseTranslation = translations.FirstOrDefault(t => t.To == "zh-Hans");
                if (chineseToChineseTranslation != null)
                {
                    if (chineseToChineseTranslation.Transliteration != null && chineseToChineseTranslation.Transliteration.Script == "Latn")
                    {
                        inputCard.Pinyin = chineseToChineseTranslation.Transliteration.Text;
                    }
                }
            }

            else if (translationRequestInfo.TranslationSource == TranslationSourceLanguage.Engilsh)
            {
                var chineseTranslationResult = translations.FirstOrDefault(t => t.To == "zh-Hans");
                if (chineseTranslationResult != null)
                {
                    inputCard.Chinese = chineseTranslationResult.Text;
                }
                if (chineseTranslationResult.Transliteration != null && chineseTranslationResult.Transliteration.Script == "Latn")
                {
                    inputCard.Pinyin = chineseTranslationResult.Transliteration.Text;
                }
            }
            return inputCard;
            

        }

        [Route("Translate")]
        public async Task<IActionResult> Run(
               CardDataDto cardDataDto, ILogger log)
        {       
            if (cardDataDto.English == null && cardDataDto.Chinese == null)
            {
                return new BadRequestResult();
            }

            var translationParams = DetermineTranslationParams(cardDataDto);

            var targetLanguages = String.Concat(translationParams.TargetLanguages.Select(l => $"&to={l}"));
            var toScripts = String.Concat(translationParams.ToScripts.Select(l => $"&toScript={l}"));

            string translateRoute = $"/translate?api-version=3.0&from={translationParams.SourceLanguage}{targetLanguages}&fromScript={translationParams.FromScript}{toScripts}";
            object[] body = new object[] { new { Text = translationParams.TextToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);
            
            using (var translateRequest = new HttpRequestMessage())
            {
                translateRequest.Method = HttpMethod.Post;
                translateRequest.RequestUri = new Uri(endpoint + translateRoute);
                translateRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                translateRequest.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                translateRequest.Headers.Add("Ocp-Apim-Subscription-Region", location);
                     
                HttpResponseMessage translationResponse = await httpClient.SendAsync(translateRequest).ConfigureAwait(false);
     
                string translationResult = await translationResponse.Content.ReadAsStringAsync();
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(translationResult);

                CardDataDto toReturn = PopulateCardData(cardDataDto, translationParams, deserializedOutput);
                    
                return new OkObjectResult(cardDataDto);

            }
        }
    }
}
