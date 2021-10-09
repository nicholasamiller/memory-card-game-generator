using MemoryCardGenerator.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            var testData = new CardDataDto() { Chinese = "太冷了"};
            var httpClient = new HttpClient();
            var underTest = new Api.CharacterTranslateFunction(httpClient);
            var result = underTest.Run(testData, null).Result as OkObjectResult;
            CardDataDto card = result.Value as CardDataDto;
            Assert.IsTrue(!String.IsNullOrWhiteSpace(card.English));
        }

        [TestMethod]
        public void IntegrationTestEnglish()
        {
            var testData = new CardDataDto() { English = "it's too cold" };
            var httpClient = new HttpClient();
            var underTest = new Api.CharacterTranslateFunction(httpClient);
            var result = underTest.Run(testData, null).Result as OkObjectResult;
            CardDataDto card = result.Value as CardDataDto;
            Assert.IsTrue(!String.IsNullOrWhiteSpace(card.Chinese));
        }


    }
}
