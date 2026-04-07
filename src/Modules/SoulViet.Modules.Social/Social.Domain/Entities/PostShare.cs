using SoulViet.Modules.Social.Social.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class PostShare : BaseAuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Caption { get; set; } = string.Empty;
        // Navigation
        public Post Post { get; set; } = null!;
    }
}
