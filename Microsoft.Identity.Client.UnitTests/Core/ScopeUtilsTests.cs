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
using System.Linq;
using Microsoft.Identity.Client.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Core
{
    [TestClass]
    public class ScopeUtilsTests
    {
        [TestMethod]
        public void TestJoinSimple()
        {
            string joined = ScopeUtils.Join(
                new List<string>
                {
                    "a",
                    "b",
                    "c"
                });
            Assert.AreEqual("a b c", joined);
        }

        [TestMethod]
        public void TestJoinComplex()
        {
            string joined = ScopeUtils.Join(
                new List<string>
                {
                    "scope1",
                    "r2/scope2",
                    "https://foo.com/.default"
                });
            Assert.AreEqual("scope1 r2/scope2 https://foo.com/.default", joined);
        }

        [TestMethod]
        public void TestJoinEmpty()
        {
            string joined = ScopeUtils.Join(new List<string>());
            Assert.AreEqual(string.Empty, joined);
        }

        [TestMethod]
        public void TestSplitSimple()
        {
            var split = ScopeUtils.Split("a b c").ToList();
            Assert.AreEqual(3, split.Count);
            Assert.AreEqual("a", split[0]);
            Assert.AreEqual("b", split[1]);
            Assert.AreEqual("c", split[2]);
        }

        [TestMethod]
        public void TestSplitComplex()
        {
            var split = ScopeUtils.Split("scope1 r2/scope https://foo.com/.default").ToList();
            Assert.AreEqual(3, split.Count);
            Assert.AreEqual("scope1", split[0]);
            Assert.AreEqual("r2/scope", split[1]);
            Assert.AreEqual("https://foo.com/.default", split[2]);
        }

        [TestMethod]
        public void TestSplitEmpty()
        {
            var split = ScopeUtils.Split(string.Empty).ToList();
            Assert.AreEqual(0, split.Count);
        }

        [TestMethod]
        public void TestSplitNull()
        {
            var split = ScopeUtils.Split(null).ToList();
            Assert.AreEqual(0, split.Count);
        }

        [TestMethod]
        public void TestSplitWhitespace()
        {
            var split = ScopeUtils.Split("    ").ToList();
            Assert.AreEqual(0, split.Count);
        }
    }
}