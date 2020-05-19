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

using Microsoft.Extensions.Logging;
using Prism.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Covi.Logs
{
    /// <summary>
    /// Prism logger facade that sends logs into Microsoft.Extensions.Logging engine.
    /// </summary>
    public class MicrosoftExtensionsLoggingFacade : ILoggerFacade
    {
        private ILogger _logger;

        public MicrosoftExtensionsLoggingFacade(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _logger.LogDebug(message);
                    break;
                case Category.Info:
                    _logger.LogInformation(message);
                    break;
                case Category.Warn:
                    _logger.LogWarning(message);
                    break;
                case Category.Exception:
                    _logger.LogError(message);
                    break;
            }
        }
    }
}
