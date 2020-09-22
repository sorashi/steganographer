using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganographer
{
    public class MessageHeader
    {
        public FillStart Fill { get; set; }
        public ushort DataLength { get; set; }
        public byte DataSpacing { get; set; }
        public byte[] Checksum { get; set; }
        /// <summary>
        /// Type of checksum used
        /// </summary>
        public ChecksumCalculator ChecksumType { get; set; }

        /// <summary>
        /// Length of the header in bytes
        /// </summary>
        public int Length =>
            1 + 2 + 1 + (ChecksumType?.ChecksumSize ?? Checksum?.Length ?? throw new NullReferenceException());
        public byte[] GetBytes() {
            return new[] {(byte) Fill}.Concat(BitConverter.GetBytes(DataLength)).Concat(new[] {DataSpacing})
                .Concat(Checksum).ToArray();
        }
        /// <summary>
        /// Checksum from data and saves it into the header
        /// </summary>
        /// <param name="data"></param>
        public void SetChecksumFromData(byte[] data) {
            Checksum = ChecksumType.GetChecksum(data);
        }

        public static MessageHeader FromBytes(byte[] data) {
            if(data == null) throw new NullReferenceException();
            if(data.Length < 5) throw new ArgumentException();
            var fill = (FillStart) data[0];
            if(!Enum.IsDefined(typeof(FillStart), fill)) throw new FormatException();
            return new MessageHeader {
                Fill = fill,
                DataLength = BitConverter.ToUInt16(data, 1),
                DataSpacing = data[3],
                Checksum = data.Skip(4).ToArray()
            };
        }
    }
}
