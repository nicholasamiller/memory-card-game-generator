using KmipCards.Shared;
using System.Threading.Tasks;

namespace KmipCards.Client.Interfaces
{
    public interface ITranslationProvider
    {
        public Task<CardDataDto> TranslateAsync(CardDataDto cardDataDto);
    }
}
