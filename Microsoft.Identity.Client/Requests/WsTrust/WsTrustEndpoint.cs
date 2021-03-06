﻿// ------------------------------------------------------------------------------
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
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.Requests.WsTrust
{
    internal enum WsTrustVersion
    {
        WsTrust13,
        WsTrust2005
    }

    internal class WsTrustEndpoint
    {
        private const string EnvelopeNamespaceValue = "http://www.w3.org/2003/05/soap-envelope";

        private const string WsuNamespaceValue =
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

        private readonly IGuidService _guidService;
        private readonly ITimeService _timeService;

        public WsTrustEndpoint(
            WsTrustVersion version,
            Uri uri,
            IGuidService guidService,
            ITimeService timeService)
        {
            Version = version;
            Uri = uri;
            _guidService = guidService;
            _timeService = timeService;
        }

        public WsTrustVersion Version { get; }
        public Uri Uri { get; }

        public string BuildTokenRequestMessageWia(string cloudAudienceUrn)
        {
            return BuildTokenRequestMessage(
                AuthorizationType.WindowsIntegratedAuth,
                cloudAudienceUrn,
                string.Empty,
                string.Empty);
        }

        public string BuildTokenRequestMessageUsernamePassword(
            string cloudAudienceUrn,
            string userName,
            string password)
        {
            return BuildTokenRequestMessage(AuthorizationType.UsernamePassword, cloudAudienceUrn, userName, password);
        }

        private string BuildTokenRequestMessage(
            AuthorizationType authType,
            string cloudAudienceUri,
            string username,
            string password)
        {
            string soapAction;
            string trustNamespace;
            string keyType;
            string requestType;

            if (Version == WsTrustVersion.WsTrust2005)
            {
                soapAction = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue";
                trustNamespace = "http://schemas.xmlsoap.org/ws/2005/02/trust";
                keyType = "http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey";
                requestType = "http://schemas.xmlsoap.org/ws/2005/02/trust/Issue";
            }
            else
            {
                soapAction = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue";
                trustNamespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512";
                keyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer";
                requestType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue";
            }

            const string WsaNamespaceValue = "http://www.w3.org/2005/08/addressing";

            using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (var writer = XmlWriter.Create(
                    sw,
                    new XmlWriterSettings()
                    {
                        Async = false,
                        Encoding = Encoding.UTF8,
                        CloseOutput = false
                    }))
                {
                    writer.WriteStartElement("s", "Envelope", EnvelopeNamespaceValue);
                    writer.WriteAttributeString("wsa", "http://www.w3.org/2000/xmlns/", WsaNamespaceValue);
                    writer.WriteAttributeString("wsu", "http://www.w3.org/2000/xmlns/", WsuNamespaceValue);

                    writer.WriteStartElement("Header", EnvelopeNamespaceValue);
                    writer.WriteStartElement("Action", WsaNamespaceValue);
                    writer.WriteAttributeString("mustUnderstand", EnvelopeNamespaceValue, "1");
                    writer.WriteString(soapAction);
                    writer.WriteEndElement(); // Action

                    writer.WriteStartElement("messageID", WsaNamespaceValue);
                    writer.WriteString($"urn:uuid:{_guidService.NewGuid():D}");
                    writer.WriteEndElement(); // messageID

                    writer.WriteStartElement("ReplyTo", WsaNamespaceValue);
                    writer.WriteStartElement("Address", WsaNamespaceValue);
                    writer.WriteString("http://www.w3.org/2005/08/addressing/anonymous");
                    writer.WriteEndElement(); // Address
                    writer.WriteEndElement(); // ReplyTo

                    writer.WriteStartElement("To", WsaNamespaceValue);
                    writer.WriteAttributeString("mustUnderstand", EnvelopeNamespaceValue, "1");
                    writer.WriteString(Uri.ToString());
                    writer.WriteEndElement(); // To

                    if (authType == AuthorizationType.UsernamePassword)
                    {
                        AppendSecurityHeader(writer, username, password);
                    }

                    writer.WriteEndElement(); // Header

                    writer.WriteStartElement("Body", EnvelopeNamespaceValue);
                    writer.WriteStartElement("wst", "RequestSecurityToken", trustNamespace);
                    writer.WriteStartElement("wsp", "AppliesTo", "http://schemas.xmlsoap.org/ws/2004/09/policy");
                    writer.WriteStartElement("EndpointReference", WsaNamespaceValue);
                    writer.WriteStartElement("Address", WsaNamespaceValue);
                    writer.WriteString(cloudAudienceUri);
                    writer.WriteEndElement(); // Address
                    writer.WriteEndElement(); // EndpointReference
                    writer.WriteEndElement(); // AppliesTo

                    writer.WriteStartElement("KeyType", trustNamespace);
                    writer.WriteString(keyType);
                    writer.WriteEndElement(); // KeyType

                    writer.WriteStartElement("RequestType", trustNamespace);
                    writer.WriteString(requestType);
                    writer.WriteEndElement(); // RequestType

                    writer.WriteEndElement(); // RequestSecurityToken

                    writer.WriteEndElement(); // Body
                    writer.WriteEndElement(); // Envelope
                }

                return sw.ToString();
            }
        }

        private void AppendSecurityHeader(
            XmlWriter writer,
            string username,
            string password)
        {
            const string WsseNamespaceValue =
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

            var createdTime = _timeService.GetUtcNow();
            string createdTimeString = BuildTimeString(createdTime);

            // Expiry is 10 minutes after creation
            var expiryTime = createdTime.AddMinutes(10);
            string expiryTimeString = BuildTimeString(expiryTime);

            string versionString = Version == WsTrustVersion.WsTrust2005 ? "UnPwSecTok2005-" : "UnPwSecTok13-";
            string trustId = $"{versionString}{_guidService.NewGuid():D}";

            writer.WriteStartElement("wsse", "Security", WsseNamespaceValue);
            writer.WriteAttributeString("mustUnderstand", EnvelopeNamespaceValue, "1");

            writer.WriteStartElement("Timestamp", WsuNamespaceValue);
            writer.WriteAttributeString("Id", WsuNamespaceValue, "MSATimeStamp");

            writer.WriteElementString("Created", WsuNamespaceValue, createdTimeString);
            writer.WriteElementString("Expires", WsuNamespaceValue, expiryTimeString);

            writer.WriteEndElement(); // Timestamp

            writer.WriteStartElement("UsernameToken", WsseNamespaceValue);
            writer.WriteAttributeString("Id", WsuNamespaceValue, trustId);

            writer.WriteElementString("Username", WsseNamespaceValue, username);
            writer.WriteElementString("Password", WsseNamespaceValue, password);

            writer.WriteEndElement(); // UsernameToken

            writer.WriteEndElement(); // Security
        }

        private static string BuildTimeString(DateTime utcTime)
        {
            return utcTime.ToString("yyyy-MM-ddTHH:mm:ss.068Z", CultureInfo.InvariantCulture);
        }
    }
}