using MagicBytesValidator.Models;

namespace Business.Helpers
{
    public class PngFileType : FileByteFilter
    {
        public PngFileType() : base(
            ["image/png"],
            ["png"]
        )
        {
            StartsWith([
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A 
            ]);
        }
    }
}