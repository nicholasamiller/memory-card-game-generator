
namespace MemoryCardGenerator.Shared
{
    public class CardsGenerationRequestDto
    {
        public string Name { get; set; }

        public CardsPerPage CardsPerPage { get; set; }

        public CardRecord[] Cards { get; set; }
    }

    public enum CardsPerPage
    {
        One = 1,
        Four = 4,
        Twelve = 12,
        Twenty = 20
    }
}
