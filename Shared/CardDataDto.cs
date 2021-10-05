using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryCardGenerator.Shared
{
    public class CardDataDto
    {
        public string Chinese { get; set;  }
        public string Pinyin {  get; set; }
        public string English {  get; set; }

        public override bool Equals(object obj)
        {
            return obj is CardDataDto dto &&
                   Chinese == dto.Chinese &&
                   Pinyin == dto.Pinyin &&
                   English == dto.English;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Chinese, Pinyin, English);
        }
    }
}
