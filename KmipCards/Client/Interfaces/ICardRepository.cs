using KmipCards.Client.Model;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        Task<AppData> GetAppDataAsync();
        Task SetAppDataAsync(AppData appData);
    }

}
