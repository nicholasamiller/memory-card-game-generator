using MemoryCardGameGenerator.Drawing;
using MemoryCardGameGenerator.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.IO;
using System.Linq;

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class DrawingTests
    {
        
        private Stream GetTestOutputDirectoryStream(string fileName)
        {
            var testOutputDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "GeneratedTestImages"));
            if (!testOutputDir.Exists)
                testOutputDir.Create();
            return File.OpenWrite(Path.Combine(testOutputDir.FullName, fileName));
        }
        
       
       
        [TestMethod]
        public void TestLineSpacing()
        {
            var numberOfLines = 3;
            var boundingBoxWidth = 200;
            var boundingBoxHeight = 60;
            var lineSpacing = 20;
            var expectedSpace = (numberOfLines - 1) * lineSpacing;
            
            var boundingBox = SKRect.Create(new SKPoint(0, 0), new SKSize(boundingBoxWidth, boundingBoxHeight));
            var result = CardDrawingFunctions.GetLineRects(boundingBox, numberOfLines, lineSpacing);
            Assert.IsTrue(result.Count() == 3);
            var actualSpace = boundingBoxHeight - result.Sum(r => r.Height);
            Assert.IsTrue(expectedSpace == actualSpace);
        }

        [TestMethod]
        public void TestLineSpacingForSingleLine()
        {
            var numberOfLines = 1;
            var boundingBoxWidth = 200;
            var boundingBoxHeight = 60;
            var lineSpacing = 20;
            var expectedSpace = (numberOfLines - 1) * lineSpacing;

            var boundingBox = SKRect.Create(new SKPoint(0, 0), new SKSize(boundingBoxWidth, boundingBoxHeight));
            var result = CardDrawingFunctions.GetLineRects(boundingBox, numberOfLines, lineSpacing);
            Assert.IsTrue(result.Count() == 1);
            var actualSpace = boundingBoxHeight - result.Sum(r => r.Height);
            Assert.IsTrue(expectedSpace == actualSpace);
            Assert.IsTrue(boundingBox == result.First());
        }

        [TestMethod]
        public void TestPdfGen()
        {
            var testCardsData = @"一 (yì) one
二(èr) two
三(sān) three
人(rén) a person, human
大(dà) big
小(xiǎo) small
个(gè) measuring word
天(tiān) sky, days
上(shàng) up, top
下(xià) down, bottom
四(sì) four
五(wǔ) five
六(liù) six
七(qī) seven
八(bā) eight
九(jiǔ) nine
十(shi) ten
口(kǒu) mouth
日(ri) sun, day
中(zhōng) middle
长(cháng) long
手(shŏu) hand
山(shān) mountain, hill
木(mù) wood, tree
水(shuǐ) water
火(huǒ) fire
土(tǔ) soil
石(shí) rock, stone
月(yuè) moon, month
云(yún) cloud
目(mù) eye
田(tián) rice field
三角形(sān jiǎo xíng) triangle
圆形(yuán xíng) circle
方形(fāng xíng) square
两个(liǎng gè) a couple
天气(tián qī) weather
下雪(xià xuě) snowing
下雨(xià yǔ) raining
晴朗(qíng lǎng) sunny
多云(duō yún) cloudy
太热了(tài rè le) too hot
太冷了(tài lěng le) too cold";


            using (var bt = new MemoryStream(Drawing.Properties.Resources.msyhbd))
            using (var lt = new MemoryStream(Drawing.Properties.Resources.msyhl))
            
            using (var pdfOutput = GetTestOutputDirectoryStream("Lockdown KMIP Game.pdf"))
            {
                var typeFaces = new TypeFacesConfig(null, SKTypeface.FromStream(bt), SKTypeface.FromStream(lt));
                var specs = CardsData.LoadSpecsFromString(testCardsData);
                var ut = new PdfCardsDocument(specs, 4,typeFaces);
                ut.Render(pdfOutput);
            }
            
        }

        [TestMethod]
        public void TestLineSplit()
        {
            var test = " mountain, hill";
            var result = PagePair.splitToLines(test);
            Assert.IsTrue(result[0] == "mountain,");
            Assert.IsTrue(result[1] == "hill");
        }
    }
}
