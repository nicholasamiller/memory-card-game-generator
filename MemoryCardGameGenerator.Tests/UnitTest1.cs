using MemoryCardGameGenerator.Drawing;
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

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSkiaSharpGridDraw()
        {
            var skImage = Play.DrawSkGrid(4, 5);
            using (var data = skImage.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite("skOutput.png"))
            {
                data.SaveTo(stream);
            }

        }
        
        
        [TestMethod]
        public void TestMethod1()
        {
            var spec = new Play.CardPairSpec(new Play.ChineseCardSpec("二", "èr"), new Play.EnglishCardSpec("Two"));
                
            var testImage = Play.BuildPage(new[] { spec });
            using (var output = new FileStream("test.png", FileMode.Create, FileAccess.ReadWrite))
            {
                testImage.Save(output, new PngEncoder());
            }
            //Process.Start("test.png");
                        
        }

        [TestMethod]
        public void TestFontLoad()
        {
            var r = Play.LoadChineseFonts();
            foreach (var f in r.Families)
            {
                System.Console.WriteLine(f.Name);
            }

        }

        [TestMethod]
        public void TestFontDraw()
        {
            var fonts = Play.LoadChineseFonts();
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
