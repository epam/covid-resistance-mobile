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

namespace Covi.Features.MedicalChangeStatus.Services
{
    public class MedicalChangeStatusServiceErrorHandler : ErrorResponseHandler, IMedicalChangeStatusServiceErrorHandler
    {
        public MedicalChangeStatusServiceErrorHandler(IHttpExceptionContextRetriever httpExceptionContextRetriever)
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

            if (TryHandleValidation(errors, out generatedException))
            {
                return true;
            }

            generatedException = null;
            return false;
        }

        private bool TryHandleValidation(
            List<ResponseInnerError> errors,
            out Exception generatedException)
        {
            var validationErrors = new List<string>();

            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.StatusId.IsError(errorItem.ErrorTarget))
                {
                    validationErrors.Add(Localization.ChangeStatusCodeGenerationException_StatusId_ErrorText);
                }
                else if (ResponseErrorCode.StatusChangedOn.IsError(errorItem.ErrorTarget))
                {
                    validationErrors.Add(Localization.ChangeStatusCodeGenerationException_StatusChangedOn_ErrorText);
                }
            }

            if (validationErrors.Any())
            {
                var message = string.Join(";", validationErrors);
                generatedException = new ChangeStatusCodeGenerationException(message);
                return true;
            }

            generatedException = null;
            return false;
        }
    }
}
