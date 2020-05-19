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

using System.Threading;
using System.Threading.Tasks;
using Covi.Client.Services.Platform.Models;

namespace Covi.Services.ApplicationMetadata
{
    /// <summary>
    /// Provides operations to get application metadata.
    /// </summary>
    public interface IMetadataService
    {
        /// <summary>
        /// Retrieves application metadata from the server.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated <see cref="Metadata"/>.</returns>
        Task<Metadata> UpdateMetadataAsync(CancellationToken cancellationToken = default);
    }
}
