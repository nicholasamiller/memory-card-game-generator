using MemoryCardGameGenerator.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp.Formats.Png;
using System.Diagnostics;
using System.IO;

namespace MemoryCardGameGenerator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var spec = new Play.CardSpec("二", "èr", "Two (2)");
            var testImage = Play.BuildChineseCard(new[] { spec });
            using (var output = new FileStream("test.png", FileMode.Create, FileAccess.ReadWrite))
            {
                testImage.Save(output, new PngEncoder());
            }
            //Process.Start("test.png");



            
        }
    }
}
