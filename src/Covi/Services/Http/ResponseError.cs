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
using Newtonsoft.Json;

namespace Covi.Services.Http
{
    [Serializable]
    public class ResponseError
    {
        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public IEnumerable<ResponseInnerError> Errors { get; set; }
    }

    [Serializable]
    public class ResponseInnerError
    {
        [JsonProperty(PropertyName = "errorTarget")]
        public string ErrorTarget { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
