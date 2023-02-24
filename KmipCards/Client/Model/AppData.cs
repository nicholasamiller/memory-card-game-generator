using KmipCards.Shared;
using System.Collections.Generic;

namespace KmipCards.Client.Model
{

    public class AppData
    {
        public AppData(string defaultCardSetName, List<CardSet> cardsets)
        {
            DefaultCardSetName = defaultCardSetName;
            CardSets = cardsets;
        }

        public string DefaultCardSetName { get; set; }
        public List<CardSet> CardSets { get; set; }
        
    }

  }
