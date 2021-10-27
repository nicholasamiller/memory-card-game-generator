
namespace KmipCards.Shared
{
    public class CardsGenerationRequestDto
    {
        public string Name { get; set; }

        public CardsPerPage CardsPerPage { get; set; }

        public CardRecord[] Cards { get; set; }
    }
}
