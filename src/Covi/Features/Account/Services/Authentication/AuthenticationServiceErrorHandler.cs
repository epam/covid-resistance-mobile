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

namespace Covi.Features.Account.Services.Authentication
{
    public class AuthenticationServiceErrorHandler : ErrorResponseHandler, IAuthenticationServiceErrorHandler
    {
        public AuthenticationServiceErrorHandler(IHttpExceptionContextRetriever httpExceptionContextRetriever)
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

            if (TryHandleTokenValidation(errors, out generatedException))
            {
                return true;
            }

            if (TryHandleCreateProfileValidation(errors, out generatedException))
            {
                return true;
            }

            if (TryHandleLoginValidation(errors, out generatedException))
            {
                return true;
            }

            generatedException = null;
            return false;
        }

        private bool TryHandleTokenValidation(
            List<ResponseInnerError> errors,
            out Exception generatedException)
        {
            var userTokenErrors = new List<string>();

            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.UserToken.IsError(errorItem.ErrorTarget))
                {
                    userTokenErrors.Add(Localization.CreateProfileValidationException_TokenIsEmpty_ErrorText);
                }
                else if (ResponseErrorCode.DuplicateUserToken.IsError(errorItem.ErrorTarget))
                {
                    userTokenErrors.Add(Localization.CreateProfileValidationException_DuplicateUserToken_ErrorText);
                }
            }

            if (userTokenErrors.Any())
            {
                generatedException = new CreateProfileTokenValidationException(userTokenErrors);
                return true;
            }

            generatedException = null;
            return false;
        }

        private bool TryHandleCreateProfileValidation(
            List<ResponseInnerError> errors,
            out Exception generatedException)
        {
            var usernameErrors = new List<string>();
            var passwordErrors = new List<string>();

            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.Username.IsError(errorItem.ErrorTarget))
                {
                    usernameErrors.Add(Localization.CreateProfileValidationException_Username_ErrorText);
                }
                else if (ResponseErrorCode.DuplicateUserName.IsError(errorItem.ErrorTarget))
                {
                    usernameErrors.Add(Localization.CreateProfileValidationException_DuplicateUserName_ErrorText);
                }
                else if (ResponseErrorCode.Password.IsError(errorItem.ErrorTarget))
                {
                    passwordErrors.Add(Localization.CreateProfileValidationException_Password_ErrorText);
                }
                else if (ResponseErrorCode.PasswordTooShort.IsError(errorItem.ErrorTarget))
                {
                    passwordErrors.Add(Localization.CreateProfileValidationException_PasswordTooShort_ErrorText);
                }
                else if (ResponseErrorCode.PasswordRequiresUpper.IsError(errorItem.ErrorTarget))
                {
                    passwordErrors.Add(Localization.CreateProfileValidationException_PasswordRequiresUpper_ErrorText);
                }
                else if (ResponseErrorCode.PasswordRequiresDigit.IsError(errorItem.ErrorTarget))
                {
                    passwordErrors.Add(Localization.CreateProfileValidationException_PasswordRequiresDigit_ErrorText);
                }
                else if (ResponseErrorCode.PasswordRequiresNonAlphanumeric.IsError(errorItem.ErrorTarget))
                {
                    passwordErrors.Add(Localization.CreateProfileValidationException_PasswordRequiresNonAlphanumeric_ErrorText);
                }
            }

            if (usernameErrors.Any() || passwordErrors.Any())
            {
                generatedException = new CreateProfileCredentialsValidationException(usernameErrors, passwordErrors);
                return true;
            }

            generatedException = null;
            return false;
        }

        private bool TryHandleLoginValidation(
            List<ResponseInnerError> errors,
            out Exception generatedException)
        {
            foreach (var errorItem in errors)
            {
                if (ResponseErrorCode.InvalidGrant.IsError(errorItem.ErrorTarget))
                {
                    generatedException = new LoginOperationException(Localization.LoginException_InvalidGrant_ErrorText);
                    return true;
                }
            }

            generatedException = null;
            return false;
        }
    }
}
