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

using System.Threading.Tasks;
using Covi.Client.Services.Platform.Models;

namespace Covi.Features.Account.Services.Authentication
{
    /// <summary>
    /// Provides information about currently active session.
    /// </summary>
    public interface IAuthenticationInfoService
    {
        /// <summary>
        /// Initializes the active session.
        /// </summary>
        /// <param name="metadata"><see cref="Metadata"/> information.</param>
        /// <param name="profile"><see cref="UserState"/> information.</param>
        /// <param name="token"><see cref="Token"/> information.</param>
        /// <returns>Task to await.</returns>
        Task InitUserInfoAsync(Metadata metadata, UserState profile, Token token);

        /// <summary>
        /// Checks whether there is any active session at the moment.
        /// </summary>
        /// <returns><c>True</c> if session is present.</returns>
        bool IsAuthenticated();
    }
}
