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
    public record CardPairSpec(ChineseCardSpec ChineseCardSpec, EnglishCardSpec EnglishCardSpec);
    public record ChineseCardSpec(string chineseCharacter, string pinyin);
    public record EnglishCardSpec(string englishWord);

    public class Grid
    {
        public const int PAGE_WIDTH = 2480;
        public const int PAGE_HEIGHT = 3508;
        public const int PAGE_WIDTH_BLED = 2362;
        public const int PAGE_HEIGHT_BLED = 3390;
        private readonly int _cardsPerRow;
        private readonly int _cardsPerColumn;
        private SKRect _gridArea;
        private Queue<SKRect> _cardRegions;
        private Queue<CardPairSpec> _cards = new Queue<CardPairSpec>();
        private SKTypeface _boldTypeFace;
        private SKTypeface _regularTypeFace;
        private readonly SKTypeface _lightTypeFace;
        
        public Grid(int cardsPerRow, int cardsPerColumn)
        {

            var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
            var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;

            _gridArea = new SKRect(horizontalBleed, verticalBleed, PAGE_WIDTH_BLED, PAGE_HEIGHT_BLED);
            _cardsPerRow = cardsPerRow;
            _cardsPerColumn = cardsPerColumn;
            
            _cardRegions = new Queue<SKRect>(GetCardRegions(_gridArea));

            using (var ms = new MemoryStream(Properties.Resources.msyhbd))
            {
                _boldTypeFace = SKTypeface.FromStream(ms);
            }

            using (var ms = new MemoryStream(Properties.Resources.msyh))
            {
                _regularTypeFace = SKTypeface.FromStream(ms);
            }
            
            using (var ms = new MemoryStream(Properties.Resources.msyhl))
            {
                _lightTypeFace = SKTypeface.FromStream(ms);
            }



        }


        public void AddCard(CardPairSpec cardPairSpec)
        {
            _cards.Enqueue(cardPairSpec);
        }
        
        private string PadTextTo(int totalWidth, string text)
        {
            var textLength = text.Length;
            var padding = totalWidth - textLength;
            if (padding <= 0)
            {
                return text;
            }
            return text.PadLeft(totalWidth / 2).PadRight(totalWidth / 2);
        }

        private void DrawTextVisuallyCenteredInsideRect(SKRect rect, string text, SKTypeface sKTypeface, float upwardsOffsetProportion, SKCanvas canvas)
        {
            var paddingPropertion = 0.5f;
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Typeface = sKTypeface
            };

            var initialTextWidth = paint.MeasureText(text);
            paint.TextSize = paint.TextSize / initialTextWidth * rect.Width * paddingPropertion;

            SKRect textBounds = new SKRect();
            paint.MeasureText(text, ref textBounds);
        
            var centerOfText = new SKPoint(rect.Left +  rect.Width / 2 - textBounds.MidX, rect.Top +  (rect.Height / 2 - rect.Height * upwardsOffsetProportion) - textBounds.MidY);
            canvas.DrawText(text, centerOfText, paint);
        }
                

        private void DrawChineseCard(SKRect region, ChineseCardSpec chineseCardSpec, SKCanvas canvas)
        {
            float amountToLeaveForSubtitles = region.Height * 1 / 4;
            var cardAreaForCharacter = new SKRect(region.Left, region.Top, region.Right, region.Bottom - amountToLeaveForSubtitles);
       
            DrawTextVisuallyCenteredInsideRect(cardAreaForCharacter, chineseCardSpec.chineseCharacter, _boldTypeFace,0, canvas);

            var cardAreaForPinyin = new SKRect(cardAreaForCharacter.Left, cardAreaForCharacter.Bottom, cardAreaForCharacter.Right, cardAreaForCharacter.Bottom + (region.Height - cardAreaForCharacter.Height));

            DrawTextVisuallyCenteredInsideRect(cardAreaForPinyin, PadTextTo(20,chineseCardSpec.pinyin), _regularTypeFace, 0.15f, canvas);

            var rectPaint = new SKPaint()
            {
                Color = SKColors.Red,
                StrokeWidth = 5,
                Style = SKPaintStyle.Stroke
            };

            
            //canvas.DrawRect(cardAreaForPinyin,rectPaint);
            //canvas.DrawRect(cardAreaForCharacter, rectPaint);
                
        }

        public void RenderToPng(Stream targetStream)
        {

            var info = new SKImageInfo(PAGE_WIDTH, PAGE_HEIGHT);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                CardDrawingFunctions.AddGridLines(_cardsPerRow, _cardsPerColumn, _gridArea, canvas);
                
                while (_cardRegions.Any() && _cards.Any())
                {
                    var region = _cardRegions.Dequeue();
                    var card = _cards.Dequeue();
                    DrawChineseCard(region, card.ChineseCardSpec, canvas);
                }


                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    data.SaveTo(targetStream);

            }

        }

        private IEnumerable<SKRect> GetCardRegions(SKRect gridArea)
        {
            var cardSize = gridArea.Width / _cardsPerRow;
            var startPoints =
                    from r in Enumerable.Range(0,_cardsPerRow)
                    from c in Enumerable.Range(0,_cardsPerColumn)
                    let topLeft = new SKPoint(gridArea.Left + r * cardSize, gridArea.Top + c * cardSize)
                    select topLeft;
            var cardRectangles = startPoints.Select(sp => new SKRect(sp.X,sp.Y, sp.X + cardSize, sp.Y + cardSize));
            return cardRectangles;
        }




    }
    
    public static class CardDrawingFunctions
    {
        // pixels in A4 at 300dpi with 3mm bleed
      

       

        

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
             
        //public static Image BuildPage(IEnumerable<CardPairSpec> cardPairs)
        //{
        //    var cardsPerRow = 3;
        //    var cardsPerColumn = 4;
        //    var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
        //    var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;
        //    var area = new Rectangle(new Point(horizontalBleed, verticalBleed), new Size(PAGE_WIDTH_BLED, PAGE_HEIGHT_BLED));
        //    var cardSize = PAGE_WIDTH_BLED / cardsPerRow;
        //    var startPoints =
        //        from r in Enumerable.Range(0,cardsPerRow)
        //        from c in Enumerable.Range(0,cardsPerColumn)
        //        let topLeft = new Point(area.Left + r * cardSize, area.Top + c * cardSize)
        //        select topLeft;
        //    var cardRectangles = startPoints.Select(sp => new Rectangle(sp, new Size(cardSize, cardSize)));

        //    var firstChineseCardAction = DrawChineseCard(cardPairs.First().ChineseCardSpec, cardRectangles.ElementAt(3));
                       
        //    var gridActions = BuildGridActions(cardsPerRow,cardsPerColumn, area);
        //    var image = new Image<Bgr24>(PAGE_WIDTH, PAGE_HEIGHT);

        //    image.Mutate(x => x.Clear(Color.White));
        //    image.Mutate(firstChineseCardAction);
        //    foreach(var ga in gridActions)
        //    {
        //        image.Mutate(ga);
        //    }
        //    return image;

        //}
        

        
              

        
        
        public static void AddGridLines(int numberOfCardsPerRow, int numberOfCardsPerColumn, SKRect area, SKCanvas canvas)
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

        }

             

        }
}
