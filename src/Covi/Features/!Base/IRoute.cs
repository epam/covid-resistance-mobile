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
using Prism.Navigation;

namespace Covi.Features
{
    /// <summary>
    /// Provides a basic navigation interface.
    /// </summary>
    public interface IRoute
    {
        /// <summary>
        /// Navigates using provided <paramref name="navigationService"/>.
        /// </summary>
        /// <param name="navigationService">Navigation services used for navigation.</param>
        /// <returns>Task to await.</returns>
        Task ExecuteAsync(INavigationService navigationService);
    }
}
