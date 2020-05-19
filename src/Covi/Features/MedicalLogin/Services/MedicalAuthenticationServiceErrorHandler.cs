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
using System.Linq;
using Covi.Features.Exceptions;
using Covi.Resources;
using Covi.Services.Http;
using Covi.Services.Http.ExceptionsHandling;

namespace Covi.Features.MedicalLogin.Services
{
    public class MedicalAuthenticationServiceErrorHandler : ErrorResponseHandler, IMedicalAuthenticationServiceErrorHandler
    {
        public MedicalAuthenticationServiceErrorHandler(IHttpExceptionContextRetriever httpExceptionContextRetriever)
            : base(httpExceptionContextRetriever)
        {
        }

        protected override bool TryHandleBusinessExceptionByPayload(
            ResponseError error,
            out Exception generatedException)
        {
            var errors = error?.Errors?.ToList();

            if (!errors.Any())
            {
                generatedException = null;
                return false;
            }

            if (MedicalAuthenticationValidation(errors, out generatedException))
            {
                return true;
            }

            generatedException = null;
            return false;
        }

        private bool MedicalAuthenticationValidation(
            List<ResponseInnerError> errors,
            out Exception generatedException)
        {
            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.InvalidHealthSecurityId.IsError(errorItem.ErrorTarget))
                {
                    generatedException = new MedicalAuthenticationException(Localization.MedicalAuthenticationException_InvalidHealthSecurityId_ErrorText);
                    return true;
                }
            }

            generatedException = null;
            return false;
        }
    }
}
