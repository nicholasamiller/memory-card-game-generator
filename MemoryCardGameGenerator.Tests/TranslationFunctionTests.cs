using KmipCards.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class TranslationFunctionTests
    {

        [TestMethod]
        public void IntegrationTestChinese()
        {
            var testData = new TranslationRequestDto() { Chinese = "太冷了"};
            var httpClient = new HttpClient();
            var underTest = new KmipCards.Server.CharacterTranslateFunction(httpClient);
            var result = underTest.Run(testData).Result as OkObjectResult;
            CardDataDto card = result.Value as CardDataDto;
            Assert.IsTrue(!String.IsNullOrWhiteSpace(card.English));
        }

        [TestMethod]
        public void IntegrationTestEnglish()
        {
            var testData = new TranslationRequestDto() { English = "it's too cold" };
            var httpClient = new HttpClient();
            var underTest = new KmipCards.Server.CharacterTranslateFunction(httpClient);
            var result = underTest.Run(testData).Result as OkObjectResult;
            CardDataDto card = result.Value as CardDataDto;
            Assert.IsTrue(!String.IsNullOrWhiteSpace(card.Chinese));
        }
        

      

    }
}
