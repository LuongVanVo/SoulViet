using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoulViet.Modules.SoulMap.SoulMap.Domain.Entities
{
    public class PlaceMediaInfo
    {
        public string MainImage { get; set; } = string.Empty;
        public List<string> LandImages { get; set; } = new();
        public string? VideoUrl { get; set; }
    }
}