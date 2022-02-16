using KmipCards.Client.Model;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        Task<CardSet> GetCardSetAsync(string name);
        Task<CardSet> GetDefaultCardSetAsync();
        Task SetDefaultCardSetName(string name);
        Task SaveCardSetAsync(CardSet cardSet);
    }

}
