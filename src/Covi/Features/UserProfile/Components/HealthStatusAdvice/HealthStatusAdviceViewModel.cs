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
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Covi.Client.Services.Platform.Models;
using Covi.Features.UserProfile.Services;
using Covi.Services.ApplicationMetadata;
using ReactiveUI;

namespace Covi.Features.UserProfile.Components.HealthStatusAdvice
{
    public class HealthStatusAdviceViewModel : ComponentViewModelBase
    {
        private readonly IUserProfileContainer _userProfileContainer;
        private readonly IMetadataContainer _metadataContainer;
        private UserState _profile;
        private string _moreInfoUrl;

        public HealthStatusAdviceViewModel(IUserProfileContainer userProfileContainer, IMetadataContainer metadataContainer)
        {
            _userProfileContainer = userProfileContainer;
            _metadataContainer = metadataContainer;
            MoreInfoCommand = ReactiveUI.ReactiveCommand.CreateFromTask(ShowMoreInfoAsync);
            this.WhenActivated((d) =>
                               {
                                   _userProfileContainer.Changes.DistinctUntilChanged(new UserStateStatusEqualityComparer()).SubscribeOn(RxApp.TaskpoolScheduler).ObserveOn(RxApp.MainThreadScheduler).Subscribe(HandleProfileModelChanged).DisposeWith(d);
                               });
        }

        public ReactiveCommand<Unit, Unit> MoreInfoCommand { get; }

        public bool HasMoreInfo { get; private set; }

        public string Description { get; private set; }

        private void HandleProfileModelChanged(UserState profile)
        {
            SetProfile(profile);
        }

        private async void SetProfile(UserState profile)
        {
            if (profile != null)
            {
                _profile = profile;
                var metadata = await _metadataContainer.GetAsync();
                if (metadata != null)
                {
                    var currentStatus = metadata.Statuses.Values.FirstOrDefault(x => x.Id == _profile.StatusId);
                    if (currentStatus != null)
                    {
                        if (!string.IsNullOrEmpty(currentStatus.Description))
                        {
                            Description = currentStatus.Description;
                        }
                        else
                        {

                            Description = string.Format(
                                Resources.Localization.HealthStatus_Advice_Description_Default_TextFormat,
                                currentStatus.Name);
                        }

                        HasMoreInfo = !string.IsNullOrEmpty(currentStatus.Url);
                        _moreInfoUrl = currentStatus.Url;
                    }
                }
                else
                {
                    Description = string.Empty;
                }
            }
        }

        private async Task ShowMoreInfoAsync()
        {
            if (HasMoreInfo)
            {
                await Xamarin.Essentials.Browser.OpenAsync(_moreInfoUrl);
            }
        }

        private class UserStateStatusEqualityComparer : IEqualityComparer<UserState>
        {
            public bool Equals(UserState x, UserState y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null || y == null)
                    return false;
                return x.StatusId == y.StatusId;
            }

            public int GetHashCode(UserState obj)
            {
                return obj.StatusId.GetHashCode();
            }
        }
    }
}
