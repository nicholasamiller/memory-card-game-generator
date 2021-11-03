using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace KmipCards.Shared
{

    public class CardRecord
    {
        public CardDataDto CardDataDto { get; set; }
        public IEnumerable<string> Tags { get; set; }

        private static Regex tagRegex = new Regex(@"\s#[^\s]");
        private static Regex lineRegex = new Regex(@"^(.*\s)\((.*)\)(\s.*)(\s#[^\s])*$");
        public static CardRecord ParseFromLine(string line)
        {
            var r = lineRegex.Match(line);
            if (r.Success)
            {
                var cardDto = new CardDataDto() { Chinese = r.Groups[1].Value, Pinyin = r.Groups[2].Value, English = r.Groups[3].Value };
                var tags = r.Groups[4].Success ? tagRegex.Matches(r.Groups[4].Value).Select(m => m.Value) : new List<string>();
                return new CardRecord() { CardDataDto = cardDto, Tags = tags };

            }
            return null;
            
        }

		public override bool Equals(object obj)
		{
            return obj is CardRecord record &&
                   EqualityComparer<CardDataDto>.Default.Equals(CardDataDto, record.CardDataDto);				   
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CardDataDto, Tags);
		}
	}
}
