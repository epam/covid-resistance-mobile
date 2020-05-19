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

using System.Linq;
using Xamarin.Forms;

namespace Covi.Effects
{
    public static class BorderlessEntryEffect
    {
        private const string PropertyName = "BorderlessEntryEffect";
        public const string EffectName = "BorderlessEntryEffect";

        public static readonly BindableProperty UseEntryEffectProperty =
          BindableProperty.CreateAttached(PropertyName, typeof(bool), typeof(BorderlessEntryEffectInternal), false, propertyChanged: OnUseEntryEffectChanged);

        public static bool GetUseEntryEffect(BindableObject view)
        {
            return (bool)view.GetValue(UseEntryEffectProperty);
        }

        public static void SetUseEntryEffect(BindableObject view, bool value)
        {
            view.SetValue(UseEntryEffectProperty, value);
        }

        private static void OnUseEntryEffectChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Entry;
            if (view == null)
            {
                return;
            }

            bool hasEffect = (bool)newValue;
            if (hasEffect)
            {
                view.Effects.Add(new BorderlessEntryEffectInternal());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is BorderlessEntryEffectInternal);
                if (toRemove != null)
                {
                    view.Effects.Remove(toRemove);
                }
            }
        }

        internal class BorderlessEntryEffectInternal : RoutingEffect
        {
            public BorderlessEntryEffectInternal()
                : base($"{Constants.EffectsGroupName}.{EffectName}")
            {
            }
        }
    }
}
