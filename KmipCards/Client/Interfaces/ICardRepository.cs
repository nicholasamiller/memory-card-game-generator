using KmipCards.Client.Model;
using KmipCards.Shared;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ICardRepository
    {
        Task<CardSet> GetCardSetAsync(string cardsetName);
        Task SaveCardSetAsync(CardSet cardSet);

    }

}
