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

using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Covi.Client.Services
{
    public class PlatformClient : IPlatformClient
    {
        private readonly HttpClient _httpClient;
        private Platform.IPlatformEndpoints _endpointsClient;

        public PlatformClient(PlatformClientOptions options, HttpMessageHandler httpClientHandler, params DelegatingHandler[] handlers)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (httpClientHandler == null)
            {
                throw new ArgumentNullException(nameof(httpClientHandler));
            }

            _httpClient = CreateClient(httpClientHandler, handlers);
            if (options.Timeout.HasValue)
            {
                _httpClient.Timeout = options.Timeout.Value;
            }
        }

        public Platform.IPlatformEndpoints Endpoints
        {
            get
            {
                if (_endpointsClient == null)
                {
                    var service = new Platform.PlatformEndpoints(_httpClient, true);
                    service.BaseUri = new Uri(Configuration.Constants.PlatformConstants.EndpointUrl);
                    _endpointsClient = service;
                }

                return _endpointsClient;
            }
        }

        public void SetTracingMode(bool enabled)
        {
            Microsoft.Rest.ServiceClientTracing.IsEnabled = enabled;
        }

        private HttpClient CreateClient(HttpMessageHandler clientHandler, DelegatingHandler[] handlers)
        {
            HttpMessageHandler handler = ConstructHandlersChain(clientHandler, handlers);

            var result = new HttpClient(handler, true);

            result.DefaultRequestHeaders
                .AcceptEncoding
                .Add(new StringWithQualityHeaderValue("gzip"));

            // Prevent caching of sensitive data
            var cacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };

            result.DefaultRequestHeaders.CacheControl = cacheControl;

            result.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return result;
        }

        private static HttpMessageHandler ConstructHandlersChain(HttpMessageHandler clientHandler, DelegatingHandler[] handlers)
        {
            HttpMessageHandler handler = clientHandler;
            if (handlers != null)
            {
                for (int handlerIndex = handlers.Length - 1; handlerIndex >= 0; handlerIndex--)
                {
                    DelegatingHandler val2 = handlers[handlerIndex];
                    while (val2.InnerHandler is DelegatingHandler)
                    {
                        val2 = (val2.InnerHandler as DelegatingHandler);
                    }

                    val2.InnerHandler = handler;
                    handler = handlers[handlerIndex];
                }
            }

            return handler;
        }
    }
}
