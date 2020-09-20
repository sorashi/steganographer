using System.Linq;

namespace Steganographer
{
    public class XorChecksumCalculator : ChecksumCalculator
    {
        public override byte[] GetChecksum(byte[] data, int startIndex = 0, int length = -1) {
            if (length == -1) length = data.Length; // take until the end
            return new[] {
                data.Skip(startIndex)
                    .Take(length)
                    .Aggregate((byte) 0, (a, c) => (byte) (a ^ c))
            };
        }
        public override int ChecksumSize { get; } = 1;
    }
}
