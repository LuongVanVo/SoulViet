using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Results
{
    public class ComboExperienceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public Guid GuideId { get; set; }
        public Guid ServicePartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public List<string> MediaUrls { get; set; } = new(); 
        public bool IsActive { get; set; }
    }
}
