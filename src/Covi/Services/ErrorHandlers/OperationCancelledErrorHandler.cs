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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Covi.Services.ErrorHandlers
{
    /// <summary>
    /// Error handler that ignores operation cancelled exceptions as they are not processed anywhere else.
    /// </summary>
    public class OperationCancelledErrorHandler : IErrorHandler
    {
        public Task<bool> HandleAsync(Exception error)
        {
            if (error is OperationCanceledException)
            {
                // Operation cancelled - do nothing
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
