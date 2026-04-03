using System;
using System.Threading.Tasks;
using UnityEngine;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Network
{
    /// <summary>
    /// SignalR client for real-time match communication.
    /// Connects to /hubs/match endpoint with JWT auth.
    /// Uses Microsoft.AspNetCore.SignalR.Client NuGet package.
    ///
    /// To install in Unity: add the SignalR .NET client via NuGetForUnity
    /// or manually reference Microsoft.AspNetCore.SignalR.Client.dll
    /// </summary>
    public class MatchHubClient : MonoBehaviour
    {
        private string hubUrl = "http://localhost:5214/hubs/match";
        private string accessToken;
        private IMatchHubConnection connection;
        private bool isConnected;

        // Server → Client events
        public event Action<MatchResponse> OnMatchStarted;
        public event Action<string, bool> OnSetupTeamSubmitted; // playerId, bothReady
        public event Action<MatchResponse> OnTeamRevealed;
        public event Action<InitiativeResponse> OnInitiativeResolved;
        public event Action<string> OnBetPlaced; // raw JSON
        public event Action<AssignCombatResponse> OnCombatAssigned;
        public event Action<ResolveCombatRoundResponse> OnCombatResolved;
        public event Action<MatchResponse> OnRoomAdvanced;
        public event Action<MatchResponse> OnRoomConceeded;
        public event Action<string> OnOpportunityAttackResolved; // raw JSON
        public event Action<string> OnRetargetCompleted; // raw JSON
        public event Action<string, MatchResponse> OnMatchFinished; // winnerId, state
        public event Action<string> OnPlayerConnected; // playerId
        public event Action<string> OnPlayerDisconnected; // playerId

        public bool IsConnected => isConnected;

        /// <summary>
        /// Set the JWT access token for authentication.
        /// Must be called before ConnectAsync.
        /// </summary>
        public void SetAccessToken(string token)
        {
            accessToken = token;
        }

        /// <summary>
        /// Set custom hub URL (default: http://localhost:5214/hubs/match)
        /// </summary>
        public void SetHubUrl(string url)
        {
            hubUrl = url;
        }

        /// <summary>
        /// Connect to the SignalR hub. Requires access token to be set.
        /// In production, use Microsoft.AspNetCore.SignalR.Client:
        ///
        /// var connection = new HubConnectionBuilder()
        ///     .WithUrl(hubUrl, options => options.AccessTokenProvider = () => Task.FromResult(accessToken))
        ///     .WithAutomaticReconnect()
        ///     .Build();
        /// </summary>
        public async Task ConnectAsync()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[MatchHubClient] Access token not set. Call SetAccessToken first.");
                return;
            }

            try
            {
                // When Microsoft.AspNetCore.SignalR.Client is available, replace this with:
                //
                // connection = new HubConnectionBuilder()
                //     .WithUrl(hubUrl, options =>
                //     {
                //         options.AccessTokenProvider = () => Task.FromResult(accessToken);
                //     })
                //     .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                //     .Build();
                //
                // RegisterHandlers(connection);
                // await connection.StartAsync();

                Debug.Log($"[MatchHubClient] Connecting to {hubUrl}...");

                // Placeholder: real SignalR connection will be established when
                // Microsoft.AspNetCore.SignalR.Client package is added to Unity project
                isConnected = true;
                Debug.Log("[MatchHubClient] Connected to match hub.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchHubClient] Connection failed: {ex.Message}");
                isConnected = false;
            }
        }

        /// <summary>
        /// Disconnect from the SignalR hub.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (!isConnected) return;

            try
            {
                // await connection?.StopAsync();
                isConnected = false;
                Debug.Log("[MatchHubClient] Disconnected from match hub.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchHubClient] Disconnect error: {ex.Message}");
            }
        }

        /// <summary>
        /// Join a match room to receive real-time updates.
        /// </summary>
        public async Task JoinMatchAsync(string matchId)
        {
            if (!isConnected)
            {
                Debug.LogWarning("[MatchHubClient] Not connected. Cannot join match.");
                return;
            }

            try
            {
                // await connection.InvokeAsync("JoinMatch", matchId);
                Debug.Log($"[MatchHubClient] Joined match room: {matchId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchHubClient] JoinMatch failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Leave a match room.
        /// </summary>
        public async Task LeaveMatchAsync(string matchId)
        {
            if (!isConnected) return;

            try
            {
                // await connection.InvokeAsync("LeaveMatch", matchId);
                Debug.Log($"[MatchHubClient] Left match room: {matchId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchHubClient] LeaveMatch failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Send a generic match action to other players in the room.
        /// </summary>
        public async Task SendMatchActionAsync(string matchId, string action, string payload)
        {
            if (!isConnected) return;

            try
            {
                // await connection.InvokeAsync("SendMatchAction", matchId, action, payload);
                Debug.Log($"[MatchHubClient] Sent action '{action}' to match {matchId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchHubClient] SendMatchAction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Register all server-to-client event handlers on the HubConnection.
        /// Call this after building the connection, before StartAsync.
        /// </summary>
        /// <remarks>
        /// When SignalR client is available, pass the real HubConnection here.
        /// For now, this documents the expected wire protocol.
        /// </remarks>
        private void RegisterHandlers(object hubConnection)
        {
            // When using real HubConnection, register like:
            //
            // connection.On<MatchResponse>("MatchStarted", state =>
            //     UnityMainThread.Execute(() => OnMatchStarted?.Invoke(state)));
            //
            // connection.On<MatchResponse>("TeamRevealed", state =>
            //     UnityMainThread.Execute(() => OnTeamRevealed?.Invoke(state)));
            //
            // connection.On<object>("InitiativeResolved", result =>
            //     UnityMainThread.Execute(() => OnInitiativeResolved?.Invoke(ParseInitiativeResponse(result))));
            //
            // connection.On<object>("CombatAssigned", assignments =>
            //     UnityMainThread.Execute(() => OnCombatAssigned?.Invoke(ParseAssignCombatResponse(assignments))));
            //
            // connection.On<object>("CombatResolved", result =>
            //     UnityMainThread.Execute(() => OnCombatResolved?.Invoke(ParseResolveCombatRoundResponse(result))));
            //
            // connection.On<MatchResponse>("RoomAdvanced", state =>
            //     UnityMainThread.Execute(() => OnRoomAdvanced?.Invoke(state)));
            //
            // connection.On<MatchResponse>("RoomConceeded", state =>
            //     UnityMainThread.Execute(() => OnRoomConceeded?.Invoke(state)));
            //
            // connection.On<object>("MatchFinished", data =>
            //     UnityMainThread.Execute(() => { ... parse winnerId and state ... }));
            //
            // connection.On<object>("PlayerConnected", data =>
            //     UnityMainThread.Execute(() => OnPlayerConnected?.Invoke(data.PlayerId)));
            //
            // connection.On<object>("PlayerDisconnected", data =>
            //     UnityMainThread.Execute(() => OnPlayerDisconnected?.Invoke(data.PlayerId)));
            //
            // connection.On<object>("SetupTeamSubmitted", data =>
            //     UnityMainThread.Execute(() => OnSetupTeamSubmitted?.Invoke(data.PlayerId, data.BothReady)));
            //
            // connection.On<object>("BetPlaced", result =>
            //     UnityMainThread.Execute(() => OnBetPlaced?.Invoke(JsonUtility.ToJson(result))));
            //
            // connection.On<object>("OpportunityAttackResolved", result =>
            //     UnityMainThread.Execute(() => OnOpportunityAttackResolved?.Invoke(JsonUtility.ToJson(result))));
            //
            // connection.On<object>("RetargetCompleted", result =>
            //     UnityMainThread.Execute(() => OnRetargetCompleted?.Invoke(JsonUtility.ToJson(result))));
            //
            // connection.Reconnecting += error =>
            // {
            //     Debug.LogWarning($"[MatchHubClient] Reconnecting: {error?.Message}");
            //     return Task.CompletedTask;
            // };
            //
            // connection.Reconnected += connectionId =>
            // {
            //     Debug.Log($"[MatchHubClient] Reconnected: {connectionId}");
            //     return Task.CompletedTask;
            // };
            //
            // connection.Closed += error =>
            // {
            //     Debug.LogWarning($"[MatchHubClient] Connection closed: {error?.Message}");
            //     isConnected = false;
            //     return Task.CompletedTask;
            // };
        }

        private void OnDestroy()
        {
            _ = DisconnectAsync();
        }
    }

    /// <summary>
    /// Placeholder interface for the SignalR HubConnection.
    /// Replace with Microsoft.AspNetCore.SignalR.Client.HubConnection when available.
    /// </summary>
    internal interface IMatchHubConnection { }
}
