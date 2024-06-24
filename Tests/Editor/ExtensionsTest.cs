using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Trackman.CommonUtils.Tests.Editor
{

    public class ExtensionsTest
    {
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
        #endregion
    }
}