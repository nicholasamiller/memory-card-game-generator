using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MemoryCardGameGenerator.Drawing
{
    public static class Play
    {
        // pixels in A4 at 300dpi with 3mm bleed
        private const int PAGE_WIDTH = 2480;
        private const int PAGE_HEIGHT = 3508;
        private const int PAGE_WIDTH_BLED = 2362;
        private const int PAGE_HEIGHT_BLED = 3390;

        public record CardSpec(string chineseCharacter, string pinyin, string englishMeaning);
        
        public static Image BuildChineseCard(IEnumerable<CardSpec> cardSpecs)
        {
            
            var image = new Image<Bgr24>(PAGE_WIDTH, PAGE_HEIGHT);

            var cardWidth = PAGE_WIDTH_BLED / 3;
            var bledTopLeftOrigin = new Point((PAGE_WIDTH - PAGE_WIDTH_BLED) / 2, ((PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2));

            var numberOfCardsPerRow = 3;
            var numberOfCardsPerColumn = 4;
            var verticalLines =
                Enumerable.Range(0, numberOfCardsPerRow + 1)
                .Select(c => new { 
                    Start = new PointF(bledTopLeftOrigin.X + c * cardWidth, bledTopLeftOrigin.Y), 
                    End = new PointF(bledTopLeftOrigin.X + c * cardWidth, bledTopLeftOrigin.Y + cardWidth * numberOfCardsPerColumn) })
                .Select(s => new LinearLineSegment(s.Start, s.End))
                .Select(s => new Path(s));

            var horizontalLines =
                Enumerable.Range(0, numberOfCardsPerColumn + 1)
                .Select(c => new { 
                    Start = new PointF(bledTopLeftOrigin.X, bledTopLeftOrigin.Y + c * cardWidth), 
                    End = new Point(bledTopLeftOrigin.X + cardWidth * numberOfCardsPerRow, bledTopLeftOrigin.Y + c * cardWidth) })
                .Select(s => new LinearLineSegment(s.Start, s.End))
                .Select(s => new Path(s));

            // lay out grid 

            // white background
            image.Mutate(x => x.Clear(Color.White));

            // dashed line around whole card
            //var boundaryRectangle = new RectangularPolygon(new Rectangle(new Point(0, 0), new Size(size)));
            var dotPen = Pens.Dash(Color.LightGray, 3);
            image.Mutate(x => x.Draw(dotPen, new PathCollection(verticalLines)));
            image.Mutate(x => x.Draw(dotPen, new PathCollection(horizontalLines)));

            //var padding = (int)(size * 0.05);

            //int innerWidth = size - (padding*2);
            //int innerHeight = innerWidth;

            //var topRectangleHeight = (int)(0.65 * innerHeight);
            //var topRectangleStart = new Point(padding, padding);

            //var topRectangle = new RectangularPolygon(new Rectangle(topRectangleStart, new Size(innerWidth,topRectangleHeight)));
            //image.Mutate(x => x.Draw(Color.Black,2,topRectangle)); // fill the star with red

            return image;

        }

    }
}
