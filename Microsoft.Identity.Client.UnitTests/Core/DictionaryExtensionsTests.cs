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

using System.Collections.Generic;
using Microsoft.Identity.Client.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Core
{
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void TestCopyDictionarySuccess()
        {
            var dict = new Dictionary<string, string>
            {
                ["1"] = "12345",
                ["2"] = "23456",
                ["abcde"] = "fghij"
            };

            IDictionary<string, string> dict2 = dict.CopyDictionary();
            Assert.AreNotSame(dict2, dict);
            Assert.AreEqual(dict.Count, dict2.Count);

            foreach (KeyValuePair<string, string> kvp in dict)
            {
                Assert.IsTrue(dict2.ContainsKey(kvp.Key));
                Assert.AreEqual(dict[kvp.Key], dict2[kvp.Key]);
            }
        }

        [TestMethod]
        public void TestCopyEmptyDictionary()
        {
            var dict = new Dictionary<string, string>();
            IDictionary<string, string> dict2 = dict.CopyDictionary();

            Assert.AreNotSame(dict2, dict);
            Assert.AreEqual(0, dict2.Count);
        }
    }
}