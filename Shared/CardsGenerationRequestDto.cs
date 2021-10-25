
namespace MemoryCardGenerator.Shared
{
    public class CardsGenerationRequestDto
    {
        public string Name { get; set; }

        public int? CardsPerPage { get; set; }

        public CardRecord[] Cards { get; set; }
    }
}
