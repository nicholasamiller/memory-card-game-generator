using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KmipCards.Shared
{
    public class CardDataDto
    {
        [Required]
        [StringLength(6, ErrorMessage = "Six characters maximum.")]
        public string Chinese { get; set;  }
        
        [Required]
        [StringLength(20, ErrorMessage = "20 characters maximum.")]
        public string Pinyin {  get; set; }
        
        [Required]
        [StringLength(20, ErrorMessage = "20 characters maximum.")]
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
