using MagicBytesValidator.Models;

namespace Business.Helpers
{
    public class TpfFileType : FileByteFilter
    {
        public TpfFileType() : base(
            ["application/octet-stream"],
            ["tpf"]
        )
        {
            StartsWith([
                0xF4, 0x74, 0xA7, 0x3B, 0xB0, 0x3F, 0xAD, 0x3F, 0xAC, 0x3F
            ]);
        }
    }
}