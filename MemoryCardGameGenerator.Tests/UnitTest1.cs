using MemoryCardGameGenerator.Drawing;
using MemoryCardGameGenerator.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSkiaSharpGridDraw()
        {
            var ut = new Grid(4, 5);
            var spec = new CardPairSpec(new ChineseCardSpec("二", "èr"), new EnglishCardSpec("Two"));
            ut.AddCard(spec);
            var spec2 = new CardPairSpec(new ChineseCardSpec("一", "yi"), new EnglishCardSpec("One"));
            ut.AddCard(spec2);
            var spec3 = new CardPairSpec(new ChineseCardSpec("人", "ren"), new EnglishCardSpec("person"));
            ut.AddCard(spec3);
            using (var stream = File.OpenWrite("skOutput2.png"))
            {
                ut.RenderToPng(stream);
            }
        }


        [TestMethod]
        public void TestLoadDataFromText()
        {
            var r = CardsData.LoadSpecsFromResources().ToList();
        }

        [TestMethod]
        public void TestWithRealData()
        {
            var specs = CardsData.LoadSpecsFromResources().ToList();
            var ut = new Grid(4, 5);
            var first20Specs = specs.Skip(20);
            foreach (var s in first20Specs)
            {
                ut.AddCard(s);
            }

            using (var stream = File.OpenWrite("skOutput2.png"))
            {
                ut.RenderToPng(stream);
            }

        }

        [TestMethod]
        public void TestFontLoad()
        {
            var r = CardDrawingFunctions.LoadChineseFonts();
            foreach (var f in r.Families)
            {
                System.Console.WriteLine(f.Name);
            }

        }

        [TestMethod]
        public void TestFontDraw()
        {
            var fonts = CardDrawingFunctions.LoadChineseFonts();
            var font = fonts.CreateFont("Microsoft YaHei", 36, FontStyle.Bold);
            var image = new Image<Bgr24>(200, 200);
            image.Mutate(i => i.DrawText("hello", font, Color.Red, new Point(0, 0)));
            using (var output = new FileStream("testFont.png", FileMode.Create, FileAccess.ReadWrite))
            {
                image.Save(output, new PngEncoder());
            }





        }

    }
}
