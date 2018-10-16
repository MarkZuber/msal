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

using Microsoft.Identity.Client.Requests.WsTrust;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Requests.WsTrust
{
    [TestClass]
    public class SamlTokenInfoTests
    {
        [TestMethod]
        public void TestConstructorV1()
        {
            var samlTokenInfo = new SamlTokenInfo(SamlAssertionType.SamlV1, "the_assertion");
            Assert.AreEqual(SamlAssertionType.SamlV1, samlTokenInfo.AssertionType);
            Assert.AreEqual("the_assertion", samlTokenInfo.Assertion);
        }

        [TestMethod]
        public void TestConstructorV2()
        {
            var samlTokenInfo = new SamlTokenInfo(SamlAssertionType.SamlV2, "an_assertion");
            Assert.AreEqual(SamlAssertionType.SamlV2, samlTokenInfo.AssertionType);
            Assert.AreEqual("an_assertion", samlTokenInfo.Assertion);
        }
    }
}