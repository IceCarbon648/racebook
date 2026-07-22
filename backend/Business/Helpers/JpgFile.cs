using MagicBytesValidator.Models;

namespace Business.Helpers
{
    public class JpgFileType : FileByteFilter
    {
        public JpgFileType() : base(
            ["image/jpeg"],
            ["jpg", "jpeg"]
        )
        {
            StartsWith([
                0xFF, 0xD8, 0xFF
            ]);
        }
    }
}