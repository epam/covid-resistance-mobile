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
using System.Reactive.Disposables;
using Covi.Features.Account.Models;
using Covi.Features.Account.Services;
using Prism.Navigation;
using System.Linq;
using Covi.Features.MedicalChangeStatus;
using Covi.Features.MedicalLogin.Components.LogIn;

namespace Covi.Features.Medical
{
    public class MedicalViewModel : CompositeViewModelBase
    {
        private readonly IAccountContainer _accountContainer;

        public bool ShowLogin { get; private set; }
        public bool ShowChangeStatus { get; private set; }

        public MedicalViewModel(INavigationService navigationService, IAccountContainer accountContainer, MedicalChangeStatus.MedicalChangeStatusViewModel medicalChangeStatusViewModel, MedicalLoginViewModel medicalLoginViewModel)
            : base(navigationService)
        {
            _accountContainer = accountContainer;
            MedicalChangeStatusViewModel = medicalChangeStatusViewModel;
            MedicalLoginViewModel = medicalLoginViewModel;
            AttachComponent(MedicalChangeStatusViewModel, MedicalLoginViewModel);
        }

        public MedicalLoginViewModel MedicalLoginViewModel { get; }

        public MedicalChangeStatusViewModel MedicalChangeStatusViewModel { get; }

        public override void OnActivated(CompositeDisposable lifecycleDisposable)
        {
            base.OnActivated(lifecycleDisposable);
            _accountContainer.Changes.Subscribe(HandleAccountChanged).DisposeWith(lifecycleDisposable);
        }

        private void HandleAccountChanged(AccountInformation obj)
        {
            if (obj!=null && obj.Roles.Contains(Account.Services.Roles.Medical))
            {
                ShowLogin = false;
                ShowChangeStatus = true;
            }
            else
            {
                ShowLogin = true;
                ShowChangeStatus = false;
            }
        }
    }
}
