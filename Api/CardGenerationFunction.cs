using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MemoryCardGenerator.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Api
{
    public class CardGenerationFunction
    {
        //[FunctionName("GenerateCards")]
        //public async Task<IAsyncResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] CardsGenerationRequest cardsGenerationRequest)
        //{
        //    using (var bt = new MemoryStream(Drawing.Properties.Resources.msyhbd))
        //    using (var rt = new MemoryStream(Drawing.Properties.Resources.msyh))
        //    using (var lt = new MemoryStream(Drawing.Properties.Resources.msyhl))

        //    using (var pdfOutput = GetTestOutputDirectoryStream("Lockdown KMIP Game.pdf"))
        //    {
        //        var typeFaces = new TypeFacesConfig(SKTypeface.FromStream(rt), SKTypeface.FromStream(bt), SKTypeface.FromStream(lt));
        //        var specs = CardsData.LoadSpecsFromString(testCardsData);
        //        var ut = new PdfCardsDocument(specs, 4, typeFaces);
        //        ut.Render(pdfOutput);
        //    }
        //}
    }
}
