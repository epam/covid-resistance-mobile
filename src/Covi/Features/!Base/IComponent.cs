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

using Prism.AppModel;
using Prism.Navigation;

namespace Covi.Features
{
    /// <summary>
    /// Represents a component on the ui that is dependent on the host <see cref="CompositeViewModelBase"/> lifecycle.
    /// </summary>
    public interface IComponent : IPageLifecycleAware, IInitialize
    {
        /// <summary>
        /// Attach component to the host.
        /// </summary>
        /// <param name="navigationService">Host navigation service.</param>
        void Attach(INavigationService navigationService);
    }
}
