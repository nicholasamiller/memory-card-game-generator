using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MemoryCardGameGenerator.Model
{
    public static class CardsData
    {

        public static  IEnumerable<CardPairSpec> LoadSpecsFromResources()
        {
            return Properties.Resources.cardsData.Split(Environment.NewLine).Select(ParseLine);
        }

        private static Regex lineRegex = new Regex(@"^(.*)\((.*)\)(.*)$");
        private static CardPairSpec ParseLine(string line)
        {
            var r = lineRegex.Match(line);
            return new CardPairSpec(new ChineseCardSpec(r.Groups[1].Value, r.Groups[2].Value), new EnglishCardSpec(r.Groups[3].Value.Trim()));
        }
    }
}
