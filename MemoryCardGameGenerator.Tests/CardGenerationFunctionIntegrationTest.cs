using MemoryCardGenerator.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class CardGenerationFunctionIntegrationTest
    {

        //[TestMethod]
        //public void GenerateCards()
        //{
        //    var testData = new CardDataDto() { Chinese = "太冷了", English = "some english", Pinyin = "some pinyin" };
        //    var testRecord = new CardRecord() { CardDataDto = testData, Tags = new[] { "test", "meat" } };

        //    var httpClient = new HttpClient();
        //    var testRequestData = new CardsGenerationRequestDto() { Cards = new[] { testRecord },CardsPerPage = CardsPerPage.Twenty,Name = "TestGenerate" };
        //    var result = Api.CardGenerationFunction.Run(testRequestData, null).Result as FileContentResult;
            
        //    var tempFileName = Path.GetTempFileName() + ".pdf";
        //    File.WriteAllBytes(tempFileName, result.FileContents);
        //    System.Console.WriteLine(tempFileName);
        //}
    }
}
