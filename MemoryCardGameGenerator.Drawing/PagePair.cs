using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MemoryCardGameGenerator.Model;
using SkiaSharp;
using System.Text.RegularExpressions;

namespace MemoryCardGameGenerator.Drawing
{

    public record TypeFacesConfig(SKTypeface regular, SKTypeface bold, SKTypeface light);

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
        private List<CardPairSpec> _cards = new List<CardPairSpec>();
        private SKTypeface _boldTypeFace;
        private SKTypeface _regularTypeFace;
        private SKTypeface _lightTypeFace;

        public PagePair(int numberOfColumns, int numberOfRows, TypeFacesConfig typeFacesConfig)
        {

            var horizontalBleed = (PAGE_WIDTH - PAGE_WIDTH_BLED) / 2;
            var verticalBleed = (PAGE_HEIGHT - PAGE_HEIGHT_BLED) / 2;


            _gridArea = new SKRect(horizontalBleed, verticalBleed, PAGE_WIDTH_BLED + horizontalBleed, PAGE_HEIGHT_BLED + verticalBleed);
            _numberOfColumns = numberOfColumns;
            _numberOfRows = numberOfRows;

            _cardRegions = GetCardRegionsByRowsThenColumns(_gridArea);

            _boldTypeFace = typeFacesConfig.bold;
            _regularTypeFace = typeFacesConfig.regular;
            _lightTypeFace = typeFacesConfig.light;

        }


        public void AddCard(CardPairSpec cardPairSpec)
        {
            _cards.Add(cardPairSpec);
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
            var matches = Regex.Matches(text, @"(.*,|.*\n|.*$)");
            return matches.ToList().Select(m => m.Value.Trim()).Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
        }

    
      



        private void DrawEnglishCard(SKRect region, EnglishCardSpec englishCardSpec, SKCanvas canvas)
        {
            var vertialPaddingProportion = 0.2f;
            var horizontalPaddingPropertion = 0.1f;

            var paddedRegion = new SKRect(region.Left + horizontalPaddingPropertion * region.Width, region.Top + vertialPaddingProportion * region.Height, region.Right - horizontalPaddingPropertion * region.Width, region.Bottom - vertialPaddingProportion * region.Height);

            var lines = splitToLines(englishCardSpec.englishWord);
            CardDrawingFunctions.DrawTextBox(lines, _boldTypeFace, paddedRegion, canvas);
        }
        private void DrawChineseCard(SKRect region, ChineseCardSpec chineseCardSpec, SKCanvas canvas)
        {
            float amountToLeaveForSubtitles = region.Height * 1 / 4;
            var cardAreaForCharacter = new SKRect(region.Left, region.Top, region.Right, region.Bottom - amountToLeaveForSubtitles);

            CardDrawingFunctions.DrawTextVisuallyCenteredInsideRectWithResize(cardAreaForCharacter, chineseCardSpec.chineseCharacter, _boldTypeFace, 0, canvas);

            var cardAreaForPinyin = new SKRect(cardAreaForCharacter.Left, cardAreaForCharacter.Bottom, cardAreaForCharacter.Right, cardAreaForCharacter.Bottom + (region.Height - cardAreaForCharacter.Height));

            CardDrawingFunctions.DrawTextVisuallyCenteredInsideRectWithResize(cardAreaForPinyin, PadTextTo(20, chineseCardSpec.pinyin), _lightTypeFace, 0.15f, canvas);
        }
       
        public void RenderBackPage(SKCanvas backPageCanvas)
        {
            backPageCanvas.Clear(SKColors.White);

            CardDrawingFunctions.AddGridLines(_numberOfColumns, _numberOfRows, _gridArea, backPageCanvas);

            // for printing double sided, long edge

            var cardsQueue = new Queue<CardPairSpec>(_cards);

            for (int r = 0; r < _numberOfRows; r++)
            {
                for (int c = 0; c < _numberOfColumns; c++)
                {
                    if (cardsQueue.Count() != 0)
                    {
                        var card = cardsQueue.Dequeue();
                        var backOfCardOverPage = _cardRegions[r, _numberOfColumns - c - 1];
                        DrawEnglishCard(backOfCardOverPage, card.EnglishCardSpec, backPageCanvas);
                    }
                }
            }
        }

        public void RenderFrontPage(SKCanvas frontPageCanvas)
        {

            frontPageCanvas.Clear(SKColors.White);

            CardDrawingFunctions.AddGridLines(_numberOfColumns, _numberOfRows, _gridArea, frontPageCanvas);

            // for printing double sided, long edge

            var cardsQueue = new Queue<CardPairSpec>(_cards);

            for (int r = 0; r < _numberOfRows; r++)
            {
                for (int c = 0; c < _numberOfColumns; c++)
                {
                    if (cardsQueue.Count() != 0)
                    {
                        var card = cardsQueue.Dequeue();
                        var firstPageCardRegion = _cardRegions[r, c];
                        DrawChineseCard(firstPageCardRegion, card.ChineseCardSpec, frontPageCanvas);
                    }
                }
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
