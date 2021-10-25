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

        public PdfCardsDocument(IEnumerable<CardPairSpec> cards, int numberOfCardsPerRow, TypeFacesConfig typeFacesConfig)
        {
            if (numberOfCardsPerRow < 3)
            {
                throw new ArgumentException("Three cards per row minimum");
            }
            var cardsPerPage = numberOfCardsPerRow * (numberOfCardsPerRow + 1);
            var batched = Batch(cards,cardsPerPage);
            List<PagePair> pagePairs = new List<PagePair>();
            foreach (var b in batched)
            {
                var pp = new PagePair(numberOfCardsPerRow, numberOfCardsPerRow + 1,typeFacesConfig);
                foreach (var c in b)
                {
                    pp.AddCard(c);
                }
                pagePairs.Add(pp);
            }
            // add empty one at end
            pagePairs.Add(new PagePair(numberOfCardsPerRow, numberOfCardsPerRow + 1, typeFacesConfig));
            
            _pagePairs = pagePairs;


            this._typeFacesConfig = typeFacesConfig;
        }
        

        private void RenderTitlePage(SKCanvas sKCanvas)
        {
            var text = @"Lockdown Game for Mawson Primary KMIP: Fun!
Print A4, from page 2; cut along dashed lines.
Memory game: print single sided.
Suggest loading printer with alternating paper colours.
Flash cards: print double sided, flip on long edge.
Use card stock or glue to cardboard if keen.
Extra blank cards at end.";

            var lines = text.Split(Environment.NewLine).ToList();
            var regionForText = SKRect.Create(new SKPoint(PagePair.PAGE_WIDTH / 20, PagePair.PAGE_HEIGHT / 20), new SKSize(PagePair.PAGE_WIDTH / 1.1f, PagePair.PAGE_HEIGHT / 1.1f));

            CardDrawingFunctions.DrawTextBox(lines, _typeFacesConfig.regular, regionForText, sKCanvas);
                        
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

            //var titlePage = pdfDoc.BeginPage(PagePair.PAGE_WIDTH, PagePair.PAGE_HEIGHT);
            //RenderTitlePage(titlePage);
            //pdfDoc.EndPage();

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
