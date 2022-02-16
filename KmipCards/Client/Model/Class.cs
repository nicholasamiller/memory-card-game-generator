using KmipCards.Shared;
using System.Collections.Generic;

namespace KmipCards.Client.Model
{
    public record CardSet(string name, List<CardRecord> cards);

    public record CardSetCollection(string defaultCardSetName, List<CardSet> cardSets);

  }
