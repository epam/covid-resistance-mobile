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
using System.Threading.Tasks;
using Covi.Resources;
using Covi.Services.Notification;
using Microsoft.Extensions.Logging;

namespace Covi.Services.ErrorHandlers
{
    /// <summary>
    /// Last chance handler that is triggered last and decides what to do with unhandled error.
    /// </summary>
    public class LastChanceErrorHandler : IErrorHandler
    {
        private readonly INotificationManager _notificationManager;
        private readonly ILogger _logger;

        public LastChanceErrorHandler(INotificationManager notificationManager, ILoggerFactory loggerFactory)
        {
            _notificationManager = notificationManager;
            _logger = loggerFactory.CreateLogger<LastChanceErrorHandler>();
        }

        public async Task<bool> HandleAsync(Exception error)
        {
            _logger.LogError(error, error.FormatForLog());

            await _notificationManager.ShowNotificationAsync(
                Localization.ErrorDialog_Title,
                Localization.ErrorDialog_Message_Generic,
                Localization.ErrorDialog_Accept);

            return true;
        }
    }
}
