// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Covi.Client.Services.Platform
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Available API's.
    /// </summary>
    public partial interface IPlatformEndpoints : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// </summary>
        string XCorrelationId { get; set; }


        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <remarks>
        /// Called on initial registration. Validate the uniqueness of the user
        /// token.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<TokenAndProfileResponse>> RegisterUserWithHttpMessagesAsync(RegisterRequest body = default(RegisterRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Refreshes the access token.
        /// </summary>
        /// <remarks>
        /// Called when the access token has expired.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<TokenResponse>> RefreshTokenWithHttpMessagesAsync(RefreshTokenRequest body = default(RefreshTokenRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <remarks>
        /// Called when an existing user reinstalls the app.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<LoginResponse>> LoginWithHttpMessagesAsync(LoginRequest body = default(LoginRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        /// <remarks>
        /// Called whenever the mobile needs the user state and profile
        /// metadata.
        /// </remarks>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<UserProfileResponse>> GetUserProfileWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Registers the user as a doctor.
        /// </summary>
        /// <remarks>
        /// Called when a user wants to register as a doctor.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<UserProfileResponse>> RegisterMedicalWithHttpMessagesAsync(RegisterDoctorRequest body = default(RegisterDoctorRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Setups push notifications.
        /// </summary>
        /// <remarks>
        /// Called by the mobile to share the push notification token and
        /// mobile OS identification.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> SetupNotificationsWithHttpMessagesAsync(NotificationInfo body = default(NotificationInfo), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Initiates a status change request.
        /// </summary>
        /// <remarks>
        /// Called when a doctor initiates a status change request.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<MedicalCode>> InitStatusChangeWithHttpMessagesAsync(ChangeRequest body = default(ChangeRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Accepts the status change request.
        /// </summary>
        /// <remarks>
        /// Called when the user accepts the status change request.
        /// </remarks>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<UserProfileResponse>> AcceptStatusChangeWithHttpMessagesAsync(AcceptRequest body = default(AcceptRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Searches for recommendations.
        /// </summary>
        /// <remarks>
        /// Retrieves recommendations according to the status.
        /// </remarks>
        /// <param name='statusId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<RecommendationsList>> GetRecommendationsWithHttpMessagesAsync(int statusId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

    }
}
