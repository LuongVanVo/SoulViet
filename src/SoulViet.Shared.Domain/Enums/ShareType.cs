using System.ComponentModel;

namespace SoulViet.Shared.Domain.Enums
{
    public enum ShareType
    {
        [Description("Share lên Timeline")]
        Timeline = 1,

        [Description("Share qua tin nhắn")]
        Message = 2,

        [Description("Share ra bên ngoài")]
        External = 3
    }
}
