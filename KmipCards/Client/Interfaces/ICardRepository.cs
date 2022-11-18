using KmipCards.Client.Model;
using KmipCards.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface IAppDataRepository
    {
        Task SetDefaultCardSetName(string cardsetName);
        Task<CardSet> GetCardSetAsync(string cardsetName);
        Task SaveCardSetAsync(CardSet cardSet);
        Task<IEnumerable<string>> GetCardSetNames();

    }

}
