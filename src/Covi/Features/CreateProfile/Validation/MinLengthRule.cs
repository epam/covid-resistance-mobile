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

using FluentValidation;

namespace Covi.Features.CreateProfile.Validation
{
    /// <summary>
    /// Rule that validates the minimum length.
    /// </summary>
    public class MinLengthRule : AbstractValidator<string>
    {
        private const int MinimumLengthDefaultValue = 8;

        public MinLengthRule()
        {
            RuleFor(customer => customer).MinimumLength(MinimumLength);
        }

        public int MinimumLength { get; set; } = MinimumLengthDefaultValue;
    }
}
