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

namespace Covi.Services.Http
{
    public class ResponseErrorCode
    {
        private ResponseErrorCode(string code)
        {
            Code = code;
        }

        public static ResponseErrorCode Username { get; } = new ResponseErrorCode("Username");

        public static ResponseErrorCode DuplicateUserName { get; } = new ResponseErrorCode("DuplicateUserName");

        public static ResponseErrorCode UserToken { get; } = new ResponseErrorCode("UserToken");

        public static ResponseErrorCode DuplicateUserToken { get; } = new ResponseErrorCode("DuplicateUserToken");

        public static ResponseErrorCode Password { get; } = new ResponseErrorCode("Password");

        public static ResponseErrorCode PasswordTooShort { get; } = new ResponseErrorCode("PasswordTooShort");

        public static ResponseErrorCode PasswordRequiresUpper { get; } = new ResponseErrorCode("PasswordRequiresUpper");

        public static ResponseErrorCode PasswordRequiresDigit { get; } = new ResponseErrorCode("PasswordRequiresDigit");

        public static ResponseErrorCode PasswordRequiresNonAlphanumeric { get; } = new ResponseErrorCode("PasswordRequiresNonAlphanumeric");

        public static ResponseErrorCode InvalidGrant { get; } = new ResponseErrorCode("InvalidGrant");

        public static ResponseErrorCode InvalidHealthSecurityId { get; } = new ResponseErrorCode("InvalidHealthSecurityId");

        public static ResponseErrorCode StatusId { get; } = new ResponseErrorCode("StatusId");

        public static ResponseErrorCode StatusChangedOn { get; } = new ResponseErrorCode("StatusChangedOn");

        public static ResponseErrorCode MedicalCodeNotGenerated { get; } = new ResponseErrorCode("MedicalCodeNotGenerated");

        public static ResponseErrorCode StatusChangeRequestRejected { get; } = new ResponseErrorCode("StatusChangeRequestRejected");

        public static ResponseErrorCode AcceptStatusCodeNotFound { get; } = new ResponseErrorCode("AcceptStatusCodeNotFound");

        public static ResponseErrorCode AcceptStatusCodeExpired { get; } = new ResponseErrorCode("AcceptStatusCodeExpired");

        public static ResponseErrorCode AcceptStatusFailed { get; } = new ResponseErrorCode("AcceptStatusFailed");

        public static ResponseErrorCode AcceptStatusRejected { get; } = new ResponseErrorCode("AcceptStatusRejected");

        public string Code { get; }

        public bool IsError(string responseCode)
        {
            return responseCode.Equals(Code, StringComparison.OrdinalIgnoreCase);
        }
    }
}
