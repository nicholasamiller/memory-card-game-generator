using System;

namespace MemoryCardGameGenerator.Model
{

    public record CardPairSpec(ChineseCardSpec ChineseCardSpec, EnglishCardSpec EnglishCardSpec);
    public record ChineseCardSpec(string chineseCharacter, string pinyin);
    public record EnglishCardSpec(string englishWord);

    
}
