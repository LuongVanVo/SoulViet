using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoulViet.Shared.Domain.Enums
{
    public enum PostStatus
    {
        [Description("Bản nháp")]
        Draft = 0,
        [Description("Đã đăng")]
        Published = 1,
        [Description("Đã xóa")]
        Deleted = 2
    }
}
