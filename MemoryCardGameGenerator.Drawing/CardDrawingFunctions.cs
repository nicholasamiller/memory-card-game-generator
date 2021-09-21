using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MemoryCardGameGenerator.Model;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using Topten.RichTextKit;
using Path = SixLabors.ImageSharp.Drawing.Path;

namespace MemoryCardGameGenerator.Drawing
{


 
    public static class CardDrawingFunctions
    {
        public static List<SKRect> GetLineRects(SKRect boundingBox, int numberOfLines, float lineSpacing)
        {
            var numberOfSpaces = numberOfLines - 1;
            var lineSpaceHeight = numberOfSpaces * lineSpacing;
            var remainingHeightForLines = boundingBox.Height - lineSpaceHeight;
            var lineHeight = remainingHeightForLines / numberOfLines;
            List<SKRect> acc = new List<SKRect>();
            // add first one, then add space plus line
            var currentTopLeft = new SKPoint(boundingBox.Left, boundingBox.Top);
            for (int i = 0; i < numberOfLines; i++)
            {
                var lineRect = SKRect.Create(currentTopLeft, new SKSize(boundingBox.Width, lineHeight));
                // move current top left to next line
                currentTopLeft.Y = currentTopLeft.Y + lineRect.Height + lineSpacing;
                acc.Add(lineRect);
            }
            acc.Reverse();
            return acc;
        }


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
                .Select(c => new
                {
                    Start = new SKPoint(area.Left, area.Top + c * cardSize),
                    End = new SKPoint(area.Left + cardSize * numberOfCardsPerRow, area.Top + c * cardSize)
                })
                .ToList()
                .ForEach(p => canvas.DrawLine(p.Start, p.End, skPaint));

        }

        public record TextBoxResult(SKRect rect, SKPaint paintToUse);
        public static TextBoxResult sizeTextForPaddedRect(string text, SKPaint paint, SKRect boundingBox, float paddingProportion)
        {
            var initialWidth = paint.MeasureText(text);
            var textSize = paint.TextSize / initialWidth * boundingBox.Width * paddingProportion;
            var newPaint = paint.Clone();
            newPaint.TextSize = textSize;
            SKRect textBounds = new SKRect();
            newPaint.MeasureText(text, ref textBounds);
            return new TextBoxResult(textBounds, newPaint);
        }

        
        public static void DrawTextBox(List<string> lines, SKTypeface sKTypeface, SKRect sKRect, SKCanvas canvas)
        {
            var longestLine = lines.OrderByDescending(l => l.Length).First();
            var initialPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Typeface = sKTypeface
            };
            var resultForLongestLine = CardDrawingFunctions.sizeTextForPaddedRect(longestLine, initialPaint, sKRect, 0.75f);
            var heightForLongestLine = resultForLongestLine.rect.Height;
            var lineSpace = 0.2f * heightForLongestLine;
            var lineRects = new Stack<SKRect>(CardDrawingFunctions.GetLineRects(sKRect, lines.Count(), lineSpace));

            foreach (var l in lines)
            {
                SKRect textBoundsForLine = new SKRect();
                resultForLongestLine.paintToUse.MeasureText(l, ref textBoundsForLine);
                CardDrawingFunctions.DrawTextCentredInsideRect(l, lineRects.Pop(), textBoundsForLine, resultForLongestLine.paintToUse, canvas);
            }
        }

        public static void DrawTextCentredInsideRect(string text, SKRect boundingBox, SKRect textRect, SKPaint paint, SKCanvas sKCanvas)
        {
            var topLeftPointToGetTextCentred = new SKPoint(boundingBox.Left + boundingBox.Width / 2 - textRect.MidX, boundingBox.Top + (boundingBox.Height / 2) - textRect.MidY);
            sKCanvas.DrawText(text, topLeftPointToGetTextCentred, paint);
        }

        public static void DrawTextVisuallyCenteredInsideRectWithResize(SKRect rect, string text, SKTypeface sKTypeface, float upwardsOffsetProportion, SKCanvas canvas)
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



    }
}
