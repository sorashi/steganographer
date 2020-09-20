using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Steganographer.Test
{
    public class ByteHiderTest
    {
        [Theory]
        [MemberData(nameof(GetTestData), parameters:false)]
        public void HidesByteTheory(uint hideout, byte treasure, uint expected) {
            Assert.Equal(expected, ByteHider.HideByte(hideout, treasure));
        }
        [Theory]
        [MemberData(nameof(GetTestData), parameters: true)]
        public void RecoversByteTheory(uint hideout, byte expected) {
            Assert.Equal(expected, ByteHider.RecoverByte(hideout));
        }

        /// <summary>
        /// Changes the order of parameters for the test data to fit the theory
        /// </summary>
        public static IEnumerable<object[]> GetTestData(bool recovering = false) =>
            !recovering ? TestData : TestData.Select(x => new[] {x[2], x[1]});

        public static object[][] TestData = {
            new object[] {0xffffffff, 0, 0xfcfcfcfc},
            new object[] {0xaaaaaaaa, 0b00_01_10_11, 0b10101000_10101001_10101010_10101011}
        };
    }
}
