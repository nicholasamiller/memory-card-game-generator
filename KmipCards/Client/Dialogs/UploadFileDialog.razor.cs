using KmipCards.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace KmipCards.Client.Dialogs
{
    public partial class UploadFileDialog : ComponentBase
    {
        [Inject]
        private ICardSetViewModel CardRepository { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private void UploadFile(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles(1);
        }

    }
}
