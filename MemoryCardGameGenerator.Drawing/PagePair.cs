using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MemoryCardGameGenerator.Model;
using SkiaSharp;
using System.Text.RegularExpressions;

namespace MemoryCardGameGenerator.Drawing
{

    public class PagePair
    {
        public const int PAGE_WIDTH = 2480;
        public const int PAGE_HEIGHT = 3508;
        public const int PAGE_WIDTH_BLED = 2362;
        public const int PAGE_HEIGHT_BLED = 3390;
        private readonly int _numberOfColumns;
        private readonly int _numberOfRows;
        private SKRect _gridArea;
        private SKRect[,] _cardRegions;
        private Queue<CardPairSpec> _cards = new Queue<CardPairSpec>();
        private SKTypeface _boldTypeFace;
        private SKTypeface _regularTypeFace;
        private readonly SKTypeface _lightTypeFace;

        public PagePair(int numberOfColumns, int numberOfRows)
        {

            var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
            var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;

            _gridArea = new SKRect(horizontalBleed, verticalBleed, PAGE_WIDTH_BLED, PAGE_HEIGHT_BLED);
            _numberOfColumns = numberOfColumns;
            _numberOfRows = numberOfRows;

            _cardRegions = GetCardRegionsByRowsThenColumns(_gridArea);

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

        public static List<string> splitToLines(string text)
        {
            // any words followed by a comma or end of string
            var matches = Regex.Matches(text, @"(.*,|.*$)");
            return matches.ToList().Select(m => m.Value.Trim()).Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
        }

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

        private void DrawTextCentredInsideRect(string text, SKRect boundingBox, SKRect textRect, SKPaint paint, SKCanvas sKCanvas)
        {
            var topLeftPointToGetTextCentred = new SKPoint(boundingBox.Left + boundingBox.Width / 2 - textRect.MidX, boundingBox.Top + (boundingBox.Height / 2) - textRect.MidY);
            sKCanvas.DrawText(text, topLeftPointToGetTextCentred, paint);
        }

        private void DrawTextVisuallyCenteredInsideRectWithResize(SKRect rect, string text, SKTypeface sKTypeface, float upwardsOffsetProportion, SKCanvas canvas)
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

            var topLeftPointToGetTextCentred = new SKPoint(rect.Left + rect.Width / 2 - textSizeInfo.rect.MidX, rect.Top + (rect.Height / 2 - rect.Height * upwardsOffsetProportion) - textSizeInfo.rect.MidY);

            canvas.DrawText(text, topLeftPointToGetTextCentred, textSizeInfo.paintToUse);
        }



        private void DrawEnglishCard(SKRect region, EnglishCardSpec englishCardSpec, SKCanvas canvas)
        {
            var vertialPaddingProportion = 0.2f;
            var horizontalPaddingPropertion = 0.1f;

            var paddedRegion = new SKRect(region.Left + horizontalPaddingPropertion * region.Width, region.Top + vertialPaddingProportion * region.Height, region.Right - horizontalPaddingPropertion * region.Width, region.Bottom - vertialPaddingProportion * region.Height);
            var lines = splitToLines(englishCardSpec.englishWord);
            var longestLine = lines.OrderByDescending(l => l.Length).First();
            var initialPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Typeface = _regularTypeFace
            };
            var resultForLongestLine = sizeTextForPaddedRect(longestLine, initialPaint, paddedRegion, 0.75f);
            var heightForLongestLine = resultForLongestLine.rect.Height;
            var lineSpace = 0.2f * heightForLongestLine;
            var lineRects = new Stack<SKRect>(CardDrawingFunctions.GetLineRects(paddedRegion, lines.Count(), lineSpace));

            foreach (var l in lines)
            {
                SKRect textBoundsForLine = new SKRect();
                resultForLongestLine.paintToUse.MeasureText(l, ref textBoundsForLine);
                DrawTextCentredInsideRect(l, lineRects.Pop(), textBoundsForLine, resultForLongestLine.paintToUse, canvas);
            }
        }
        private void DrawChineseCard(SKRect region, ChineseCardSpec chineseCardSpec, SKCanvas canvas)
        {
            float amountToLeaveForSubtitles = region.Height * 1 / 4;
            var cardAreaForCharacter = new SKRect(region.Left, region.Top, region.Right, region.Bottom - amountToLeaveForSubtitles);

            DrawTextVisuallyCenteredInsideRectWithResize(cardAreaForCharacter, chineseCardSpec.chineseCharacter, _boldTypeFace, 0, canvas);

            var cardAreaForPinyin = new SKRect(cardAreaForCharacter.Left, cardAreaForCharacter.Bottom, cardAreaForCharacter.Right, cardAreaForCharacter.Bottom + (region.Height - cardAreaForCharacter.Height));

            DrawTextVisuallyCenteredInsideRectWithResize(cardAreaForPinyin, PadTextTo(20, chineseCardSpec.pinyin), _regularTypeFace, 0.15f, canvas);
        }
               
        public void RenderToPng(Stream frontPageTargetStream, Stream backPageTargetStream)
        {

            var info = new SKImageInfo(PAGE_WIDTH, PAGE_HEIGHT);
            
            using (var frontPageSurface = SKSurface.Create(info))
            using (var backPageSurface = SKSurface.Create(info))
            {
                var frontPageCanvas = frontPageSurface.Canvas;
                var backPageCanvas = backPageSurface.Canvas;

                frontPageCanvas.Clear(SKColors.White);
                backPageCanvas.Clear(SKColors.White);

                CardDrawingFunctions.AddGridLines(_numberOfColumns, _numberOfRows, _gridArea, frontPageCanvas);
                CardDrawingFunctions.AddGridLines(_numberOfColumns, _numberOfRows, _gridArea, backPageCanvas);

                // for printing double sided, long edge

                for (int r = 0; r < _numberOfRows; r++)
                {
                    for (int c = 0; c < _numberOfColumns; c++)
                    {
                        if (_cards.Peek() != null)
                        {
                            var card = _cards.Dequeue();
                            var firstPageCardRegion = _cardRegions[r,c];
                            var backOfCardOverPage = _cardRegions[r,_numberOfColumns - c - 1];
                            DrawChineseCard(firstPageCardRegion, card.ChineseCardSpec, frontPageCanvas);
                            DrawEnglishCard(backOfCardOverPage, card.EnglishCardSpec, backPageCanvas);
                        }
                    }
                }


                using (var image = frontPageSurface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    data.SaveTo(frontPageTargetStream);

                using (var image = backPageSurface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    data.SaveTo(backPageTargetStream);
            }
        }

        private SKRect[,] GetCardRegionsByRowsThenColumns(SKRect gridArea)
        {
            var cardSize = gridArea.Width / _numberOfColumns;

            SKRect[,] grid = new SKRect[_numberOfRows, _numberOfColumns];
            for (int row = 0; row < _numberOfRows; row++)
            {
                for (int column = 0; column < _numberOfColumns; column++)
                {
                    var topLeft = new SKPoint(gridArea.Left + column * cardSize, gridArea.Top + row * cardSize);
                    var rect = new SKRect(topLeft.X, topLeft.Y, topLeft.X + cardSize, topLeft.Y + cardSize);
                    grid[row,column] = rect;
                }
            }
            return grid;
        }

    }
}
