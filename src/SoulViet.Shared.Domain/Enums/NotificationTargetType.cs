using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoulViet.Shared.Domain.Enums
{
    public enum NotificationTargetType
    {
        [Description("None")]
        None = 0,
        [Description("Post")]
        Post = 1,
        [Description("Comment")]
        Comment = 2,
        [Description("User")]
        User = 3,
        [Description("Follow")]
        Follow = 4,
        [Description("Message")]
        Message = 5
    }
}
