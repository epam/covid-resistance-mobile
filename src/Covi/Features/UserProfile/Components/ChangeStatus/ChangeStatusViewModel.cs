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

using System.Reactive;
using System.Threading.Tasks;
using Covi.Features.ApplyCode.Routes;
using ReactiveUI;

namespace Covi.Features.UserProfile.Components.ChangeStatus
{
    public class ChangeStatusViewModel : ComponentViewModelBase
    {
        private readonly IApplyCodeRoute _applyCodeRoute;

        public ReactiveCommand<Unit, Unit> ChangeStatusCommand { get; }

        public ChangeStatusViewModel(IApplyCodeRoute applyCodeRoute)
        {
            _applyCodeRoute = applyCodeRoute;
            ChangeStatusCommand = ReactiveCommand.CreateFromTask(ChangeStatusAsync);
        }

        private async Task ChangeStatusAsync()
        {
            await _applyCodeRoute.ExecuteAsync(Navigation);
        }
    }
}
