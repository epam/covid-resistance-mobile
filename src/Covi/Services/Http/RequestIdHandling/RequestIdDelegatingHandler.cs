// =========================================================================
// Copyright 2020 EPAM Systems, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// =========================================================================

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Covi.Services.Http.RequestIdHandling
{
    public class RequestIdDelegatingHandler : DelegatingHandler
    {
        private const string _requestIdHeader = "X-Correlation-Id";
        private readonly IRequestIdGenerator _requestIdGenerator;

        public RequestIdDelegatingHandler(IRequestIdGenerator requestIdGenerator)
        {
            _requestIdGenerator = requestIdGenerator;
        }

        public RequestIdDelegatingHandler(IRequestIdGenerator requestIdGenerator, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _requestIdGenerator = requestIdGenerator;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var id = _requestIdGenerator.CreateRequestId();
            request.AddHeaderSafely(_requestIdHeader, id);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
