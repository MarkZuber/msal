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
using System.Globalization;
using System.Text;

namespace Microsoft.Identity.Client.Core
{
    public static class EncodingUtils
    {
        private const char Base64PadCharacter = '=';
        private const char Base64Character62 = '+';
        private const char Base64Character63 = '/';
        private const char Base64UrlCharacter62 = '-';
        private const char Base64UrlCharacter63 = '_';
        private const int MaxUrlEncodingSize = 2000;
        private static readonly Encoding TextEncoding = Encoding.UTF8;

        private static readonly string DoubleBase64PadCharacter = string.Format(
            CultureInfo.InvariantCulture,
            "{0}{0}",
            Base64PadCharacter);

        public static string Base64RfcEncodePadded(string input)
        {
            if (input == null)
            {
                return null;
            }

            return Encode(TextEncoding.GetBytes(input));
        }

        public static string Base64UrlDecodeUnpadded(string input)
        {
            byte[] decoded = DecodeToBytes(input);
            return Encoding.UTF8.GetString(decoded, 0, decoded.Length);
        }

        private static string Encode(byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            string s = Convert.ToBase64String(input);
            s = s.Split(Base64PadCharacter)[0]; // Remove any trailing padding
            s = s.Replace(Base64Character62, Base64UrlCharacter62); // 62nd char of encoding
            s = s.Replace(Base64Character63, Base64UrlCharacter63); // 63rd char of encoding

            return s;
        }

        private static byte[] DecodeToBytes(string arg)
        {
            string s = arg;
            s = s.Replace(Base64UrlCharacter62, Base64Character62); // 62nd char of encoding
            s = s.Replace(Base64UrlCharacter63, Base64Character63); // 63rd char of encoding

            switch (s.Length % 4)
            {
            // Pad 
            case 0:
                break; // No pad chars in this case
            case 2:
                s += DoubleBase64PadCharacter;
                break; // Two pad chars
            case 3:
                s += Base64PadCharacter;
                break; // One pad char
            default:
                throw new ArgumentException("Illegal base64url string!", nameof(arg));
            }

            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public static string UrlEncode(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return message;
            }

            if (message.Length < MaxUrlEncodingSize)
            {
                // This optimization is done for most common scenarios where the message length will not exceed MaxUrlEncodingSize
                message = Uri.EscapeDataString(message);
                message = message.Replace("%20", "+");
            }
            else
            {
                var sb = new StringBuilder();
                int loops = message.Length / MaxUrlEncodingSize;

                for (int i = 0; i <= loops; i++)
                {
                    if (i < loops)
                    {
                        sb.Append(Uri.EscapeDataString(message.Substring(MaxUrlEncodingSize * i, MaxUrlEncodingSize)));
                    }
                    else
                    {
                        sb.Append(Uri.EscapeDataString(message.Substring(MaxUrlEncodingSize * i)));
                    }
                }

                message = sb.ToString();
                message = message.Replace("%20", "+");
            }

            return message;
        }
    }
}