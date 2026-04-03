using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.Core
{
    /// <summary>
    /// Singleton MonoBehaviour managing global state, API connection, and scene transitions.
    /// Persists across scenes via DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
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

        private ApiClient _apiClient;

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

            string savedPlayerId = PlayerPrefs.GetString("PlayerId", string.Empty);
            if (!string.IsNullOrEmpty(savedPlayerId) && Guid.TryParse(savedPlayerId, out var playerId))
            {
                CurrentPlayerId = playerId;
            }
            else
            {
                CurrentPlayerId = Guid.NewGuid();
                PlayerPrefs.SetString("PlayerId", CurrentPlayerId.ToString());
                PlayerPrefs.Save();
            }

            IsConnected = false;
            CurrentMatchId = null;

            Debug.Log($"[GameManager] Initialized. PlayerId: {CurrentPlayerId}");
        }

        /// <summary>
        /// Set the current player ID explicitly.
        /// </summary>
        public void SetPlayerId(Guid playerId)
        {
            CurrentPlayerId = playerId;
            PlayerPrefs.SetString("PlayerId", playerId.ToString());
            PlayerPrefs.Save();
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
