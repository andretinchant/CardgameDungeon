using System;
using System.Collections;
using UnityEngine;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Core
{
    /// <summary>
    /// Singleton MonoBehaviour managing global state, API connection, and scene transitions.
    /// Persists across scenes via DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public sealed class MarketplaceFocusContext
        {
            public string CardId { get; set; } = string.Empty;
            public string CardName { get; set; } = string.Empty;
            public string CardType { get; set; } = string.Empty;
            public string Rarity { get; set; } = string.Empty;
        }

        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("GameManager instance is null. Ensure a GameManager exists in the scene.");
                }
                return _instance;
            }
        }

        [Header("API")]
        [SerializeField] private string _apiBaseUrl = "http://localhost:5214";

        [Header("Scenes")]
        [SerializeField] private string _loadingSceneName = "Loading";
        [SerializeField] private string _loginSceneName = "Login";
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        private ApiClient _apiClient;

        private const string PlayerIdPrefKey = "PlayerId";
        private const string AccessTokenPrefKey = "AccessToken";
        private const string RefreshTokenPrefKey = "RefreshToken";
        private const string UsernamePrefKey = "Username";

        /// <summary>
        /// The API client used for all server communication.
        /// </summary>
        public ApiClient Api => _apiClient;

        /// <summary>
        /// The current player's unique identifier.
        /// </summary>
        public Guid CurrentPlayerId { get; private set; }

        /// <summary>
        /// Whether the client is connected to the API server.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// The current match ID, if any.
        /// </summary>
        public Guid? CurrentMatchId { get; set; }

        /// <summary>
        /// One-time marketplace deep-link context set by collection UI.
        /// </summary>
        private MarketplaceFocusContext _pendingMarketplaceFocus;

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string Username { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken) && CurrentPlayerId != Guid.Empty;
        public string LoginSceneName => _loginSceneName;
        public string MainMenuSceneName => _mainMenuSceneName;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        /// <summary>
        /// Initializes the GameManager, creating the ApiClient and loading persisted state.
        /// </summary>
        public void Initialize()
        {
            _apiClient = new ApiClient(_apiBaseUrl);

            var savedPlayerId = PlayerPrefs.GetString(PlayerIdPrefKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(savedPlayerId) && Guid.TryParse(savedPlayerId, out var playerId))
            {
                CurrentPlayerId = playerId;
            }
            else
            {
                CurrentPlayerId = Guid.NewGuid();
                PlayerPrefs.SetString(PlayerIdPrefKey, CurrentPlayerId.ToString());
                PlayerPrefs.Save();
            }

            AccessToken = PlayerPrefs.GetString(AccessTokenPrefKey, string.Empty);
            RefreshToken = PlayerPrefs.GetString(RefreshTokenPrefKey, string.Empty);
            Username = PlayerPrefs.GetString(UsernamePrefKey, string.Empty);

            if (!string.IsNullOrWhiteSpace(AccessToken))
                _apiClient.SetAccessToken(AccessToken);

            IsConnected = false;
            CurrentMatchId = null;

            Debug.Log($"[GameManager] Initialized. PlayerId: {CurrentPlayerId}, Auth: {IsAuthenticated}");
        }

        /// <summary>
        /// Set the current player ID explicitly.
        /// </summary>
        public void SetPlayerId(Guid playerId)
        {
            CurrentPlayerId = playerId;
            PlayerPrefs.SetString(PlayerIdPrefKey, playerId.ToString());
            PlayerPrefs.Save();
        }

        public void ApplyAuth(AuthResponse authResponse)
        {
            if (authResponse == null)
                throw new ArgumentNullException(nameof(authResponse));

            if (!Guid.TryParse(authResponse.playerId, out var parsedPlayerId))
                throw new InvalidOperationException("Auth response returned invalid playerId.");

            CurrentPlayerId = parsedPlayerId;
            Username = authResponse.username ?? string.Empty;
            AccessToken = authResponse.accessToken ?? string.Empty;
            RefreshToken = authResponse.refreshToken ?? string.Empty;

            _apiClient.SetAccessToken(AccessToken);

            PlayerPrefs.SetString(PlayerIdPrefKey, CurrentPlayerId.ToString());
            PlayerPrefs.SetString(UsernamePrefKey, Username);
            PlayerPrefs.SetString(AccessTokenPrefKey, AccessToken);
            PlayerPrefs.SetString(RefreshTokenPrefKey, RefreshToken);
            PlayerPrefs.Save();
        }

        public void ClearAuth()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            Username = string.Empty;
            _apiClient.SetAccessToken(string.Empty);

            PlayerPrefs.DeleteKey(AccessTokenPrefKey);
            PlayerPrefs.DeleteKey(RefreshTokenPrefKey);
            PlayerPrefs.DeleteKey(UsernamePrefKey);
            PlayerPrefs.Save();
        }

        public IEnumerator TryRefreshSession(Action<bool, string> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(AccessToken) || string.IsNullOrWhiteSpace(RefreshToken))
            {
                onCompleted?.Invoke(false, "No persisted auth session.");
                yield break;
            }

            var completed = false;
            var success = false;
            string errorMessage = null;

            var request = new RefreshTokenRequest
            {
                accessToken = AccessToken,
                refreshToken = RefreshToken
            };

            yield return _apiClient.Refresh(this, request,
                auth =>
                {
                    try
                    {
                        ApplyAuth(auth);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        success = false;
                    }
                    finally
                    {
                        completed = true;
                    }
                },
                error =>
                {
                    errorMessage = error;
                    success = false;
                    completed = true;
                });

            while (!completed)
                yield return null;

            onCompleted?.Invoke(success, errorMessage);
        }

        /// <summary>
        /// Mark the connection status.
        /// </summary>
        public void SetConnected(bool connected)
        {
            IsConnected = connected;
        }

        /// <summary>
        /// Load a scene by name (synchronous).
        /// </summary>
        public void GoToScene(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }

        public void GoToSceneWithLoading(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(_loadingSceneName))
            {
                GoToScene(sceneName);
                return;
            }

            SceneLoader.LoadSceneWithLoadingScreen(sceneName, _loadingSceneName);
        }

        /// <summary>
        /// Backward-compatible alias for legacy scripts.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            GoToScene(sceneName);
        }

        public void SetMarketplaceFocus(string cardId, string cardName, string cardType, string rarity)
        {
            _pendingMarketplaceFocus = new MarketplaceFocusContext
            {
                CardId = cardId ?? string.Empty,
                CardName = cardName ?? string.Empty,
                CardType = cardType ?? string.Empty,
                Rarity = rarity ?? string.Empty
            };
        }

        public MarketplaceFocusContext ConsumeMarketplaceFocus()
        {
            var context = _pendingMarketplaceFocus;
            _pendingMarketplaceFocus = null;
            return context;
        }

        /// <summary>
        /// Quit the application.
        /// </summary>
        public void Quit()
        {
            Debug.Log("[GameManager] Quitting application.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
