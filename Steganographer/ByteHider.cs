using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganographer
{
    public static class ByteHider
    { 
        /// <summary>
        /// Hides every two bits in <paramref name="treasure"/> into the last two bits of each byte in <paramref name="hideout"/>
        /// </summary>
        public static uint HideByte(uint hideout, byte treasure) {
            for (int i = 0; i < 4; i++) {
                uint treasureChest = (uint) (treasure & (0b11 << (i * 2)));
                // move the bits by 8i-2i=6i
                treasureChest <<= i * 6;
                uint mask = (uint)(0b11 << (i * 8));
                // at this point, (treasureChest & mask) should be equal to treasureChest
                // clear the bits in hideout and set them to the bits in treasureChest
                hideout = (hideout & ~mask) | treasureChest;
            }

            return hideout;
        }
        /// <summary>
        /// Recovers the byte hidden by <see cref="HideByte"/>
        /// </summary>
        public static byte RecoverByte(uint hideout) {
            byte result = 0;
            for (int i = 0; i < 4; i++) {
                uint mask = (uint) (0b11 << (i * 8));
                uint treasureChest = hideout & mask;
                treasureChest >>= i * 6;
                result |= (byte)treasureChest;
            }

            return result;
        }
    }
}
