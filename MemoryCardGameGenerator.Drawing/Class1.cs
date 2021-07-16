using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using Path = SixLabors.ImageSharp.Drawing.Path;

namespace MemoryCardGameGenerator.Drawing
{
    public static class Play
    {
        // pixels in A4 at 300dpi with 3mm bleed
        public const int PAGE_WIDTH = 2480;
        public const int PAGE_HEIGHT = 3508;
        public const int PAGE_WIDTH_BLED = 2362;
        public const int PAGE_HEIGHT_BLED = 3390;

        public record CardPairSpec(ChineseCardSpec ChineseCardSpec, EnglishCardSpec EnglishCardSpec);
        public record ChineseCardSpec(string chineseCharacter, string pinyin);
        public record EnglishCardSpec(string englishWord);

        public record Grid(IEnumerable<Action<IImageProcessingContext>> actions, IEnumerable<Rectangle> cardAreas);

        private static FontCollection _fontFamily = LoadChineseFonts();

        public static FontCollection LoadChineseFonts()
        {
            FontCollection collection = new FontCollection();
            IEnumerable<FontDescription> descriptions;
            using (var ms = new MemoryStream(Properties.Resources.msyhbd))
            {
                collection.InstallCollection(ms, out descriptions);
                return collection;
            }
        }

        
        // for skiasharp - create skiimageinfo
        // create canvas
        // clear
        // make paint
        // 
        

        
        public static Action<IImageProcessingContext> DrawChineseCard(ChineseCardSpec chineseCardSpec, Rectangle boundingBox) 
        {
            var font = _fontFamily.CreateFont("Microsoft YaHei", 400, FontStyle.Bold);
            RendererOptions rendererOptions = new RendererOptions(font, 300)
            {
                
            };
            
            FontRectangle fontRectangle = TextMeasurer.Measure(chineseCardSpec.chineseCharacter, rendererOptions);

            
            // centre horizontally 

            var startX = (boundingBox.Width - fontRectangle.Width) / 2;

            var fontBrush = Brushes.Solid(Color.Black);
            Action<IImageProcessingContext> drawChar = c => c.DrawText(chineseCardSpec.chineseCharacter, font, fontBrush, new PointF(100, boundingBox.Top));
            return drawChar; 
        }
             
        public static Image BuildPage(IEnumerable<CardPairSpec> cardPairs)
        {
            var cardsPerRow = 3;
            var cardsPerColumn = 4;
            var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
            var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;
            var area = new Rectangle(new Point(horizontalBleed, verticalBleed), new Size(PAGE_WIDTH_BLED, PAGE_HEIGHT_BLED));
            var cardSize = PAGE_WIDTH_BLED / cardsPerRow;
            var startPoints =
                from r in Enumerable.Range(0,cardsPerRow)
                from c in Enumerable.Range(0,cardsPerColumn)
                let topLeft = new Point(area.Left + r * cardSize, area.Top + c * cardSize)
                select topLeft;
            var cardRectangles = startPoints.Select(sp => new Rectangle(sp, new Size(cardSize, cardSize)));

            var firstChineseCardAction = DrawChineseCard(cardPairs.First().ChineseCardSpec, cardRectangles.ElementAt(3));
                       
            var gridActions = BuildGridActions(cardsPerRow,cardsPerColumn, area);
            var image = new Image<Bgr24>(PAGE_WIDTH, PAGE_HEIGHT);

            image.Mutate(x => x.Clear(Color.White));
            image.Mutate(firstChineseCardAction);
            foreach(var ga in gridActions)
            {
                image.Mutate(ga);
            }
            return image;

        }


        public static SKImage DrawSkGrid(int numberOfCardsPerNow, int numberOfCardsPerColumn)
        {
            var info = new SKImageInfo(PAGE_WIDTH, PAGE_HEIGHT);

            var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
            var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;

            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                var gridArea = new SKRect(horizontalBleed,verticalBleed, PAGE_WIDTH_BLED, PAGE_HEIGHT_BLED);
                var canvasWithGrid = BuildGrid(numberOfCardsPerNow, numberOfCardsPerColumn, gridArea, canvas);
                return surface.Snapshot();                
            }
        }

        public static SKCanvas BuildGrid(int numberOfCardsPerRow, int numberOfCardsPerColumn, SKRect area, SKCanvas canvas)
        {
            var cardSize = area.Width / numberOfCardsPerRow;

            var skPaint = new SKPaint()
            {
                Color = SKColors.LightGray,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                PathEffect = SKPathEffect.CreateDash(new[] { 10f, 5f }, 0)
            };

            // draw vertical lines
            Enumerable.Range(0, numberOfCardsPerRow + 1)
                .Select(c => new
                {
                    Start = new SKPoint(area.Left + c * cardSize, area.Top),
                    End = new SKPoint(area.Left + c * cardSize, area.Top + cardSize * numberOfCardsPerColumn)
                })
                .ToList()
                .ForEach(p => canvas.DrawLine(p.Start, p.End, skPaint));

            // draw horizontal lines
            Enumerable.Range(0, numberOfCardsPerColumn + 1)
                .Select(c => new {
                    Start = new SKPoint(area.Left, area.Top + c * cardSize),
                    End = new SKPoint(area.Left + cardSize * numberOfCardsPerRow, area.Top + c * cardSize)
                })
                .ToList()
                .ForEach(p => canvas.DrawLine(p.Start, p.End, skPaint));

            return canvas;

        }

        public static IEnumerable<Action<IImageProcessingContext>> BuildGridActions(int numberOfCardsPerRow, int numberOfCardsPerColumn, Rectangle area)
        {
            var cardSize = area.Width / numberOfCardsPerRow;

            var verticalLines =
                Enumerable.Range(0, numberOfCardsPerRow + 1)
                .Select(c => new {
                    Start = new PointF(area.Left + c * cardSize, area.Top),
                    End = new PointF(area.Left + c * cardSize, area.Top + cardSize * numberOfCardsPerColumn)
                })
                .Select(s => new LinearLineSegment(s.Start, s.End))
                .Select(s => new Path(s));

            var horizontalLines =
                Enumerable.Range(0, numberOfCardsPerColumn + 1)
                .Select(c => new {
                    Start = new PointF(area.Left, area.Top + c * cardSize),
                    End = new Point(area.Left + cardSize * numberOfCardsPerRow, area.Top + c * cardSize)
                })
                .Select(s => new LinearLineSegment(s.Start, s.End))
                .Select(s => new Path(s));

            var dotPen = Pens.Dash(Color.LightGray, 3);
            var actions =  new List<Action<IImageProcessingContext>>()
            {
                x => x.Draw(dotPen, new PathCollection(verticalLines)),
                x => x.Draw(dotPen, new PathCollection(horizontalLines))
            };

            return actions;
        }
       
       

        }
}
