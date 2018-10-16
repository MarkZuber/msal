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

using System.Net;
using Microsoft.Identity.Client.Requests.WsTrust;
using Microsoft.Identity.Client.TestInfrastructure.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Requests.WsTrust
{
    [TestClass]
    public class WsTrustMexDocumentTests
    {
        private SimpleTimeAndGuidServiceHelper _timeAndGuidServiceHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _timeAndGuidServiceHelper = new SimpleTimeAndGuidServiceHelper();
        }

        [TestMethod]
        public void TestCreate_OK_EmptyResponse()
        {
            var doc = CreateMexDocument(HttpStatusCode.OK, string.Empty);
            Assert.Fail();
        }

        [TestMethod]
        public void TestCreate_Error_EmptyResponse()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestCreate_Error_ErrorResponse()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestCreate_OK_ValidResponseWithEndpoints_WsTrust13()
        {
            var doc = CreateMexDocument(HttpStatusCode.OK, string.Empty);
            var upEndpoint = doc.GetWsTrustUsernamePasswordEndpoint();
            var wtEndpoint = doc.GetWsTrustWindowsTransportEndpoint();

            Assert.IsNotNull(upEndpoint);
            Assert.IsNotNull(wtEndpoint);

            Assert.AreEqual(null, upEndpoint.Uri);
            Assert.AreEqual(WsTrustVersion.WsTrust13, upEndpoint.Version);

            Assert.AreEqual(null, wtEndpoint.Uri);
            Assert.AreEqual(WsTrustVersion.WsTrust13, wtEndpoint.Version);
        }

        [TestMethod]
        public void TestCreate_OK_ValidResponseWithEndpoints_WsTrust2005()
        {
            var doc = CreateMexDocument(HttpStatusCode.OK, string.Empty);
            var upEndpoint = doc.GetWsTrustUsernamePasswordEndpoint();
            var wtEndpoint = doc.GetWsTrustWindowsTransportEndpoint();

            Assert.IsNotNull(upEndpoint);
            Assert.IsNotNull(wtEndpoint);

            Assert.AreEqual(null, upEndpoint.Uri);
            Assert.AreEqual(WsTrustVersion.WsTrust2005, upEndpoint.Version);

            Assert.AreEqual(null, wtEndpoint.Uri);
            Assert.AreEqual(WsTrustVersion.WsTrust2005, wtEndpoint.Version);
        }

        private WsTrustMexDocument CreateMexDocument(
            HttpStatusCode httpStatusCode,
            string response)
        {
            return WsTrustMexDocument.Create(
                httpStatusCode,
                response,
                _timeAndGuidServiceHelper.TimeService,
                _timeAndGuidServiceHelper.GuidService);
        }
    }
}