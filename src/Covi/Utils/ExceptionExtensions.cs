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
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Covi
{
    public static class ExceptionExtensions
    {
        public static void Rethrow<TException>(this TException exception)
            where TException : Exception
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        public static string FormatForLog(this Exception exception, string description = null, [CallerMemberName] string memberName = "")
        {
            string message = null;
            if (string.IsNullOrEmpty(description))
            {
                message = memberName + " failed";
            }
            else
            {
                message = memberName + " : " + description;
            }

            return $"{message}, reason: '{exception.Message}'.";
        }

        public static string FormatForLog(string description = null, [CallerMemberName] string memberName = "")
        {
            string message = null;
            if (string.IsNullOrEmpty(description))
            {
                message = memberName + " failed";
            }
            else
            {
                message = memberName + " : " + description;
            }

            return $"{message}.";
        }
    }
}
