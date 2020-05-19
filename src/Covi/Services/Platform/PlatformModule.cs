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

using System.Net.Http;
using Covi.Services.Http.AuthorizationHandling;
using Covi.Services.Http.Connectivity;
using Covi.Services.Http.ExceptionsHandling;
using Covi.Services.Http.RequestIdHandling;
using Covi.Services.Http.SessionContainer;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Modularity;

namespace Covi.Services.Platform
{
    public class PlatformModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConnectivityService, ConnectivityService>();
            containerRegistry.RegisterSingleton<IHttpExceptionContextRetriever, HttpExceptionContextRetriever>();
            containerRegistry.RegisterSingleton<IErrorResponseHandler, ErrorResponseHandler>();
            containerRegistry.RegisterSingleton<IRequestIdGenerator, RequestIdGenerator>();
            containerRegistry.RegisterSingleton<ISessionContainer, SessionContainer>();
            containerRegistry.RegisterSingleton<ITokenRefreshStrategy, TokenRefreshStrategy>();
            containerRegistry.RegisterSingletonFromDelegate<ITokenRefreshInvoker>(provider =>
            {
                var options = provider.GetRequiredService<Client.Services.PlatformClientOptions>();
                var handler = provider.GetRequiredService<Services.Http.IHttpClientHandlerProvider>()
                    .GetPlatformHandler();
                var handlers = new DelegatingHandler[]
                {
                    new RequestIdDelegatingHandler(provider.GetRequiredService<IRequestIdGenerator>()),
                };

                var client = Client.Services.ClientBuilder.Create(options, handler, handlers);
                return new TokenRefreshInvoker(client);
            });
            containerRegistry.RegisterSingletonFromDelegate<Covi.Client.Services.IPlatformClient>(
                (provider) =>
                {
                    var handler = provider.GetRequiredService<Services.Http.IHttpClientHandlerProvider>()
                        .GetPlatformHandler();
                    var options = provider.GetRequiredService<Client.Services.PlatformClientOptions>();
                    var handlers = new DelegatingHandler[]
                    {
                        new AuthDelegatingHandler(provider.GetRequiredService<ISessionContainer>()),
                        new TokenRefreshDelegatingHandler(
                            provider.GetRequiredService<ISessionContainer>(),
                            provider.GetRequiredService<ITokenRefreshStrategy>()),
                        new RequestIdDelegatingHandler(provider.GetRequiredService<IRequestIdGenerator>()),
                    };

                    var client = Client.Services.ClientBuilder.Create(options, handler, handlers);
                    return client;
                });
        }
    }
}
