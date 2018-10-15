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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.Requests.WsTrust
{
    internal class WsTrustMexDocument
    {
        public const string Trust2005Spec = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue";
        public const string Trust13Spec = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue";
        private const string WsTrustSoapTransport = "http://schemas.xmlsoap.org/soap/http";
        private readonly Dictionary<string, MexPolicy> _bindings = new Dictionary<string, MexPolicy>();
        private readonly IGuidService _guidService;
        private readonly Dictionary<string, MexPolicy> _policies = new Dictionary<string, MexPolicy>();
        private readonly ITimeService _timeService;

        internal WsTrustMexDocument(string response)
        {
            var mexDocument = XDocument.Parse(response, LoadOptions.None);
            ReadPolicies(mexDocument);
            ReadPolicyBindings(mexDocument);
            SetPolicyEndpointAddresses(mexDocument);

            // TODO: inject
            _guidService = new GuidService();
            _timeService = new TimeService();
        }

        public static WsTrustMexDocument Create(string response)
        {
            return new WsTrustMexDocument(response);
        }

        public WsTrustEndpoint GetWsTrustUsernamePasswordEndpoint()
        {
            return GetWsTrustEndpoint(AuthorizationType.UsernamePassword);
        }

        public WsTrustEndpoint GetWsTrustWindowsTransportEndpoint()
        {
            return GetWsTrustEndpoint(AuthorizationType.WindowsIntegratedAuth);
        }

        private WsTrustEndpoint GetWsTrustEndpoint(AuthorizationType userAuthType)
        {
            var policy = SelectPolicy(userAuthType);
            if (policy == null)
            {
                return null;
            }

            return new WsTrustEndpoint(policy.Version, policy.Url, _guidService, _timeService);
        }

        private MexPolicy SelectPolicy(AuthorizationType userAuthType)
        {
            //try ws-trust 1.3 first
            return _policies.Values.Where(
                       p => p.Url != null && p.AuthType == userAuthType &&
                            p.Version == WsTrustVersion.WsTrust13).FirstOrDefault() ?? _policies
                                                                                       .Values.Where(
                                                                                           p =>
                                                                                               p.Url !=
                                                                                               null &&
                                                                                               p.AuthType ==
                                                                                               userAuthType)
                                                                                       .FirstOrDefault();
        }

        private void ReadPolicies(XContainer mexDocument)
        {
            IEnumerable<XElement> policyElements = mexDocument.Elements().First().Elements(XmlNamespace.Wsp + "Policy");
            foreach (var policy in policyElements)
            {
                var exactlyOnePolicy = policy.Elements(XmlNamespace.Wsp + "ExactlyOne").FirstOrDefault();
                if (exactlyOnePolicy == null)
                {
                    continue;
                }

                IEnumerable<XElement> all = exactlyOnePolicy.Descendants(XmlNamespace.Wsp + "All");
                foreach (var element in all)
                {
                    var securityPolicy = XmlNamespace.Sp;
                    var auth = element.Elements(XmlNamespace.Http + "NegotiateAuthentication").FirstOrDefault();
                    if (auth != null)
                    {
                        AddPolicy(policy, AuthorizationType.WindowsIntegratedAuth);
                    }

                    auth = element.Elements(securityPolicy + "SignedEncryptedSupportingTokens").FirstOrDefault();
                    if (auth == null &&
                        (auth = element.Elements(XmlNamespace.Sp2005 + "SignedSupportingTokens").FirstOrDefault()) ==
                        null)
                    {
                        continue;
                    }

                    securityPolicy = XmlNamespace.Sp2005;
                    var wspPolicy = auth.Elements(XmlNamespace.Wsp + "Policy").FirstOrDefault();
                    if (wspPolicy == null)
                    {
                        continue;
                    }

                    var usernameToken = wspPolicy.Elements(securityPolicy + "UsernameToken").FirstOrDefault();
                    if (usernameToken == null)
                    {
                        continue;
                    }

                    var wspPolicy2 = usernameToken.Elements(XmlNamespace.Wsp + "Policy").FirstOrDefault();
                    if (wspPolicy2 == null)
                    {
                        continue;
                    }

                    var wssUsernameToken10 =
                        wspPolicy2.Elements(securityPolicy + "WssUsernameToken10").FirstOrDefault();
                    if (wssUsernameToken10 != null)
                    {
                        AddPolicy(policy, AuthorizationType.UsernamePassword);
                    }
                }
            }
        }

        private void ReadPolicyBindings(XContainer mexDocument)
        {
            IEnumerable<XElement> bindingElements =
                mexDocument.Elements().First().Elements(XmlNamespace.Wsdl + "binding");
            foreach (var binding in bindingElements)
            {
                IEnumerable<XElement> policyReferences = binding.Elements(XmlNamespace.Wsp + "PolicyReference");
                foreach (var policyReference in policyReferences)
                {
                    var policyUri = policyReference.Attribute("URI");
                    if (policyUri == null || !_policies.ContainsKey(policyUri.Value))
                    {
                        continue;
                    }

                    var bindingName = binding.Attribute("name");
                    if (bindingName == null)
                    {
                        continue;
                    }

                    var bindingOperation = binding.Elements(XmlNamespace.Wsdl + "operation").FirstOrDefault();
                    if (bindingOperation == null)
                    {
                        continue;
                    }

                    var soapOperation = bindingOperation.Elements(XmlNamespace.Soap12 + "operation").FirstOrDefault();
                    if (soapOperation == null)
                    {
                        continue;
                    }

                    var soapAction = soapOperation.Attribute("soapAction");
                    if (soapAction == null ||
                        string.Compare(
                            XmlNamespace.Issue.ToString(),
                            soapAction.Value,
                            StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(
                            XmlNamespace.Issue2005.ToString(),
                            soapAction.Value,
                            StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }

                    bool isWsTrust2005 = string.Compare(
                                             XmlNamespace.Issue2005.ToString(),
                                             soapAction.Value,
                                             StringComparison.OrdinalIgnoreCase) == 0;
                    _policies[policyUri.Value].Version =
                        isWsTrust2005 ? WsTrustVersion.WsTrust2005 : WsTrustVersion.WsTrust13;

                    var soapBinding = binding.Elements(XmlNamespace.Soap12 + "binding").FirstOrDefault();
                    if (soapBinding == null)
                    {
                        continue;
                    }

                    var soapBindingTransport = soapBinding.Attribute("transport");
                    if (soapBindingTransport != null && string.Compare(
                            WsTrustSoapTransport,
                            soapBindingTransport.Value,
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _bindings.Add(bindingName.Value, _policies[policyUri.Value]);
                    }
                }
            }
        }

        private void SetPolicyEndpointAddresses(XContainer mexDocument)
        {
            var serviceElement = mexDocument.Elements().First().Elements(XmlNamespace.Wsdl + "service").First();
            IEnumerable<XElement> portElements = serviceElement.Elements(XmlNamespace.Wsdl + "port");
            foreach (var port in portElements)
            {
                var portBinding = port.Attribute("binding");
                if (portBinding == null)
                {
                    continue;
                }

                string portBindingName = portBinding.Value;
                string[] portBindingNameSegments = portBindingName.Split(
                    new[]
                    {
                        ':'
                    },
                    2);
                if (portBindingNameSegments.Length < 2 || !_bindings.ContainsKey(portBindingNameSegments[1]))
                {
                    continue;
                }

                var endpointReference = port.Elements(XmlNamespace.Wsa10 + "EndpointReference").FirstOrDefault();
                if (endpointReference == null)
                {
                    continue;
                }

                var endpointAddress = endpointReference.Elements(XmlNamespace.Wsa10 + "Address").FirstOrDefault();
                if (endpointAddress != null && Uri.IsWellFormedUriString(endpointAddress.Value, UriKind.Absolute))
                {
                    _bindings[portBindingNameSegments[1]].Url = new Uri(endpointAddress.Value);
                }
            }
        }

        private void AddPolicy(
            XElement policy,
            AuthorizationType policyAuthType)
        {
            var binding = policy.Descendants(XmlNamespace.Sp + "TransportBinding").FirstOrDefault() ??
                          policy.Descendants(XmlNamespace.Sp2005 + "TransportBinding").FirstOrDefault();

            if (binding != null)
            {
                var id = policy.Attribute(XmlNamespace.Wsu + "Id");
                if (id != null)
                {
                    _policies.Add(
                        "#" + id.Value,
                        new MexPolicy
                        {
                            Id = id.Value,
                            AuthType = policyAuthType
                        });
                }
            }
        }

        private class MexPolicy
        {
            public WsTrustVersion Version { get; set; }
            public string Id { get; set; }
            public AuthorizationType AuthType { get; set; }
            public Uri Url { get; set; }
        }
    }
}