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
using System.Threading.Tasks;

namespace Covi.Services.ErrorHandlers
{
    /// <summary>
    /// Aggregates several <see cref="IErrorHandler"/> to sequentially process an error.
    /// </summary>
    public class CompositeErrorHandler : IErrorHandler
    {
        private readonly List<IErrorHandler> _errorHandlers = new List<IErrorHandler>();

        public CompositeErrorHandler()
        {
        }

        public CompositeErrorHandler(params IErrorHandler[] innerHandlers)
            : this()
        {
            _errorHandlers.AddRange(innerHandlers);
        }

        public void Add(IErrorHandler handler)
        {
            if (handler != null)
            {
                _errorHandlers.Add(handler);
            }
        }

        public async Task<bool> HandleAsync(Exception error)
        {
            var handled = false;
            foreach (var handler in _errorHandlers)
            {
                handled = await handler.HandleAsync(error);
                if (handled)
                {
                    break;
                }
            }

            return handled;
        }
    }
}
