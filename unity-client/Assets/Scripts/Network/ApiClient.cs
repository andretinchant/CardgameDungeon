using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CardgameDungeon.Unity.Network
{
    /// <summary>
    /// Main HTTP client for communicating with the CardgameDungeon API server.
    /// Uses UnityWebRequest with coroutines. All methods accept a MonoBehaviour host
    /// to run coroutines and callbacks for async results.
    /// </summary>
    public class ApiClient
    {
        private readonly string _baseUrl;
        private string _accessToken;

        public ApiClient(string baseUrl = "http://localhost:5214")
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        // ──────────────────────────────────────────────
        //  AUTH ENDPOINTS  /api/auth
        // ──────────────────────────────────────────────

        /// <summary>POST /api/auth/register</summary>
        public Coroutine Register(MonoBehaviour host, RegisterRequest request, Action<AuthResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AuthResponse>("/api/auth/register", request, onSuccess, onError, includeAuth: false));
        }

        /// <summary>POST /api/auth/login</summary>
        public Coroutine Login(MonoBehaviour host, LoginRequest request, Action<AuthResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AuthResponse>("/api/auth/login", request, onSuccess, onError, includeAuth: false));
        }

        /// <summary>POST /api/auth/refresh</summary>
        public Coroutine Refresh(MonoBehaviour host, RefreshTokenRequest request, Action<AuthResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AuthResponse>("/api/auth/refresh", request, onSuccess, onError, includeAuth: false));
        }

        /// <summary>POST /api/auth/revoke</summary>
        public Coroutine Revoke(MonoBehaviour host, RevokeTokenRequest request, Action<RevokeTokenResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<RevokeTokenResponse>("/api/auth/revoke", request, onSuccess, onError, includeAuth: false));
        }

        // ──────────────────────────────────────────────
        //  DECK ENDPOINTS  /api/decks
        // ──────────────────────────────────────────────

        /// <summary>POST /api/decks</summary>
        public Coroutine CreateDeck(MonoBehaviour host, CreateDeckRequest request, Action<DeckResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<DeckResponse>("/api/decks", request, onSuccess, onError));
        }

        /// <summary>GET /api/decks/{id}</summary>
        public Coroutine GetDeck(MonoBehaviour host, Guid deckId, Action<DeckResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<DeckResponse>($"/api/decks/{deckId}", onSuccess, onError));
        }

        /// <summary>PUT /api/decks/{id}</summary>
        public Coroutine UpdateDeck(MonoBehaviour host, Guid deckId, UpdateDeckRequest request, Action<DeckResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Put<DeckResponse>($"/api/decks/{deckId}", request, onSuccess, onError));
        }

        /// <summary>POST /api/decks/{id}/validate</summary>
        public Coroutine ValidateDeck(MonoBehaviour host, Guid deckId, Action<ValidateDeckResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<ValidateDeckResponse>($"/api/decks/{deckId}/validate", null, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  MATCH ENDPOINTS  /api/matches
        // ──────────────────────────────────────────────

        /// <summary>POST /api/matches</summary>
        public Coroutine CreateMatch(MonoBehaviour host, CreateMatchRequest request, Action<MatchResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<MatchResponse>("/api/matches", request, onSuccess, onError));
        }

        /// <summary>GET /api/matches/{id}</summary>
        public Coroutine GetMatchState(MonoBehaviour host, Guid matchId, Action<MatchResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<MatchResponse>($"/api/matches/{matchId}", onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/setup-team</summary>
        public Coroutine SetupInitialTeam(MonoBehaviour host, Guid matchId, SetupInitialTeamRequest request, Action<SetupInitialTeamResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<SetupInitialTeamResponse>($"/api/matches/{matchId}/setup-team", request, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/reveal-teams</summary>
        public Coroutine RevealInitialTeams(MonoBehaviour host, Guid matchId, Action<MatchResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<MatchResponse>($"/api/matches/{matchId}/reveal-teams", null, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/resolve-initiative</summary>
        public Coroutine ResolveInitiative(MonoBehaviour host, Guid matchId, Action<InitiativeResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<InitiativeResponse>($"/api/matches/{matchId}/resolve-initiative", null, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/place-bet</summary>
        public Coroutine PlaceBet(MonoBehaviour host, Guid matchId, PlaceBetRequest request, Action<PlaceBetResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<PlaceBetResponse>($"/api/matches/{matchId}/place-bet", request, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/assign-combat</summary>
        public Coroutine AssignCombat(MonoBehaviour host, Guid matchId, AssignCombatRequest request, Action<AssignCombatResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AssignCombatResponse>($"/api/matches/{matchId}/assign-combat", request, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/resolve-combat</summary>
        public Coroutine ResolveCombatRound(MonoBehaviour host, Guid matchId, Action<ResolveCombatRoundResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<ResolveCombatRoundResponse>($"/api/matches/{matchId}/resolve-combat", null, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/retarget</summary>
        public Coroutine Retarget(MonoBehaviour host, Guid matchId, RetargetRequest request, Action<RetargetResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<RetargetResponse>($"/api/matches/{matchId}/retarget", request, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/opportunity-attack</summary>
        public Coroutine OpportunityAttack(MonoBehaviour host, Guid matchId, OpportunityAttackRequest request, Action<OpportunityAttackResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<OpportunityAttackResponse>($"/api/matches/{matchId}/opportunity-attack", request, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/advance-room</summary>
        public Coroutine AdvanceRoom(MonoBehaviour host, Guid matchId, Action<MatchResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<MatchResponse>($"/api/matches/{matchId}/advance-room", null, onSuccess, onError));
        }

        /// <summary>POST /api/matches/{id}/concede-room</summary>
        public Coroutine ConcedeRoom(MonoBehaviour host, Guid matchId, ConcedeRoomRequest request, Action<MatchResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<MatchResponse>($"/api/matches/{matchId}/concede-room", request, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  COLLECTION ENDPOINTS  /api/collection
        // ──────────────────────────────────────────────

        /// <summary>GET /api/collection/{playerId}</summary>
        public Coroutine GetCollection(MonoBehaviour host, Guid playerId, Action<CollectionResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<CollectionResponse>($"/api/collection/{playerId}", onSuccess, onError));
        }

        /// <summary>GET /api/collection/booster-sets</summary>
        public Coroutine GetBoosterSets(MonoBehaviour host, Action<BoosterSetsResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<BoosterSetsResponse>("/api/collection/booster-sets", onSuccess, onError));
        }

        /// <summary>POST /api/collection/open-booster</summary>
        public Coroutine OpenBooster(MonoBehaviour host, OpenBoosterRequest request, Action<OpenBoosterResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<OpenBoosterResponse>("/api/collection/open-booster", request, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  WALLET ENDPOINTS  /api/wallet
        // ──────────────────────────────────────────────

        /// <summary>GET /api/wallet/{playerId}</summary>
        public Coroutine GetBalance(MonoBehaviour host, Guid playerId, Action<BalanceResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<BalanceResponse>($"/api/wallet/{playerId}", onSuccess, onError));
        }

        /// <summary>POST /api/wallet/add-funds</summary>
        public Coroutine AddFunds(MonoBehaviour host, AddFundsRequest request, Action<AddFundsResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AddFundsResponse>("/api/wallet/add-funds", request, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  MARKETPLACE ENDPOINTS  /api/marketplace
        // ──────────────────────────────────────────────

        /// <summary>GET /api/marketplace?cardType={cardType}&amp;rarity={rarity}</summary>
        public Coroutine GetMarketplace(MonoBehaviour host, string cardType, string rarity, Action<GetMarketplaceResponse> onSuccess, Action<string> onError = null)
        {
            var queryParts = new List<string>();
            if (!string.IsNullOrEmpty(cardType)) queryParts.Add($"cardType={UnityWebRequest.EscapeURL(cardType)}");
            if (!string.IsNullOrEmpty(rarity)) queryParts.Add($"rarity={UnityWebRequest.EscapeURL(rarity)}");

            string path = "/api/marketplace";
            if (queryParts.Count > 0) path += "?" + string.Join("&", queryParts);

            return host.StartCoroutine(Get<GetMarketplaceResponse>(path, onSuccess, onError));
        }

        /// <summary>POST /api/marketplace/list</summary>
        public Coroutine ListCard(MonoBehaviour host, ListCardRequest request, Action<ListingDto> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<ListingDto>("/api/marketplace/list", request, onSuccess, onError));
        }

        /// <summary>POST /api/marketplace/{id}/buy</summary>
        public Coroutine BuyCard(MonoBehaviour host, Guid listingId, BuyCardRequest request, Action<BuyCardResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<BuyCardResponse>($"/api/marketplace/{listingId}/buy", request, onSuccess, onError));
        }

        /// <summary>DELETE /api/marketplace/{id}?sellerId={sellerId}</summary>
        public Coroutine CancelListing(MonoBehaviour host, Guid listingId, Guid sellerId, Action<CancelListingResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Delete<CancelListingResponse>($"/api/marketplace/{listingId}?sellerId={sellerId}", onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  QUEUE ENDPOINTS  /api/queue
        // ──────────────────────────────────────────────

        /// <summary>POST /api/queue/join</summary>
        public Coroutine JoinQueue(MonoBehaviour host, JoinQueueRequest request, Action<JoinQueueResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<JoinQueueResponse>("/api/queue/join", request, onSuccess, onError));
        }

        /// <summary>POST /api/queue/leave</summary>
        public Coroutine LeaveQueue(MonoBehaviour host, LeaveQueueRequest request, Action<LeaveQueueResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<LeaveQueueResponse>("/api/queue/leave", request, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  TOURNAMENT ENDPOINTS  /api/tournaments
        // ──────────────────────────────────────────────

        /// <summary>POST /api/tournaments</summary>
        public Coroutine CreateTournament(MonoBehaviour host, CreateTournamentRequest request, Action<TournamentResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<TournamentResponse>("/api/tournaments", request, onSuccess, onError));
        }

        /// <summary>POST /api/tournaments/{id}/join</summary>
        public Coroutine JoinTournament(MonoBehaviour host, Guid tournamentId, JoinTournamentRequest request, Action<JoinTournamentResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<JoinTournamentResponse>($"/api/tournaments/{tournamentId}/join", request, onSuccess, onError));
        }

        /// <summary>POST /api/tournaments/{id}/advance</summary>
        public Coroutine AdvanceTournament(MonoBehaviour host, Guid tournamentId, AdvanceTournamentRequest request, Action<AdvanceTournamentResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<AdvanceTournamentResponse>($"/api/tournaments/{tournamentId}/advance", request, onSuccess, onError));
        }

        /// <summary>POST /api/tournaments/{id}/finalize</summary>
        public Coroutine FinalizeTournament(MonoBehaviour host, Guid tournamentId, Action<FinalizeTournamentResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<FinalizeTournamentResponse>($"/api/tournaments/{tournamentId}/finalize", null, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  RANKING ENDPOINTS  /api/ranking
        // ──────────────────────────────────────────────

        /// <summary>GET /api/ranking/{playerId}</summary>
        public Coroutine GetPlayerRank(MonoBehaviour host, Guid playerId, Action<PlayerRankResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Get<PlayerRankResponse>($"/api/ranking/{playerId}", onSuccess, onError));
        }

        /// <summary>POST /api/ranking/update-elo</summary>
        public Coroutine UpdateElo(MonoBehaviour host, UpdateEloRequest request, Action<UpdateEloResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<UpdateEloResponse>("/api/ranking/update-elo", request, onSuccess, onError));
        }

        /// <summary>POST /api/ranking/recalculate-tiers</summary>
        public Coroutine RecalculateTiers(MonoBehaviour host, Action<RecalculateTiersResponse> onSuccess, Action<string> onError = null)
        {
            return host.StartCoroutine(Post<RecalculateTiersResponse>("/api/ranking/recalculate-tiers", null, onSuccess, onError));
        }

        // ──────────────────────────────────────────────
        //  HTTP HELPERS
        // ──────────────────────────────────────────────

        private IEnumerator Get<TResponse>(string path, Action<TResponse> onSuccess, Action<string> onError, bool includeAuth = true)
        {
            string url = _baseUrl + path;

            using (var request = UnityWebRequest.Get(url))
            {
                ConfigureJsonHeaders(request, includeAuth);

                yield return request.SendWebRequest();

                HandleResponse(request, onSuccess, onError);
            }
        }

        private IEnumerator Post<TResponse>(string path, object body, Action<TResponse> onSuccess, Action<string> onError, bool includeAuth = true)
        {
            string url = _baseUrl + path;
            string json = body != null ? JsonUtility.ToJson(body) : "{}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                ConfigureJsonHeaders(request, includeAuth);

                yield return request.SendWebRequest();

                HandleResponse(request, onSuccess, onError);
            }
        }

        private IEnumerator Put<TResponse>(string path, object body, Action<TResponse> onSuccess, Action<string> onError, bool includeAuth = true)
        {
            string url = _baseUrl + path;
            string json = body != null ? JsonUtility.ToJson(body) : "{}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                ConfigureJsonHeaders(request, includeAuth);

                yield return request.SendWebRequest();

                HandleResponse(request, onSuccess, onError);
            }
        }

        private IEnumerator Delete<TResponse>(string path, Action<TResponse> onSuccess, Action<string> onError, bool includeAuth = true)
        {
            string url = _baseUrl + path;

            using (var request = UnityWebRequest.Delete(url))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                ConfigureJsonHeaders(request, includeAuth);

                yield return request.SendWebRequest();

                HandleResponse(request, onSuccess, onError);
            }
        }

        private void ConfigureJsonHeaders(UnityWebRequest request, bool includeAuth)
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            if (!includeAuth || string.IsNullOrWhiteSpace(_accessToken))
                return;

            request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
        }

        private void HandleResponse<TResponse>(UnityWebRequest request, Action<TResponse> onSuccess, Action<string> onError)
        {
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                string error = $"[ApiClient] Network error: {request.error}";
                Debug.LogError(error);
                onError?.Invoke(error);
                return;
            }

            string responseBody = request.downloadHandler?.text ?? string.Empty;
            long statusCode = request.responseCode;

            if (statusCode >= 400)
            {
                string error;
                try
                {
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(responseBody);
                    error = $"[ApiClient] HTTP {statusCode}: {errorResponse.detail ?? errorResponse.title ?? responseBody}";
                }
                catch
                {
                    error = $"[ApiClient] HTTP {statusCode}: {responseBody}";
                }

                Debug.LogWarning(error);
                onError?.Invoke(error);
                return;
            }

            try
            {
                var result = JsonUtility.FromJson<TResponse>(responseBody);
                onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                string error = $"[ApiClient] Deserialization error for {typeof(TResponse).Name}: {ex.Message}\nBody: {responseBody}";
                Debug.LogError(error);
                onError?.Invoke(error);
            }
        }
    }
}
