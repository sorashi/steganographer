using System;
using Xunit;
using Xunit.Abstractions;

namespace Steganographer.Test
{
    public class XorChecksumCalculatorTest
    {
        private readonly ITestOutputHelper _output;

        [Theory]
        [MemberData(nameof(TestData))]
        public void ChecksumLengthHoldsTest(byte[] data) {
            var calc = new XorChecksumCalculator();
            var checksum = calc.GetChecksum(data);
            Assert.Equal(calc.ChecksumSize, checksum.Length);
        }
        [Theory]
        [MemberData(nameof(TestData))]
        public void SameDataSameChecksumTest(byte[] data) {
            var calc = new XorChecksumCalculator();
            var c1 = calc.GetChecksum(data);
            var c2 = calc.GetChecksum(data);
            Assert.Equal(c1, c2);
            _output.WriteLine(string.Join(", ", c1));
        }

        [Fact]
        public void SliceChecksumTest() {
            var calc = new XorChecksumCalculator();
            var data = new byte[] {
                9, 8, 7, 6, 5, 4, 3, 2, 1
            };
            var c1 = calc.GetChecksum(data, 2, 3);
            var c2 = calc.GetChecksum(new byte[] {7, 6, 5});
            Assert.Equal(c1, c2);
        }

        [Fact]
        public void NullDataThrowsExceptionTest() {
            var calc = new XorChecksumCalculator();
            Assert.Throws<NullReferenceException>(() => {
                calc.GetChecksum(null);
            });
        }

        public static object[][] TestData = {
            new object[] {new byte[] {1, 2, 3, 4, 5, 6, 7, 8}},
            new object[] {new byte[] {0, 0, 0, 0, 0, 0}},
            new object[] {new byte[] {}},
            new object[] {new byte[] {255}}
        };

        public XorChecksumCalculatorTest(ITestOutputHelper output) {
            _output = output;
        }
    }
}
