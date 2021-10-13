using MemoryCardGenerator.Shared;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface ITranslationProvider
    {
        public Task<CardDataDto> TranslateAsync(CardDataDto cardDataDto);
    }
}
