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
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Covi.Client.Services.Platform.Models;
using Covi.Features.Recommendations.Services;
using Covi.Features.UserProfile.Services;
using Covi.Services.ErrorHandlers;
using DynamicData;
using ReactiveUI;

namespace Covi.Features.Recommendations
{
    public class RecommendationsViewModel : ComponentViewModelBase
    {
        private readonly IUserProfileContainer _userProfileContainer;
        private readonly IErrorHandler _errorHandler;
        private readonly IRecommendationsService _recommendationsService;

        private readonly ReadOnlyObservableCollection<RecommendationItemViewModel> _recommendations;

        private readonly SourceList<Recommendation> _models = new SourceList<Recommendation>();

        public ReadOnlyObservableCollection<RecommendationItemViewModel> Recommendations => _recommendations;

        public RecommendationsViewModel(
            IUserProfileContainer userProfileContainer,
            IErrorHandler errorHandler,
            IRecommendationsService recommendationsService)
        {
            _userProfileContainer = userProfileContainer;
            _errorHandler = errorHandler;
            _recommendationsService = recommendationsService;

            _models.Connect()
                   .SubscribeOn(RxApp.TaskpoolScheduler)
                   .Transform(x => new RecommendationItemViewModel(x))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out _recommendations, 1)
                   .Subscribe();
        }

        public override void OnActivated(CompositeDisposable lifecycleDisposable)
        {
            base.OnActivated(lifecycleDisposable);

            SetRecommendationsAsync().FireAndForget();
        }

        private async Task SetRecommendationsAsync()
        {
            try
            {
                var profile = await _userProfileContainer.GetAsync();
                var statusId = profile?.StatusId;
                if (statusId != null)
                {
                    IsBusy = true;

                    var recommendationsList =
                    await _recommendationsService.GetRecommendationsAsync(statusId.Value, LifecycleToken);

                    _models.Edit((innerList) =>
                                 {
                                     innerList.Clear();
                                     innerList.AddRange(recommendationsList.Data);
                                 });
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleAsync(e);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
