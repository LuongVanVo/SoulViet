using System.ComponentModel;

namespace SoulViet.Shared.Domain.Enums
{
    public enum VibeTag
    {
        [Description("Chữa lành & Yên bình")]
        ChuaLanhVaYenBinh = 1,
        [Description("Năng động & Phiêu lưu")]
        NangDongVaPhieuLuu = 2,
        [Description("Sang trọng & Đẳng cấp")]
        SangTrongVaDangCap = 3,

        [Description("Sáng tạo & Truyền cảm hứng")]
        SangTaoVaTruyenCamHung = 4,

        [Description("Trải nghiệm đa dạng")]
        TraiNghiemDaDang = 5,

        [Description("Đậm văn hóa & Bản địa")]
        DamVanHoaVaBanDia = 6
    }
}