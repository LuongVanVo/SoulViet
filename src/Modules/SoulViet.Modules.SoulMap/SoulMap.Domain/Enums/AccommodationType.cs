using System.ComponentModel;

namespace SoulViet.Modules.SoulMap.SoulMap.Domain.Enums
{
    public enum AccommodationType
    {
        [Description("Bungalow")]
        Bungalow = 1,

        [Description("Căn hộ dịch vụ")]
        CanHoDichVu = 2,

        [Description("Homestay")]
        Homestay = 3,

        [Description("Hostel")]
        Hostel = 4,

        [Description("Khách sạn")]
        KhachSan = 5,

        [Description("Resort")]
        Resort = 6,

        [Description("Villa")]
        Villa = 7
    }
}