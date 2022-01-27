using KmipCards.Shared;
using System.Collections.Generic;

namespace KmipCards.Client.Model
{
    public record CardSet(string name, IEnumerable<CardRecord> cards);
    public record CardSetCollection(IEnumerable<CardSet> cardSets);
}
