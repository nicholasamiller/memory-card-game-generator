using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KmipCards.Shared;
using System.Linq;
using MemoryCardGameGenerator.Model;

namespace KmicCards.Server
{
    [ApiController]
    public class CardGenerationFunction
    {
        [HttpPost("api/generateCards")]
        public static async Task<IActionResult> Run(
            CardsGenerationRequestDto cardsGenerationRequestDto)
        {

            var specs = cardsGenerationRequestDto.Cards.Select(c => new CardPairSpec(new ChineseCardSpec(c.CardDataDto.Chinese, c.CardDataDto.Pinyin), new EnglishCardSpec(c.CardDataDto.English))).ToList();
            using (var ms = new MemoryStream())
            { 
                MemoryCardGameGenerator.Drawing.Generate.WritePdf(ms, specs,ConvertNumberOfCardsToCardsPerRow(cardsGenerationRequestDto.CardsPerPage));

                return new FileContentResult(ms.ToArray(), "application/pdf");
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
