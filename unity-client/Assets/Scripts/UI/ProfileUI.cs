using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CardgameDungeon.Unity.Core;
using CardgameDungeon.Unity.Network;

namespace CardgameDungeon.Unity.UI
{
    public class ProfileUI : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text playerIdText;
        [SerializeField] private TMP_Text statusText;

        [Header("Actions")]
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button backButton;

        [Header("Navigation")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string loginSceneName = "Login";

        private void OnEnable()
        {
            if (logoutButton != null) logoutButton.onClick.AddListener(OnLogoutClicked);
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
            RefreshView();
        }

        private void OnDisable()
        {
            if (logoutButton != null) logoutButton.onClick.RemoveListener(OnLogoutClicked);
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void RefreshView()
        {
            if (usernameText != null)
                usernameText.text = $"User: {GameManager.Instance.Username}";

            if (playerIdText != null)
                playerIdText.text = $"PlayerId: {GameManager.Instance.CurrentPlayerId}";
        }

        private void OnLogoutClicked()
        {
            var refreshToken = GameManager.Instance.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                GameManager.Instance.ClearAuth();
                GameManager.Instance.GoToScene(loginSceneName);
                return;
            }

            SetStatus("Signing out...");
            if (logoutButton != null) logoutButton.interactable = false;

            var request = new RevokeTokenRequest { refreshToken = refreshToken };
            GameManager.Instance.Api.Revoke(this, request, _ =>
            {
                GameManager.Instance.ClearAuth();
                SetStatus("Logged out.");
                GameManager.Instance.GoToScene(loginSceneName);
            }, error =>
            {
                // Force local logout even if revoke failed remotely.
                Debug.LogWarning($"[ProfileUI] Revoke failed: {error}");
                GameManager.Instance.ClearAuth();
                GameManager.Instance.GoToScene(loginSceneName);
            });
        }

        private void OnBackClicked()
        {
            GameManager.Instance.GoToSceneWithLoading(mainMenuSceneName);
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            Debug.Log($"[ProfileUI] {message}");
        }
    }
}
