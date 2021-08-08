using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MemoryCardGameGenerator.Model;
using SkiaSharp;

namespace MemoryCardGameGenerator.Drawing
{
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

        Func<string, List<string>> splitToLines = s => s.Split(',').Select(l => l.Trim()).ToList();

        
        private record TextBoxResult(SKRect rect, SKPaint paintToUse);
        private TextBoxResult sizeTextForPaddedRect(string text, SKPaint paint, SKRect boundingBox, float paddingProportion)
        {
            
            var initialWidth = paint.MeasureText(text);
            var textSize = paint.TextSize / initialWidth * boundingBox.Width * paddingProportion;
            var newPaint = paint.Clone();
            newPaint.TextSize = textSize;
            SKRect textBounds = new SKRect();
            newPaint.MeasureText(text, ref textBounds);
            return new TextBoxResult(textBounds, newPaint);
        }

               


        private void DrawTextVisuallyCenteredInsideRect(SKRect rect, string text, SKTypeface sKTypeface, float upwardsOffsetProportion, SKCanvas canvas)
        {
            var paddingPropertion = 0.5f;
            var initialPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Typeface = sKTypeface
            };

            var textSizeInfo = sizeTextForPaddedRect(text, initialPaint, rect, paddingPropertion);
       
            var topLeftPointToGetTextCentred = new SKPoint(rect.Left +  rect.Width / 2 - textSizeInfo.rect.MidX, rect.Top +  (rect.Height / 2 - rect.Height * upwardsOffsetProportion) - textSizeInfo.rect.MidY);
                      
            canvas.DrawText(text, topLeftPointToGetTextCentred, textSizeInfo.paintToUse);
        }

        private void DrawEnglishCard(SKRect region, EnglishCardSpec englishCardSpec, SKCanvas canvas)
        {
            DrawTextVisuallyCenteredInsideRect(region, englishCardSpec.englishWord, _regularTypeFace,0.05f, canvas);
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
                    var card = _cards.Dequeue();
                    var chineseCardRegion = _cardRegions.Dequeue();
                    DrawChineseCard(chineseCardRegion, card.ChineseCardSpec, canvas);
                    var englishCardRegion = _cardRegions.Dequeue();
                    DrawEnglishCard(englishCardRegion, card.EnglishCardSpec, canvas);
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
}
