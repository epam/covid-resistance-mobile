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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Covi.Services.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static void AddHeaderSafely(this HttpRequestMessage request, string headerKey, string value)
        {
            if (request.Headers.Contains(headerKey))
            {
                request.Headers.Remove(headerKey);
            }

            request.Headers.TryAddWithoutValidation(headerKey, value);
        }

        public static void AddHeaderSafely(this HttpRequestMessage request, string headerKey, IEnumerable<string> values)
        {
            if (request.Headers.Contains(headerKey))
            {
                request.Headers.Remove(headerKey);
            }

            request.Headers.TryAddWithoutValidation(headerKey, values);
        }

        public static HttpRequestMessage GetCopyWithoutContent(this HttpRequestMessage request)
        {
            var targetRequest = new HttpRequestMessage();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            targetRequest.Method = request.Method;
            targetRequest.RequestUri = request.RequestUri;

            targetRequest.CloneHeaders(request, request.Headers);
            targetRequest.CloneHeaders(request, request.GetContentHeaders());

            if (request.Properties != null)
            {
                foreach (KeyValuePair<string, object> property in request.Properties)
                {
                    targetRequest.Properties[property.Key] = property.Value;
                }
            }

            return targetRequest;
        }

        public static void CloneHeaders(this HttpRequestMessage requestTo, HttpRequestMessage requestFrom, HttpHeaders headers)
        {
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    IEnumerable<string> value = null;
                    value = !requestFrom.Headers.TryGetValues(header.Key, out value) ? header.Value : value.Concat(header.Value);
                    requestTo.Headers.TryAddWithoutValidation(header.Key, value);
                }
            }
        }

        public static void SetContent(this HttpRequestMessage requestTo, string content)
        {
            requestTo.Content = new StringContent(content, System.Text.Encoding.UTF8);
            requestTo.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        }

        public static HttpHeaders GetContentHeaders(this HttpRequestMessage request)
        {
            return request?.Content?.Headers;
        }
    }
}
