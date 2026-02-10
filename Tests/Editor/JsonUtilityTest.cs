using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace Trackman.CommonUtils.Tests.Editor
{
    public class JsonUtilityTest
    {
        #region Containers
        public struct TestStruct
        {
            public string TestString;
            public int TestInt;
            public Vector3[] TestVector3Array;

            public void AssertEquals(TestStruct expected)
            {
                Assert.AreEqual(expected.TestString, TestString);
                Assert.AreEqual(expected.TestInt, TestInt);
                Assert.AreEqual(expected.TestVector3Array, TestVector3Array);
            }
        }
        #endregion

        #region Fields
        TestStruct testStruct = new() { TestString = "John", TestInt = 30, TestVector3Array = new Vector3[]{ new(1, 2, 3), new(4, 5, 6)} };
        string serializedTestStruct = "{\"TestString\":\"John\",\"TestInt\":30,\"TestVector3Array\":[{\"x\":1.0,\"y\":2.0,\"z\":3.0},{\"x\":4.0,\"y\":5.0,\"z\":6.0}]}";
        #endregion

        #region Methods
        [Test]
        public void ToJsonAllocByteArrayTest()
        {
            byte[] buffer = JsonUtility.ToJsonAlloc(testStruct);

            using MemoryStream memoryStream = new MemoryStream(buffer);
            using StreamReader streamReader = new StreamReader(memoryStream);
            string actualJson = streamReader.ReadToEnd();

            Assert.AreEqual(serializedTestStruct, actualJson);
        }
        [Test]
        public void ToJsonStringTest()
        {
            string actualJson = JsonUtility.ToJson(testStruct, false, false);
            Assert.AreEqual(serializedTestStruct, actualJson);
        }
        [Test]
        public void TestFromJsonByteArray()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(serializedTestStruct);
            TestStruct result = JsonUtility.FromJson<TestStruct>(bytes);
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonByteArrayWithType()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(serializedTestStruct);
            TestStruct result = (TestStruct)JsonUtility.FromJson(bytes, typeof(TestStruct));
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonStream()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(serializedTestStruct);
            using MemoryStream stream = new MemoryStream(bytes);
            TestStruct result = (TestStruct)JsonUtility.FromJson(stream, typeof(TestStruct));
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonString()
        {
            TestStruct result = (TestStruct)JsonUtility.FromJson(serializedTestStruct, typeof(TestStruct));
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonStringWithUseArrayPool()
        {
            TestStruct result = (TestStruct)JsonUtility.FromJson(serializedTestStruct, typeof(TestStruct));
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonStringWithTypeT()
        {
            TestStruct result = JsonUtility.FromJson<TestStruct>(serializedTestStruct);
            result.AssertEquals(testStruct);
        }
        [Test]
        public void TestFromJsonStringWithTypeTAndUseArrayPool()
        {
            TestStruct result = JsonUtility.FromJson<TestStruct>(serializedTestStruct);
            result.AssertEquals(testStruct);
        }
        [Test]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void TestToJsonAndBack(bool prettyPrint, bool useArrayPool)
        {
            string actualJson = JsonUtility.ToJson(testStruct, prettyPrint, useArrayPool);
            TestStruct result = JsonUtility.FromJson<TestStruct>(actualJson);

            result.AssertEquals(testStruct);
        }
        #endregion
    }
}