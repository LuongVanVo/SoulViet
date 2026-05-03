using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoulViet.Shared.Domain.Enums
{
    public enum NotificationType
    {
        [Description("Thích bài viết")]
        Liked = 1,
        [Description("Bình luận bài viết")]
        Commented = 2,
        [Description("Theo dõi")]
        Followed = 3,
        [Description("Nhắc đến")]
        Mentioned = 4,
        [Description("Chia sẻ bài viết")]
        Shared = 5,
        [Description("Tài khoản đã được xác minh")]
        PartnerVerified = 6,
        [Description("Tin nhắn mới")]
        Message = 7
    }
}
