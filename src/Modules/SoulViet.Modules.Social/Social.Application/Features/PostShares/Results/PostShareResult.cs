using System;

namespace SoulViet.Modules.Social.Social.Application.Features.PostShares.Results
{
    public class PostShareResult
    {
        public Guid ShareId { get; set; }
        public int TotalShares { get; set; }
        public string ShareUrl { get; set; }

        public PostShareResult(Guid shareId, int totalShares, string shareUrl)
        {
            ShareId = shareId;
            TotalShares = totalShares;
            ShareUrl = shareUrl;
        }
    }
}
