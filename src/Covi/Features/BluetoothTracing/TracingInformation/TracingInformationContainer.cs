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
using Covi.Features.BluetoothTracing.TracingInformation.Internal;

namespace Covi.Features.BluetoothTracing.TracingInformation
{
    public static class TracingInformationContainer
    {
        private static Func<ITracingInformationContainer> _instanceProvider;
        private static Func<ITracingInformationContainer> _defaultInstanceProvider = () => DefaultInstance.Value;

        private static readonly Lazy<ITracingInformationContainer> DefaultInstance =
            new Lazy<ITracingInformationContainer>(() => new DefaultTracingInformationContainer());

        /// <summary>
        /// Gets the current tracing information container instance.
        /// </summary>
        public static ITracingInformationContainer Instance
        {
            get { return (_instanceProvider ?? _defaultInstanceProvider).Invoke(); }
        }

        public static void Setup(Func<ITracingInformationContainer> containerProvider)
        {
            _instanceProvider = containerProvider;
        }
    }
}
