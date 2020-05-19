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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Covi.Services.Http.ExceptionsHandling
{
    public class HttpExceptionContextRetriever : IHttpExceptionContextRetriever
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public HttpExceptionContextRetriever()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Iso8601TimeSpanConverter(),
                },
            };
        }

        public HttpExceptionContext RetrieveContext(HttpOperationException httpOperationException)
        {
            var requestUrl = httpOperationException.Request?.RequestUri?.ToString();
            var requestMethod = httpOperationException.Request?.Method?.ToString();
            var responseCode = httpOperationException.Response?.StatusCode;
            string requestId = null;

            var requestHeaders = httpOperationException.Request?.Headers;
            if (requestHeaders != null && requestHeaders.TryGetValue("X-Correlation-Id", out var requestIdValues))
            {
                requestId = requestIdValues.FirstOrDefault();
            }

            var parsedError = GetResponseError(httpOperationException.Response?.Content);

            return new HttpExceptionContext(httpOperationException, requestUrl, responseCode, parsedError);
        }

        private ResponseError GetResponseError(string content)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return SafeJsonConvert.DeserializeObject<ResponseError>(content, _jsonSerializerSettings);
                }
            }
            catch
            {
                // Ignore deserialization error.
            }

            return null;
        }
    }
}
