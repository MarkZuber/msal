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
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Http
{
    public sealed class RetryWithExponentialBackoff
    {
        private readonly int _delayMilliseconds;
        private readonly int _maxDelayMilliseconds;
        private readonly int _maxRetries;

        public RetryWithExponentialBackoff(int maxRetries = 50, int delayMilliseconds = 200, int maxDelayMilliseconds = 2000)
        {
            _maxRetries = maxRetries;
            _delayMilliseconds = delayMilliseconds;
            _maxDelayMilliseconds = maxDelayMilliseconds;
        }

        public async Task RunAsync(Func<Task> func)
        {
            var backoff = new ExponentialBackoff(_maxRetries, _delayMilliseconds, _maxDelayMilliseconds);
            retry:
            try
            {
                await func();
            }
            catch (Exception ex) when (ex is TimeoutException || ex is System.Net.Http.HttpRequestException)
            {
                await backoff.Delay();
                goto retry;
            }
        }
    }
}