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
using ReactiveUI;

namespace Covi.Features.MedicalChangeStatus
{
    public class MedicalOptionItemViewModel : ViewModelBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                UpdateAction?.Invoke(this);
                this.RaiseAndSetIfChanged(ref _isSelected, value);
            }
        }

        public int OptionId { get; set; }

        public string OptionText { get; set; }

        private Action<MedicalOptionItemViewModel> UpdateAction { get; set; }

        public MedicalOptionItemViewModel(int optionId, string optionText, Action<MedicalOptionItemViewModel> action)
        {
            OptionId = optionId;
            OptionText = optionText;
            UpdateAction = action;
        }
    }
}
