using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Trackman.CommonUtils.Tests.Editor
{
    public class ExtensionsTest
    {
        #region Containers
        [Flags] enum TestEnumInts { A = 0x1, B = 0x2, C = 0x4, D = 0x8, F = 1 << 31 }
        [Flags] enum TestEnumShorts : short { A = 0x1, B = 0x2, C = 0x4, D = 0x8 }
        [Flags] enum TestEnumBytes : byte { A = 0x1, B = 0x2, C = 0x4, D = 0x8 }
        [Flags] enum TestEnumLong : long { A = 0x1, B = 0x2, C = 0x4, D = 0x8, F = 1L << 63 }
        #endregion

        #region Methods
        [Test]
        public void Reversing()
        {
            IList<int> data = new[] { 4, 8, 15, 16, 23, 42 };
            IList<int> dataReversed = new[] { 42, 23, 16, 15, 8, 4 };
            data.ReverseInPlace();
            Assert.AreEqual(dataReversed, data);

            IList<int> dataOdd = new[] { 4, 8, 15, 0, 16, 23, 42 };
            IList<int> dataOddReversed = new[] { 42, 23, 16, 0, 15, 8, 4 };
            dataOdd.ReverseInPlace();
            Assert.AreEqual(dataOddReversed, dataOdd);

            IList<int> empty = Array.Empty<int>();
            empty.ReverseInPlace();

            IList<int> none = default;
            none.ReverseInPlace();
        }
        [Test]
        public void TestEnumFlagsInts()
        {
            const TestEnumInts ab = TestEnumInts.A | TestEnumInts.B;
            Assert.IsTrue(ab.ANY(TestEnumInts.A));
            Assert.IsTrue(ab.ANY(TestEnumInts.B));
            Assert.IsFalse(ab.ANY(TestEnumInts.C));
            Assert.IsFalse(ab.ANY(TestEnumInts.D));

            const TestEnumInts bd = TestEnumInts.B | TestEnumInts.D;
            Assert.IsTrue(bd.AND(TestEnumInts.D));
            Assert.IsTrue(bd.AND(TestEnumInts.D | TestEnumInts.B));
            Assert.IsFalse(bd.AND(TestEnumInts.A));
            Assert.IsFalse(bd.AND(TestEnumInts.B | TestEnumInts.B | TestEnumInts.A));
        }
        [Test]
        public void TestEnumFlagsShorts()
        {
            const TestEnumShorts ab = TestEnumShorts.A | TestEnumShorts.B;
            Assert.IsTrue(ab.ANY(TestEnumShorts.A));
            Assert.IsTrue(ab.ANY(TestEnumShorts.B));
            Assert.IsFalse(ab.ANY(TestEnumShorts.C));
            Assert.IsFalse(ab.ANY(TestEnumShorts.D));

            const TestEnumShorts bd = TestEnumShorts.B | TestEnumShorts.D;
            Assert.IsTrue(bd.AND(TestEnumShorts.D));
            Assert.IsTrue(bd.AND(TestEnumShorts.D | TestEnumShorts.B));
            Assert.IsFalse(bd.AND(TestEnumShorts.A));
            Assert.IsFalse(bd.AND(TestEnumShorts.B | TestEnumShorts.B | TestEnumShorts.A));
        }
        [Test]
        public void TestEnumFlagsBytes()
        {
            const TestEnumBytes ab = TestEnumBytes.A | TestEnumBytes.B;
            Assert.IsTrue(ab.ANY(TestEnumBytes.A));
            Assert.IsTrue(ab.ANY(TestEnumBytes.B));
            Assert.IsFalse(ab.ANY(TestEnumBytes.C));
            Assert.IsFalse(ab.ANY(TestEnumBytes.D));

            const TestEnumBytes bd = TestEnumBytes.B | TestEnumBytes.D;
            Assert.IsTrue(bd.AND(TestEnumBytes.D));
            Assert.IsTrue(bd.AND(TestEnumBytes.D | TestEnumBytes.B));
            Assert.IsFalse(bd.AND(TestEnumBytes.A));
            Assert.IsFalse(bd.AND(TestEnumBytes.B | TestEnumBytes.B | TestEnumBytes.A));
        }
        [Test]
        public void TestEnumFlagsLong()
        {
            const TestEnumLong ab = TestEnumLong.A | TestEnumLong.B;
            Assert.IsTrue(ab.ANY(TestEnumLong.A));
            Assert.IsTrue(ab.ANY(TestEnumLong.B));
            Assert.IsFalse(ab.ANY(TestEnumLong.C));
            Assert.IsFalse(ab.ANY(TestEnumLong.D));

            const TestEnumLong bd = TestEnumLong.B | TestEnumLong.D;
            Assert.IsTrue(bd.AND(TestEnumLong.D));
            Assert.IsTrue(bd.AND(TestEnumLong.D | TestEnumLong.B));
            Assert.IsFalse(bd.AND(TestEnumLong.A));
            Assert.IsFalse(bd.AND(TestEnumLong.B | TestEnumLong.B | TestEnumLong.A));

            const TestEnumLong af = TestEnumLong.A | TestEnumLong.F;
            Assert.IsTrue(af.ANY(TestEnumLong.A));
            Assert.IsTrue(af.ANY(TestEnumLong.F));
            Assert.IsTrue(af.AND(TestEnumLong.A | TestEnumLong.F));
        }
        #endregion
    }
}