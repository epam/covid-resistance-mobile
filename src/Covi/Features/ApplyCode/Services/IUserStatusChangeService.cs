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

namespace Covi.Features.ApplyCode.Services
{
    /// <summary>
    /// Provides operations to change user status.
    /// </summary>
    public interface IUserStatusChangeService
    {
        /// <summary>
        /// Changes current user status.
        /// </summary>
        /// <param name="code">Status code to apply.</param>
        /// <returns>If successful - returns <see cref="UserProfileResponse"/>.</returns>
        Task<UserProfileResponse> ApplyStatusChangeCode(string code);
    }
}
