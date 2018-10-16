// ------------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation.
// All rights reserved.
// 
// This code is licensed under the MIT License.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ------------------------------------------------------------------------------

using System.Runtime.Serialization;
using Microsoft.Identity.Client.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Core
{
    [TestClass]
    public class JsonHelperTests
    {
        [TestMethod]
        public void TestSerializeToJson()
        {
            var data = new TestDataStruct
            {
                SomeValue = "hello there",
                SomeIntValue = 24789
            };

            string json = JsonHelper.SerializeToJson(data);
            Assert.AreEqual(
                "{\"some_int_value\":24789,\"some_missing_value\":null,\"some_value\":\"hello there\"}",
                json);
        }

        [TestMethod]
        public void TestDeserializeFromJson()
        {
            string json = "{\"some_int_value\":24789,\"some_value\":\"hello there\"}";
            var data = JsonHelper.DeserializeFromJson<TestDataStruct>(json);

            Assert.IsNotNull(data);
            Assert.AreEqual("hello there", data.SomeValue);
            Assert.AreEqual(24789, data.SomeIntValue);
            Assert.IsNull(data.MissingValue);
        }

        [DataContract]
        public class TestDataStruct
        {
            [DataMember(Name = "some_value")]
            public string SomeValue { get; set; }

            [DataMember(Name = "some_int_value")]
            public int SomeIntValue { get; set; }

            [DataMember(Name = "some_missing_value")]
            public string MissingValue { get; set; }
        }
    }
}