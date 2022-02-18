using KmipCards.Shared;
using System.Collections.Generic;

namespace KmipCards.Client.Model
{
    public class CardSet
    {
        public CardSet(string name, List<CardRecord> cards)
        {
            Name = name;
            Cards = cards;
        }

        public string Name { get; set; }
        public List<CardRecord> Cards { get; set; }
    }

  }
