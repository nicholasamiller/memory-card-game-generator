using System;

namespace MemoryCardGameGenerator.Model
{
    public class CardPairSpec
    {
        public CardPairSpec(ChineseCardSpec chineseCardSpec, EnglishCardSpec englishCardSpec)
        {
            ChineseCardSpec = chineseCardSpec;
            EnglishCardSpec = englishCardSpec;
        }

        public ChineseCardSpec ChineseCardSpec { get; }
        public EnglishCardSpec EnglishCardSpec { get; }
    }

    public class ChineseCardSpec
    {
        public ChineseCardSpec(string chineseCharacter, string pinyin)
        {
            ChineseCharacter = chineseCharacter;
            Pinyin = pinyin;
        }

        public string ChineseCharacter { get; }
        public string Pinyin { get; }
    }
       

    public class EnglishCardSpec
    {
        public EnglishCardSpec(string englishWord)
        {
            EnglishWord = englishWord;
        }

        public string EnglishWord { get; }
    }


    
}
