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
using Microsoft.Extensions.Logging;

namespace Covi.Logs
{
    public static class Logger
    {
        private static readonly Lazy<ILoggerFactory> Instance =
            new Lazy<ILoggerFactory>(() => new LoggerFactory());

        /// <summary>
        /// Gets the current serializer instance.
        /// </summary>
        public static ILoggerFactory Factory => Instance.Value;

        /// <summary>
        /// Creates dedicated logger for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source to be used as logger source.</param>
        /// <typeparam name="T">Source type.</typeparam>
        /// <returns><see cref="ILogger"/> for requested type <typeparamref name="T"/>.</returns>
        public static ILogger<T> Get<T>(T source)
        {
            return Factory.CreateLogger<T>();
        }
    }
}
