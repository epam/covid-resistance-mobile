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
using System.Linq;
using Covi.Resources;
using Covi.Services.Http;
using Covi.Services.Http.ExceptionsHandling;

namespace Covi.Features.ApplyCode.Services
{
    public class UserStatusChangeServiceErrorHandler : ErrorResponseHandler, IUserStatusChangeServiceErrorHandler
    {
        public UserStatusChangeServiceErrorHandler(IHttpExceptionContextRetriever httpExceptionContextRetriever)
            : base(httpExceptionContextRetriever)
        {
        }

        protected override bool TryHandleBusinessExceptionByPayload(
            ResponseError error,
            out Exception generatedException)
        {
            var errors = error?.Errors?.ToList();

            if (errors == null || !errors.Any())
            {
                generatedException = null;
                return false;
            }

            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.AcceptStatusCodeNotFound.IsError(errorItem.ErrorTarget))
                {
                    generatedException = new ApplyCodeNotFoundException(Localization.ApplyCodeNotFoundException_ErrorText);
                    return true;
                }

                if (ResponseErrorCode.AcceptStatusCodeExpired.IsError(errorItem.ErrorTarget))
                {
                    generatedException = new ApplyCodeExpiredException(Localization.ApplyCodeExpiredException_ErrorText);
                    return true;
                }
            }

            generatedException = null;
            return false;
        }
    }
}
