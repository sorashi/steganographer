using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganographer
{
    public abstract class ChecksumCalculator
    {
        public abstract byte[] GetChecksum(byte[] data, int startIndex = 0, int length = -1);
        /// <summary>
        /// The resulting checksum size in bytes
        /// </summary>
        public abstract int ChecksumSize { get; }
    }
}
