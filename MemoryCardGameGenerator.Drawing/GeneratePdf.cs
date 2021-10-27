using MemoryCardGameGenerator.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCardGameGenerator.Drawing
{
    public static class Generate
    {
        
         
        
        
        public static void WritePdf(Stream outputStream, List<CardPairSpec> specs, int cardsPerRow)
        {
            
            using (var bt = new MemoryStream(Drawing.Properties.Resources.msyhbd))
            using (var lt = new MemoryStream(Drawing.Properties.Resources.msyhl))
            using (var rt = new MemoryStream(Drawing.Properties.Resources.msyh))
            using (var pdfOutput = outputStream)
            {
                var typeFaces = new TypeFacesConfig(SKTypeface.FromStream(rt), SKTypeface.FromStream(bt), SKTypeface.FromStream(lt));
                var doc = new PdfCardsDocument(specs, cardsPerRow, typeFaces);
                doc.Render(pdfOutput);
            }

        }
    }
}
