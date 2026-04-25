using System.ComponentModel;

namespace SoulViet.Modules.Social.Social.Domain.Enums
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
