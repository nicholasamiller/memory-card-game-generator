using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;

namespace KmipCards.Client.Dialogs
{
    public partial class OpenSetDialog : ComponentBase
    {
        // needs a list of folders
        // return a name to open
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        void Cancel() => MudDialog.Cancel();
        [Parameter] public List<string> Sets { get; set; }
        
        MudList SelectedItem { get; set; }
        object selectedValue;

        void Open()
        {
            // return name of selected
        }

    }
}
