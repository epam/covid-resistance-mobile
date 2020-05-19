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
using Prism.Navigation;

namespace Covi.Features
{
    public class CompositeViewModelBase : ViewModelBase
    {
        private readonly HashSet<IComponent> _components = new HashSet<IComponent>();
        private readonly INavigationService _navigationService;

        public CompositeViewModelBase(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            foreach (var component in _components.ToArray())
            {
                component.OnAppearing();
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            foreach (var component in _components.ToArray())
            {
                component.OnDisappearing();
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            foreach (var component in _components.ToArray())
            {
                component.Initialize(parameters);
            }
        }

        public void AttachComponent(params IComponent[] components)
        {
            if (components?.Any() == true)
            {
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        _components.Add(component);
                        component.Attach(_navigationService);
                    }
                }
            }
        }
    }
}
