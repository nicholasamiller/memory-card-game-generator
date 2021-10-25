using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MemoryCardGenerator.Shared;
using System.Linq;
using MemoryCardGameGenerator.Model;

namespace Api
{
    public static class CardGenerationFunction
    {
        [FunctionName("GenerateCards")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] CardsGenerationRequestDto cardsGenerationRequestDto,
            ILogger log)
        {

            var specs = cardsGenerationRequestDto.Cards.Select(c => new CardPairSpec(new ChineseCardSpec(c.CardDataDto.Chinese, c.CardDataDto.Pinyin), new EnglishCardSpec(c.CardDataDto.English))).ToList();
            using (var ms = new MemoryStream())
            { 
                MemoryCardGameGenerator.Drawing.Generate.WritePdf(ms, specs,ConvertNumberOfCardsToCardsPerRow(cardsGenerationRequestDto.CardsPerPage.Value));

                return new FileContentResult(ms.ToArray(), "application/pdf");
            }


        }

        private static int ConvertNumberOfCardsToCardsPerRow(int numberOfCards)
        {
            switch (numberOfCards)
            {
                case 1: return 1;
                case 4: return 2;
                case 12: return 3;
                case 20: return 4;
                default: return 4;
            }
        }
    }
}
