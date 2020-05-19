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
using System.Net;
using System.Threading;
using Covi.Services.Http.ApiExceptions;
using Microsoft.Rest;

namespace Covi.Services.Http.ExceptionsHandling
{
    public class ErrorResponseHandler : IErrorResponseHandler
    {
        private readonly IHttpExceptionContextRetriever _httpExceptionContextRetriever;

        public ErrorResponseHandler(IHttpExceptionContextRetriever httpExceptionContextRetriever)
        {
            _httpExceptionContextRetriever = httpExceptionContextRetriever;
        }

        public bool TryHandle(
            Exception operationException,
            out Exception generatedException,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            generatedException = null;
            ResponseError payload = null;
            switch (operationException)
            {
                case OperationCanceledException operationCanceledException:
                    generatedException = !cancellationToken.IsCancellationRequested
                        ? (Exception) new ConnectionTimeoutException()
                        : operationCanceledException;

                    return true;
                case ConnectivityException connectivityException:
                    generatedException = connectivityException;
                    return true;
                case HttpOperationException httpException:
                {
                    var context = _httpExceptionContextRetriever.RetrieveContext(httpException);
                    if (TryHandleHttpExceptionByStatusCode(context, out generatedException))
                    {
                        return true;
                    }

                    payload = context.ResponseError;
                    if (payload != null)
                    {
                        if (TryHandleBusinessExceptionByPayload(payload, out generatedException))
                        {
                            return true;
                        }
                    }

                    break;
                }
            }

            generatedException = new UnknownException(operationException, payload);
            return true;
        }

        private static bool TryHandleHttpExceptionByStatusCode(HttpExceptionContext context, out Exception generatedException)
        {
            generatedException = null;
            var statusCode = context?.ResponseCode;
            if (statusCode == null)
            {
                generatedException = null;
                return false;
            }

            generatedException = statusCode switch
            {
                // HttpStatusCode.UpgradeRequired => new UpdateResponseException(context.ResponseError),
                // HttpStatusCode.ServiceUnavailable => new MaintenanceResponseException(context.ResponseError),
                // HttpStatusCode.NoContent => new NoContentResponseException(),
                // HttpStatusCode.NotModified => new NotModifiedResponseException(),
                HttpStatusCode.Unauthorized => new UnauthorizedResponseException(context.ResponseError),
                _ => generatedException
            };

            return generatedException != null;
        }

        protected virtual bool TryHandleBusinessExceptionByPayload(ResponseError error, out Exception generatedException)
        {
            generatedException = null;
            return false;
        }
    }
}
