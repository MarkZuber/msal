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

using System;
using Microsoft.Identity.Client.Requests.WsTrust;
using Microsoft.Identity.Client.TestInfrastructure.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.UnitTests.Requests.WsTrust
{
    [TestClass]
    public class WsTrustEndpointTests
    {
        private const string EndpointUriString = "https://foo.com/bar";

        private const string CloudAudienceUrn = "whatshouldgohere";
        private const string UserName = "me@here.com";
        private const string Password = "mySecurePassw0rd!";

        private SimpleTimeAndGuidServiceHelper _timeAndGuidServiceHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _timeAndGuidServiceHelper = new SimpleTimeAndGuidServiceHelper();
        }

        [TestMethod]
        public void TestConstructor()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);
            
            Assert.AreEqual(new Uri(EndpointUriString), wsTrustEndpoint.Uri);
            Assert.AreEqual(WsTrustVersion.WsTrust13, wsTrustEndpoint.Version);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust13()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, UserName, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePassword_WsTrust13_NullCloudAudienceUrn()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(null, UserName, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust13_NullUserName()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, null, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust13_NullPassword()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, UserName, null);
            Assert.AreEqual("not this", message);
        }

                [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust2005()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, UserName, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePassword_WsTrust2005_NullCloudAudienceUrn()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(null, UserName, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust2005_NullUserName()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, null, Password);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageUsernamePasswordValid_WsTrust2005_NullPassword()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageUsernamePassword(CloudAudienceUrn, UserName, null);
            Assert.AreEqual("not this", message);
        }

                [TestMethod]
        public void TestBuildTokenRequestMessageWiaValid_WsTrust13()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageWia(CloudAudienceUrn);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageWia_WsTrust13_NullCloudAudienceUrn()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust13);

            string message = wsTrustEndpoint.BuildTokenRequestMessageWia(null);
            Assert.AreEqual("not this", message);
        }

                [TestMethod]
        public void TestBuildTokenRequestMessageWiaValid_WsTrust2005()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageWia(CloudAudienceUrn);
            Assert.AreEqual("not this", message);
        }

        [TestMethod]
        public void TestBuildTokenRequestMessageWia_WsTrust2005_NullCloudAudienceUrn()
        {
            var wsTrustEndpoint = CreateWsTrustEndpoint(WsTrustVersion.WsTrust2005);

            string message = wsTrustEndpoint.BuildTokenRequestMessageWia(null);
            Assert.AreEqual("not this", message);
        }

        private WsTrustEndpoint CreateWsTrustEndpoint(WsTrustVersion version)
        {
            var wsTrustEndpoint = new WsTrustEndpoint(
                version,
                new Uri(EndpointUriString),
                _timeAndGuidServiceHelper.GuidService,
                _timeAndGuidServiceHelper.TimeService);
            return wsTrustEndpoint;
        }
    }
}