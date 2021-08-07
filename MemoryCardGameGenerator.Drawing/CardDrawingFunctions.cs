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
