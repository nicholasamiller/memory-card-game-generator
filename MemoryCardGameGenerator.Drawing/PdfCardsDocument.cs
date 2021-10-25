using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MemoryCardGameGenerator.Model;
using SkiaSharp;

namespace MemoryCardGameGenerator.Drawing
{
    public class PdfCardsDocument
    {
        private List<PagePair> _pagePairs;
        private readonly TypeFacesConfig _typeFacesConfig;
        
        private class CardNumberData
        {
            public int CardsPerRow { get; set; }
            public int NumberOfRows { get; set; }
            public int GetCardsPerPage() => CardsPerRow * NumberOfRows;
        }

        private CardNumberData InferCardNumberData(int cardsPerRow)
        {
            if (cardsPerRow == 1)
            {
                return new CardNumberData() { CardsPerRow = cardsPerRow, NumberOfRows = 1 };
            }
            else if (cardsPerRow == 2)
            {
                // six runs off end of page
                return new CardNumberData() { CardsPerRow = cardsPerRow, NumberOfRows = 2 };
            }
            else if (cardsPerRow == 3)
            {
                return new CardNumberData() { CardsPerRow = cardsPerRow, NumberOfRows = 4 };
            }
            else if (cardsPerRow == 4)
            {

                return new CardNumberData() { CardsPerRow = cardsPerRow, NumberOfRows = 5 };
            }
            else
            {
                return new CardNumberData() { CardsPerRow = cardsPerRow, NumberOfRows = cardsPerRow + 1 };
            }
        }

        public PdfCardsDocument(IEnumerable<CardPairSpec> cards, int numberOfCardsPerRow, TypeFacesConfig typeFacesConfig)
        {
            var cardNumberData = InferCardNumberData(numberOfCardsPerRow);
            var batched = Batch(cards,cardNumberData.GetCardsPerPage());
            List<PagePair> pagePairs = new List<PagePair>();
            foreach (var b in batched)
            {
                var pp = new PagePair(cardNumberData.CardsPerRow,cardNumberData.NumberOfRows,typeFacesConfig);
                foreach (var c in b)
                {
                    pp.AddCard(c);
                }
                pagePairs.Add(pp);
            }
            // add empty one at end
            pagePairs.Add(new PagePair(cardNumberData.CardsPerRow,cardNumberData.NumberOfRows, typeFacesConfig));
            
            _pagePairs = pagePairs;

            this._typeFacesConfig = typeFacesConfig;
        }
        

        public void Render(Stream outputPdfStream)
        {
            var pdfMetadata = SKDocumentPdfMetadata.Default;
            pdfMetadata.Author = "Nick Miller";
            pdfMetadata.Creation = System.DateTime.Now;
            pdfMetadata.Creator = "Nick's Memory Card Generator Program";
            pdfMetadata.Title = "Memory Game and Flashcards for KMIP Mawson Primary";
            pdfMetadata.RasterDpi = 300;

            var pdfDoc = SKDocument.CreatePdf(outputPdfStream, pdfMetadata);

     
            foreach (var pp in _pagePairs)
            {
                var frontPage = pdfDoc.BeginPage(PagePair.PAGE_WIDTH, PagePair.PAGE_HEIGHT);
                pp.RenderFrontPage(frontPage);
                pdfDoc.EndPage();
                var backPage = pdfDoc.BeginPage(PagePair.PAGE_WIDTH, PagePair.PAGE_HEIGHT);
                pp.RenderBackPage(backPage);
                pdfDoc.EndPage();
            }

            pdfDoc.Close();
          
        }


        private static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> items,
                                                       int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / maxItems)
                        .Select(g => g.Select(x => x.item));
        }
    }
}
